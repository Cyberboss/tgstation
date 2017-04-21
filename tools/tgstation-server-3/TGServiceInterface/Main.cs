using System.ServiceModel;

namespace TGServiceInterface
{
	[ServiceContract]
	public interface ITGStationServer
	{
		[OperationContract]
		ITGRepository Repository();
	}
}
