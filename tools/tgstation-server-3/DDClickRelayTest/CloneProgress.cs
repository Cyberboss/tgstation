using System;
using System.Windows.Forms;
using System.ServiceModel;
using TGServiceInterface;

namespace DDClickRelayTest
{
	public partial class CloneProgress : Form
	{
		bool settingUp = false;
		public CloneProgress()
		{
			InitializeComponent();
		}

		private void CloneProgress_Load(object sender, EventArgs e)
		{
			MessageBox.Show("I will now clone the repo");
			backgroundWorker1.RunWorkerAsync();
		}

		private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			settingUp = true;
			Program.repo.Setup("https://github.com/tgstation/tgstation.git");
			settingUp = false;
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (settingUp)
				progressBar1.Value = Program.repo.GetProgress();
		}
	}
}
