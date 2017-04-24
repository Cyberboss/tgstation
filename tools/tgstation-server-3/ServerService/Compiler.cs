using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using TGServiceInterface;

namespace TGServerService
{
	partial class TGStationServer : ITGCompiler
	{
		#region Win32 Shit
		[DllImport("kernel32.dll")]
		static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);
		enum SymbolicLink
		{
			File = 0,
			Directory = 1
		}
		#endregion

		const string StaticDirs = "Static";
		const string StaticDataDir = StaticDirs + "/data";
		const string StaticConfigDir = StaticDirs + "/config";
		const string StaticLogDir = StaticDirs + "/logs";
		const string StaticBackupDir = "Static_BACKUP";

		const string LibMySQLFile = "/libmysql.dll";

		const string GameDir = "Game";
		const string GameDirA = GameDir + "/A";
		const string GameDirB = GameDir + "/B";
		const string GameDirLive = GameDir + "/Live";

		const string LiveFile = "/TestLive.lk";
		const string ADirTest = GameDirA + LiveFile;
		const string BDirTest = GameDirB + LiveFile;
		const string LiveDirTest = GameDirLive + LiveFile;

		object CompilerLock = new object();
		object LiveDirCheckLock = new object();
		object CompilerThreadLock = new object();

		List<string> copyExcludeList = new List<string> { ".git", "data", "config", "libmysql.dll" };	//shit we handle

		bool compiledSucessfully = false;
		
		Thread CompilerThread;
	
		void InitCompiler()
		{
			if(File.Exists(LiveDirTest))
				File.Delete(LiveDirTest);
		}

		void DisposeCompiler()
		{
			lock (CompilerThreadLock)
			{
				if (CompilerThread == null)
					return;
				CompilerThread.Abort(); //this will safely kill dm
			}
		}

		void CreateSymlink(string link, string target)
		{
			if (!CreateSymbolicLink(new DirectoryInfo(link).FullName, new DirectoryInfo(target).FullName, File.Exists(target) ? SymbolicLink.File : SymbolicLink.Directory))
				throw new Exception(String.Format("Failed to create symlink from {0} to {1}!", target, link));
		}

		public string Initialize()
		{
			if (DaemonStatus() != TGDreamDaemonStatus.Offline)
				return "Dream daemon must not be running";
			lock (CompilerLock)
			{
				if (!Exists())  //repo
					return "Repository is not setup!";
				try
				{
					SendMessage("DM: Setting up symlinks...");
					if(Directory.Exists(GameDirLive))
						Directory.Delete(GameDirLive);
					Program.DeleteDirectory(GameDir);
					Directory.CreateDirectory(GameDirA + "/.git/logs");

					if (!Monitor.TryEnter(RepoLock))
						return "Unable to lock repository!";
					try
					{
						Program.CopyDirectory(RepoPath, GameDirA, copyExcludeList);
						//just the tip
						const string HeadFile = "/.git/logs/HEAD";
						File.Copy(RepoPath + HeadFile, GameDirA + HeadFile);
					}
					finally
					{
						Monitor.Exit(RepoLock);
					}

					Program.CopyDirectory(GameDirA, GameDirB);

					CreateSymlink(GameDirA + "/data", StaticDataDir);
					CreateSymlink(GameDirB + "/data", StaticDataDir);

					CreateSymlink(GameDirA + "/config", StaticConfigDir);
					CreateSymlink(GameDirB + "/config", StaticConfigDir);

					CreateSymlink(GameDirA + LibMySQLFile, StaticDirs + LibMySQLFile);
					CreateSymlink(GameDirB + LibMySQLFile, StaticDirs + LibMySQLFile);

					CreateSymlink(GameDirLive, GameDirA);

					Program.Shell("pip install PyYaml");
					Program.Shell("pip install beautifulsoup4");

					SendMessage("DM: Symlinks set up!");

					return null;
				}
				catch (Exception e)
				{
					SendMessage("DM: Setup failed!");
					return e.ToString();
				}
			}
		}

		public bool Compiling()
		{
			if (Monitor.TryEnter(CompilerLock))
			{
				Monitor.Exit(CompilerLock);
				return false;
			}
			return true;
		}

