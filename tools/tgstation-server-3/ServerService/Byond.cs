using System;
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

		TGByondStatus updateStat = TGByondStatus.Idle;
		object ByondLock = new object();
		string lastError = null;

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
				if (Directory.Exists(StagingDirectory))
					Directory.Delete(StagingDirectory, true);

				var client = new WebClient();
				var vi = (VersionInfo)param;
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

				if (true)  //TODO: Some way to check if the server is running
					ApplyStagedUpdate();
				else
					lastError = "Awaiting server restart...";
			}
			catch (Exception e)
			{
				lock (ByondLock)
				{
					updateStat = TGByondStatus.Idle;
					lastError = e.ToString();
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
					new Thread(new ParameterizedThreadStart(UpdateToVersionImpl)).Start(new VersionInfo { major = ma, minor = mi });
					return true;
				}
				return false; 
			}
		}

		public void ApplyStagedUpdate()
		{
			lock (ByondLock)
			{
				if (updateStat != TGByondStatus.Staged)
					return;
				try
				{
					if (Directory.Exists(ByondDirectory))
						Directory.Delete(ByondDirectory, true);
					Directory.Move(StagingDirectoryInner, ByondDirectory);
					Directory.Delete(StagingDirectory, true);
					lastError = null;
				}
				catch(Exception e)
				{
					lastError = e.ToString();
				}
				updateStat = TGByondStatus.Idle;
			}
		}
	}
}
