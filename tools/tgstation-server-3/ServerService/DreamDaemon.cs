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

		const string HardRebootRequestFile = "/HardReboot.lk";

		Process Proc;

		object watchdogLock = new object();
		Thread DDWatchdog;
		TGDreamDaemonStatus currentStatus;
		ushort currentPort = 0;

		const int DDHangStartTime = 30;
		const int DDBadStartTime = 10;
		const int DDRestartMaxRetries = 5;

		object restartLock = new object();
		bool RestartInProgress = false;

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

		public TGDreamDaemonStatus DaemonStatus()
		{
			lock (watchdogLock)
			{
				return currentStatus;
			}
		}

		void RequestRestart()
		{
			SendCommand("hard_reboot");
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
				RequestRestart();
			}
		}

		public string Restart()
		{
			if (!Monitor.TryEnter(restartLock))
				return "Restart already in progress";
			SendMessage("DD: Hard restart triggered");
			RestartInProgress = true;
			try
			{
				Stop();
				var res = Start();
				if(res != null)
					RestartInProgress = false;
				return res;
			}
			finally
			{
				Monitor.Exit(restartLock);
			}
		}

		void Watchdog()
		{
			try
			{
				lock (restartLock)
				{
					if (!RestartInProgress)
						SendMessage("DD: Server started, watchdog active...");
					else
						RestartInProgress = false;
				}
				var retries = DDRestartMaxRetries;
				while (true)
				{
					var starttime = DateTime.Now;
					Proc.WaitForExit();

					lock (watchdogLock)
					{
						currentStatus = TGDreamDaemonStatus.HardRebooting;
						currentPort = 0;
						Proc.Close();

						if ((DateTime.Now - starttime).Seconds < DDBadStartTime)
						{
							if (retries == 0)
							{
								SendMessage("DD: DEFCON 0: Watchdog unable to restart server!");
								TGServerService.ActiveService.EventLog.WriteEntry("Watchdog failed to restart server! Shutting down!", EventLogEntryType.Error);
								return;
							}

							SendMessage(String.Format("DD: DEFCON {0}: Watchdog server startup failed!", retries));

							--retries;
						}
						else
						{
							retries = DDRestartMaxRetries;
							SendMessage("DD: DreamDaemon crashed or exited! Rebooting...");
						}
					}

					var res = StartImpl(true);
					if (res != null)
						throw new Exception("Hard restart failed: " + res);
				}
			}
			catch(ThreadAbortException)
			{
				if(!RestartInProgress)
					SendMessage("DD: Watchdog exiting...");
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
					currentPort = 0;
				}
			}
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
			ApplyStagedUpdate();
			lock (watchdogLock)
			{
				if (currentStatus != TGDreamDaemonStatus.Offline)
					return "Server already running";
				currentStatus = TGDreamDaemonStatus.HardRebooting;
				currentPort = 0;
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

		string StartImpl(bool watchdog)
		{
			try
			{
				var res = CanStart();
				if (res != null)
					return res;

				lock (watchdogLock)
				{
					var Config = Properties.Settings.Default;
					var DMB = GameDirLive + "/" + Config.ProjectName + ".dmb";

					GenCommsKey();
					Proc.StartInfo.Arguments = String.Format("{0} -port {1} -close -verbose -params server_service={4} -{2} -{3}", DMB, Config.ServerPort, SecurityWord(), VisibilityWord(), serviceCommsKey);
					Proc.Start();

					if (!Proc.WaitForInputIdle(DDHangStartTime * 1000))
					{
						Proc.Kill();
						Proc.Close();
						currentStatus = TGDreamDaemonStatus.Offline;
						currentPort = 0;
						return String.Format("Server start is taking more that {0}s! Aborting!", DDHangStartTime);
					}
					currentPort = Config.ServerPort;
					currentStatus = TGDreamDaemonStatus.Online;
					if (!watchdog)
					{
						DDWatchdog = new Thread(new ThreadStart(Watchdog));
						DDWatchdog.Start();
					}
					return null;
				}
			}
			catch (Exception e)
			{
				currentStatus = TGDreamDaemonStatus.Offline;
				return e.ToString();
			}
		}

		public void SetVisibility(TGDreamDaemonVisibility NewVis)
		{
			Properties.Settings.Default.ServerVisiblity = (int)NewVis;
			RequestRestart();
		}

		public void SetSecurityLevel(TGDreamDaemonSecurity level)
		{
			Properties.Settings.Default.ServerSecurity = (int)level;
			RequestRestart();
		}
	}
}
