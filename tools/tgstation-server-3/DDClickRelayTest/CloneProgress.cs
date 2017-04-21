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
			timer1.Start();
		}

		private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{
			settingUp = true;
			Program.repo.Setup("https://github.com/tgstation/tgstation.git");
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (settingUp)
			{
				var value = Program.repo.GetProgress();
				if (value != -1)
					progressBar1.Value = value;
			}		
		}
	}
}
