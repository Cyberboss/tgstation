using System;
using System.Threading;
using TGServiceInterface;

namespace TestProg
{

	class Program
	{
		static void Main(string[] args)
		{

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