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
		public void Setup(string url, ushort port, string username, string password, string[] channels, string adminChannel)
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
			if (adminChannel != null)
				Config.IRCAdminChannel = adminChannel;
			if (channels != null)
			{
				var si = new StringCollection();
				si.AddRange(channels);
				si.Add(Config.IRCAdminChannel);
				Config.IRCChannels = si;
			}
			if(Connected())
				Reconnect();
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
				//because of a bug in smart irc this takes forever and there's nothing we can really do about it 
				//If you want it fixed, get this damn pull request through https://github.com/meebey/SmartIrc4net/pull/31
				irc.Disconnect();
			}
			catch
			{ }
		}
		public bool Connected()
		{
			return irc.IsConnected;
		}
		public string SendMessage(string message, bool adminOnly = false)
		{
			try
			{
				if (!Connected())
					return "Disconnected";
				var Config = Properties.Settings.Default;
				if (adminOnly)
					irc.SendMessage(SendType.Message, Config.IRCAdminChannel, message);
				else
					foreach(var I in Config.IRCChannels)
						irc.SendMessage(SendType.Message, I, message);
				return null;
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}
	}
}
