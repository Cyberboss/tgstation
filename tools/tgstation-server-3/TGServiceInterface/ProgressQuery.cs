using System.ServiceModel;

namespace TGServiceInterface
{
	[ServiceContract]
	public interface ITGProgressQuery
	{
		//returns 1 to 100 to represent current progress
		//returns -1 if there is no applicable task running
		//is atomic
		[OperationContract]
		int GetProgress();
	}
}
