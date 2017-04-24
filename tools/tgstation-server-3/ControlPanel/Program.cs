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
			Application.Run(new Main());
		}
	}
}
