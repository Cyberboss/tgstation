using System;
using System.Collections.Generic;
using TGServiceInterface;

namespace TGCommandLine
{
	class DDCommand : RootCommand
	{
		public DDCommand()
		{
			Keyword = "dd";
			Children = new Command[] { new DDStartCommand(), new DDStopCommand(), new DDRestartCommand(), new DDStatusCommand(), new DDAutostartCommand() };
		}
		public override void PrintHelp()
		{
			Console.WriteLine("dd\t-\tManage DreamDaemon");
		}
	}

	class DDStartCommand : Command
	{
		public DDStartCommand()
		{
			Keyword = "start";
		}

		public override void PrintHelp()
		{
			Console.WriteLine("start\t-\tStarts the server and watchdog");
		}

		public override ExitCode Run(IList<string> parameters)
		{
			var res = Server.GetComponent<ITGDreamDaemon>().Start();
			Console.WriteLine(res ?? "Success!");
			return res == null ? ExitCode.Normal : ExitCode.ServerError;
		}
	}

	class DDStopCommand : Command
	{
		public DDStopCommand()
		{
			Keyword = "stop";
		}

		public override void PrintHelp()
		{
			Console.WriteLine("stop [--graceful]\t-\tStops the server and watchdog optionally waiting for the current round to end");
		}

		public override ExitCode Run(IList<string> parameters)
		{
			var DD = Server.GetComponent<ITGDreamDaemon>();
			if (parameters.Count > 0 && parameters[0].ToLower() == "--graceful")
			{
				if (DD.DaemonStatus() != TGDreamDaemonStatus.Online)
				{
					Console.WriteLine("Error: The game is not currently running!");
					return ExitCode.ServerError;
				}
				DD.RequestStop();
				return ExitCode.Normal;
			}
			var res = DD.Stop();
			Console.WriteLine(res ?? "Success!");
			return res == null ? ExitCode.Normal : ExitCode.ServerError;
		}
	}
	class DDRestartCommand : Command
	{
		public DDRestartCommand()
		{
			Keyword = "restart";
		}

		public override void PrintHelp()
		{
			Console.WriteLine("restart [--graceful]\t-\tRestarts the server and watchdog optionally waiting for the current round to end");
		}

		public override ExitCode Run(IList<string> parameters)
		{
			var DD = Server.GetComponent<ITGDreamDaemon>();
			if (parameters.Count > 0 && parameters[0].ToLower() == "--graceful")
			{
				if (DD.DaemonStatus() != TGDreamDaemonStatus.Online)
				{
					Console.WriteLine("Error: The game is not currently running!");
					return ExitCode.ServerError;
				}
				DD.RequestRestart();
				return ExitCode.Normal;
			}
			var res = DD.Restart();
			Console.WriteLine(res ?? "Success!");
			return res == null ? ExitCode.Normal : ExitCode.ServerError;
		}
	}
	class DDStatusCommand : Command
	{
		public DDStatusCommand()
		{
			Keyword = "status";
		}

		public override void PrintHelp()
		{
			Console.WriteLine("status\t-\tGets the current status of the watchdog");
		}

		public override ExitCode Run(IList<string> parameters)
		{
			var DD = Server.GetComponent<ITGDreamDaemon>();
			switch (DD.DaemonStatus()) {
				case TGDreamDaemonStatus.HardRebooting:
					Console.WriteLine("Rebooting");
					break;
				case TGDreamDaemonStatus.Offline:
					Console.WriteLine("Offline");
					break;
				case TGDreamDaemonStatus.Online:
					Console.WriteLine("Online");
					break;
				default:
					Console.WriteLine("Null and errors!");
					return ExitCode.ServerError;
			}
			return ExitCode.Normal;
		}
	}

	class DDAutostartCommand : Command
	{
		public DDAutostartCommand()
		{
			Keyword = "autostart";
			RequiredParameters = 1;
		}

		public override ExitCode Run(IList<string> parameters)
		{
			var DD = Server.GetComponent<ITGDreamDaemon>();
			switch (parameters[0].ToLower())
			{
				case "on":
					DD.SetAutostart(true);
					break;
				case "off":
					DD.SetAutostart(false);
					break;
				case "check":
					Console.WriteLine("Autostart is: " + (DD.Autostart() ? "On" : "Off"));
					break;
				default:
					Console.WriteLine("Invalid parameter: " + parameters[0]);
					return ExitCode.BadCommand;
			}
			return ExitCode.Normal;
		}

		public override void PrintHelp()
		{
			Console.WriteLine("autostart <on|off|check>\t-\tChange or check autostarting of the game server");
		}
	}
}
