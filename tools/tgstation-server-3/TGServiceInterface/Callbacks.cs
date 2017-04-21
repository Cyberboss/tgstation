using System.ServiceModel;

namespace TGServiceInterface
{
	interface ITGProgressCallback
	{
		//progress, an int from 1 to 100 representing operation progress
		//cancellable, true if the return value is not ignored
		//returns true to continue, false to cancel. Ignored if cancellable is false
		[OperationContract]
		bool OnProgress(int progress, bool cancellable);
	}
}
