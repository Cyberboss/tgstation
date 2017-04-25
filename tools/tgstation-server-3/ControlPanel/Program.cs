using System;
using System.Windows.Forms;
using TGServiceInterface;

namespace TGControlPanel
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			var res = Server.VerifyConnection();
			if(res != null)
			{
				MessageBox.Show("Unable to connect to service! Error: " + res);
				return;
			}
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			try
			{
				Application.Run(new Main());
			}
			catch (Exception e)
			{
				MessageBox.Show("An unhandled exception occurred. This usually means we lost connection to the service. Error" + e.ToString());
			}
			finally
			{
				Properties.Settings.Default.Save();
			}
		}
	}
}
