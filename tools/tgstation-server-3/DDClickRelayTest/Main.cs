using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace DDClickRelayTest
{
	public partial class Main : Form
	{
		DreamDaemon DD;
		public Main()
		{
			InitializeComponent();
			DD = new DreamDaemon();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			//try and click "File"
			DD.ChangeVisibility(DreamDaemon.Visibility.Invisible);
		}
		private void MainDispose()
		{
			if (DD != null)
			{
				DD.Dispose();
				DD = null;
			}
		}
		bool ready = false;
		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			if(ready)
				Size = new Size((int)numericUpDown1.Value, (int)numericUpDown2.Value);
		}

		private void numericUpDown2_ValueChanged(object sender, EventArgs e)
		{
			if (ready)
				Size = new Size((int)numericUpDown1.Value, (int)numericUpDown2.Value);
		}

		private void Main_Load(object sender, EventArgs e)
		{
			numericUpDown1.Maximum = 99999;
			numericUpDown2.Maximum = 99999;
			numericUpDown1.Value = Size.Width;
			numericUpDown2.Value = Size.Height;
			ready = true;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
