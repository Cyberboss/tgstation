using LibGit2Sharp;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System;
using System.Text;
using TGServiceInterface;
using System.Security.Cryptography;

namespace ServerService
{
	//Everything here returns an error message if it failed or null if it succeeded
	class Git : ITGRepository
	{
		const string RepoPath = "../gamecode";

		object RepoLock = new object();

		Repository Repo;
		
		public bool IsBusy()
		{
			if (Monitor.TryEnter(RepoLock))
			{
				Monitor.Exit(RepoLock);
				return false;
			}
			return true;
		}

		//Sets up the repo object
		public string LoadRepo()
		{
			if (Repo != null)
				return null;
			if (!Repository.IsValid(RepoPath))
				return "Repository does not exist";
			try
			{
				Repo = new Repository(RepoPath);
			}
			catch (Exception e)
			{
				return e.ToString();
			}
			return null;
		}

		//Cleans up the repo object
		void DisposeRepo()
		{
			if (Repo != null)
			{
				Repo.Dispose();
				Repo = null;
			}
		}

		public bool Exists()
		{
			lock (RepoLock)
			{
				return Repository.IsValid(RepoPath);
			}
		}

		bool HandleTransferProgress(TransferProgress progress)
		{
			//TODO
			return true;
		}
		void HandleCheckoutProgress(string path, int completedSteps, int totalSteps)
		{
			//TODO
		}

		public string Setup(string RepoURL, string BranchName)
		{
			lock (RepoLock)
			{
				try
				{
					DisposeRepo();
					if (Exists())
					{
						try
						{
							//check if we have the right remote set
							var TempRepo = new Repository(RepoPath);
							if (TempRepo.Network.Remotes.First().Url == RepoURL)
								return Checkout(BranchName);
							TempRepo.Dispose();
						}
						catch { }


						if (Directory.Exists(RepoPath))
							Directory.Delete(RepoPath, true);
					}

					var Opts = new CloneOptions()
					{
						BranchName = BranchName,
						RecurseSubmodules = true,
						OnTransferProgress = HandleTransferProgress,
						OnCheckoutProgress = HandleCheckoutProgress
					};
					Repository.Clone(RepoURL, RepoPath, Opts);

					return LoadRepo();
				}
				catch (Exception e)
				{
					return e.ToString();
				}
			}
		}

		string GetShaOrBranch(out string error, bool branch)
		{
			lock (RepoLock)
			{
				var result = LoadRepo();
				if (result != null)
				{
					error = result;
					return null;
				}

				try
				{
					error = null;
					return branch ? Repo.Head.CanonicalName : Repo.Head.Tip.Sha; ;
				}
				catch (Exception e)
				{
					error = e.ToString();
					return null;
				}
			}
		}

		public string GetHead(out string error)
		{
			return GetShaOrBranch(out error, true);
		}
		public string GetBranch(out string error)
		{
			return GetShaOrBranch(out error, true);
		}

