using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TGServiceInterface;

namespace TGControlPanel
{
	static class Program
	{
		[STAThread]
		static void Main(string [] args)
		{
			try
			{
				var res = Server.VerifyConnection();
				if (res != null)
				{
					MessageBox.Show("Unable to connect to service! Error: " + res);
					return;
				}
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new Main());
				return;
			}
			catch (Exception e)
			{
				MessageBox.Show("An unhandled exception occurred. This usually means we lost connection to the service. Error" + e.ToString());
				return;
			}
			finally
			{
				Properties.Settings.Default.Save();
			}
		}
	}
}
