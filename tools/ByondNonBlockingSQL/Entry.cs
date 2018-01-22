using RGiesecke.DllExport;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ByondNonBlockingSQL
{
	/// <summary>
	/// Contains exports for the DMAPI
	/// </summary>
	public static class Entry
	{
		const string Yes = "YES";
		const string No = "NO";

		static Dictionary<int, IDbConnection> connections = new Dictionary<int, IDbConnection>();
		static Dictionary<int, Task<string>> jobTasks = new Dictionary<int, Task<string>>();

		static string lastError;
		static string currentNativeString;
		static IntPtr currentNativeStringPointer;

		static IntPtr NativeReturnString(string retVal)
		{
			if (currentNativeString != retVal)
			{
				if (currentNativeStringPointer != IntPtr.Zero)
					Marshal.FreeHGlobal(currentNativeStringPointer);
				if (retVal == null)
					currentNativeStringPointer = IntPtr.Zero;
				else
				{
					var bytes = Encoding.ASCII.GetBytes(retVal);
					int size = Marshal.SizeOf(bytes[0]) * bytes.Length;

					currentNativeStringPointer = Marshal.AllocHGlobal(size);

					Marshal.Copy(bytes, 0, currentNativeStringPointer, bytes.Length);
				}
				currentNativeString = retVal;
			}
			return currentNativeStringPointer;
		}

		static IntPtr AllocateTask(Func<string> action)
		{
			for (var I = 0; I < Int32.MaxValue; ++I)
				if (!jobTasks.ContainsKey(I))
				{
					jobTasks.Add(I, Task.Factory.StartNew(action, TaskCreationOptions.LongRunning));
					return NativeReturnString(I.ToString());
				}
			lastError = "No available job IDs";
			return NativeReturnString(null);
		}

		/// <summary>
		/// Closes all connections
		/// </summary>
		/// <returns><see langword="null"/> <see cref="string"/> on success, error message on failure</returns>
		static IntPtr Cleanup()
		{
			try
			{
				try
				{
					Task.WaitAll(jobTasks.Select(x => x.Value).ToArray());
				}
				// we don't care if queries fail at this point, anything that did should've waited for it individually
				catch (Exception) { }
				jobTasks.Clear();
				foreach (var I in connections)
					I.Value.Dispose();
				connections.Clear();
				return NativeReturnString(null);
			}
			catch (Exception e)
			{
				return NativeReturnString(e.ToString());
			}
		}

		/// <summary>
		/// Initializes the library. Should be called at world startup
		/// </summary>
		/// <param name="argc">Ignored</param>
		/// <param name="args">Ignored</param>
		/// <returns><see langword="null"/> <see cref="string"/> on success, error message on failure</returns>
		[DllExport(CallingConvention = CallingConvention.Cdecl)]
		public static IntPtr Initialize(int argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 0)]string[] args)
		{
			return Cleanup();
		}

		/// <summary>
		/// Cleans up the library. Should be called before world reboot
		/// </summary>
		/// <param name="argc">Ignored</param>
		/// <param name="args">Ignored</param>
		/// <returns><see langword="null"/> <see cref="string"/> on success, error message on failure</returns>
		[DllExport(CallingConvention = CallingConvention.Cdecl)]
		public static IntPtr Shutdown(int argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 0)]string[] args)
		{
			return Cleanup();
		}

		/// <summary>
		/// Cleans up the library. Should be called before world reboot
		/// </summary>
		/// <param name="argc">Ignored</param>
		/// <param name="args">1st element: connection identifer, 2nd element: Database name, 3rd element: IP, 4th element: Port, 5th element: User name, 6th element: password, optional 7th element: connection/command timeout in seconds</param>
		/// <returns>Job ID on success, <see langword="null"/> on failure</returns>
		[DllExport(CallingConvention = CallingConvention.Cdecl)]
		public static IntPtr Connect(int argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 0)]string[] args)
		{
			try
			{
				var connectionID = Convert.ToInt32(args[0]);
				var database = args[1];
				var ip = args[2];
				var port = Convert.ToUInt16(args[3]);
				var user = args[4];
				var pass = args[5];

				var connection = connections[connectionID];

				var builder = new MySqlConnectionStringBuilder
				{
					Database = database,
					Password = pass,
					Port = port,
					Server = ip,
					UserID = user
				};
				if (args.Length > 6)
				{
					builder.ConnectionTimeout = Convert.ToUInt32(args[6]);
					builder.DefaultCommandTimeout = builder.ConnectionTimeout;
				}

				connection.ConnectionString = builder.ConnectionString;

				return AllocateTask(() => {
					lock(connection)
						connection.Open();
					return Yes;
				});
			}
			catch (Exception e)
			{
				lastError = e.ToString();
				return NativeReturnString(null);
			}
		}

		/// <summary>
		/// Checks if a connection ID is connected
		/// </summary>
		/// <param name="argc">Ignored</param>
		/// <param name="args">1st element: Connection ID</param>
		/// <returns><see cref="Yes"/> if the connection is connected, <see cref="No"/> if it isn't, <see langword="null"/> if an error occurred</returns>
		[DllExport(CallingConvention = CallingConvention.Cdecl)]
		public static IntPtr IsConnected(int argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 0)]string[] args)
		{
			try
			{
				return NativeReturnString(connections[Convert.ToInt32(args[0])].State.HasFlag(ConnectionState.Open) ? Yes : No);
			}
			catch (Exception e)
			{
				lastError = e.ToString();
				return NativeReturnString(null);
			}
		}

		/// <summary>
		/// Disconnect a connection ID
		/// </summary>
		/// <param name="argc">Ignored</param>
		/// <param name="args">1st element: Connection ID</param>
		/// <returns>Job ID on success, <see langword="null"/> on failure</returns>
		[DllExport(CallingConvention = CallingConvention.Cdecl)]
		public static IntPtr Disconnect(int argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 0)]string[] args)
		{
			try
			{
				var connection = connections[Convert.ToInt32(args[0])];
				return AllocateTask(() => {
					lock (connection)
					{
						if (!connection.State.HasFlag(ConnectionState.Closed))
							connection.Close();
					}
					return Yes;
				});
			}
			catch (Exception e)
			{
				lastError = e.ToString();
				return NativeReturnString(null);
			}
		}

		/// <summary>
		/// Checks if a job is complete
		/// </summary>
		/// <param name="argc">Ignored</param>
		/// <param name="args">List of job IDs</param>
		/// <returns>A <see cref="string"/> seperated by semicolons for each requested job; <see cref="Yes"/> if the job is complete, <see cref="No"/> if it isn't, <see langword="null"/> if an error occurred</returns>
		[DllExport(CallingConvention = CallingConvention.Cdecl)]
		public static IntPtr IsJobComplete(int argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 0)]string[] args)
		{
			try
			{
				var results = new List<string>();
				for (var I = 0; I < args.Length; ++I)
				{
					var task = jobTasks[Convert.ToInt32(args[I])];
					results.Add(task.IsCompleted || task.IsFaulted ? Yes : No);
				}
				return NativeReturnString(String.Join(";", results));
			}
			catch (Exception e)
			{
				lastError = e.ToString();
				return NativeReturnString(null);
			}
		}

		/// <summary>
		/// Checks if a job is complete
		/// </summary>
		/// <param name="argc">Ignored</param>
		/// <param name="args">1st element: Job ID</param>
		/// <returns>Result of the job on success, <see langword="null"/> on error</returns>
		[DllExport(CallingConvention = CallingConvention.Cdecl)]
		public static IntPtr CompleteJob(int argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 0)]string[] args)
		{
			try
			{
				return NativeReturnString(jobTasks[Convert.ToInt32(args[0])].Result);
			}
			catch (Exception e)
			{
				lastError = e.ToString();
				return NativeReturnString(null);
			}
		}

		/// <summary>
		/// Creates and registers a MySql <see cref="IDbConnection"/>
		/// </summary>
		/// <param name="argc">Ignored</param>
		/// <param name="args">Ignored</param>
		/// <returns>Connection identifier on success, <see langword="null"/> on failure</returns>
		[DllExport(CallingConvention = CallingConvention.Cdecl)]
		public static IntPtr CreateConnection(int argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 0)]string[] args)
		{
			try
			{
				var factory = new MySqlClientFactory();
				var res = factory.CreateConnection();
				for (var I = 0; I < Int32.MaxValue; ++I)
					if (!connections.ContainsKey(I))
					{
						connections.Add(I, res);
						return NativeReturnString(I.ToString());
					}
				lastError = "No available connection IDs!";
				return NativeReturnString(null);
			}
			catch (Exception e)
			{
				return NativeReturnString(e.ToString());
			}
		}

		/// <summary>
		/// Returns <see cref="lastError"/> and clears it
		/// </summary>
		/// <param name="argc">Ignored</param>
		/// <param name="args">Ignored</param>
		/// <returns>The value of <see cref="lastError"/></returns>
		[DllExport(CallingConvention = CallingConvention.Cdecl)]
		public static IntPtr GetLastError(int argc, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr, SizeParamIndex = 0)]string[] args)
		{
			var lE = lastError;
			lastError = null;
			return NativeReturnString(lE);
		}
	}
}
