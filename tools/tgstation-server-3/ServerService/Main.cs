using System;
using System.ServiceModel;
using TGServiceInterface;

namespace TGServerService
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
	[ServiceKnownType(typeof(Repository))]
	[ServiceKnownType(typeof(Byond))]
	class TGStationServer: ITGStationServer
	{
		Repository Repo = new Repository();
		Byond _Byond = new Byond();

		public ITGRepository Repository()
		{
			return Repo;
		}

		public ITGByond Byond()
		{
			return _Byond;
		}
	}
}
