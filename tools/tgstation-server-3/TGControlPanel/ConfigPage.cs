using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TGServiceInterface;

namespace TGControlPanel
{

	partial class Main
	{
		const int ConfigConfig = 0;

		List<ConfigSetting> GeneralChangelist = new List<ConfigSetting>(),
			DatabaseChangelist = new List<ConfigSetting>(),
			GameChangelist = new List<ConfigSetting>();



		FlowLayoutPanel ConfigConfigFlow;
		void LoadConfig()
		{
			ConfigConfigFlow = new FlowLayoutPanel()
			{
				AutoSize = true,
				MaximumSize = new Size(ConfigConfigPanel.Width - 90, 9999999),
				FlowDirection = FlowDirection.TopDown,
			};
			ConfigConfigPanel.Controls.Add(ConfigConfigFlow);
			LoadConfigConfig();
			Resize += ReadjustFlow;
		}

		private void ReadjustFlow(object sender, EventArgs e)
		{
			ConfigConfigFlow.MaximumSize = new Size(ConfigConfigPanel.Width - 90, 9999999);
		}

		public void LoadConfigConfig()
		{

			GeneralChangelist = new List<ConfigSetting>();
			ConfigConfigFlow.Controls.Clear();
			ConfigConfigFlow.SuspendLayout();

			var ConfigConfigEntries = Server.GetComponent<ITGConfig>().Retrieve(TGConfigType.General, out string error);
			if (ConfigConfigEntries != null)
				foreach (var I in ConfigConfigEntries)
					HandleConfigEntry(I, ConfigConfigFlow, GeneralChangelist, TGConfigType.General);
			else
				ConfigConfigPanel.Controls.Add(new Label() { Text = "Unable to load config.txt!" });

			ConfigConfigFlow.ResumeLayout();
		}

		private void ConfigRefresh_Click(object sender, System.EventArgs e)
		{
			RefreshCurrentPage();
		}

		private void RefreshCurrentPage()
		{
			switch (ConfigPanels.SelectedIndex)
			{
				case ConfigConfig:
					LoadConfigConfig();
					break;
			}
		}

		private void ConfigApply_Click(object sender, System.EventArgs e)
		{
			var Config = Server.GetComponent<ITGConfig>();
			switch (ConfigPanels.SelectedIndex)
			{
				case ConfigConfig:
					for(var I = 0; I < GeneralChangelist.Count; ++I)
					{
						var error = Config.SetItem(TGConfigType.General, GeneralChangelist[I]);
						if (error != null)
						{
							MessageBox.Show("An error occurred: {1}" + error);
							break;
						}
					}
					GeneralChangelist.Clear();
					RefreshCurrentPage();
					break;
			}
		}

		private void ConfigUpload_Click(object sender, EventArgs eva)
		{
			var ofd = new OpenFileDialog()
			{
				CheckFileExists = true,
				CheckPathExists = true,
				DefaultExt = ".txt",
				Multiselect = false,
				Title = "Config Upload",
				ValidateNames = true,
				Filter = "Text files (*.txt)|*.txt|PNG files (*.png)|*.png|All files (*.*)|*.*",
				AddExtension = false,
				SupportMultiDottedExtensions = true,
			};
			if (ofd.ShowDialog() != DialogResult.OK)
				return;

			var fileToUpload = ofd.FileName;

			var originalFileName = Program.TextPrompt("Config Upload", "Enter the path of the destination file in the config folder:");
			if (originalFileName == null)
				return;

			string fileContents = null;
			string error = null;
			try
			{
				fileContents = File.ReadAllText(fileToUpload);
			} catch (Exception e)
			{
				error = e.ToString();
			}
			if (error == null)
				error = Server.GetComponent<ITGConfig>().WriteRaw(originalFileName, fileContents);
			if (error != null)
				MessageBox.Show("An error occurred: " + error);
		}

		private void DownloadConfig(string remotePath, bool repo)
		{
			if (remotePath == null)
				return;
			var text = Server.GetComponent<ITGConfig>().ReadRaw(remotePath, repo, out string error);
			if (text != null)
			{

				var ofd = new SaveFileDialog()
				{
					CheckFileExists = false,
					CheckPathExists = true,
					DefaultExt = ".txt",
					Title = "Config Download",
					ValidateNames = true,
					Filter = "Text files (*.txt)|*.txt|PNG files (*.png)|*.png|All files (*.*)|*.*",
					AddExtension = false,
					CreatePrompt = false,
					OverwritePrompt = true,
					SupportMultiDottedExtensions = true,
				};
				if (ofd.ShowDialog() != DialogResult.OK)
					return;

				try
				{
					File.WriteAllText(ofd.FileName, text);
					return;
				}
				catch (Exception e)
				{
					error = e.ToString();
				}
			}
			MessageBox.Show("An error occurred: " + error);
		}

		private void ConfigDownload_Click(object sender, EventArgs eva)
		{
			DownloadConfig(Program.TextPrompt("Config Download", "Enter the path of the source file in the config folder:"), false);
		}

		private void ConfigDownloadRepo_Click(object sender, EventArgs e)
		{
			DownloadConfig(Program.TextPrompt("Repo Config Download", "Enter the path of the source file in the repository's config folder:"), true);
		}

		void HandleConfigEntry(ConfigSetting setting, FlowLayoutPanel flow, IList<ConfigSetting> changelist, TGConfigType type)
		{
			flow.Controls.Add(new Label()
			{
				Text = setting.Name + (setting.ExistsInRepo ? "" : " (Does not exist in repository!)"),
				AutoSize = true,
				Font = new Font("Verdana", 10.0f),
				ForeColor = Color.FromArgb(248, 248, 242)
			});

			if (setting.ExistsInRepo)
				flow.Controls.Add(new Label()
				{
					AutoSize = true,
					Font = new Font("Verdana", 8.0f),
					ForeColor = Color.FromArgb(248, 248, 242),
					Text = setting.Comment
				});
			
			var IsSwitch = setting.DefaultValue == "" || setting.DefaultValue == null;

			if (!IsSwitch || !setting.ExistsInRepo)
				flow.Controls.Add(new ConfigAddRemoveButton(setting, this, type));			

			if(IsSwitch || setting.ExistsInStatic)
				flow.Controls.Add(IsSwitch ? (Control)new ConfigCheckBox(setting, changelist) : new ConfigTextBox(setting, changelist));

			flow.Controls.Add(new Label()); //line break
		}
	}
}
