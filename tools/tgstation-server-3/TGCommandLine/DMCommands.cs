using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGCommandLine
{
	class DMCommand : RootCommand
	{
		public DMCommand()
		{
			Keyword = "dm";
			Children = new Command[] { };
		}
		public override void PrintHelp()
		{
			Console.WriteLine("dm\t-\tManage compiling the server");
		}
	}
}
