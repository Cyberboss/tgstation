using System;
using System.Threading;
using System.ServiceModel;
using System.Windows.Forms;
using TGServiceInterface;

namespace DDClickRelayTest
{

	class Program
	{
		static void Main(string[] args)
		{

			var repo = Server.GetComponent<ITGRepository>();
			string error;
			var thing = repo.MergedPullRequests(out error);

		}
		static void TestRun()
		{

			MessageBox.Show("Now setting up repo");
			var repo = Server.GetComponent<ITGRepository>();
			repo.Setup("https://github.com/tgstation/tgstation");


			MessageBox.Show("In the meantime lets update byond to 511.1381... give me a second...");
			var byond = Server.GetComponent<ITGByond>();

			if (!byond.UpdateToVersion(511, 1381))
			{
				MessageBox.Show("Last op failed");
				return;
			}

			do
			{
				Thread.Sleep(1000);
			} while (byond.CurrentStatus() != TGByondStatus.Idle);

			var error = byond.GetError();
			if (error != null)
			{
				MessageBox.Show(error);
				return;
			}

			MessageBox.Show("Done! Now we wait for the git clone....");


			do
			{
				Thread.Sleep(1000);
			} while (repo.OperationInProgress());

			if (!repo.Exists())
			{
				MessageBox.Show("Badness happened");
				return;
			}

			MessageBox.Show("Now lets try merging a PR");
		}
	}
}