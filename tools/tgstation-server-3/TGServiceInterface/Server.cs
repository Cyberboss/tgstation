using System;
using System.ServiceModel;
namespace TGServiceInterface
{
	public class Server
	{
		public static string MasterPipeName = "TGStationServerService";
		public static T GetComponent<T>()
		{
			return new ChannelFactory<T>(new NetNamedPipeBinding(), new EndpointAddress(String.Format("net.pipe://localhost/{0}/{1}", MasterPipeName, typeof(T).Name))).CreateChannel();
		}
	}
}
