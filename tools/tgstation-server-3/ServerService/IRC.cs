using System;
using System.Collections.Specialized;
using System.Threading;
using TGServiceInterface;
using Meebey.SmartIrc4net;


namespace TGServerService
{

	partial class TGStationServer : ITGIRC
	{
		public static IrcClient irc = new IrcClient() { SupportNonRfc = true };
		int reconnectAttempt = 0;
		public void Setup(string url, ushort port, string username, string password, string[] channels)
		{
			var Config = Properties.Settings.Default;
			if (url != null)
				Config.IRCServer = url;
			if (port != 0)
				Config.IRCPort = port;
			if (username != null)
				Config.IRCNick = username;
			if (password != null)
				Config.IRCPass = password;
			if (channels != null)
			{
				var si = new StringCollection();
				si.AddRange(channels);
				Config.IRCChannels = si;
			}
		}
		public string Connect()
		{
			if (Connected())
				return null;
			try
			{
				//irc.OnChannelMessage += new IrcEventHandler(OnChannelMessage); TODO
				var Config = Properties.Settings.Default;
				try
				{
					irc.Connect(Config.IRCServer, Config.IRCPort);
					reconnectAttempt = 0;
				}
				catch (Exception e)
				{
					reconnectAttempt++;
					if (reconnectAttempt <= 5)
					{
						Thread.Sleep(5000); //Reconnecting after 5 seconds.
						return Connect();
					}
					else
					{
						return "IRC server unreachable: " + e.ToString();
					}
				}

				try
				{
					irc.Login(Config.IRCNick, Config.IRCNick);
				}
				catch (Exception e)
				{
					return "Bot name is already taken: " + e.ToString();
				}

				irc.SendMessage(SendType.Message, "NickServ", "identify " + Config.IRCPass);
				foreach (var I in Config.IRCChannels)
					irc.RfcJoin(I);
				new Thread(new ThreadStart(IRCListen)) { IsBackground = true }.Start();
				Thread.Sleep(5000);	//let it breathe
				return null;
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}

		void IRCListen()
		{
			try
			{
				irc.Listen();
			}
			catch { }
			Disconnect();
		}

		public string Reconnect()
		{
			Disconnect();
			return Connect();
		}

		public void Disconnect()
		{ 
			try
			{
				irc.Disconnect();
			}
			catch
			{ }
		}
		public bool Connected()
		{
			return irc.IsConnected;
		}
		public string SendMessage(string message)
		{
			try
			{
				if (!Connected())
					return "Disconnected";
				irc.SendMessage(SendType.Message, Properties.Settings.Default.IRCChannels[0], message);
				return null;
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}
	}
}
