using System.IO;
using System.ServiceProcess;

namespace TGServerService
{
	public static class Program
	{
		static void Main()
		{
			ServiceBase.Run(new ServiceBase[] { new TGServerService() });	//wondows
		}
		//http://stackoverflow.com/questions/1701457/directory-delete-doesnt-work-access-denied-error-but-under-windows-explorer-it
		public static void DeleteDirectory(string path)
		{
			NormalizeAndDelete(new DirectoryInfo(path));
		}
		static void NormalizeAndDelete(DirectoryInfo dir)
		{
			foreach (var subDir in dir.GetDirectories())
				NormalizeAndDelete(subDir);
			foreach (var file in dir.GetFiles())
			{
				file.Attributes = FileAttributes.Normal;
			}
			dir.Delete(true);
		}
	}
}
