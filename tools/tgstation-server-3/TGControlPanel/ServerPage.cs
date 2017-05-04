using System.Windows.Forms;
using TGServiceInterface;

namespace TGControlPanel
{
	partial class Main
	{
		string DDStatusString = null;
		void InitServerPage()
		{
			LoadServerPage();
			ServerTimer.Start();
			WorldStatusChecker.RunWorkerAsync();
			WorldStatusChecker.RunWorkerCompleted += WorldStatusChecker_RunWorkerCompleted;
		}

		private void WorldStatusChecker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			if (DDStatusString != "Topic recieve error!" || ServerStatusLabel.Text == "OFFLINE")
				ServerStatusLabel.Text = DDStatusString;
			WorldStatusTimer.Start();
		}

		void LoadServerPage()
		{
			var DM = Server.GetComponent<ITGCompiler>();
			var DD = Server.GetComponent<ITGDreamDaemon>();

			AutostartCheckbox.Checked = DD.Autostart();

			switch (DM.GetStatus())
			{
				case TGCompilerStatus.Compiling:
					CompilerStatusLabel.Text = "Compiling...";
					compilerProgressBar.Style = ProgressBarStyle.Marquee;
					compileButton.Enabled = false;
					initializeButton.Enabled = false;
					break;
				case TGCompilerStatus.Initializing:
					CompilerStatusLabel.Text = "Initializing...";
					compilerProgressBar.Style = ProgressBarStyle.Marquee;
					compileButton.Enabled = false;
					initializeButton.Enabled = false;
					break;
				case TGCompilerStatus.Initialized:
					CompilerStatusLabel.Text = "Idle";
					compilerProgressBar.Style = ProgressBarStyle.Blocks;
					initializeButton.Enabled = true;
					compileButton.Enabled = true;
					break;
				case TGCompilerStatus.Uninitialized:
					CompilerStatusLabel.Text = "Uninitialized";
					compilerProgressBar.Style = ProgressBarStyle.Blocks;
					compileButton.Enabled = false;
					initializeButton.Enabled = true;
					break;
				default:
					CompilerStatusLabel.Text = "Unknown!";
					compilerProgressBar.Style = ProgressBarStyle.Blocks;
					initializeButton.Enabled = true;
					compileButton.Enabled = true;
					break;
			}
			var error = DM.CompileError();
			if (error != null)
				MessageBox.Show("Error: " + error);
		}

		private void ServerTimer_Tick(object sender, System.EventArgs e)
		{
			LoadServerPage();
		}

		private void InitializeButton_Click(object sender, System.EventArgs e)
		{
			if (!Server.GetComponent<ITGCompiler>().Initialize())
				MessageBox.Show("Unable to start initialization!");
		}
		private void CompileButton_Click(object sender, System.EventArgs e)
		{
			if(!Server.GetComponent<ITGCompiler>().Compile())
				MessageBox.Show("Unable to start compilation!");
		}
		//because of lol byond this can take some time...
		private void WorldStatusChecker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			DDStatusString = Server.GetComponent<ITGDreamDaemon>().StatusString();
		}

		private void WorldStatusTimer_Tick(object sender, System.EventArgs e)
		{
			WorldStatusTimer.Stop();
			WorldStatusChecker.RunWorkerAsync();
		}

		private void AutostartCheckbox_CheckedChanged(object sender, System.EventArgs e)
		{
			var DD = Server.GetComponent<ITGDreamDaemon>();
			if(DD.Autostart() != AutostartCheckbox.Checked)
				DD.SetAutostart(AutostartCheckbox.Checked);
		}
		private void ServerStartButton_Click(object sender, System.EventArgs e)
		{
			var res = Server.GetComponent<ITGDreamDaemon>().Start();
			if (res != null)
				MessageBox.Show(res);
		}

		private void ServerStopButton_Click(object sender, System.EventArgs e)
		{
			var DialogResult = MessageBox.Show("This will immediately shut down the server. Continue?", "Confim", MessageBoxButtons.YesNo);
			if (DialogResult == DialogResult.No)
				return;
			var res = Server.GetComponent<ITGDreamDaemon>().Stop();
			if (res != null)
				MessageBox.Show(res);
		}

		private void ServerRestartButton_Click(object sender, System.EventArgs e)
		{
			var DialogResult = MessageBox.Show("This will immediately restart the server. Continue?", "Confim", MessageBoxButtons.YesNo);
			if (DialogResult == DialogResult.No)
				return;
			var res = Server.GetComponent<ITGDreamDaemon>().Restart();
			if (res != null)
				MessageBox.Show(res);
		}

		private void ServerGStopButton_Click(object sender, System.EventArgs e)
		{
			var DialogResult = MessageBox.Show("This will shut down the server when the current round ends. Continue?", "Confim", MessageBoxButtons.YesNo);
			if (DialogResult == DialogResult.No)
				return;
			Server.GetComponent<ITGDreamDaemon>().RequestStop();
		}

		private void ServerGRestartButton_Click(object sender, System.EventArgs e)
		{
			var DialogResult = MessageBox.Show("This will restart the server when the current round ends. Continue?", "Confim", MessageBoxButtons.YesNo);
			if (DialogResult == DialogResult.No)
				return;
			Server.GetComponent<ITGDreamDaemon>().RequestRestart();
		}
	}
}
