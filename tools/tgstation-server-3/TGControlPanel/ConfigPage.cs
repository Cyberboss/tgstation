using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TGServiceInterface;

namespace TGControlPanel
{
	class ConfigTextBox : TextBox
	{
		IList<ConfigSetting> ChangeList;
		ConfigSetting Setting;
		public ConfigTextBox(ConfigSetting c, IList<ConfigSetting> cl)
		{
			Setting = c;
			TextChanged += ConfigTextbox_TextChanged;
			ChangeList = cl;
			Text = Setting.Value;
			Multiline = true;
			Width = 300;
			Height *= 2;
			ScrollBars = ScrollBars.Both;
		}

		private void ConfigTextbox_TextChanged(object sender, EventArgs e)
		{
			Setting.Value = Text;
			if (!ChangeList.Contains(Setting))
				ChangeList.Add(Setting);
		}
	}

	class ConfigCheckBox : CheckBox
	{
		IList<ConfigSetting> ChangeList;
		ConfigSetting Setting;
		public ConfigCheckBox(ConfigSetting c, IList<ConfigSetting> cl)
		{
			Setting = c;
			CheckStateChanged += ConfigCheckBox_CheckStateChanged;
			ChangeList = cl;
			Text = "Enabled";
			Font = new Font("Verdana", 10.0f);
			ForeColor = Color.FromArgb(248, 248, 242);
			Checked = Setting.Value != null;
		}

		private void ConfigCheckBox_CheckStateChanged(object sender, EventArgs e)
		{
			Setting.Value = Checked ? "" : null;
			if (!ChangeList.Contains(Setting)) 
				ChangeList.Add(Setting);
		}
	}

	partial class Main
	{
		List<ConfigSetting> GeneralChangelist;
		void LoadConfig()
		{

			GeneralChangelist = new List<ConfigSetting>();
			var Config = Server.GetComponent<ITGConfig>();
			ConfigConfigPanel.Controls.Clear();
			var ConfigConfigFlow = new FlowLayoutPanel()
			{
				AutoSize = true,
				MaximumSize = new Size(ConfigConfigPanel.Width, 9999999),
				FlowDirection = FlowDirection.TopDown,
			};
			ConfigConfigPanel.Controls.Add(ConfigConfigFlow);

			var ConfigConfigEntries = Config.Retrieve(TGConfigType.General, out string error);
			if (ConfigConfigEntries != null)
				foreach(var I in ConfigConfigEntries)
				{
					HandleConfigEntry(I, ConfigConfigFlow, GeneralChangelist);
				}
			else
				ConfigConfigPanel.Controls.Add(new Label() { Text = "Unable to load config.txt!" }); 
			
		}

		void HandleConfigEntry(ConfigSetting setting, FlowLayoutPanel flow, IList<ConfigSetting> changelist)
		{
			flow.Controls.Add(new Label()
			{
				Text = setting.Name + (setting.ExistsInRepo ? "" : " (Does not exist in repository!)"),
				AutoSize = true,
				Font = new Font("Verdana", 12.0f),
				ForeColor = Color.FromArgb(248, 248, 242)
			});

			if (setting.ExistsInRepo)
				flow.Controls.Add(new Label()
				{
					AutoSize = true,
					Font = new Font("Verdana", 10.0f),
					ForeColor = Color.FromArgb(248, 248, 242),
					Text = setting.Comment
				});
			
			var IsSwitch = setting.DefaultValue == "" || setting.DefaultValue == null;
			
			flow.Controls.Add(IsSwitch ? (Control)new ConfigCheckBox(setting, changelist) : new ConfigTextBox(setting, changelist));

			flow.Controls.Add(new Label()); //line break
		}
	}
}
