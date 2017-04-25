using System;
using TGServiceInterface;

namespace TGCommandLine
{

	class Program
	{
		enum ExitCode
		{
			Normal = 0,
			ConnectionError = 1,
			BadCommand = 2,
		}
		static ExitCode RunCommandLine(string[] args)
		{

			string command = null, param1 = null, param2 = null;
			if(args.Length > 0)
				command = args[0].Trim().ToLower();

			if (args.Length > 1)
				param1 = args[1].Trim().ToLower();

			if (args.Length > 2)
				param2 = args[2].Trim().ToLower();

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
				case "?":
				case "help":
					Console.WriteLine("IRC commands:");
					Console.WriteLine();
					Console.WriteLine("nick\t-\tSets the IRC nickname");
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
		}

		static int Main(string[] args)
		{
			return (int)RunCommandLine(args);
		}
	}
}