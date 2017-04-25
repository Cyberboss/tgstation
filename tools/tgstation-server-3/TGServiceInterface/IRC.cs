using System.ServiceModel;

namespace TGServiceInterface
{
	[ServiceContract]
	public interface ITGIRC
	{
		//Sets up IRC info, default fields don't change the current value
		[OperationContract]
		void Setup(string url_port = null, ushort port = 0, string username = null, string[] channels = null, string adminchannel = null, bool enabled = true);

		//Sets up auth IRC info, null fields don't change the current value
		[OperationContract]
		void SetupAuth(string identifyTarget, string identifyCommand, bool required);

		//returns true if the irc bot is connected, false otherwise
		[OperationContract]
		bool Connected();

		//returns null if the irc bot successfully logged into its assigned channels
		//returns error otherwise
		[OperationContract]
		string Connect();

		//what is says on the tin
		[OperationContract]
		void Disconnect();

		//what is says on the tin
		[OperationContract]
		string Reconnect();

		[OperationContract]
		//Sends a message to irc
		//returns null on success, error on failure
		string SendMessage(string msg, bool adminOnly = false);
	}
}