		string ResetNoLock()
		{
			try
			{
				var result = LoadRepo();
				if (result != null)
					return result;
				Repo.Reset(ResetMode.Hard);
				return null;
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}

		public string Checkout(string sha)
		{
			lock (RepoLock)
			{
				var result = LoadRepo();
				if (result != null)
					return result;
				try
				{
					var Opts = new CheckoutOptions()
					{
						CheckoutModifiers = CheckoutModifiers.Force
					};
					Commands.Checkout(Repo, sha, Opts);
					return ResetNoLock();
				}
				catch (Exception E)
				{
					return E.ToString();
				}
			}
		}

		public string Update()
		{
			lock (RepoLock)
			{
				var result = LoadRepo();
				if (result != null)
					return result;
				try
				{
					string logMessage = "";
					foreach (Remote R in Repo.Network.Remotes)
					{
						IEnumerable<string> refSpecs = R.FetchRefSpecs.Select(X => X.Specification);
						Commands.Fetch(Repo, R.Name, refSpecs, null, logMessage);
					}
					Repo.Reset(ResetMode.Hard, String.Format("origin/{0}", Repo.Head.CanonicalName));
					return ResetNoLock();
				}
				catch (Exception E)
				{
					return E.ToString();
				}
			}
		}

		public string Reset()
		{
			lock (RepoLock)
			{
				return ResetNoLock();
			}
		}
		Signature MakeSig()
		{
			var Config = Properties.Settings.Default;
			return new Signature(new Identity(Config.CommitterName, Config.CommitterEmail), DateTimeOffset.Now);
		}
		public string MergePullRequest(int PRNumber)
		{
			lock (RepoLock)
			{
				var result = LoadRepo();
				if (result != null)
					return result;
				try
				{
					//only supported with github
					if (!Repo.Network.Remotes.First().Url.Contains("github"))
						return "Only supported with Github based repositories.";


					var Refspec = new List<string>();
					var PRBranchName = String.Format("pr-{0}", PRNumber);
					Refspec.Add(String.Format("pull/{0}/head:", PRNumber, PRBranchName));
					var logMessage = "";
					Commands.Fetch(Repo, "origin", Refspec, null, logMessage);  //shitty api has no failure state for this

					var Config = Properties.Settings.Default;

					//so we'll know if this fails
					var Result = Repo.Merge(PRBranchName, MakeSig());
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
		}

		public IDictionary<int, string> MergedPullRequests(out string error)
		{
			lock (RepoLock)
			{
				var result = LoadRepo();
				if (result != null)
				{
					error = result;
					return null;
				}

				error = null;
				return new Dictionary<int, string>();
			}
		}

		public string GetCommitterName()
		{
			lock (RepoLock)
			{
				return Properties.Settings.Default.CommitterName;
			}
		}

		public void SetCommitterName(string newName)
		{
			lock (RepoLock)
			{
				Properties.Settings.Default.CommitterName = newName;
			}
		}

		public string GetCommitterEmail()
		{
			lock (RepoLock)
			{
				return Properties.Settings.Default.CommitterEmail;
			}
		}

		public void SetCommitterEmail(string newEmail)
		{
			lock (RepoLock)
			{
				Properties.Settings.Default.CommitterEmail = newEmail;
			}
		}

		public string GetCredentialUsername()
		{
			lock (RepoLock)
			{
				return Properties.Settings.Default.CredentialUsername;
			}
		}

		public void SetCredentials(string username, string password)
		{
			lock (RepoLock)
			{
				byte[] plaintext = Encoding.UTF8.GetBytes(password);

				// Generate additional entropy (will be used as the Initialization vector)
				byte[] entropy = new byte[20];
				using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
				{
					rng.GetBytes(entropy);
				}

				byte[] ciphertext = ProtectedData.Protect(plaintext, entropy, DataProtectionScope.CurrentUser);

				var Config = Properties.Settings.Default;
				Config.CredentialUsername = username;
				Config.CredentialEntropy = Convert.ToBase64String(entropy, 0, entropy.Length);
				Config.CredentialCyphertext = Convert.ToBase64String(ciphertext, 0, ciphertext.Length);
			}
		}

		public string Commit(string message)
		{
			lock (RepoLock)
			{
				var result = LoadRepo();
				if (result != null)
					return result;
				try
				{
					// Stage the file
					Commands.Stage(Repo, "*");

					// Create the committer's signature and commit
					var authorandcommitter = MakeSig();

					// Commit to the repository
					Repo.Commit(message, authorandcommitter, authorandcommitter);
					return null;
				}
				catch (Exception e)
				{
					return e.ToString();
				}
			}
		}

		public string Push()
		{
			lock (RepoLock)
			{
				var result = LoadRepo();
				if (result != null)
					return result;

				var Config = Properties.Settings.Default;
				try
				{
					byte[] plaintext = ProtectedData.Unprotect(Convert.FromBase64String(Config.CredentialCyphertext), Convert.FromBase64String(Config.CredentialEntropy), DataProtectionScope.CurrentUser);

					var options = new PushOptions();
					options.CredentialsProvider = new LibGit2Sharp.Handlers.CredentialsHandler(
						(url, usernameFromUrl, types) =>
							new UsernamePasswordCredentials()
							{
								Username = Config.CredentialUsername,
								Password = Encoding.UTF8.GetString(plaintext)
							});
					Repo.Network.Push(Repo.Head, options);
					return null;
				}
				catch (Exception e)
				{
					return e.ToString();
				}
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
					DisposeRepo();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
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
