using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TGServiceInterface;

namespace TGCommandLine
{
	//if i was a smart man i would've made this modular before it got out of hand
	//but i didn't
	//help i cant stop
	class Program
	{
		enum ExitCode
		{
			Normal = 0,
			ConnectionError = 1,
			BadCommand = 2,
			ServerError = 3,
		}
		static bool SpecialTactics()
		{
			return false;
#pragma warning disable CS0162 // Unreachable code detected


			//Use this proc for one off testing


			var result = Server.GetComponent<ITGConfig>().Retrieve(TGConfigType.Game, out string error);
			return true;





#pragma warning restore CS0162 // Unreachable code detected
		}
		static ExitCode RunCommandLine(string[] args)
		{
			if (SpecialTactics())
				return ExitCode.Normal;

			string command = null, param1 = null, param2 = null;
			if (args.Length > 0)
				command = args[0].Trim().ToLower();

			if (args.Length > 1)
				param1 = args[1].Trim().ToLower();

			if (args.Length > 2)
				param2 = args[2].Trim();

			var res = Server.VerifyConnection();
			if (res != null)
			{
				Console.WriteLine("Unable to connect to service!");
				return ExitCode.ConnectionError;
			}

			try
			{
				switch (command)
				{
					case "irc":
						return IRCCommand(param1, param2);
					case "byond":
						return BYONDCommand(param1, param2);
					case "dm":
						return DMCommand(param1, param2);
					case "dd":
						return DDCommand(param1, param2);
					case "repo":
						return RepoCommand(param1, param2);
					case "config":
						return ConfigCommand(param1, param2);
					case "update":
						if(param1 == null)
						{
							Console.WriteLine("Missing parameter!");
							return ExitCode.BadCommand;
						}
						var gen_cl = param2 != null && param2.ToLower() == "--cl";
						TGRepoUpdateMethod method;
						switch (param1.ToLower())
						{
							case "hard":
								method = TGRepoUpdateMethod.Hard;
								break;
							case "merge":
								method = TGRepoUpdateMethod.Merge;
								break;
							default:
								Console.WriteLine("Please specify hard or merge");
								return ExitCode.BadCommand;
						}
						var result = Server.GetComponent<ITGServerUpdater>().UpdateServer(method, gen_cl);
						Console.WriteLine(result ?? "Success!");
						if (result != null)
							return ExitCode.ServerError;
						break;
					case "testmerge":
						if (param1 == null)
						{
							Console.WriteLine("Missing parameter!");
							return ExitCode.BadCommand;
						}
						ushort tm;
						try {
							tm = Convert.ToUInt16(param1);
							if (tm == 0)
								throw new Exception();
						}
						catch
						{
							Console.WriteLine("Invalid tesmerge #: " + param1);
							return ExitCode.BadCommand;
						}
						result = Server.GetComponent<ITGServerUpdater>().UpdateServer(TGRepoUpdateMethod.None, false, tm);
						Console.WriteLine(result ?? "Success!");
						if(result != null)
							return ExitCode.ServerError;
						break;
					case "?":
					case "help":
						ConsoleHelp();
						break;
					default:
						Console.WriteLine("Invalid command: " + command);
						Console.WriteLine("Type '?' or 'help' for available commands.");
						return ExitCode.BadCommand;
				}
			}
			catch
			{
				Console.WriteLine("Connection interrupted!");
				return ExitCode.ConnectionError;
			}
			return ExitCode.Normal;
		}
		static string ReadLineSecure()
		{
			string result = "";
			while (true)
			{
				ConsoleKeyInfo i = Console.ReadKey(true);
				if (i.Key == ConsoleKey.Enter)
				{
					break;
				}
				else if (i.Key == ConsoleKey.Backspace)
				{
					if (result.Length > 0)
					{
						result = result.Substring(0, result.Length - 1);
						Console.Write("\b \b");
					}
				}
				else
				{
					result += i.KeyChar;
					Console.Write("*");
				}
			}
			return result;
		}
		static ExitCode RepoCommand(string command, string param)
		{
			var Repo = Server.GetComponent<ITGRepository>();
			switch (command)
			{
				case "setup":
					if (param == null)
					{
						Console.WriteLine("Missing parameter!");
						return ExitCode.BadCommand;
					}
					var splits = param.Split(';');
					if (!Repo.Setup(splits[0], splits.Length > 1 ? splits[1] : "master"))
					{
						Console.WriteLine("Error: Repo is busy!");
						return ExitCode.ServerError;
					}
					Console.Write("Setting up repo. This will take a while...");
					break;
				case "update":
					if (param == null)
					{
						Console.WriteLine("Missing parameter!");
						return ExitCode.BadCommand;
					}
					string res;
					switch (param.ToLower())
					{
						case "hard":
							res = Repo.Update(true);
							break;
						case "merge":
							res = Repo.Update(false);
							break;
						default:
							Console.WriteLine("Invalid parameter: " + param);
							return ExitCode.BadCommand;
					}
					Console.WriteLine(res ?? "Success");
					if (res != null)
						return ExitCode.ServerError;
					break;
				case "gen-changelog":
					var result = Repo.GenerateChangelog(out string error);
					Console.WriteLine(error ?? "Success!");
					if (result != null)
						Console.WriteLine(result);
					if (error != null)
						return ExitCode.ServerError;
					break;
				case "commit":
					if(param == null)
					{
						Console.WriteLine("Missing parameter!");
						return ExitCode.BadCommand;
					}
					res = Repo.Commit(param);
					Console.WriteLine(res ?? "Success!");
					if (res != null)
						return ExitCode.ServerError;
					break;
				case "push":
					res = Repo.Push();
					Console.WriteLine(res ?? "Success!");
					if (res != null)
						return ExitCode.ServerError;
					break;
				case "set-email":
					if (param == null)
					{
						Console.WriteLine("Missing parameter!");
						return ExitCode.BadCommand;
					}
					Repo.SetCommitterEmail(param);
					break;
				case "set-name":
					if (param == null)
					{
						Console.WriteLine("Missing parameter!");
						return ExitCode.BadCommand;
					}
					Repo.SetCommitterName(param);
					break;
				case "set-credentials":
					Console.WriteLine("Enter username:");
					var user = Console.ReadLine();
					Console.WriteLine("Enter password:");
					var pass = ReadLineSecure();
					Repo.SetCredentials(user, pass);
					break;
				case "python-path":
					if (param == null)
					{
						Console.WriteLine("Missing parameter!");
						return ExitCode.BadCommand;
					}
					var found = Repo.SetPythonPath(param);
					Console.WriteLine(found ? "Success!" : "Error: Directory does not exist!");
					if (!found)
						return ExitCode.ServerError;
					break;
				case "?":
				case "help":
					Console.WriteLine("Repo commands:");
					Console.WriteLine();
					Console.WriteLine("setup <git-url>[;branchname]\t-\tClean up everything and clones the repo at git-url with optional branch name");
					Console.WriteLine("update <hard|merge>\t-\tUpdates the current branch the repo is on either via a merge or hard reset");
					Console.WriteLine("gen-changelog\t-\tCompiles the game changelog");
					Console.WriteLine("commit <message>\t-\tCommits all current changes to the repository using the configured identity");
					Console.WriteLine("push\t-\tPushes commits to the origin branch using the configured credentials");
					Console.WriteLine("set-email <e-mail>\t-\tSet the e-mail used for commits");
					Console.WriteLine("set-name <name>\t-\tSet the name used for commits");
					Console.WriteLine("set-credentials\t-\tSet the credentials used for pushing commits");
					break;
				default:
					Console.WriteLine("Invalid command: " + command);
					Console.WriteLine("Type 'repo help' for available commands.");
					return ExitCode.BadCommand;
			}
			return ExitCode.Normal;
		}
		static ExitCode ConfigCommand(string command, string param)
		{
			var Config = Server.GetComponent<ITGConfig>();
			switch (command)
			{
				case "move-server":
					if(param == null)
					{
						Console.WriteLine("Missing parameter!");
						return ExitCode.BadCommand;
					}
					var res = Config.MoveServer(param);
					Console.WriteLine(res ?? "Success!");
					if (res != null)
						return ExitCode.ServerError;
					break;
				case "server-dir":
					Console.WriteLine(Config.ServerDirectory());
					break;
				case "?":
				case "help":
					Console.WriteLine("Config commands:");
					Console.WriteLine();
					Console.WriteLine("move-server <new-path>\t-\tMove the server installation (BYOND, Repo, Game) to a new location. Nothing else may be running for this task to complete");
					Console.WriteLine("server-dir\t-\tPrint the directory the server is installed in");
					break;
				default:
					Console.WriteLine("Invalid command: " + command);
					Console.WriteLine("Type 'config help' for available commands.");
					return ExitCode.BadCommand;
			}
			return ExitCode.Normal;
		}
		static ExitCode BYONDCommand(string command, string param)
		{
			var BYOND = Server.GetComponent<ITGByond>();
			switch (command)
			{
				case "update":
					if (param == null)
					{
						Console.WriteLine("Missing parameter!");
						return ExitCode.BadCommand;
					}
					var splits = param.Split('.');
					var failed = splits.Length != 2;
					int Major = 0, Minor = 0;
					if (!failed)
					{
						try
						{
							Major = Convert.ToInt32(splits[0]);
							Minor = Convert.ToInt32(splits[1]);
						}
						catch
						{
							failed = true;
						}
					}
					if (failed)
					{

						Console.WriteLine("Please enter version as <Major>.<Minor>");
						return ExitCode.BadCommand;
					}
					if (!BYOND.UpdateToVersion(Major, Minor))

					{
						Console.WriteLine("Failed to begin update!");
						return ExitCode.ServerError;
					}
					var stat = BYOND.CurrentStatus();
					while (stat != TGByondStatus.Idle && stat != TGByondStatus.Staged)
					{
						Thread.Sleep(100);
						stat = BYOND.CurrentStatus();
					}
					var res = BYOND.GetError();
					if(res != null)
					{
						Console.WriteLine("Error: " + res);
						return ExitCode.ServerError;
					}
					Console.WriteLine(stat == TGByondStatus.Staged ? "Update staged and will apply next DD reboot": "Update finished"); 
					break;
				case "version":
					Console.WriteLine(BYOND.GetVersion(param == "--staged") ?? "Unistalled");
					break;
				case "?":
				case "help":
					Console.WriteLine("BYOND commands:");
					Console.WriteLine();
					Console.WriteLine("version [--staged]\t-\tPrint the currently installed BYOND version");
					Console.WriteLine("update <Major>.<Minor>\t-\tUpdates the BYOND installation to the specified version");
					break;
				default:
					Console.WriteLine("Invalid command: " + command);
					Console.WriteLine("Type 'byond help' for available commands.");
					return ExitCode.BadCommand;
			}
			return ExitCode.Normal;
		}
		static ExitCode DDCommand(string command, string param)
		{
			var DD = Server.GetComponent<ITGDreamDaemon>();

			switch (command)
			{
				case "start":
					var res = DD.Start();
					if (res != null)
					{
						Console.WriteLine("Failed to start: " + res);
						return ExitCode.ServerError;
					}
					break;
				case "stop":
					DD.Stop();
					break;
				case "restart":
					res = DD.Restart();
					if (res != null)
					{
						Console.WriteLine("Failed to restart: " + res);
						return ExitCode.ServerError;
					}
					break;
				case "autostart":
					if (param == null)
					{
						Console.WriteLine("Missing parameter!");
						return ExitCode.BadCommand;
					}
					switch (param.ToLower()) {
						case "on":
							DD.SetAutostart(true);
							break;
						case "off":
							DD.SetAutostart(false);
							break;
						case "check":
							Console.WriteLine(DD.Autostart() ? "Autostart is on" : "Autostart is off");
							break;
						default:
							Console.WriteLine("Please enter on, off, or check");
							return ExitCode.BadCommand;
					}
					break;
				case "?":
				case "help":
					Console.WriteLine("DD commands:");
					Console.WriteLine();
					Console.WriteLine("start\t-\tStarts the server and watchdog");
					Console.WriteLine("stop\t-\tStops the server and watchdog");
					Console.WriteLine("restart\t-\tRestarts the server and watchdog");
					Console.WriteLine("autostart <on|off|check>\t-\tChange or check autostarting of the game server");
					break;
				default:
					Console.WriteLine("Invalid command: " + command);
					Console.WriteLine("Type 'dd help' for available commands.");
					return ExitCode.BadCommand;
			}
			return ExitCode.Normal;
		}
		static ExitCode DMCommand(string command, string param)
		{
			var DM = Server.GetComponent<ITGCompiler>();
			switch (command)
			{
				case "compile":
					var stat = DM.GetStatus();
					if (stat != TGCompilerStatus.Initialized)
					{
						Console.WriteLine("Error: Compiler is " + ((stat == TGCompilerStatus.Uninitialized) ? "unintialized!" : "busy with another task!"));
						return ExitCode.ServerError;
					}
					
					if(Server.GetComponent<ITGByond>().GetVersion(false) == null)
					{
						Console.Write("Error: BYOND is not installed!");
						return ExitCode.ServerError;
					}

					if (!DM.Compile())
					{
						Console.WriteLine("Error: Unable to start compilation! If the game directory is not initialized try running 'dm initialize'");
						var err = DM.CompileError();
						if (err != null)
							Console.WriteLine(err);
						return ExitCode.ServerError;
					}
					Console.WriteLine("Compile job started");
					if(param == "--wait")
					{
						Console.WriteLine("Awaiting completion...");
						do
						{
							Thread.Sleep(1000);
						} while (DM.GetStatus() == TGCompilerStatus.Compiling);
						var res = DM.CompileError();
						Console.WriteLine(res ?? "Compilation successful");
						if (res != null)
							return ExitCode.ServerError;
					}
					break;
				case "initialize":
					stat = DM.GetStatus();
					if (stat == TGCompilerStatus.Compiling || stat == TGCompilerStatus.Initializing)
					{
						Console.WriteLine("Error: Compiler is " + ((stat == TGCompilerStatus.Initializing) ? "already initialized!" : " already running!"));
						return ExitCode.ServerError;
					}
					if (!DM.Initialize())
					{
						Console.WriteLine("Error: Unable to start initialization! If the game directory is not initialized try running 'dm initialize'");
						var err = DM.CompileError();
						if (err != null)
							Console.WriteLine(err);
						return ExitCode.ServerError;
					}
					Console.WriteLine("Initialize job started");
					if (param == "--wait")
					{
						Console.WriteLine("Awaiting completion...");
						do
						{
							Thread.Sleep(1000);
						} while (DM.GetStatus() == TGCompilerStatus.Initializing);
						var res = DM.CompileError();
						Console.WriteLine(res ?? "Initialization successful");
						if (res != null)
							return ExitCode.ServerError;
					}
					break;
				case "?":
				case "help":
					Console.WriteLine("DM commands:");
					Console.WriteLine();
					Console.WriteLine("initialize [--wait]\t-\tStarts an initialization job optionally waiting for completion");
					Console.WriteLine("compile [--wait]\t-\tStarts a compile/update job optionally waiting for completion");
					break;
				default:
					Console.WriteLine("Invalid command: " + command);
					Console.WriteLine("Type 'dm help' for available commands");
					return ExitCode.BadCommand;
			}
			return ExitCode.Normal;
		}

