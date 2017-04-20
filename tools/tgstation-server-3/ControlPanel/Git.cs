using LibGit2Sharp;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System;

namespace TGStationServer3
{
	//Everything here returns an error message if it failed or null if it succeeded
	class Git : IDisposable
	{
		const string RepoPath = "../gamecode";
		string BranchName;
		Repository Repo;
		Identity Ident;
		BackgroundWorker ProgressReporter;
		
		public Git(string RepoURL, string BranchName, string CommitterName, string CommitterEmail, BackgroundWorker BGW)
		{
			this.BranchName = BranchName;
			Ident = new Identity(CommitterName, CommitterEmail);
			ProgressReporter = BGW;
			if (!Exists())
			{
                if (Directory.Exists(RepoPath))
                    Delete();
				var Opts = new CloneOptions()
				{
					BranchName = BranchName,
					RecurseSubmodules = true,
					OnTransferProgress = HandleTransferProgress,
					OnCheckoutProgress = HandleCheckoutProgress
				};
				Repository.Clone(RepoURL, RepoPath, Opts);
			}
			Repo = new Repository(RepoPath);
		}

		public static void Delete()
        {
            Directory.Delete(RepoPath, true);
        }

		private bool HandleTransferProgress(TransferProgress progress)
		{
			if (ProgressReporter.CancellationPending)
				return false;

			ProgressReporter.ReportProgress((int) (((float) progress.ReceivedObjects / progress.TotalObjects) * 50));

			return true;
		}
		private void HandleCheckoutProgress(string path, int completedSteps, int totalSteps)
		{
			ProgressReporter.ReportProgress(50 + (int)(((float)completedSteps / totalSteps) * 50));
		}

		public static bool Exists()
		{
			return Repository.IsValid(RepoPath);
		}

		public string UpdateToOrigin()
		{
			if (Repo.Head.CanonicalName != BranchName)
			{
				var Error = ResetToSha(BranchName);
				if (Error != null)
					return Error;
			}

			try
			{
				string logMessage = "";
				foreach (Remote R in Repo.Network.Remotes)
				{
					IEnumerable<string> refSpecs = R.FetchRefSpecs.Select(X => X.Specification);
					Commands.Fetch(Repo, R.Name, refSpecs, null, logMessage);
				}
				Repo.Reset(ResetMode.Hard, String.Format("origin/{0}", BranchName));
				return null;
			}catch(Exception E)
			{
				return E.ToString();
			}
		}

		public string ResetToSha(string sha)
		{
			try
			{
				var Opts = new CheckoutOptions()
				{
					CheckoutModifiers = CheckoutModifiers.Force
				};
				Commands.Checkout(Repo, sha, Opts);
				return null;
			}
			catch(Exception E)
			{
				return E.ToString();
			}
		}

		public string GetCurrentSha()
		{
			return Repo.Head.Tip.Sha;
		}
		
		public string MergePullRequest(int PRNumber)
		{
			try
			{
				//only supported with github
				//if there's more than one remote then YOU can update this code
				if (!Repo.Network.Remotes.First().Url.Contains("github"))
					return "Only supported with Github based repositories.";


				var Refspec = new List<string>();
				var PRBranchName = String.Format("pr-{0}", PRNumber);
				Refspec.Add(String.Format("pull/{0}/head:", PRNumber, PRBranchName));
				var logMessage = "";
				Commands.Fetch(Repo, "origin", Refspec, null, logMessage);  //shitty api has no failure state for this

				//so we'll know if this fails
				var Result = Repo.Merge(PRBranchName, new Signature(Ident, DateTimeOffset.Now));
				switch (Result.Status)
				{
					case MergeStatus.Conflicts:
						return "Merge conflict occurred.";
					case MergeStatus.UpToDate:
						return "Already up to date with PR.";
				}
				return null;
			}
			catch (Exception E)
			{
				return E.ToString();
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				Repo.Dispose();
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~Git() {
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
