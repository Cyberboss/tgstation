using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using TGServiceInterface;

namespace TGServerService
{
	class Byond : ITGByond
	{
		const string ByondDirectory = "C:\\tgstation-server-3\\BYOND";
		const string StagingDirectory = "C:\\tgstation-server-3\\BYOND_staged";
		const string RevisionDownloadPath = "C:\\tgstation-server-3\\BYONDRevision.zip";
		const string ByondRevisionsURL = "https://secure.byond.com/download/build/{0}/{0}.{1}_byond.zip";
		enum UpdateStatus
		{
			None,
			Downloading,
			Staging,
			Staged,
			Updating,
		}
		UpdateStatus updateStat = UpdateStatus.None;
		object ByondLock = new object();
		string lastError = null;

		bool BusyCheckNoLock()
		{
			switch (updateStat)
			{
				default:
				case UpdateStatus.Downloading:
				case UpdateStatus.Staging:
				case UpdateStatus.Updating:
					return true;
				case UpdateStatus.None:
				case UpdateStatus.Staged:
					return false;
			}
		}

		public bool IsBusy()
		{
			lock (ByondLock)
			{
				return BusyCheckNoLock();
			}
		}
		public int GetProgress()
		{
			lock (ByondLock)
			{
				switch (updateStat)
				{
					case UpdateStatus.None:
						return 0;
					case UpdateStatus.Downloading:
						return 33;
					case UpdateStatus.Updating:
						return 66;
					default:
						return 100;
				}
			}
		}

		public string GetError()
		{
			return lastError;
		}

		class VersionInfo
		{
			public int major, minor;
		}

		public void UpdateToVersionImpl(object param)
		{
			lock (ByondLock) { 
				if (BusyCheckNoLock())
					return;
				updateStat = UpdateStatus.Downloading;
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
				client.DownloadFile(string.Format(ByondRevisionsURL, vi.major, vi.minor), RevisionDownloadPath);

				lock (ByondLock)
				{
					updateStat = UpdateStatus.Staging;
				}

				ZipFile.ExtractToDirectory(RevisionDownloadPath, StagingDirectory);

				lock (ByondLock)
				{
					updateStat = UpdateStatus.Staged;
				}

				if (false)  //TODO: Some way to check if the server is running
					ApplyStagedUpdate();
				else
					lastError = "Awaiting server restart...";
			}
			catch (Exception e)
			{
				lock (ByondLock)
				{
					updateStat = UpdateStatus.None;
					lastError = e.ToString();
				}
			}
		}
		public void UpdateToVersion(int ma, int mi)
		{
			new Thread(new ParameterizedThreadStart(UpdateToVersionImpl)).Start(new VersionInfo { major = ma, minor = mi  });
		}

		public void ApplyStagedUpdate()
		{
			lock (ByondLock)
			{
				if (updateStat != UpdateStatus.Staged)
					return;
				try
				{
					if (Directory.Exists(ByondDirectory))
						Directory.Delete(ByondDirectory, true);
					Directory.Move(StagingDirectory, ByondDirectory);
					lastError = null;
				}
				catch(Exception e)
				{
					lastError = e.ToString();
				}
				updateStat = UpdateStatus.None;
			}
		}
	}
}
