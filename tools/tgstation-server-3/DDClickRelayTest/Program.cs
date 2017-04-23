using System.Threading;
using System.Windows.Forms;
using TGServiceInterface;

namespace DDClickRelayTest
{

	class Program
	{
		static void Main(string[] args)
		{
			Server.GetComponent<ITGIRC>().Setup(null, 0, null, null, new string[] { "#botbus" });
			Server.GetComponent<ITGIRC>().Connect();

			Setup();

			MessageBox.Show(Server.GetComponent<ITGDreamDaemon>().Start() ?? "Server started");
		}

		//Sets up everything and starts the server
		static void Setup()
		{
			if (!Server.GetComponent<ITGRepository>().Exists())
			{
				Server.GetComponent<ITGRepository>().Setup("https://github.com/tgstation/tgstation");
				do
				{
					Thread.Sleep(1000);
				} while (Server.GetComponent<ITGRepository>().OperationInProgress());

				CheckByond();

				Server.GetComponent<ITGCompiler>().Initialize();
			}
			else
				CheckByond();

			if (Server.GetComponent<ITGDreamDaemon>().CanStart() != null)
			{
				Server.GetComponent<ITGCompiler>().Compile();

				do
				{
					Thread.Sleep(1000);
				} while (Server.GetComponent<ITGCompiler>().Compiling());
			}

		}

		static void CheckByond()
		{
			if (Server.GetComponent<ITGByond>().GetVersion(false) != null)
				return;
			Server.GetComponent<ITGByond>().UpdateToVersion(511, 1381);
			do
			{
				Thread.Sleep(1000);
			} while (Server.GetComponent<ITGByond>().CurrentStatus() != TGByondStatus.Idle);
		}
	}
}