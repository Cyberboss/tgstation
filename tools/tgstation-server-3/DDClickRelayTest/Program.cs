using System.Threading;
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
			var binding = new NetNamedPipeBinding();
			ChannelFactory<ITGStationServer> pipeFactory =
			  new ChannelFactory<ITGStationServer>(
				binding,
				new EndpointAddress(
				  "net.pipe://localhost/PipeTGStationServerService"));

			
			var server = pipeFactory.CreateChannel();

			var byond = server.Byond();

			byond.UpdateToVersion(551, 1381);

			do
			{
				Thread.Sleep(1000);
			} while (byond.IsBusy());

			var error = byond.GetError();
			MessageBox.Show(error != null ? error : "Operation completed successfully");

			return;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new CloneProgress());
		}
	}
}