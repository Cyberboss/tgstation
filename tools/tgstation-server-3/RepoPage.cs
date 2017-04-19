using System;
using System.ComponentModel;

namespace TGStationServer3
{
	partial class Main
	{
		enum RepoWorkerAction
		{
			Discover,
			Load,
			MergePR,
			UpdateOrigin,
			RevertToSha,
		}
		RepoWorkerAction RWA;
		Git Repo;

		string RepoError;
	
		int PRToMerge;
		string ShaToRevert;

		private void InitRepoPage()
		{
			RWA = RepoWorkerAction.Discover;
			RepoBGW.ProgressChanged += RepoBGW_ProgressChanged;
			RepoBGW.RunWorkerCompleted += RepoBGW_RunWorkerCompleted;
			RepoBGW.RunWorkerAsync();
		}

		private void RepoBGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			RepoProgressBar.Visible = false;
			if (Repo == null)
			{
				//repo unavailable
				RepoProgressBarLabel.Text = "Unable to locate repository";
				CloneRepositoryButton.Visible = true;
			}
			else
				RepoProgressBarLabel.Visible = false;
		}

		private void RepoBGW_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			var percenttouse = e.ProgressPercentage;
			if (e.ProgressPercentage == 50)
			{
				RepoProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
				RepoProgressBarLabel.Text = "Indexing Objects...";
			}
			else if (e.ProgressPercentage > 50 && (RWA == RepoWorkerAction.Discover || RWA == RepoWorkerAction.Load))
				RepoProgressBarLabel.Text = "Checking out files...";
			percenttouse -= 50;

			if (percenttouse > 50)
			RepoProgressBar.Value = percenttouse;
		}
		
		private void RepoBGW_DoWork(object sender, DoWorkEventArgs e)
		{
			RepoError = null;
			switch (RWA) {
				case RepoWorkerAction.Discover:
					if (!Git.Exists())
						return;
					goto case RepoWorkerAction.Load;
				case RepoWorkerAction.Load:
					var Config = Properties.Settings.Default;
					//otherwise, clone the repo
					DisposeRepo();
					try
					{
						Repo = new Git(Config.RepoURL, Config.RepoBranch, Config.CommitterName, Config.CommitterEmail, RepoBGW);
					}
					catch(Exception E)
					{
						RepoError = E.ToString();
					}
					break;
				case RepoWorkerAction.MergePR:
					RepoError = Repo.MergePullRequest(PRToMerge);
					break;
				case RepoWorkerAction.RevertToSha:
					RepoError = Repo.ResetToSha(ShaToRevert);
					break;
				case RepoWorkerAction.UpdateOrigin:
					RepoError = Repo.UpdateToOrigin();
					break;
			};
		}

		private void CloneRepositoryButton_Click(object sender, EventArgs e)
		{
			RWA = RepoWorkerAction.Load;
			CloneRepositoryButton.Visible = false;
			var RepoURL = Properties.Settings.Default.RepoURL;
			RepoProgressBarLabel.Text = String.Format("Cloning into {0}", RepoURL);
			RepoProgressBar.Visible = true;
			RepoProgressBar.Value = 0;
			RepoProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			RepoBGW.RunWorkerAsync();
		}
		private void DisposeRepo()
		{
			if (Repo != null)
				Repo.Dispose();
		}
	}
}
