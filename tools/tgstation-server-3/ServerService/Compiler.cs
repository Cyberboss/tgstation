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

		bool compiledSucessfully = false;

		TGStationServer()
		{
			File.Delete(LiveDirTest);
		}

		void CreateSymlink(string link, string target)
		{
			if (!CreateSymbolicLink(link, target, File.Exists(target) ? SymbolicLink.File : SymbolicLink.Directory))
				throw new Exception(String.Format("Failed to create symlink from {0} to {1}!", target, link));
		}

		public string Initialize()
		{
			lock (CompilerLock)
			{
				if (!Exists())  //repo
					return "Repository is not setup!";
				try
				{
					Program.DeleteDirectory(GameDir);
					Directory.CreateDirectory(GameDirA + ".git/logs");

					if (!Monitor.TryEnter(RepoLock))
						return "Unable to lock repository!";
					try
					{
						Program.CopyDirectory(RepoPath, GameDirA, new List<string> { ".git" });
						//just the tip
						const string HeadFile = "/.git/logs/HEAD";
						File.Copy(RepoPath + HeadFile, GameDirA + HeadFile);
					}
					finally
					{
						Monitor.Exit(RepoLock);
					}

					Program.CopyDirectory(GameDirA, GameDirB);

					CreateSymlink(StaticDataDir, GameDirA + "/data");
					CreateSymlink(StaticDataDir, GameDirB + "/data");

					CreateSymlink(StaticConfigDir, GameDirA + "/config");
					CreateSymlink(StaticConfigDir, GameDirB + "/config");

					CreateSymlink(GameDirLive, GameDirA);

					Program.Shell("pip install PyYaml");
					Program.Shell("pip install beautifulsoup4");

					return null;
				}
				catch (Exception e)
				{
					return e.ToString();
				}
			}
		}

		public bool Compiling()
		{
			if (Monitor.TryEnter(CompilerLock))
			{
				Monitor.Exit(CompilerLock);
				return true;
			}
			return false;
		}

		public bool Compiled()
		{
			return !Compiling() && compiledSucessfully;
		}

		string GetLiveDir()
		{
			string TheDir;
			File.Create(LiveDirTest);
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

			var rsclock = TheDir + Properties.Settings.Default.ProjectName + ".rsc.lk";
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
		string GetDeadDir()
		{
			return InvertDirectory(GetLiveDir());
		}

		void CompileImpl()
		{
			lock (CompilerLock)
			{
				compiledSucessfully = false;
				var DM = new Process();
				DM.StartInfo.FileName = ByondDirectory + "/bin/dm.exe";
				var resurrectee = GetDeadDir();
				DM.StartInfo.Arguments = new DirectoryInfo(resurrectee).FullName + Properties.Settings.Default.ProjectName + ".dme";
				DM.Start();
				DM.WaitForExit();
				compiledSucessfully = DM.ExitCode == 0;
				if (compiledSucessfully)
				{
					Directory.Delete(GameDirLive);
					CreateSymlink(GameDirLive, resurrectee);
				}
			}
		}

		public bool Compile()
		{
			if (Compiling())
				return false;

			var t = new Thread(new ThreadStart(CompileImpl))
			{
				IsBackground = true //don't slow me down
			};
			t.Start();
			return true;
		}
	}
}
