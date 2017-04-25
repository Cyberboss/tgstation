using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TGServerService
{
	partial class TGStationServer
	{
		//raw command string sent here via world.ExportService
		void HandleCommand(string cmd)
		{
			var splits = new List<string>(cmd.Split(' '));

			switch (splits[0])
			{
				case "irc":
					splits.RemoveAt(0);
					SendMessage("GAME: " + String.Join(" ", splits));
					break;
			}
		}

		void InitNudge()
		{
			new Thread(new ThreadStart(NudgeHandler)) { IsBackground = true }.Start() ;
		}

		void NudgeHandler()
		{
			try
			{
				const string nudgePort = StaticConfigDir + "/nudge_port.txt";
				while (!File.Exists(nudgePort))
					Thread.Sleep(10000);

				var str = File.ReadAllText(nudgePort);

				var port = Convert.ToUInt16(str);

				Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

				listener.Bind(new IPEndPoint(IPAddress.Any, 45678));
				listener.Listen(5);

				// Start listening for connections.  
				while (true)
				{
					// Program is suspended while waiting for an incoming connection.  
					Socket handler = listener.Accept();
					
					var bytes = new byte[1024];
					int bytesRec = handler.Receive(bytes);
					// Show the data on the console.  
					HandleCommand(Encoding.ASCII.GetString(bytes, 0, bytesRec));
					
					handler.Shutdown(SocketShutdown.Both);
					handler.Close();
				}

			}
			catch (Exception e)
			{
				TGServerService.ActiveService.EventLog.WriteEntry("Nudge handler thread crashed: " + e.ToString(), EventLogEntryType.Error);
			}
		}
	}
}
