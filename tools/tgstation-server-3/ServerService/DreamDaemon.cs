using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using TGServiceInterface;

namespace TGServerService
{
	//manages clicking the hidden DD window
	partial class TGStationServer : ITGDreamDaemon
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


		//TODO: API for checking when the game reboots

		Process Proc;
		IntPtr hwndMain;

		bool worldOpen = true;

		object watchdogLock = new object();
		Thread DDWatchdog;
		TGDreamDaemonStatus currentStatus;

		void InitDreamDaemon()
		{
			Proc = new Process();
			Proc.StartInfo.FileName = ByondDirectory + "/bin/dreamdaemon.exe";
			Proc.StartInfo.UseShellExecute = false;
		}

		void DisposeDreamDaemon()
		{
			Stop();
		}

		public bool WorldOpen()
		{
			lock (watchdogLock)
			{
				return worldOpen;
			}
		}

		public TGDreamDaemonStatus DaemonStatus()
		{
			lock (watchdogLock)
			{
				return currentStatus;
			}
		}

		public string Stop()
		{
			Thread t;
			lock (watchdogLock)
			{
				t = DDWatchdog;
				DDWatchdog = null;
			}
			if (t != null && t.IsAlive)
			{
				t.Abort();
				t.Join();
				return null;
			}
			else
				return "Server not running";
		}

		public void SetPort(ushort new_port)
		{
			lock (watchdogLock)
			{
				Properties.Settings.Default.ServerPort = new_port;
			}
		}

		public string Restart()
		{
			Stop();
			return Start();
		}

		void Watchdog()
		{
			try
			{
				SendMessage("DD: Server started, watchdog active...");
				while (true)
				{
					Proc.WaitForExit();
					SendMessage("DD: DreamDaemon crashed or exited! Rebooting...");
					Proc.Close();
					lock (watchdogLock)
					{
						currentStatus = TGDreamDaemonStatus.HardRebooting;
					}
					//we would have been killed if we wanted to stop so lets check for updates
					ApplyStagedUpdate();

					var res = StartImpl(true);
					if (res != null)
						throw new Exception("Hard restart failed: " + res);
				}
			}
			catch(ThreadAbortException)
			{
				//No Mr bond, I expect you to die
				try
				{
					Proc.Kill();
					Proc.Close();
				}
				catch
				{ }
				return;
			}
			catch (Exception e)
			{
				SendMessage("DD: Watchdog thread crashed!");
				TGServerService.ActiveService.EventLog.WriteEntry("Watch dog thread crashed! Exception: " + e.ToString(), EventLogEntryType.Error);
			}
			finally
			{
				lock (watchdogLock)
				{
					currentStatus = TGDreamDaemonStatus.Offline;
				}
			}
		}
		public void SetWorldOpen(bool open)
		{
			lock (watchdogLock)
			{
				if (worldOpen != open)
				{
					ClickWorldOpen();
					worldOpen = open;
				}
			}
		}
		void ClickWorldOpen()
		{
			//TODO
		}

		public string CanStart()
		{
			lock (watchdogLock)
			{
				if (GetVersion(false) == null)
					return "Byond is not installed!";
				var DMB = GameDirLive + "/" + Properties.Settings.Default.ProjectName + ".dmb";
				if (!File.Exists(DMB))
					return String.Format("Unable to find {0}!", DMB);
				return null;
			}
		}

		public string Start()
		{
			lock (watchdogLock)
			{
				if (currentStatus != TGDreamDaemonStatus.Offline)
					return "Server already running";
			}
			var res = CanStart();
			if (res != null)
				return res;
			return StartImpl(false);
		}
		string SecurityWord()
		{
			var level = Properties.Settings.Default.ServerSecurity;
			switch ((TGDreamDaemonSecurity)level)
			{
				case TGDreamDaemonSecurity.Safe:
					return "safe";
				case TGDreamDaemonSecurity.Trusted:
					return "trusted";
				case TGDreamDaemonSecurity.Ultrasafe:
					return "ultrasafe";
				default:
					throw new Exception(String.Format("Bad DreamDaemon security level: {0}", level));
			}
		}

