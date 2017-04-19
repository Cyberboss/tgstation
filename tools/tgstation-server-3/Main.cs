using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace TGStationServer3
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
			FormClosing += OnClosing;
			FormClosed += OnExit;

			MouseDown += Main_MouseDown;

			InitRepoPage();
		}
		#region Form Events
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

		private void OnClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				var DialogResult = MessageBox.Show("Are you sure you want to shutdown the server?", "Confim", MessageBoxButtons.YesNo);
				e.Cancel = DialogResult == DialogResult.No;
			}
		}
		#endregion

		private void TrayOpen(object sender, EventArgs e)
		{
			WindowState = FormWindowState.Normal;
		}

		private void OnExit(object sender, EventArgs e)
		{
			Shutdown();
			Application.Exit();
		}

		//Turns off the server and irc bot
		private void Shutdown()
		{

		}

		private void MinimizeButton_Click(object sender, EventArgs e)
		{
			WindowState = FormWindowState.Minimized;
		}

		private void CloseButton_Click(object sender, EventArgs e)
		{
			Close();
		}
		#region Form Dragging
		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int HT_CAPTION = 0x2;

		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
		[System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
		public static extern bool ReleaseCapture();

		private void Main_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ReleaseCapture();
				SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
			}
		}
		#endregion

	}
}
