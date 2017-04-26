using LibGit2Sharp;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System;
using System.Text;
using TGServiceInterface;
using System.Security.Cryptography;
using System.Web.Script.Serialization;

namespace TGServerService
{
	partial class TGStationServer : ITGRepository, IDisposable
	{
		const string RepoPath = "Repository";
		const string RepoConfig = RepoPath + "/config";
		const string RepoData = RepoPath + "/data";

		const string PRJobFile = "prtestjob.json";

		object RepoLock = new object();
		bool RepoBusy = false;

		Repository Repo;
		int currentProgress = -1;

		public bool OperationInProgress()
		{
			lock(RepoLock)
			{
				return RepoBusy;
			}
		}

		public int CheckoutProgress()
		{
			return currentProgress;
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
				Repo = new LibGit2Sharp.Repository(RepoPath);
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
				return RepoBusy || Repository.IsValid(RepoPath);
			}
		}

		bool HandleTransferProgress(TransferProgress progress)
		{
			currentProgress = (int)(((float)progress.ReceivedObjects / progress.TotalObjects) * 100);
			return true;
		}
		void HandleCheckoutProgress(string path, int completedSteps, int totalSteps)
		{
			currentProgress = (int)(((float)completedSteps / totalSteps) * 100);
		}

		private class TwoStrings
		{
			public string a, b;
		}

