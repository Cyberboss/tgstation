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
			Width = 560;
			Height *= 3;
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
			Font = new Font("Verdana", 8.0f);
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

	class ConfigAddRemoveButton : Button
	{
		ConfigSetting Setting;
		Main main;
		TGConfigType type;
		bool remove;
		public ConfigAddRemoveButton(ConfigSetting c, Main m, TGConfigType t)
		{
			Setting = c;
			main = m;
			type = t;
			Click += ConfigAddRemoveButton_Click;
			UseVisualStyleBackColor = true;
			remove = Setting.ExistsInStatic || !Setting.ExistsInRepo;
			Text = remove ? "Remove" : "Add";
		}

		private void ConfigAddRemoveButton_Click(object sender, EventArgs e)
		{
			if (remove)
			{
				Setting.Values = Setting.DefaultValues;
				Setting.Value = null;
			}
			else
			{
				Setting.Value = Setting.DefaultValue;
				Setting.Values = Setting.DefaultValues;
			}

			var Result = Server.GetComponent<ITGConfig>().SetItem(type, Setting);
			if (Result != null)
				MessageBox.Show("Error: " + Result);

			switch (type)
			{
				case TGConfigType.General:
					main.LoadConfigConfig();
					break;
				case TGConfigType.Game:
				case TGConfigType.Database:
				default:
					throw new NotImplementedException();
			}

		}
	}

	partial class Main
	{
		List<ConfigSetting> GeneralChangelist;
		FlowLayoutPanel ConfigConfigFlow;
		void LoadConfig()
		{
			ConfigConfigFlow = new FlowLayoutPanel()
			{
				AutoSize = true,
				MaximumSize = new Size(ConfigConfigPanel.Width - 30, 9999999),
				FlowDirection = FlowDirection.TopDown,
			};
			ConfigConfigPanel.Controls.Add(ConfigConfigFlow);
			LoadConfigConfig();
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

		private void ConfigConfigRefresh_Click(object sender, System.EventArgs e)
		{
			LoadConfigConfig();
		}

		private void ConfigConfigApply_Click(object sender, System.EventArgs e)
		{

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
