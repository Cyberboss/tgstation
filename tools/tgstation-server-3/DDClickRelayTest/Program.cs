using System;
using System.Threading;
using TGServiceInterface;

namespace TestProg
{

	class Program
	{
		static void RunTests()
		{
			Setup(true, false);
			Server.GetComponent<ITGDreamDaemon>().Start();
			CheckByond(true);
		}

		static void SendToBotBusForTesting()
		{
			Server.GetComponent<ITGIRC>().Setup(null, 0, "TGS3Test", null, new string[] { }, "#botbus");
		}

		static void SendPRsToIRC()
		{
			var l = Server.GetComponent<ITGRepository>().MergedPullRequests(out string error);
			if (error == null)
			{
				Server.GetComponent<ITGIRC>().SendMessage("Currently merged PRs:");
				foreach (var I in l)
				{
					Server.GetComponent<ITGIRC>().SendMessage(String.Format("PR #{0} at commit {1}", I.Key, I.Value));
				}
			}
			else
				Server.GetComponent<ITGIRC>().SendMessage("PR Check Error: " + error);
		}

		//Sets up everything and starts the server
		static void Setup(bool forceCompile, bool forceReset)
		{
			if (forceReset || !Server.GetComponent<ITGRepository>().Exists())
			{
				Server.GetComponent<ITGRepository>().Setup("https://github.com/Cyberboss/tgstation", "tgs3");
				do
				{
					Thread.Sleep(1000);
				} while (Server.GetComponent<ITGRepository>().OperationInProgress());

				CheckByond(false);

				Server.GetComponent<ITGCompiler>().Initialize();
			}
			else
				CheckByond(false);

			if (forceCompile || Server.GetComponent<ITGDreamDaemon>().CanStart() != null)
			{
				Server.GetComponent<ITGCompiler>().Compile();

				do
				{
					Thread.Sleep(1000);
				} while (Server.GetComponent<ITGCompiler>().Compiling());
			}

		}

		static void CheckByond(bool forceUpdate)
		{
			if (!forceUpdate && Server.GetComponent<ITGByond>().GetVersion(false) != null)
				return;
			Server.GetComponent<ITGByond>().UpdateToVersion(511, 1381);
			do
			{
				Thread.Sleep(1000);
			} while (Server.GetComponent<ITGByond>().CurrentStatus() != TGByondStatus.Idle);
		}
		static void Main(string[] args)
		{
			try
			{
				RunTests();
			}
			catch {
				Console.WriteLine("Failed to connect to service!");
				Console.ReadKey();
			}
		}
	}
}