using System.ServiceModel;

namespace TGServiceInterface
{
	public enum TGDreamDaemonStatus
	{
		Offline,
		HardRebooting,
		Online,
	}

	public enum TGDreamDaemonSecurity
	{
		Trusted = 0,    //default config
		Safe,
		Ultrasafe
	}

	public enum TGDreamDaemonVisibility
	{
		Public,
		Private,
		Invisible = 2,  //default config
	}

	[ServiceContract]
	public interface ITGDreamDaemon
	{
		//Returns the status of the server
		[OperationContract]
		TGDreamDaemonStatus DaemonStatus();

		//Check if a call to Start will fail
		//returns the error that would occur
		//null otherwise
		[OperationContract]
		string CanStart();

		//starts the server
		//returns null on success or error on failure
		[OperationContract]
		string Start();

		//kills the server
		//this will DC everyone in the world unless immediately rebooted
		//returns null on success or error on failure
		[OperationContract]
		string Stop();

		//Kills and restarts the server
		//returns null on success or error on failure
		[OperationContract]
		string Restart();

		//Restarts the server after the currently running round ends
		[OperationContract]
		void RequestRestart();

		//Stops the server after the currently running round ends
		[OperationContract]
		void RequestStop();

		//Sets the security level of the server. Requires reboot to apply
		//note that anything higher than Trusted will disable server commands
		[OperationContract]
		void SetSecurityLevel(TGDreamDaemonSecurity level);

		//Sets the visiblity level of the server. Requires reboot to apply
		[OperationContract]
		void SetVisibility(TGDreamDaemonVisibility vis);

		//Sets the port to use. Requires reboot to apply
		[OperationContract]
		void SetPort(ushort new_port);

		//returns true if Start() is called when the service starts, false otherwise
		[OperationContract]
		bool Autostart();

		//Set whether or not DD starts with the service
		[OperationContract]
		void SetAutostart(bool on);
	}
}