		static ExitCode IRCCommand(string command, string param)
		{
			var IRC = Server.GetComponent<ITGIRC>();
			switch (command)
			{
				case "nick":
					if(param == null)
					{
						Console.WriteLine("Missing parameter!");
						return ExitCode.BadCommand;
					}
					IRC.Setup(null, 0, param);
					break;
				case "join":
					if (param == null)
					{
						Console.WriteLine("Missing parameter!");
						return ExitCode.BadCommand;
					}
					var channels = IRC.Channels();
					var found = false;
					var lowerParam = param.ToLower();
					foreach (var I in channels)
					{
						if (I.ToLower() == lowerParam)
						{
							found = true;
							break;
						}
					}
					if (!found)
					{
						Array.Resize(ref channels, channels.Length + 1);
						channels[channels.Length - 1] = param;
					}
					IRC.Setup(null, 0, null, channels);
					break;
				case "part":
					if (param == null)
					{
						Console.WriteLine("Missing parameter!");
						return ExitCode.BadCommand;
					}
					channels = IRC.Channels();
					lowerParam = param.ToLower();
					var new_channels = new List<string>();
					foreach (var I in channels)
					{
						if (I.ToLower() == lowerParam)
							continue;
						new_channels.Add(I);
					}
					if(new_channels.Count == 0)
					{
						Console.WriteLine("Error: Cannot part from the last channel!");
						return ExitCode.BadCommand;
					}
					IRC.Setup(null, 0, null, new_channels.ToArray());
					break;
				case "announce":
					if (param == null)
					{
						Console.WriteLine("Missing parameter!");
						return ExitCode.BadCommand;
					}
					var res = IRC.SendMessage("SCP: " + param);
					if(res != null)
					{
						Console.WriteLine("Error: " + res);
						return ExitCode.ServerError;
					}
					break;
				case "channels":
					Console.WriteLine("Currently configured channels:");
					Console.WriteLine("\tAdmin Channel: " + IRC.AdminChannel());
					foreach (var I in IRC.Channels())
						Console.WriteLine("\t" + I);
					break;
				case "enable":
					IRC.Setup(null, 0, null, null, null, TGIRCEnableType.Enable);
					IRC.Connect();
					break;
				case "disable":
					IRC.Setup(null, 0, null, null, null, TGIRCEnableType.Disable);
					break;
				case "admin":
					if (param == null)
					{
						Console.WriteLine("Missing parameter!");
						return ExitCode.BadCommand;
					}
					IRC.Setup(null, 0, null, null, param);
					break;
				case "?":
				case "help":
					Console.WriteLine("IRC commands:");
					Console.WriteLine();
					Console.WriteLine("nick <name>\t-\tSets the IRC nickname");
					Console.WriteLine("join <channel>\t-\tJoins a channel");
					Console.WriteLine("part <channel>\t-\tLeaves a channel");
					Console.WriteLine("announce <message>\t-\tSends a message to all connected channels");
					Console.WriteLine("channels\t-\tList configured channels");
					Console.WriteLine("enable\t-\tEnables the IRC bot");
					Console.WriteLine("disable\t-\tDisables the IRC bot");
					Console.WriteLine("admin\t-\tSets the admin IRC channel");
					break;
				default:
					Console.WriteLine("Invalid command: " + command);
					Console.WriteLine("Type 'irc help' for available commands");
					return ExitCode.BadCommand;
			}
			return ExitCode.Normal;
		}