		public bool Compiled()
		{
			return !Compiling() && compiledSucessfully;
		}

		string GetStagingDir()
		{
			string TheDir;
			//LiveDirTest = Game/Live/LiveCheck.lk
			File.Create(LiveDirTest).Close();
			try
			{
				if (File.Exists(ADirTest))
					TheDir = GameDirA;
				else if (File.Exists(BDirTest))
					TheDir = GameDirB;
				else
					throw new Exception("Unable to determine current live directory!");
			}
			finally
			{
				File.Delete(LiveDirTest);
			}

			TheDir = InvertDirectory(TheDir);

			//So TheDir is what the Live folder is NOT pointing to
			//Now we need to check if DD is running that folder and swap it if necessary

			var rsclock = TheDir + "/" + Properties.Settings.Default.ProjectName + ".rsc.lk";
			if (File.Exists(rsclock))
			{
				try
				{
					File.Delete(rsclock);
				}
				catch	//held open by byond
				{
					return InvertDirectory(TheDir);
				}
			}
			return TheDir;
		}
		string InvertDirectory(string gameDirectory)
		{
			if (gameDirectory == GameDirA)
				return GameDirB;
			else
				return GameDirA;
		}
		void CompileImpl()
		{
			try
			{
				lock (CompilerLock)
				{
					SendMessage("DM: Starting compilation...");
					compiledSucessfully = false;
					var resurrectee = GetStagingDir();

					//clear out the syms first
					if(Directory.Exists(resurrectee + "/data"))
						Directory.Delete(resurrectee + "/data");
					if (Directory.Exists(resurrectee + "/config"))
						Directory.Delete(resurrectee + "/config");
					if(File.Exists(resurrectee + LibMySQLFile))
						File.Delete(resurrectee + LibMySQLFile);

					Program.DeleteDirectory(resurrectee);

					Directory.CreateDirectory(resurrectee);

					CreateSymlink(resurrectee + "/data", StaticDataDir);
					CreateSymlink(resurrectee + "/config", StaticConfigDir);
					CreateSymlink(resurrectee + LibMySQLFile, StaticDirs + LibMySQLFile);

					Directory.CreateDirectory(resurrectee + "/.git/logs");

					if (!Monitor.TryEnter(RepoLock))
						return;
					try
					{
						Program.CopyDirectory(RepoPath, resurrectee, copyExcludeList);
						//just the tip
						const string HeadFile = "/.git/logs/HEAD";
						File.Copy(RepoPath + HeadFile, resurrectee + HeadFile);
					}
					finally
					{
						Monitor.Exit(RepoLock);
					}

					using (var DM = new Process())  //will kill the process if the thread is terminated
					{
						DM.StartInfo.FileName = ByondDirectory + "/bin/dm.exe";
						DM.StartInfo.Arguments = new DirectoryInfo(resurrectee).FullName + "/" + Properties.Settings.Default.ProjectName + ".dme";
						DM.Start();
						DM.WaitForExit();
						compiledSucessfully = DM.ExitCode == 0;
					}

					if (compiledSucessfully)
					{
						//these two lines should be atomic but this is the best we can do
						Directory.Delete(GameDirLive);
						CreateSymlink(GameDirLive, resurrectee);

						SendMessage("DM: Compile complete, server will update next round!");
					}
					else
						SendMessage("DM: Compile failed!");
				}
			}
			catch (ThreadAbortException)
			{
				return;
			}
			catch (Exception e)
			{
				SendMessage("DM: Compiler thread crashed!");
				TGServerService.ActiveService.EventLog.WriteEntry("Compile manager errror: " + e.ToString(), EventLogEntryType.Error);
			}
			finally
			{
				lock (CompilerThreadLock)
				{
					CompilerThread = null;
				}
			}
		}

		public bool Compile()
		{
			if(GetVersion(false) == null)
				return false;

			lock (CompilerThreadLock)
			{
				if (CompilerThread != null)
					return false;
				CompilerThread = new Thread(new ThreadStart(CompileImpl));
				CompilerThread.Start();
			}
			return true;
		}
	}
}
