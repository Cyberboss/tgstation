using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace TGControlPanel
{
	public partial class Main : Form
	{
		ContextMenu TrayMenu;
		NotifyIcon TrayIcon;

		public Main()
		{
			InitializeComponent();            // Create a simple tray menu with only one item.
			TrayMenu = new ContextMenu();
			TrayMenu.MenuItems.Add("/tg/station Server", TrayOpen);
			TrayMenu.MenuItems.Add("-");    //seperator
			TrayMenu.MenuItems.Add("Exit", OnExit);

			// Create a tray icon. In this example we use a
			// standard system icon for simplicity, but you
			// can of course use your own custom icon too.
			TrayIcon = new NotifyIcon();
			TrayIcon.Text = "MyTrayApp";
			TrayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
			// Add menu to tray icon and show it.
			TrayIcon.ContextMenu = TrayMenu;
			TrayIcon.Visible = true;
			TrayIcon.DoubleClick += TrayOpen;

			Resize += OnResize;
			FormClosed += OnExit;

			InitRepoPage();
		}
		private void OnResize(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Minimized)
			{
				ShowInTaskbar = false;
			}
			else
			{
				ShowInTaskbar = true;
			}
		}

		private void TrayOpen(object sender, EventArgs e)
		{
			WindowState = FormWindowState.Normal;
		}

		private void OnExit(object sender, EventArgs e)
		{
			Application.Exit();
		}
		

		private void MinimizeButton_Click(object sender, EventArgs e)
		{
			WindowState = FormWindowState.Minimized;
		}

		private void CloseButton_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
