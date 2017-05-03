using System;
using System.Collections.Generic;
using TGServiceInterface;

namespace TGCommandLine
{
	class IRCCommand : RootCommand
	{
		public IRCCommand()
		{
			Keyword = "irc";
			Children = new Command[] { };
		}
		public override void PrintHelp()
		{
			Console.WriteLine("irc\t-\tManage the IRC bot");
		}
	}
}
