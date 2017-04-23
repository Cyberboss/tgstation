using System.Threading;
using System.Windows.Forms;
using TGServiceInterface;

namespace DDClickRelayTest
{

	class Program
	{
		static void Main(string[] args)
		{

			TestRun();
		}

		//Sets up everything and starts the server
		static void TestRun()
		{

			Server.GetComponent<ITGRepository>().Setup("https://github.com/tgstation/tgstation");
			do
			{
				Thread.Sleep(1000);
			} while (Server.GetComponent<ITGRepository>().OperationInProgress());

			Server.GetComponent<ITGByond>().UpdateToVersion(511, 1381);

			do
			{
				Thread.Sleep(1000);
			} while (Server.GetComponent<ITGByond>().CurrentStatus() != TGByondStatus.Idle);

			Server.GetComponent<ITGCompiler>().Initialize();

			Server.GetComponent<ITGCompiler>().Compile();

			do
			{
				Thread.Sleep(1000);
			} while (Server.GetComponent<ITGCompiler>().Compiling());


			MessageBox.Show("Hold on to your butts...");

			MessageBox.Show(Server.GetComponent<ITGDreamDaemon>().Start() ?? "Spared no expense");

		}
	}
}