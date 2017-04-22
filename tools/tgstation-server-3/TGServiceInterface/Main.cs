using System.ServiceModel;

namespace TGServiceInterface
{
	[ServiceContract]
	[ServiceKnownType(typeof(ITGRepository))]
	[ServiceKnownType(typeof(ITGByond))]
	public interface ITGStationServer
	{
		[OperationContract]
		ITGRepository Repository();
		[OperationContract]
		ITGByond Byond();
	}
}
