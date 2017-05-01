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
			return new ChannelFactory<T>(new NetNamedPipeBinding { SendTimeout = new TimeSpan(0, 5, 0) }, new EndpointAddress(String.Format("net.pipe://localhost/{0}/{1}", MasterPipeName, typeof(T).Name))).CreateChannel();
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

	public enum TGRepoUpdateMethod
	{
		None,	//Do not update the repo
		Merge,	//Update the repo by merging the origin branch
		Hard,	//Update the repo by resetting to the origin branch
	}
	[ServiceContract]
	public interface ITGServerUpdater
	{
		/// <summary>
		/// Updates the server fully with various options as a blocking operation
		/// </summary>
		/// <param name="updateType">How to handle the repository during the update</param>
		/// <param name="push_changelog">true if the changelog should be pushed to git</param>
		/// <param name="testmerge_pr">If not zero, will testmerge the designated pull request</param>
		/// <returns>null on success, error message on failure</returns>
		[OperationContract]
		string UpdateServer(TGRepoUpdateMethod updateType, bool push_changelog, ushort testmerge_pr = 0);
	}

}
