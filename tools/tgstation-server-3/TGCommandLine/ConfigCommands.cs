using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGCommandLine
{
	class ConfigCommand : RootCommand
	{
		public ConfigCommand()
		{
			Keyword = "config";
			Children = new Command[] { };
		}
		public override void PrintHelp()
		{
			Console.WriteLine("config\t-\tManage settings");
		}
	}
}
