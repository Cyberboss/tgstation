using System;
using System.Threading;
using System.ServiceModel;
using System.Windows.Forms;
using TGServiceInterface;

namespace DDClickRelayTest
{

	class Program
	{
		static void Main(string[] args)
		{
			Server.GetComponent<ITGRepository>().Setup("https://github.com/tgstation/tgstation");
			
		}
	}
}