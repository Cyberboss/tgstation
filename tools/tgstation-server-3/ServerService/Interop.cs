using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.Security;

namespace TGServerService
{
	partial class TGStationServer
	{
		QueuedLock topicLock = new QueuedLock();
		Socket topicSender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) { SendTimeout = 5000, ReceiveTimeout = 5000 };

		const int CommsKeyLen = 64;
		string serviceCommsKey;	//regenerated every DD restart
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
				case "killme":
					Restart();
					break;
			}
		}

		string SendCommand(string cmd)
		{
			lock (watchdogLock)
			{
				return SendTopic(String.Format("serviceCommsKey={0};command={1}", serviceCommsKey, cmd), currentPort);
			}
		}

		string SendTopic(string topicdata, ushort port, bool retry = false)
		{
			if (!retry)
				topicLock.Enter();
			try
			{
				if (!topicSender.Connected)
					topicSender.Connect(IPAddress.Loopback, port);

				//magic numbers everywhere
				var bytes = new List<byte> { 0x83 };

				StringBuilder stringPacket = new StringBuilder();
				stringPacket.Append((char)'\x00', 8); //packet[1] is 0x83, packet[3] contain length
				stringPacket.Append('?' + topicdata);
				stringPacket.Append((char)'\x00');
				string fullString = stringPacket.ToString();
				var packet = Encoding.ASCII.GetBytes(fullString);
				packet[1] = 0x83;
				packet[3] = (byte)(packet.Length - 4);

				topicSender.Send(packet);

				string returnedString = "NULL";
				try
				{
					var returnedData = new byte[512];
					topicSender.Receive(returnedData);
					var raw_string = Encoding.ASCII.GetString(returnedData);
					if (raw_string.Length > 6)
						returnedString = raw_string.Substring(5, raw_string.Length - 6);
				}
				catch {
					returnedString = "Topic recieve error!";
				}

				TGServerService.ActiveService.EventLog.WriteEntry("Topic: \"" + topicdata + "\" Returned: " + returnedString);
				return returnedString;
			}
			catch
			{
				topicSender.Disconnect(true);
				if (!retry)
					return SendTopic(topicdata, port, true);
				else
				{
					TGServerService.ActiveService.EventLog.WriteEntry("Failed to send topic: " + topicdata, EventLogEntryType.Error);
					return "Topic delivery failed!";
				}
			}
			finally
			{
				topicLock.Exit();
			}
		}

		void GenCommsKey()
		{
			var charsToRemove = new string[] { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "-", "+", "=", "[", "{", "]", "}", ";", ":", "<", ">", "|", ".", "/", "?" };
			serviceCommsKey = String.Empty;
			do {
				var tmp = Membership.GeneratePassword(CommsKeyLen, 0);
				foreach (var c in charsToRemove)
					tmp = tmp.Replace(c, String.Empty);
				serviceCommsKey += tmp;
			} while (serviceCommsKey.Length < CommsKeyLen);
			serviceCommsKey = serviceCommsKey.Substring(0, CommsKeyLen);
			TGServerService.ActiveService.EventLog.WriteEntry("Service Comms Key set to: " + serviceCommsKey);
		}

		void InitInterop()
		{
			new Thread(new ThreadStart(NudgeHandler)) { IsBackground = true }.Start();
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
