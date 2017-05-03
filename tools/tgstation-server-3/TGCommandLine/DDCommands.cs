using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGCommandLine
{
	class DDCommand : RootCommand
	{
		public DDCommand()
		{
			Keyword = "dd";
			Children = new Command[] { };
		}
		public override void PrintHelp()
		{
			Console.WriteLine("dd\t-\tManage DreamDaemon");
		}
	}
}
