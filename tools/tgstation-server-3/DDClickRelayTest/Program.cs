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

			string command = null;
			if(args.Length > 0)
				command = args[0].Trim();
			string param;
			if (args.Length > 1)
				param = args[1].Trim();

			var res = Server.VerifyConnection();
			if (res != null)
			{
				Console.WriteLine("Failed to connect to server!");
				return ExitCode.ConnectionError;
			}

			switch (command)
			{
				case "?":
				case "help":
					ConsoleHelp();
					break;
				default:
					Console.WriteLine("Invalid command: " + command);
					Console.WriteLine("Type '?' or 'help' for available commands.");
					return ExitCode.BadCommand;
			}
			return ExitCode.Normal;
		}

		static void ConsoleHelp()
		{
			Console.WriteLine("/tg/station 13 Server Control Panel:");
			Console.WriteLine();
		}

		static int Main(string[] args)
		{
			return (int)RunCommandLine(args);
		}
	}
}