		static void ConsoleHelp()
		{
			Console.WriteLine("/tg/station 13 Server Control Panel:");
			Console.WriteLine("Avaiable commands (type 'help' after command for more info):");
			Console.WriteLine();
			Console.WriteLine("irc\t-\tManage the IRC user");
			Console.WriteLine("repo\t-\tManage the git repository"); ;
			Console.WriteLine("byond\t-\tManage BYOND installation");
			Console.WriteLine("dm\t-\tManage compiling the server");
			Console.WriteLine("dd\t-\tManage DreamDaemon");
			Console.WriteLine("config\t-\tManage settings");
			Console.WriteLine("update <merge|hard> [--cl]\t-\tUpdates the server fully, optionally generating and pushing a changelog");
			Console.WriteLine("testmerge <pull request #>\t-\tMerges the specified pull request and updates the server");
		}

		static int Main(string[] args)
		{
			if(args.Length != 0)
				return (int)RunCommandLine(args);

			//interactive mode
			do
			{
				try
				{
					Console.Write("Enter command:\t");
					var NextCommand = Console.ReadLine();
					switch (NextCommand.ToLower())
					{
						case "quit":
						case "exit":
							return (int)ExitCode.Normal;
						default:
							var formattedCommand = new List<string>(NextCommand.Split(' '));
							formattedCommand = formattedCommand.Select(x => x.Trim()).ToList();
							formattedCommand.Remove("");
							RunCommandLine(formattedCommand.ToArray());
							break;
					}
				}
				catch
				{
					break;
				}
			}
			while (Server.VerifyConnection() == null);
			Console.WriteLine("Connection to service interrupted!");
			return (int)ExitCode.ConnectionError;
		}
	}
}