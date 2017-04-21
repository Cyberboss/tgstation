using System.ServiceModel;

namespace TGServiceInterface
{
	[ServiceContract]
	public interface ITGAtomic
	{
		//returns true if another call is currently going on
		//if there is, all other calls will block until that one completes
		[OperationContract]
		bool IsBusy();
	}
}
