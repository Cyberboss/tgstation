using System;
using System.ServiceModel;
using System.Threading;
using TGServiceInterface;

namespace TGServerService
{
	//this line basically says make one instance of the service, use it multithreaded for requests, and never delete it
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
	partial class TGStationServer : IDisposable, ITGStatusCheck, ITGServerUpdater
	{
		//call partial constructors/destructors from here
		//called when the service is started
		public TGStationServer()
		{
			InitIRC(); //IRC
			InitByond();
			InitCompiler();
			InitInterop();
			InitDreamDaemon();
		}

		//called when the service is stopped
		void RunDisposals()
		{
			DisposeDreamDaemon();
			DisposeCompiler();
			DisposeByond();
			DisposeRepo();
		}

		public string UpdateServer(TGRepoUpdateMethod updateType, bool push_changelog, ushort testmerge_pr)
		{
			string res;
			if (updateType != TGRepoUpdateMethod.None)
			{
				res = Update(updateType == TGRepoUpdateMethod.Hard);
				if (res != null)
					return res;
			}
			if (testmerge_pr != 0)
			{
				res = MergePullRequest(testmerge_pr);
				if (res != null && res != RepoErrorUpToDate)
					return res;
			}

			GenerateChangelog(out res);
			if (res != null)
				return res;

			if (push_changelog)
			{
				res = Commit("Automatic changelog compile, [ci skip]");
				if (res != null)
					return res;
				res = Push();
				if (res != null)
					return res;
			}

			if (!Compile())
				return "Compilation could not be started";

			do
			{
				Thread.Sleep(100);
				lock (CompilerLock)
				{
					if (CompilerIdleNoLock())
						break;
				}
			}
			while (true);

			return CompileError();
		}

		//just here to test the WCF connection
		public void VerifyConnection()
		{}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					RunDisposals();
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~TGStationServer() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}
