using System.ServiceModel;

namespace TGServiceInterface
{
	[ServiceContract]
	public interface ITGByond : ITGProgressQuery, ITGAtomic
	{
		//updates the used byond version to that of version major.minor
		//The change won't take place until dream daemon reboots
		//the latest parameter overrides the other two and forces an update to the latest (beta?) version
		//runs asyncronously, use IsBusy, GetProgress, and Exists to check status
		[OperationContract]
		void UpdateToVersion(int major, int minor);

		//Check this if IsBusy is false
		//null means the operation succeeded
		//will return an error message otherwise
		//this includes waiting for the server to reboot
		[OperationContract]
		string GetError();
	}
}
