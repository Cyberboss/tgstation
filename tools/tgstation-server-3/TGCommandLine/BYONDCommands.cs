using System;
using System.Collections.Generic;
using TGServiceInterface;

namespace TGCommandLine
{
	class BYONDCommand : RootCommand
	{
		public BYONDCommand()
		{
			Keyword = "byond";
			Children = new Command[] { };
		}
		public override void PrintHelp()
		{
			Console.WriteLine("byond\t-\tManage BYOND installation");
		}
	}
}
