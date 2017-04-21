using System;
using System.ServiceModel;
using TGServiceInterface;

namespace TGServerService
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
	class TGStationServer : Git, ITGStationServer
	{
	}
}
