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
using System.IO;

namespace TGServerService
{
	public partial class TGServerService : ServiceBase
	{
		ServiceHost host;
		public TGServerService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			Directory.CreateDirectory("C:/tgstation-server-3");

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
