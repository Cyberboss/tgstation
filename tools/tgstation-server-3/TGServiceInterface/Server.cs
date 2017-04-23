using System;
using System.ServiceModel;
namespace TGServiceInterface
{
	public class Server
	{
		//Internal
		public static string MasterPipeName = "TGStationServerService";

		//Returns the requested server component interface
		public static T GetComponent<T>()
		{
			return new ChannelFactory<T>(new NetNamedPipeBinding(), new EndpointAddress(String.Format("net.pipe://localhost/{0}/{1}", MasterPipeName, typeof(T).Name))).CreateChannel();
		}

		//Used to test if the service is avaiable on the machine
		//Note that state can technically change at any time
		//and any call may throw an exception
		//returns null on success, error message on failure
		public static string VerifyConnection()
		{
			try
			{
				GetComponent<ITGStatusCheck>().VerifyConnection();
				return null;
			}
			catch(Exception e)
			{
				return e.ToString();
			}
		}
	}
	//Internal
	[ServiceContract]
	public interface ITGStatusCheck
	{
		[OperationContract]
		void VerifyConnection();
	}

}
