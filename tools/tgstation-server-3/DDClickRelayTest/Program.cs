using System;
using System.Threading;
using System.ServiceModel;
using System.Windows.Forms;
using TGServiceInterface;

namespace DDClickRelayTest
{

	class Program
	{
		public static T GetServerComponent<T>()
		{
			return new ChannelFactory<T>(new NetNamedPipeBinding(), new EndpointAddress(String.Format("net.pipe://localhost/{0}/{1}", Declarations.MasterPipeName, typeof(T).Name))).CreateChannel();
		}
		static void Main(string[] args)
		{

			var byond = GetServerComponent<ITGByond>();

			byond.UpdateToVersion(511, 1381);

			do
			{
				Thread.Sleep(1000);
			} while (byond.CurrentStatus() != TGByondStatus.Idle);

			var error = byond.GetError();
			MessageBox.Show(error ?? "Operation completed successfully");
		}
	}
}