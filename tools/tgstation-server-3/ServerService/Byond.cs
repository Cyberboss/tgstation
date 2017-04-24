using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using TGServiceInterface;

namespace TGServerService
{
	partial class TGStationServer : ITGByond
	{
		const string ByondDirectory = "C:/tgstation-server-3/BYOND";
		const string StagingDirectory = "C:/tgstation-server-3/BYOND_staged";
		const string StagingDirectoryInner = "C:/tgstation-server-3/BYOND_staged/byond";
		const string RevisionDownloadPath = "C:/tgstation-server-3/BYONDRevision.zip";
		const string VersionFile = "/byond_version.dat";
		const string ByondRevisionsURL = "https://secure.byond.com/download/build/{0}/{0}.{1}_byond.zip";

		const string ByondConfigDir = "/BYOND/cfg";
		const string ByondDDConfig = "/daemon.txt";
		const string ByondNoPromptTrustedMode = "trusted-check 0";

		TGByondStatus updateStat = TGByondStatus.Idle;
		object ByondLock = new object();
		string lastError;

		Thread RevisionStaging;

		void InitByond()
		{
			//linger not
			if (File.Exists(RevisionDownloadPath))
				File.Delete(RevisionDownloadPath);
			Program.DeleteDirectory(StagingDirectory);
		}

		void DisposeByond()
		{
			lock (ByondLock)
			{
				if (RevisionStaging != null)
					RevisionStaging.Abort();
				InitByond();
			}
		}

		bool BusyCheckNoLock()
		{
			switch (updateStat)
			{
				default:
				case TGByondStatus.Starting:
				case TGByondStatus.Downloading:
				case TGByondStatus.Staging:
				case TGByondStatus.Updating:
					return true;
				case TGByondStatus.Idle:
				case TGByondStatus.Staged:
					return false;
			}
		}

		public TGByondStatus CurrentStatus()
		{
			lock (ByondLock)
			{
				return updateStat;
			}
		}
		public int GetProgress()
		{
			lock (ByondLock)
			{
				switch (updateStat)
				{
					case TGByondStatus.Idle:
						return 0;
					case TGByondStatus.Starting:
					case TGByondStatus.Downloading:
						return 25;
					case TGByondStatus.Staging:
						return 50;
					case TGByondStatus.Staged:
						return 75;
					case TGByondStatus.Updating:
						return 90;
					default:
						return 100;
				}
			}
		}

		public string GetError()
		{
			lock (ByondLock)
			{
				return lastError;
			}
		}

		public string GetVersion(bool staged)
		{
			lock (ByondLock)
			{
				string DirToUse = staged ? StagingDirectoryInner : ByondDirectory;
				if (Directory.Exists(DirToUse)) {
					string file = DirToUse + VersionFile;
					if(File.Exists(file))
						return File.ReadAllText(file);
				}
				return null;
			}
		}

		class VersionInfo
		{
			public int major, minor;
		}

		public void UpdateToVersionImpl(object param)
		{
			lock (ByondLock) { 
				if (updateStat != TGByondStatus.Starting)
					return;
				updateStat = TGByondStatus.Downloading;
			}


			try
			{
				//remove leftovers
				if (File.Exists(RevisionDownloadPath))
					File.Delete(RevisionDownloadPath);
				Program.DeleteDirectory(StagingDirectory);

				var client = new WebClient();
				var vi = (VersionInfo)param;
				SendMessage(String.Format("BYOND: Updating to version {0}.{1}...", vi.major, vi.minor));
				client.DownloadFile(String.Format(ByondRevisionsURL, vi.major, vi.minor), RevisionDownloadPath);

				lock (ByondLock)
				{
					updateStat = TGByondStatus.Staging;
				}

				ZipFile.ExtractToDirectory(RevisionDownloadPath, StagingDirectory);
				lock (ByondLock)
				{
					File.WriteAllText(StagingDirectoryInner + VersionFile, String.Format("{0}.{1}", vi.major, vi.minor));
				}
				File.Delete(RevisionDownloadPath);

				lock (ByondLock)
				{
					updateStat = TGByondStatus.Staged;
				}

				var stat = DaemonStatus();
				switch (stat)
				{
					case TGDreamDaemonStatus.Offline:
						lastError = "Failed to apply update!";
						if(ApplyStagedUpdate())
							lastError = null;
						break;
					default:
						lastError = "Awaiting server restart...";
						SendMessage(String.Format("BYOND: Staging complete. Awaiting server restart...", vi.major, vi.minor));
						break;
				}
			}
			catch (ThreadAbortException)
			{
				return;
			}
			catch (Exception e)
			{
				TGServerService.ActiveService.EventLog.WriteEntry("Revision staging errror: " + e.ToString(), EventLogEntryType.Error);
				lock (ByondLock)
				{
					updateStat = TGByondStatus.Idle;
					lastError = e.ToString();
					RevisionStaging = null;
				}
			}
		}
		public bool UpdateToVersion(int ma, int mi)
		{
			lock (ByondLock)
			{
				if (!BusyCheckNoLock())
				{
					updateStat = TGByondStatus.Starting;
					RevisionStaging = new Thread(new ParameterizedThreadStart(UpdateToVersionImpl))
					{
						IsBackground = true //don't slow me down
					};
					RevisionStaging.Start(new VersionInfo { major = ma, minor = mi });
					return true;
				}
				return false; 
			}
		}

		public bool ApplyStagedUpdate()
		{
			if (Compiling())
				return false;
			lock (CompilerLock)
			{
				lock (ByondLock)
				{

					if (updateStat != TGByondStatus.Staged)
						return false;
					try
					{
						//IMPORTANT: SET THE BYOND CONFIG TO NOT PROMPT FOR TRUSTED MODE REEE
						var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + ByondConfigDir;
						Directory.CreateDirectory(dir);
						File.WriteAllText(dir + ByondDDConfig, ByondNoPromptTrustedMode);

						Program.DeleteDirectory(ByondDirectory);
						Directory.Move(StagingDirectoryInner, ByondDirectory);
						Directory.Delete(StagingDirectory, true);
						lastError = null;
						SendMessage("BYOND: Update completed!");
						return true;
					}
					catch (Exception e)
					{
						lastError = e.ToString();
						SendMessage("BYOND: Update failed!");
						return false;
					}
					finally
					{
						updateStat = TGByondStatus.Idle;
					}
				}
			}
		}
	}
}
