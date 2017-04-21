using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DDClickRelayTest
{
	//manages clicking the hidden DD window
	class DreamDaemon : IDisposable
	{
		#region Imports And Declarations
		private const uint WM_LBUTTONDOWN = 0x201;
		private const uint WM_LBUTTONUP = 0x202;
		private const uint MK_LBUTTON = 0x0001;

		public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr parameter);

		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

		delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);
		[DllImport("user32.dll")]
		static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn,
			IntPtr lParam);[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int GetWindowTextLength(HandleRef hWnd);
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern int GetWindowText(HandleRef hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll")]
		static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

		[DllImport("user32.dll")]
		internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);
		#endregion
		
		public enum Visibility
		{
			Invisible,
			Public,
			Private,
		}

		Process Proc;
		IntPtr hwndMain;

		public DreamDaemon()
		{
			Proc = new Process();
			Proc.StartInfo.FileName = "D:\\byond\\bin\\dreamdaemon.exe";
			Proc.Start();
			Proc.WaitForInputIdle(5000);

			var Handles = EnumerateProcessWindowHandles(Proc.Id);

			var NameList = new List<string>();
			foreach(IntPtr I in Handles)
			{
				//We're looking for the one called "Dream Daemon"
				int capacity = GetWindowTextLength(new HandleRef(this, I)) * 2;
				StringBuilder stringBuilder = new StringBuilder(capacity);
				GetWindowText(new HandleRef(this, I), stringBuilder, stringBuilder.Capacity);
				if(stringBuilder.ToString() == "Dream Daemon")
				{
					hwndMain = I;
					return;
				}
			}

			Proc.Kill();
			throw new Exception("Could not find DD window");
		}

		public void ChangeVisibility(Visibility NewVis)
		{
			SendClick(370, 300);
			Point SecondClick;
			switch (NewVis)
			{
				case Visibility.Public:
					SecondClick = new Point(350, 315);
					break;
				case Visibility.Private:
					SecondClick = new Point(350, 330);
					break;
				case Visibility.Invisible:
					SecondClick = new Point(350, 345);
					break;
				default:
					return;
			}
			System.Threading.Thread.Sleep(1000);	//Play out the anim
			SendClick(SecondClick.X, SecondClick.Y);
		}
		
		//click functions
		public void ClickWorld()
		{
			SendClick(30, -5);
		}

		public void ClickFile()
		{
			SendClick(5, -5);
		}

		public void ClickHelp()
		{
			SendClick(90, -5);
		}

		#region Here be Dragons
		public void SendClick(int x, int y)
		{
			ClickOnPoint(hwndMain, new Point(x, y));
		}


#pragma warning disable 649
		internal struct INPUT
		{
			public UInt32 Type;
			public MOUSEKEYBDHARDWAREINPUT Data;
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct MOUSEKEYBDHARDWAREINPUT
		{
			[FieldOffset(0)]
			public MOUSEINPUT Mouse;
		}

		internal struct MOUSEINPUT
		{
			public Int32 X;
			public Int32 Y;
			public UInt32 MouseData;
			public UInt32 Flags;
			public UInt32 Time;
			public IntPtr ExtraInfo;
		}

#pragma warning restore 649

		public static void ClickOnPoint(IntPtr wndHandle, Point clientPoint)
		{
			var oldPos = Cursor.Position;

			/// get screen coordinates
			ClientToScreen(wndHandle, ref clientPoint);

			/// set cursor on coords, and press mouse
			Cursor.Position = new Point(clientPoint.X, clientPoint.Y);

			var inputMouseDown = new INPUT();
			inputMouseDown.Type = 0; /// input type mouse
			inputMouseDown.Data.Mouse.Flags = 0x0002; /// left button down

			var inputMouseUp = new INPUT();
			inputMouseUp.Type = 0; /// input type mouse
			inputMouseUp.Data.Mouse.Flags = 0x0004; /// left button up

			var inputs = new INPUT[] { inputMouseDown, inputMouseUp };
			SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));

			/// return mouse 
			Cursor.Position = oldPos;
		}
		static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
		{
			var handles = new List<IntPtr>();

			foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
				EnumThreadWindows(thread.Id,
					(hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

			return handles;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
					if (Proc != null)
					{
						if(!Proc.HasExited)
							Proc.Kill();
						Proc.Close();
						Proc = null;
					}
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~DreamDaemon() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
		#endregion
	}
}
