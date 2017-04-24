using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using TGServiceInterface;

namespace TGControlPanel
{
	partial class Main
	{
		string CloneRepoURL;
		string CloneRepoBranch;
		private void InitRepoPage()
		{
			RepoBGW.ProgressChanged += RepoBGW_ProgressChanged;
			RepoBGW.RunWorkerCompleted += RepoBGW_RunWorkerCompleted;
			PopulateRepoFields();
		}

		private void RepoBGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			PopulateRepoFields();
		}

		private void PopulateRepoFields()
		{
			var Repo = Server.GetComponent<ITGRepository>();
			

			RepoProgressBar.Visible = false;
			RemoteNameTitle.Visible = true;
			RepoRemoteTextBox.Visible = true;
			BranchNameTitle.Visible = true;
			RepoBranchTextBox.Visible = true;

			if (!Repo.Exists())
			{
				//repo unavailable
				RepoProgressBarLabel.Text = "Unable to locate repository";
				CloneRepositoryButton.Visible = true;
			}
			else
			{
				RepoProgressBarLabel.Visible = false;

				CurrentRevisionLabel.Visible = true;
				CurrentRevisionTitle.Visible = true;
				IdentityLabel.Visible = true;
				CommiterNameTitle.Visible = true;
				CommitterEmailTitle.Visible = true;
				RepoCommitterNameTextBox.Visible = true;
				RepoEmailTextBox.Visible = true;
				TestMergeButton.Visible = true;
				TestMergeListLabel.Visible = true;
				TestMergeListTitle.Visible = true;
				UpdateRepoButton.Visible = true;
				HardReset.Visible = true;
				RepoApplyButton.Visible = true;

				CurrentRevisionLabel.Text = Repo.GetHead(out string error) ?? "Unknown";
				RepoRemoteTextBox.Text = Repo.GetRemote(out error) ?? "Unknown";
				RepoBranchTextBox.Text = Repo.GetBranch(out error) ?? "Unknown";
				RepoCommitterNameTextBox.Text = Repo.GetCommitterName() ?? "Unknown";
				RepoEmailTextBox.Text = Repo.GetCommitterEmail() ?? "Unknown";


				var PRs = Repo.MergedPullRequests(out error);
				if(PRs != null)
				{
					if (PRs.Count == 0)
						TestMergeListLabel.Text = "None";
					else
					{
						TestMergeListLabel.Text = "";
						foreach (var I in PRs)
							TestMergeListLabel.Text += String.Format("#{0} at commit {1}\r\n", I.Key, I.Value);
					}
				}
				else
					TestMergeListLabel.Text = "Unknown";

			}
		}

		private void RepoBGW_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			RepoProgressBar.Value = e.ProgressPercentage;
		}

		private void RepoBGW_DoWork(object sender, DoWorkEventArgs e)
		{
			//Only for clones
			var Repo = Server.GetComponent<ITGRepository>();
			Repo.Setup(CloneRepoURL, CloneRepoBranch);

			do
			{
				Thread.Sleep(1000);
				RepoBGW.ReportProgress(Repo.CheckoutProgress());
			} while (Repo.OperationInProgress());
		}

		private void CloneRepositoryButton_Click(object sender, EventArgs e)
		{
			CloneRepoURL = RepoRemoteTextBox.Text;
			CloneRepoBranch = RepoBranchTextBox.Text;
			RepoProgressBarLabel.Text = String.Format("Cloning into {0}", RepoRemoteTextBox.Text);

			CloneRepositoryButton.Visible = false;
			RemoteNameTitle.Visible = false;
			RepoRemoteTextBox.Visible = false;
			BranchNameTitle.Visible = false;
			RepoBranchTextBox.Visible = false;
			RepoProgressBar.Visible = true;
			RepoProgressBar.Value = 0;
			RepoProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			RepoBGW.RunWorkerAsync();
		}
		private void RepoApplyButton_Click(object sender, EventArgs e)
		{
			var Repo = Server.GetComponent<ITGRepository>();

			var remote = Repo.GetRemote(out string error);
			if (remote == null) {
				MessageBox.Show("Error: " + error);
				return;
			}
	
			var Reclone = remote != RepoRemoteTextBox.Text;
			if (Reclone)
			{
				var DialogResult = MessageBox.Show("Changing the remote URL requires a re-cloning of the repository. Continue?", "Confim", MessageBoxButtons.YesNo);
				if (DialogResult == DialogResult.No)
					return;
			}

			if (!Reclone)
			{
				var branch = Repo.GetBranch(out error);
				if(branch == null)
				{
					MessageBox.Show("Error: " + error);
					return;
				}


				if (branch != RepoBranchTextBox.Text) {
					var result = Repo.Checkout(RepoBranchTextBox.Text);
					if(result != null)
					{
						MessageBox.Show("Error: " + result);
						return;
					}
				}

				Repo.SetCommitterName(RepoCommitterNameTextBox.Text);
				Repo.SetCommitterEmail(RepoEmailTextBox.Text);
			}
			else
				CloneRepositoryButton_Click(null, null);
		}
		private void UpdateRepo(bool reset)
		{
			var Repo = Server.GetComponent<ITGRepository>();
			MessageBox.Show(Repo.Update(reset) ?? "Operation success!");
			PopulateRepoFields();
		}
		private void UpdateRepoButton_Click(object sender, EventArgs e)
		{
			UpdateRepo(false);
		}
		private void HardReset_Click(object sender, EventArgs e)
		{
			UpdateRepo(true);
		}
		private void TestMergeButton_Click(object sender, EventArgs e)
		{
			string input = Interaction.InputBox("Merge PR", "Enter the number of the PR you wish to merge", "", 0, 0).Trim();
			ushort Result;
			try
			{
				Result = Convert.ToUInt16(input);
			}
			catch
			{
				MessageBox.Show("Invalid PR number: " + input);
				return;
			}

			MessageBox.Show(Server.GetComponent<ITGRepository>().MergePullRequest(Result) ?? "Operation success!");
			PopulateRepoFields();
		}
	}
}
