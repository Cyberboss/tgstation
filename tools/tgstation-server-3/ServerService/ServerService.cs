using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using TGServiceInterface;

namespace ServerService
{
	public partial class ServerService : ServiceBase
	{
		ServiceHost host;
		public ServerService()
		{
			InitializeComponent();
			ServiceName = "TGstation 13 Service";
		}

		protected override void OnStart(string[] args)
		{
			System.Diagnostics.Debugger.Launch();
			host = new ServiceHost(
			  typeof(StringReverser),
			  new Uri[]{
				new Uri("http://localhost:8000"),
				new Uri("net.pipe://localhost")
			  });
			host.AddServiceEndpoint(typeof(IStringReverser),
				new BasicHttpBinding(),
				"Reverse");

			host.AddServiceEndpoint(typeof(IStringReverser),
				new NetNamedPipeBinding(),
				"PipeReverse");

			host.Open();
		}

		protected override void OnStop()
		{
			host.Close();
		}
	}

	public class StringReverser : IStringReverser
	{
		public string ReverseString(string value)
		{
			char[] retVal = value.ToCharArray();
			int idx = 0;
			for (int i = value.Length - 1; i >= 0; i--)
				retVal[idx++] = value[i];

			return new string(retVal);
		}
	}
}
