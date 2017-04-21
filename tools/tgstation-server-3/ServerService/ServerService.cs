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
		}

		protected override void OnStart(string[] args)
		{
			System.Diagnostics.Debugger.Launch();
			host = new ServiceHost(typeof(TGStationServer),
			  new Uri[]{
				new Uri(String.Format("http://localhost:{0}", Properties.Settings.Default.WCFPort)),
				new Uri("net.pipe://localhost")
			  });

			host.AddServiceEndpoint(typeof(ITGStationServer), new NetNamedPipeBinding(), "PipeTGStationServerService");

			host.Open();
		}

		protected override void OnStop()
		{
			try
			{
				host.Close();
				host = null;
			}
			finally
			{
				Properties.Settings.Default.Save();
			}
		}
	}
}
