using System.ServiceModel;

namespace TGServiceInterface
{
	public enum TGByondStatus
	{
		Idle,	//no byond update in progress
		Starting,	//Preparing to update
		Downloading,	//byond downloading
		Staging,	//byond unzipping
		Staged,	//byond ready, waiting for dream daemon reboot
		Updating,	//applying update
	}
	[ServiceContract]
	public interface ITGByond
	{
		//returns the current status of update operations
		[OperationContract]
		TGByondStatus CurrentStatus();

		//updates the used byond version to that of version major.minor
		//The change won't take place until dream daemon reboots
		//the latest parameter overrides the other two and forces an update to the latest (beta?) version
		//runs asyncronously, use CurrentStatus to see progress
		//returns false if there is already an update operation in progress (or the current update is at the Staged phase)
		//returns true if the operation started
		[OperationContract]
		bool UpdateToVersion(int major, int minor);
		
		//null means the operation succeeded
		//will return an error message otherwise
		[OperationContract]
		string GetError();
	}
}