		void Clone(object twostrings)
		{
			//busy flag set by caller
			try
			{
				if (!Monitor.TryEnter(CompilerLock))
					return;
				try
				{
					var ts = (TwoStrings)twostrings;
					var RepoURL = ts.a;
					var BranchName = ts.b;
					SendMessage(String.Format("REPO: Full reset started! Cloning {0} branch of {1} ...", BranchName, RepoURL));
					try
					{
						DisposeRepo();
						Program.DeleteDirectory(RepoPath);
						Program.DeleteDirectory(StaticBackupDir);
						DeletePRList();
						if (Directory.Exists(StaticDirs))
							Program.CopyDirectory(StaticDirs, StaticBackupDir);
						Program.DeleteDirectory(StaticDirs);

						var Opts = new CloneOptions()
						{
							BranchName = BranchName,
							RecurseSubmodules = true,
							OnTransferProgress = HandleTransferProgress,
							OnCheckoutProgress = HandleCheckoutProgress
						};
						Repository.Clone(RepoURL, RepoPath, Opts);
						LoadRepo();
						Directory.CreateDirectory(StaticLogDir);
						Program.CopyDirectory(RepoConfig, StaticConfigDir);
						Program.CopyDirectory(RepoData, StaticDataDir);
						File.Copy(RepoPath + LibMySQLFile, StaticDirs + LibMySQLFile, true);
						SendMessage("REPO: Clone complete!");
					}
					finally
					{
						currentProgress = -1;
					}
				}
				finally
				{
					Monitor.Exit(CompilerLock);
				}
			}
			catch

			{
				SendMessage("REPO: Setup failed!");
			} //don't crash the service
			finally
			{
				lock (RepoLock)
				{
					RepoBusy = false;
				}
			}
		}
		public bool Setup(string RepoURL, string BranchName)
		{
			lock (RepoLock)
			{
				if (RepoBusy)
					return false;
				if (compilerCurrentStatus != TGCompilerStatus.Initialized && compilerCurrentStatus != TGCompilerStatus.Uninitialized)
					return false;
				RepoBusy = true;
				new Thread(new ParameterizedThreadStart(Clone))
				{
					IsBackground = true //make sure we don't hold up shutdown
				}.Start(new TwoStrings { a = RepoURL, b = BranchName });
				return true;
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
					return branch ? Repo.Head.FriendlyName : Repo.Head.Tip.Sha; ;
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
		public string GetRemote(out string error)
		{
			try
			{
				var res = LoadRepo();
				if (res != null)
				{
					error = res;
					return null;
				}
				error = null;
				return Repo.Network.Remotes.First().Url;
			}
			catch (Exception e)
			{
				error = e.ToString();
				return null;
			}
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
				SendMessage("REPO: Checking out object: " + sha);
				try
				{
					var Opts = new CheckoutOptions()
					{
						CheckoutModifiers = CheckoutModifiers.Force,
						OnCheckoutProgress = HandleCheckoutProgress,
					};
					Commands.Checkout(Repo, sha, Opts);
					var res = ResetNoLock();
					SendMessage("REPO: Checkout complete!");
					return res;
				}
				catch (Exception E)
				{
					SendMessage("REPO: Checkout failed!");
					return E.ToString();
				}
			}
		}
		string MergeBranch(string branchname)
		{
			var Result = Repo.Merge(branchname, MakeSig());
			switch (Result.Status)
			{
				case MergeStatus.Conflicts:
					ResetNoLock();
					SendMessage("REPO: Merge conflicted, aborted.");
					return "Merge conflict occurred.";
				case MergeStatus.UpToDate:
					SendMessage("REPO: Merge already up to date!");
					return "Already up to date.";
			}
			SendMessage(String.Format("REPO: Branch {0} successfully merged!", branchname));
			return null;
		}
		public string Update(bool reset)
		{
			lock (RepoLock)
			{
				var result = LoadRepo();
				if (result != null)
					return result;
				SendMessage(String.Format("REPO: Updating origin branch...({0})", reset ? "Hard Reset" : "Merge"));
				try
				{
					string logMessage = "";
					foreach (Remote R in Repo.Network.Remotes)
					{
						IEnumerable<string> refSpecs = R.FetchRefSpecs.Select(X => X.Specification);
						var fos = new FetchOptions();
						fos.OnTransferProgress += HandleTransferProgress;
						Commands.Fetch(Repo, R.Name, refSpecs, null, logMessage);
					}

					var originBranch = String.Format("origin/{0}", Repo.Head.FriendlyName);
					if (reset)
					{
						Repo.Reset(ResetMode.Hard, originBranch);
						var error = ResetNoLock();
						if (error == null)
						{
							DeletePRList();
							SendMessage("REPO: Update complete!");
						}
						else
							SendMessage("REPO: Update failed!");
						return error;
					}
					return MergeBranch(originBranch);
				}
				catch (Exception E)
				{
					SendMessage("REPO: Update failed!");
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

		void DeletePRList()
		{
			if (File.Exists(PRJobFile))
				File.Delete(PRJobFile);
		}
		IDictionary<string, string> GetCurrentPRList()
		{
			if (!File.Exists(PRJobFile))
				return new Dictionary<string, string>();
			var rawdata = File.ReadAllText(PRJobFile);
			var Deserializer = new JavaScriptSerializer();
			return Deserializer.Deserialize<Dictionary<string, string>>(rawdata);
		}
		void SetCurrentPRList(IDictionary<string, string> list)
		{
			var Serializer = new JavaScriptSerializer();
			var rawdata = Serializer.Serialize(list);
			File.WriteAllText(PRJobFile, rawdata);
		}
		public string MergePullRequest(int PRNumber)
		{
			lock (RepoLock)
			{
				var result = LoadRepo();
				if (result != null)
					return result;
				SendMessage(String.Format("REPO: Merging PR #{0}...", PRNumber));
				try
				{
					//only supported with github
					if (!Repo.Network.Remotes.First().Url.Contains("github"))
						return "Only supported with Github based repositories.";


					var Refspec = new List<string>();
					var PRBranchName = String.Format("pr-{0}", PRNumber);
					Refspec.Add(String.Format("pull/{0}/head:{1}", PRNumber, PRBranchName));
					var logMessage = "";

					Commands.Fetch(Repo, "origin", Refspec, null, logMessage);  //shitty api has no failure state for this

					var Config = Properties.Settings.Default;

					PRBranchName = String.Format("pull/{0}/headrefs/heads/{1}", PRNumber, PRBranchName);

					var branch = Repo.Branches[PRBranchName];
					if (branch == null)
					{
						SendMessage("REPO: PR could not be fetched. Does it exist?");
						return String.Format("PR #{0} could not be fetched. Does it exist?", PRNumber);
					}

					//so we'll know if this fails
					var Result = MergeBranch(PRBranchName);

					if (Result == null)
					{
						var CurrentPRs = GetCurrentPRList();
						CurrentPRs.Add(PRNumber.ToString(), branch.Tip.Sha);
						SetCurrentPRList(CurrentPRs);
					}
					return Result;
				}
				catch (Exception E)
				{
					SendMessage("REPO: PR merge failed!");
					return E.ToString();
				}
			}
		}

		public IDictionary<string, string> MergedPullRequests(out string error)
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
					return GetCurrentPRList();
				}
				catch (Exception e)
				{
					error = e.ToString();
					return null;
				}
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
					DeletePRList();
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

					var options = new PushOptions()
					{
						CredentialsProvider = new LibGit2Sharp.Handlers.CredentialsHandler(
						(url, usernameFromUrl, types) =>
							new UsernamePasswordCredentials()
							{
								Username = Config.CredentialUsername,
								Password = Encoding.UTF8.GetString(plaintext)
							})
					};
					Repo.Network.Push(Repo.Head, options);
					DeletePRList();
					return null;
				}
				catch (Exception e)
				{
					return e.ToString();
				}
			}
		}
	}
}