		string VisibilityWord()
		{
			var level = Properties.Settings.Default.ServerVisiblity;
			switch ((TGDreamDaemonVisibility)level)
			{
				case TGDreamDaemonVisibility.Invisible:
					return "invisible";
				case TGDreamDaemonVisibility.Private:
					return "private";
				case TGDreamDaemonVisibility.Public:
					return "public";
				default:
					throw new Exception(String.Format("Bad DreamDaemon visibility level: {0}", level));
			}
		}

		string StartImpl(bool watchdog) {
			try
			{
				var res = CanStart();
				if (res != null)
					return res;

				lock (watchdogLock)
				{
					var Config = Properties.Settings.Default;
					var DMB = GameDirLive + "/" + Config.ProjectName + ".dmb";

					Proc.StartInfo.Arguments = String.Format("{0} -port {1} -close -verbose -{2} -{3}", DMB, Config.ServerPort, SecurityWord(), VisibilityWord());
					Proc.Start();
				}
				if (!Proc.WaitForInputIdle(20000))
				{
					Proc.Kill();
					Proc.Close();
					return "Server start is taking more that 20s! Aborting!";
				}

				var Handles = EnumerateProcessWindowHandles(Proc.Id);

				var NameList = new List<string>();
				foreach (IntPtr I in Handles)
				{
					//We're looking for the one called "Dream Daemon"
					int capacity = GetWindowTextLength(new HandleRef(this, I)) * 2;
					StringBuilder stringBuilder = new StringBuilder(capacity);
					GetWindowText(new HandleRef(this, I), stringBuilder, stringBuilder.Capacity);
					if (stringBuilder.ToString() == "Dream Daemon")
					{
						hwndMain = I;
						lock (watchdogLock)
						{
							ClickVisibility();
							currentStatus = TGDreamDaemonStatus.Online;
							if (!watchdog)
							{
								DDWatchdog = new Thread(new ThreadStart(Watchdog));
								DDWatchdog.Start();
							}
						}
						return null;
					}
				}

				Proc.Kill();
				Proc.Close();
				return "Could not find locate the Dream Daemon window!";
			}catch (Exception e)
			{
				return e.ToString();
			}
		}

		public void SetVisibility(TGDreamDaemonVisibility NewVis)
		{
			Properties.Settings.Default.ServerVisiblity = (int)NewVis;
			if (DaemonStatus() == TGDreamDaemonStatus.Online)
				ClickVisibility();
		}

		public void SetSecurityLevel(TGDreamDaemonSecurity level)
		{
			Properties.Settings.Default.ServerSecurity = (int)level;
			if (DaemonStatus() == TGDreamDaemonStatus.Online)
				ClickSecurity();
		}

		void ClickSecurity()
		{
			//TODO
		}

		void ClickVisibility()
		{
			SendClick(370, 300);
			Point SecondClick;
			switch ((TGDreamDaemonVisibility)Properties.Settings.Default.ServerVisiblity)
			{
				case TGDreamDaemonVisibility.Public:
					SecondClick = new Point(350, 315);
					break;
				case TGDreamDaemonVisibility.Private:
					SecondClick = new Point(350, 330);
					break;
				case TGDreamDaemonVisibility.Invisible:
					SecondClick = new Point(350, 345);
					break;
				default:
					return;
			}
			Thread.Sleep(1000);    //Play out the anim
			SendClick(SecondClick.X, SecondClick.Y);
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

			var inputMouseDown = new INPUT()
			{
				Type = 0
			};
			/// input type mouse
			inputMouseDown.Data.Mouse.Flags = 0x0002; /// left button down

			var inputMouseUp = new INPUT()
			{
				Type = 0
			};
			/// input type mouse
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
		#endregion
	}
}
