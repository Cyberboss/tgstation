using System;
using System.IO;
using System.ServiceModel;
using System.ServiceProcess;
using TGServiceInterface;

namespace TGServerService
{
	public partial class TGServerService : ServiceBase
	{
		static TGServerService ActiveService;

		ServiceHost host;
		
		public TGServerService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			//System.Diagnostics.Debugger.Launch();
			ActiveService = this;
			try
			{
				var Config = Properties.Settings.Default;

				if (args.Length >= 2 && args[0].ToLower() == "--port")
				{
					try
					{
						var new_port = args[1].Trim();
						EventLog.WriteEntry("Commandline setting port to: " + new_port);
						Config.WCFPort = Convert.ToUInt16(new_port);
					}
					catch
					{
						EventLog.WriteEntry("Failure, not a valid port!");
					}
				}

				if (!Directory.Exists(Config.ServerDirectory))
					EventLog.WriteEntry("Creating server directory: " + Config.ServerDirectory);
					Directory.CreateDirectory(Config.ServerDirectory);
				Directory.SetCurrentDirectory(Config.ServerDirectory);

				EventLog.WriteEntry("Creating WCF host on port: " + Config.WCFPort);

				host = new ServiceHost(typeof(TGStationServer),
				  new Uri[]{
				new Uri(String.Format("http://localhost:{0}", Config.WCFPort)),
				new Uri("net.pipe://localhost")
				  });

				AddEndpoint<ITGRepository>();
				AddEndpoint<ITGByond>();
			}
			catch
			{
				ActiveService = null;
				throw;
			}
		}

		void AddEndpoint<T>()
		{
			var typetype = typeof(T);
			host.AddServiceEndpoint(typetype, new NetNamedPipeBinding(), Declarations.MasterPipeName);
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
