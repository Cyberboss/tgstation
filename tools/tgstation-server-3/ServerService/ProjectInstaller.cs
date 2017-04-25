using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace ServerService
{
	[RunInstaller(true)]
	public partial class ProjectInstaller : System.Configuration.Install.Installer
	{
		public ProjectInstaller()
		{
			InitializeComponent();
		}
		
		private void serviceInstaller1_AfterInstall(object sender, InstallEventArgs e)
		{
			using (ServiceController sc = new ServiceController(serviceInstaller1.ServiceName))
			{
				sc.Start();
			}
		}
	}
}
