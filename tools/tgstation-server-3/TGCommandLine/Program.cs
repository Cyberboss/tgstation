using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TGServiceInterface;

namespace TGCommandLine
{
	enum ExitCode
	{
		Normal = 0,
		ConnectionError = 1,
		BadCommand = 2,
		ServerError = 3,
	}

	abstract class Command
	{
		public string Keyword { get; protected set; }
		public Command[] Children { get; protected set; } = { };
		public int RequiredParameters { get; protected set; }
		public abstract ExitCode Run(IList<string> parameters);
		public virtual void PrintHelp()
		{
			foreach (var c in Children)
				c.PrintHelp();
		}
	}

	class Program
	{
		static ExitCode RunCommandLine(IList<string> argsAsList)
		{
			var res = Server.VerifyConnection();
			if (res != null)
			{
				Console.WriteLine("Unable to connect to service!");
				return ExitCode.ConnectionError;
			}
			try {
				return new RootCommand().Run(argsAsList);
			}
			catch
			{
				Console.WriteLine("Service connection interrupted!");
				return ExitCode.ConnectionError;
			};
		}
		public static string ReadLineSecure()
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
		
		static int Main(string[] args)
		{
			if (args.Length != 0)
				return (int)RunCommandLine(new List<string>(args));

			//interactive mode
			while (true)
			{
				Console.Write("Enter command: ");
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
						RunCommandLine(formattedCommand);
						break;
				}
			}
		}
	}
}