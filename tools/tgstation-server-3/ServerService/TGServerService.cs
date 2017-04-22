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
		static TGServerService ActiveService;
		const string ServerDirectory = "C:\\tgstation-server-3";
		ServiceHost host;
		public TGServerService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			ActiveService = this;
			try
			{
				if (args.Length >= 2 && args[0].ToLower() == "--set-port")
				{
					try
					{
						var new_port = args[1].Trim();
						EventLog.WriteEntry("Attempting to set opening port to: " + new_port);
						Properties.Settings.Default.WCFPort = Convert.ToUInt16(args[1]);
					}
					catch
					{
						EventLog.WriteEntry("Failure, not a valid port!");
					}
				}

				if (!Directory.Exists(ServerDirectory))
					EventLog.WriteEntry("Creating server directory: " + ServerDirectory);
					Directory.CreateDirectory(ServerDirectory);

				EventLog.WriteEntry("Creating WCF host on port: " + Properties.Settings.Default.WCFPort);

				host = new ServiceHost(typeof(TGStationServer),
				  new Uri[]{
				new Uri(String.Format("http://localhost:{0}", Properties.Settings.Default.WCFPort)),
				new Uri("net.pipe://localhost")
				  });

				host.AddServiceEndpoint(typeof(ITGStationServer), new NetNamedPipeBinding(), "PipeTGStationServerService");
				host.Open();
			}
			catch
			{
				ActiveService = null;
				throw;
			}
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
				ActiveService = null;
			}
		}
	}
}
