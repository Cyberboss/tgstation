using System.ServiceModel;
using System.Windows.Forms;
using TGServiceInterface;

namespace DDClickRelayTest
{

	class Program
	{
		public static ITGRepository repo;
		static void Main(string[] args)
		{
			ChannelFactory<ITGStationServer> pipeFactory =
			  new ChannelFactory<ITGStationServer>(
				new NetNamedPipeBinding(),
				new EndpointAddress(
				  "net.pipe://localhost/PipeTGStationServerService"));

			
			ITGStationServer server = pipeFactory.CreateChannel();

			repo = server;

			if (repo.Exists())
			{
				MessageBox.Show("The repo already exists");
				return;
			}
			
	
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new CloneProgress());
		}
	}
}