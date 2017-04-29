using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TGServiceInterface;

namespace TGServerService
{
	partial class TGStationServer : ITGConfig
	{
		const string AdminRanksConfig = StaticConfigDir + "/admin_ranks.txt";
		const string AdminConfig = StaticConfigDir + "/admins.txt";


		object configLock = new object();

		public string AddEntry(TGStringConfig type, string entry)
		{
			var currentEntries = GetEntries(type, out string error);
			if (currentEntries == null)
				return error;

			if (currentEntries.Contains(entry))
				return null;

			lock (configLock) {
				try
				{
					using (var f = File.AppendText(StringConfigToPath(type)))
					{
						f.WriteLine(entry);
					}
					return null;
				}
				catch (Exception e)
				{
					return e.ToString();
				}
			}
		}

		string StringConfigToPath(TGStringConfig type)
		{
			var result = StaticConfigDir + "/";
			switch (type)
			{
				case TGStringConfig.Admin_NickNames:
					result += "Admin_NickNames";
					break;
				case TGStringConfig.Silicon_Laws:
					result += "Silicon_Laws";
					break;
				case TGStringConfig.SillyTips:
					result += "SillyTips";
					break;
				case TGStringConfig.Tips:
					result += "Tips";
					break;
				case TGStringConfig.Whitelist:
					result += "Whitelist";
					break;
			}
			return result + ".txt";
		}

		public string Addmin(string ckey, string rank)
		{
			var Aranks = AdminRanks(out string error);
			if (Aranks != null)
			{
				if (Aranks.Keys.Contains(rank))
				{
					var current_mins = Admins(out error);
					if (current_mins != null)
					{
						current_mins[ckey] = rank;

						string outText = "";
						foreach(var I in current_mins)
							outText += I.Key + " = " + I.Value + "\r\n";

						try
						{
							lock (configLock)
							{
								File.WriteAllText(AdminConfig, outText);
							}
							return null;
						}
						catch (Exception e)
						{
							return e.ToString();
						}
					}
					return error;
				}
				return "Rank " + rank + " does not exist";
			}
			return error;
		}

		public IDictionary<string, IList<TGPermissions>> AdminRanks(out string error)
		{

			List<string> fileLines;
			lock (configLock)
			{
				try
				{
					fileLines = new List<string>(File.ReadAllLines(AdminRanksConfig));
				}
				catch (Exception e)
				{
					error = e.ToString();
					return null;
				}
			}

			var result = new Dictionary<string, IList<TGPermissions>>();
			IList<TGPermissions> previousPermissions = new List<TGPermissions>();
			foreach (var L in fileLines)
			{
				if (L.Length > 0 && L[0] == '#')
					continue;

				var splits = L.Split('=');

				if (splits.Length < 2)  //???
					continue;

				var rank = splits[0].Trim();

				var asList = new List<string>(splits);
				asList.RemoveAt(0);

				var perms = ProcessPermissions(asList, previousPermissions);
				result.Add(rank, perms);
				previousPermissions = perms;
			}
			error = null;
			return result;
		}

		IList<TGPermissions> ProcessPermissions(IList<string> text, IList<TGPermissions> previousPermissions)
		{
			IList<TGPermissions> permissions = new List<TGPermissions>();
			foreach(var E in text)
			{
				var trimmed = E.Trim();
				bool removing;
				switch (trimmed[0])
				{
					case '-':
						removing = true;
						break;
					case '+':
					default:
						removing = false;
						break;
				}
				trimmed = trimmed.Substring(1).ToLower();

				if (trimmed.Length == 0)
					continue;

				var perms = StringToPermission(trimmed, previousPermissions);

				if(perms != null)
					foreach(var perm in perms)
					{
						if (removing)
							permissions.Remove(perm);
						else if (!permissions.Contains(perm))
							permissions.Add(perm);
					}

			}
			return permissions;
		}

		IList<TGPermissions> StringToPermission(string permstring, IList<TGPermissions> oldpermissions)
		{
			TGPermissions perm;
			switch (permstring)
			{
				case "@":
				case "prev":
					return oldpermissions;
				case "buildmode":
				case "build":
					perm = TGPermissions.BUILD;
					break;
				case "admin":
					perm = TGPermissions.ADMIN;
					break;
				case "ban":
					perm = TGPermissions.BAN;
					break;
				case "fun":
					perm = TGPermissions.FUN;
					break;
				case "server":
					perm = TGPermissions.SERVER;
					break;
				case "debug":
					perm = TGPermissions.DEBUG;
					break;
				case "permissions":
				case "rights":
					perm = TGPermissions.RIGHTS;
					break;
				case "possess":
					perm = TGPermissions.POSSESS;
					break;
				case "stealth":
					perm = TGPermissions.STEALTH;
					break;
				case "rejuv":
				case "rejuvinate":
					perm = TGPermissions.REJUV;
					break;
				case "varedit":
					perm = TGPermissions.VAREDIT;
					break;
				case "everything":
				case "host":
				case "all":
					return new List<TGPermissions> {
						TGPermissions.ADMIN,
						TGPermissions.SPAWN,
						TGPermissions.FUN,
						TGPermissions.BAN,
						TGPermissions.STEALTH,
						TGPermissions.POSSESS,
						TGPermissions.REJUV,
						TGPermissions.BUILD,
						TGPermissions.SERVER,
						TGPermissions.DEBUG,
						TGPermissions.VAREDIT,
						TGPermissions.RIGHTS,
						TGPermissions.SOUND,
					};
				case "sound":
				case "sounds":
					perm = TGPermissions.SOUND;
					break;
				case "spawn":
				case "create":
					perm = TGPermissions.SPAWN;
					break;
				default:
					return null;
			}
			return new List<TGPermissions> { perm };
		}

		public IDictionary<string, string> Admins(out string error)
		{
			throw new NotImplementedException();
		}

		public string Deadmin(string admin)
		{
			throw new NotImplementedException();
		}

		public IList<string> GetEntries(TGStringConfig type, out string error)
		{
			throw new NotImplementedException();
		}

		public IList<JobSetting> Jobs(out string error)
		{
			throw new NotImplementedException();
		}

		public IList<MapEnabled> Maps(TGMapListType type, out string error)
		{
			throw new NotImplementedException();
		}

		public string MoveServer(string new_location)
		{
			throw new NotImplementedException();
		}

		public ushort NudgePort(out string error)
		{
			throw new NotImplementedException();
		}

		public string RemoveEntry(TGStringConfig type, string entry)
		{
			throw new NotImplementedException();
		}

		public IList<ConfigSetting> Retrieve(TGConfigType type, out string error)
		{
			throw new NotImplementedException();
		}

		public string ServerDirectory(out string error)
		{
			throw new NotImplementedException();
		}

		public string SetItem(TGConfigType type, string newValue)
		{
			throw new NotImplementedException();
		}

		public string SetJob(JobSetting job)
		{
			throw new NotImplementedException();
		}

		public string SetMap(TGMapListType type, MapEnabled mapfile)
		{
			throw new NotImplementedException();
		}

		public string SetNudgePort(ushort port)
		{
			throw new NotImplementedException();
		}
	}
}
