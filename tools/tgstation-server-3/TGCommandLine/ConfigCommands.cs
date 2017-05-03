using System;
using System.Collections.Generic;
using TGServiceInterface;

namespace TGCommandLine
{
	class ConfigCommand : RootCommand
	{
		public ConfigCommand()
		{
			Keyword = "config";
			Children = new Command[] { new ConfigMoveServerCommand(), new ConfigServerDirectoryCommand() };
		}
		public override void PrintHelp()
		{
			Console.WriteLine("config\t-\tManage settings");
		}
	}

	class ConfigMoveServerCommand : Command
	{
		public ConfigMoveServerCommand()
		{
			Keyword = "move-server";
			RequiredParameters = 1;
		}

		public override ExitCode Run(IList<string> parameters)
		{
			var res = Server.GetComponent<ITGConfig>().MoveServer(parameters[0]);
			Console.WriteLine(res ?? "Success");
			return res == null ? ExitCode.Normal : ExitCode.ServerError;
		}

		public override void PrintHelp()
		{
			Console.WriteLine("move-server <new-path>\t-\tMove the server installation (BYOND, Repo, Game) to a new location. Nothing else may be running for this task to complete");
		}
	}

	class ConfigServerDirectoryCommand : Command
	{
		public ConfigServerDirectoryCommand()
		{
			Keyword = "server-dir";
			RequiredParameters = 1;
		}

		public override ExitCode Run(IList<string> parameters)
		{
			Console.WriteLine(Server.GetComponent<ITGConfig>().ServerDirectory());
			return ExitCode.Normal;
		}

		public override void PrintHelp()
		{
			Console.WriteLine("server-dir\t-\tPrint the directory the server is installed in");
		}
	}
}
