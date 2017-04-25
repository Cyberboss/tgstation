namespace TGControlPanel
{
	partial class Main
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.Panels = new System.Windows.Forms.TabControl();
            this.RepoPanel = new System.Windows.Forms.TabPage();
            this.TestMergeListLabel = new System.Windows.Forms.TextBox();
            this.CurrentRevisionLabel = new System.Windows.Forms.Label();
            this.RepoEmailTextBox = new System.Windows.Forms.TextBox();
            this.RepoCommitterNameTextBox = new System.Windows.Forms.TextBox();
            this.RepoApplyButton = new System.Windows.Forms.Button();
            this.RepoBranchTextBox = new System.Windows.Forms.TextBox();
            this.RepoRemoteTextBox = new System.Windows.Forms.TextBox();
            this.HardReset = new System.Windows.Forms.Button();
            this.UpdateRepoButton = new System.Windows.Forms.Button();
            this.TestMergeButton = new System.Windows.Forms.Button();
            this.CommitterEmailTitle = new System.Windows.Forms.Label();
            this.CommiterNameTitle = new System.Windows.Forms.Label();
            this.IdentityLabel = new System.Windows.Forms.Label();
            this.TestMergeListTitle = new System.Windows.Forms.Label();
            this.RemoteNameTitle = new System.Windows.Forms.Label();
            this.BranchNameTitle = new System.Windows.Forms.Label();
            this.CurrentRevisionTitle = new System.Windows.Forms.Label();
            this.CloneRepositoryButton = new System.Windows.Forms.Button();
            this.RepoProgressBarLabel = new System.Windows.Forms.Label();
            this.RepoProgressBar = new System.Windows.Forms.ProgressBar();
            this.BYONDPanel = new System.Windows.Forms.TabPage();
            this.StagedVersionLabel = new System.Windows.Forms.Label();
            this.StagedVersionTitle = new System.Windows.Forms.Label();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.VersionTitle = new System.Windows.Forms.Label();
            this.MinorVersionLabel = new System.Windows.Forms.Label();
            this.MajorVersionLabel = new System.Windows.Forms.Label();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.MinorVersionNumeric = new System.Windows.Forms.NumericUpDown();
            this.MajorVersionNumeric = new System.Windows.Forms.NumericUpDown();
            this.UpdateProgressBar = new System.Windows.Forms.ProgressBar();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.RepoBGW = new System.ComponentModel.BackgroundWorker();
            this.BYONDTimer = new System.Windows.Forms.Timer(this.components);
            this.Panels.SuspendLayout();
            this.RepoPanel.SuspendLayout();
            this.BYONDPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MinorVersionNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MajorVersionNumeric)).BeginInit();
            this.SuspendLayout();
            // 
            // Panels
            // 
            this.Panels.Controls.Add(this.RepoPanel);
            this.Panels.Controls.Add(this.BYONDPanel);
            this.Panels.Controls.Add(this.tabPage2);
            this.Panels.Controls.Add(this.tabPage3);
            this.Panels.Location = new System.Drawing.Point(12, 12);
            this.Panels.Name = "Panels";
            this.Panels.SelectedIndex = 0;
            this.Panels.Size = new System.Drawing.Size(876, 392);
            this.Panels.TabIndex = 3;
            // 
            // RepoPanel
            // 
            this.RepoPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(40)))), ((int)(((byte)(34)))));
            this.RepoPanel.Controls.Add(this.TestMergeListLabel);
            this.RepoPanel.Controls.Add(this.CurrentRevisionLabel);
            this.RepoPanel.Controls.Add(this.RepoEmailTextBox);
            this.RepoPanel.Controls.Add(this.RepoCommitterNameTextBox);
            this.RepoPanel.Controls.Add(this.RepoApplyButton);
            this.RepoPanel.Controls.Add(this.RepoBranchTextBox);
            this.RepoPanel.Controls.Add(this.RepoRemoteTextBox);
            this.RepoPanel.Controls.Add(this.HardReset);
            this.RepoPanel.Controls.Add(this.UpdateRepoButton);
            this.RepoPanel.Controls.Add(this.TestMergeButton);
            this.RepoPanel.Controls.Add(this.CommitterEmailTitle);
            this.RepoPanel.Controls.Add(this.CommiterNameTitle);
            this.RepoPanel.Controls.Add(this.IdentityLabel);
            this.RepoPanel.Controls.Add(this.TestMergeListTitle);
            this.RepoPanel.Controls.Add(this.RemoteNameTitle);
            this.RepoPanel.Controls.Add(this.BranchNameTitle);
            this.RepoPanel.Controls.Add(this.CurrentRevisionTitle);
            this.RepoPanel.Controls.Add(this.CloneRepositoryButton);
            this.RepoPanel.Controls.Add(this.RepoProgressBarLabel);
            this.RepoPanel.Controls.Add(this.RepoProgressBar);
            this.RepoPanel.Location = new System.Drawing.Point(4, 22);
            this.RepoPanel.Name = "RepoPanel";
            this.RepoPanel.Padding = new System.Windows.Forms.Padding(3);
            this.RepoPanel.Size = new System.Drawing.Size(868, 366);
            this.RepoPanel.TabIndex = 0;
            this.RepoPanel.Text = "Repository";
            // 
            // TestMergeListLabel
            // 
            this.TestMergeListLabel.Location = new System.Drawing.Point(184, 264);
            this.TestMergeListLabel.Multiline = true;
            this.TestMergeListLabel.Name = "TestMergeListLabel";
            this.TestMergeListLabel.ReadOnly = true;
            this.TestMergeListLabel.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TestMergeListLabel.Size = new System.Drawing.Size(499, 96);
            this.TestMergeListLabel.TabIndex = 21;
            // 
            // CurrentRevisionLabel
            // 
            this.CurrentRevisionLabel.AutoSize = true;
            this.CurrentRevisionLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentRevisionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.CurrentRevisionLabel.Location = new System.Drawing.Point(162, 14);
            this.CurrentRevisionLabel.Name = "CurrentRevisionLabel";
            this.CurrentRevisionLabel.Size = new System.Drawing.Size(82, 18);
            this.CurrentRevisionLabel.TabIndex = 20;
            this.CurrentRevisionLabel.Text = "Unknown";
            this.CurrentRevisionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CurrentRevisionLabel.Visible = false;
            // 
            // RepoEmailTextBox
            // 
            this.RepoEmailTextBox.Location = new System.Drawing.Point(90, 217);
            this.RepoEmailTextBox.Name = "RepoEmailTextBox";
            this.RepoEmailTextBox.Size = new System.Drawing.Size(535, 20);
            this.RepoEmailTextBox.TabIndex = 19;
            this.RepoEmailTextBox.Visible = false;
            // 
            // RepoCommitterNameTextBox
            // 
            this.RepoCommitterNameTextBox.Location = new System.Drawing.Point(90, 191);
            this.RepoCommitterNameTextBox.Name = "RepoCommitterNameTextBox";
            this.RepoCommitterNameTextBox.Size = new System.Drawing.Size(535, 20);
            this.RepoCommitterNameTextBox.TabIndex = 18;
            this.RepoCommitterNameTextBox.Visible = false;
            // 
            // RepoApplyButton
            // 
            this.RepoApplyButton.Location = new System.Drawing.Point(722, 331);
            this.RepoApplyButton.Name = "RepoApplyButton";
            this.RepoApplyButton.Size = new System.Drawing.Size(140, 29);
            this.RepoApplyButton.TabIndex = 17;
            this.RepoApplyButton.Text = "Apply Changes";
            this.RepoApplyButton.UseVisualStyleBackColor = true;
            this.RepoApplyButton.Visible = false;
            this.RepoApplyButton.Click += new System.EventHandler(this.RepoApplyButton_Click);
            // 
            // RepoBranchTextBox
            // 
            this.RepoBranchTextBox.Location = new System.Drawing.Point(122, 89);
            this.RepoBranchTextBox.Name = "RepoBranchTextBox";
            this.RepoBranchTextBox.Size = new System.Drawing.Size(535, 20);
            this.RepoBranchTextBox.TabIndex = 15;
            this.RepoBranchTextBox.Visible = false;
            // 
            // RepoRemoteTextBox
            // 
            this.RepoRemoteTextBox.Location = new System.Drawing.Point(122, 57);
            this.RepoRemoteTextBox.Name = "RepoRemoteTextBox";
            this.RepoRemoteTextBox.Size = new System.Drawing.Size(535, 20);
            this.RepoRemoteTextBox.TabIndex = 14;
            this.RepoRemoteTextBox.Visible = false;
            // 
            // HardReset
            // 
            this.HardReset.Location = new System.Drawing.Point(722, 81);
            this.HardReset.Name = "HardReset";
            this.HardReset.Size = new System.Drawing.Size(140, 29);
            this.HardReset.TabIndex = 13;
            this.HardReset.Text = "Reset To Remote";
            this.HardReset.UseVisualStyleBackColor = true;
            this.HardReset.Visible = false;
            this.HardReset.Click += new System.EventHandler(this.HardReset_Click);
            // 
            // UpdateRepoButton
            // 
            this.UpdateRepoButton.Location = new System.Drawing.Point(722, 11);
            this.UpdateRepoButton.Name = "UpdateRepoButton";
            this.UpdateRepoButton.Size = new System.Drawing.Size(140, 29);
            this.UpdateRepoButton.TabIndex = 12;
            this.UpdateRepoButton.Text = "Merge from Remote";
            this.UpdateRepoButton.UseVisualStyleBackColor = true;
            this.UpdateRepoButton.Visible = false;
            this.UpdateRepoButton.Click += new System.EventHandler(this.UpdateRepoButton_Click);
            // 
            // TestMergeButton
            // 
            this.TestMergeButton.Location = new System.Drawing.Point(722, 46);
            this.TestMergeButton.Name = "TestMergeButton";
            this.TestMergeButton.Size = new System.Drawing.Size(140, 29);
            this.TestMergeButton.TabIndex = 11;
            this.TestMergeButton.Text = "Test Merge PR";
            this.TestMergeButton.UseVisualStyleBackColor = true;
            this.TestMergeButton.Visible = false;
            this.TestMergeButton.Click += new System.EventHandler(this.TestMergeButton_Click);
            // 
            // CommitterEmailTitle
            // 
            this.CommitterEmailTitle.AutoSize = true;
            this.CommitterEmailTitle.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommitterEmailTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.CommitterEmailTitle.Location = new System.Drawing.Point(6, 217);
            this.CommitterEmailTitle.Name = "CommitterEmailTitle";
            this.CommitterEmailTitle.Size = new System.Drawing.Size(67, 18);
            this.CommitterEmailTitle.TabIndex = 10;
            this.CommitterEmailTitle.Text = "E-mail:";
            this.CommitterEmailTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CommitterEmailTitle.Visible = false;
            // 
            // CommiterNameTitle
            // 
            this.CommiterNameTitle.AutoSize = true;
            this.CommiterNameTitle.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommiterNameTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.CommiterNameTitle.Location = new System.Drawing.Point(6, 191);
            this.CommiterNameTitle.Name = "CommiterNameTitle";
            this.CommiterNameTitle.Size = new System.Drawing.Size(62, 18);
            this.CommiterNameTitle.TabIndex = 9;
            this.CommiterNameTitle.Text = "Name:";
            this.CommiterNameTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.CommiterNameTitle.Visible = false;
            // 
            // IdentityLabel
            // 
            this.IdentityLabel.AutoSize = true;
            this.IdentityLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IdentityLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.IdentityLabel.Location = new System.Drawing.Point(6, 142);
            this.IdentityLabel.Name = "IdentityLabel";
            this.IdentityLabel.Size = new System.Drawing.Size(257, 18);
            this.IdentityLabel.TabIndex = 8;
            this.IdentityLabel.Text = "Changelog Committer Identity";
            this.IdentityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.IdentityLabel.Visible = false;
            // 
            // TestMergeListTitle
            // 
            this.TestMergeListTitle.AutoSize = true;
            this.TestMergeListTitle.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TestMergeListTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.TestMergeListTitle.Location = new System.Drawing.Point(6, 266);
            this.TestMergeListTitle.Name = "TestMergeListTitle";
            this.TestMergeListTitle.Size = new System.Drawing.Size(169, 18);
            this.TestMergeListTitle.TabIndex = 6;
            this.TestMergeListTitle.Text = "Active Test Merges:";
            this.TestMergeListTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.TestMergeListTitle.Visible = false;
            // 
            // RemoteNameTitle
            // 
            this.RemoteNameTitle.AutoSize = true;
            this.RemoteNameTitle.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RemoteNameTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.RemoteNameTitle.Location = new System.Drawing.Point(6, 57);
            this.RemoteNameTitle.Name = "RemoteNameTitle";
            this.RemoteNameTitle.Size = new System.Drawing.Size(78, 18);
            this.RemoteNameTitle.TabIndex = 5;
            this.RemoteNameTitle.Text = "Remote:";
            this.RemoteNameTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.RemoteNameTitle.Visible = false;
            // 
            // BranchNameTitle
            // 
            this.BranchNameTitle.AutoSize = true;
            this.BranchNameTitle.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BranchNameTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.BranchNameTitle.Location = new System.Drawing.Point(6, 89);
            this.BranchNameTitle.Name = "BranchNameTitle";
            this.BranchNameTitle.Size = new System.Drawing.Size(110, 18);
            this.BranchNameTitle.TabIndex = 4;
            this.BranchNameTitle.Text = "Branch/SHA:";
            this.BranchNameTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BranchNameTitle.Visible = false;
            // 
            // CurrentRevisionTitle
            // 
            this.CurrentRevisionTitle.AutoSize = true;
            this.CurrentRevisionTitle.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentRevisionTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.CurrentRevisionTitle.Location = new System.Drawing.Point(6, 14);
            this.CurrentRevisionTitle.Name = "CurrentRevisionTitle";
            this.CurrentRevisionTitle.Size = new System.Drawing.Size(150, 18);
            this.CurrentRevisionTitle.TabIndex = 3;
            this.CurrentRevisionTitle.Text = "Current Revision:";
            this.CurrentRevisionTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CurrentRevisionTitle.Visible = false;
            // 
            // CloneRepositoryButton
            // 
            this.CloneRepositoryButton.Location = new System.Drawing.Point(311, 191);
            this.CloneRepositoryButton.Name = "CloneRepositoryButton";
            this.CloneRepositoryButton.Size = new System.Drawing.Size(229, 34);
            this.CloneRepositoryButton.TabIndex = 2;
            this.CloneRepositoryButton.Text = "Clone Repository";
            this.CloneRepositoryButton.UseVisualStyleBackColor = true;
            this.CloneRepositoryButton.Visible = false;
            this.CloneRepositoryButton.Click += new System.EventHandler(this.CloneRepositoryButton_Click);
            // 
            // RepoProgressBarLabel
            // 
            this.RepoProgressBarLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RepoProgressBarLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.RepoProgressBarLabel.Location = new System.Drawing.Point(184, 142);
            this.RepoProgressBarLabel.Name = "RepoProgressBarLabel";
            this.RepoProgressBarLabel.Size = new System.Drawing.Size(499, 46);
            this.RepoProgressBarLabel.TabIndex = 1;
            this.RepoProgressBarLabel.Text = "Searching for Repository...";
            this.RepoProgressBarLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RepoProgressBar
            // 
            this.RepoProgressBar.Location = new System.Drawing.Point(184, 191);
            this.RepoProgressBar.Name = "RepoProgressBar";
            this.RepoProgressBar.Size = new System.Drawing.Size(499, 23);
            this.RepoProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.RepoProgressBar.TabIndex = 0;
            // 
            // BYONDPanel
            // 
            this.BYONDPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(40)))), ((int)(((byte)(34)))));
            this.BYONDPanel.Controls.Add(this.StagedVersionLabel);
            this.BYONDPanel.Controls.Add(this.StagedVersionTitle);
            this.BYONDPanel.Controls.Add(this.StatusLabel);
            this.BYONDPanel.Controls.Add(this.VersionLabel);
            this.BYONDPanel.Controls.Add(this.VersionTitle);
            this.BYONDPanel.Controls.Add(this.MinorVersionLabel);
            this.BYONDPanel.Controls.Add(this.MajorVersionLabel);
            this.BYONDPanel.Controls.Add(this.UpdateButton);
            this.BYONDPanel.Controls.Add(this.MinorVersionNumeric);
            this.BYONDPanel.Controls.Add(this.MajorVersionNumeric);
            this.BYONDPanel.Controls.Add(this.UpdateProgressBar);
            this.BYONDPanel.Location = new System.Drawing.Point(4, 22);
            this.BYONDPanel.Name = "BYONDPanel";
            this.BYONDPanel.Size = new System.Drawing.Size(868, 366);
            this.BYONDPanel.TabIndex = 1;
            this.BYONDPanel.Text = "BYOND";
            // 
            // StagedVersionLabel
            // 
            this.StagedVersionLabel.AutoSize = true;
            this.StagedVersionLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StagedVersionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.StagedVersionLabel.Location = new System.Drawing.Point(425, 112);
            this.StagedVersionLabel.Name = "StagedVersionLabel";
            this.StagedVersionLabel.Size = new System.Drawing.Size(82, 18);
            this.StagedVersionLabel.TabIndex = 11;
            this.StagedVersionLabel.Text = "Unknown";
            this.StagedVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.StagedVersionLabel.Visible = false;
            // 
            // StagedVersionTitle
            // 
            this.StagedVersionTitle.AutoSize = true;
            this.StagedVersionTitle.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StagedVersionTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.StagedVersionTitle.Location = new System.Drawing.Point(214, 112);
            this.StagedVersionTitle.Name = "StagedVersionTitle";
            this.StagedVersionTitle.Size = new System.Drawing.Size(202, 18);
            this.StagedVersionTitle.TabIndex = 10;
            this.StagedVersionTitle.Text = "Current staged version:";
            this.StagedVersionTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.StagedVersionTitle.Visible = false;
            // 
            // StatusLabel
            // 
            this.StatusLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.StatusLabel.Location = new System.Drawing.Point(303, 320);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(253, 37);
            this.StatusLabel.TabIndex = 9;
            this.StatusLabel.Text = "Idle";
            this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VersionLabel
            // 
            this.VersionLabel.AutoSize = true;
            this.VersionLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VersionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.VersionLabel.Location = new System.Drawing.Point(425, 74);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(82, 18);
            this.VersionLabel.TabIndex = 8;
            this.VersionLabel.Text = "Unknown";
            this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // VersionTitle
            // 
            this.VersionTitle.AutoSize = true;
            this.VersionTitle.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VersionTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.VersionTitle.Location = new System.Drawing.Point(214, 74);
            this.VersionTitle.Name = "VersionTitle";
            this.VersionTitle.Size = new System.Drawing.Size(205, 18);
            this.VersionTitle.TabIndex = 7;
            this.VersionTitle.Text = "Current BYOND version:";
            this.VersionTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MinorVersionLabel
            // 
            this.MinorVersionLabel.AutoSize = true;
            this.MinorVersionLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinorVersionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.MinorVersionLabel.Location = new System.Drawing.Point(521, 180);
            this.MinorVersionLabel.Name = "MinorVersionLabel";
            this.MinorVersionLabel.Size = new System.Drawing.Size(59, 18);
            this.MinorVersionLabel.TabIndex = 6;
            this.MinorVersionLabel.Text = "Minor:";
            this.MinorVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MajorVersionLabel
            // 
            this.MajorVersionLabel.AutoSize = true;
            this.MajorVersionLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MajorVersionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.MajorVersionLabel.Location = new System.Drawing.Point(276, 180);
            this.MajorVersionLabel.Name = "MajorVersionLabel";
            this.MajorVersionLabel.Size = new System.Drawing.Size(60, 18);
            this.MajorVersionLabel.TabIndex = 5;
            this.MajorVersionLabel.Text = "Major:";
            this.MajorVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UpdateButton
            // 
            this.UpdateButton.Location = new System.Drawing.Point(372, 252);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(118, 28);
            this.UpdateButton.TabIndex = 3;
            this.UpdateButton.Text = "Update";
            this.UpdateButton.UseVisualStyleBackColor = true;
            this.UpdateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // MinorVersionNumeric
            // 
            this.MinorVersionNumeric.Location = new System.Drawing.Point(490, 210);
            this.MinorVersionNumeric.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.MinorVersionNumeric.Name = "MinorVersionNumeric";
            this.MinorVersionNumeric.Size = new System.Drawing.Size(120, 20);
            this.MinorVersionNumeric.TabIndex = 2;
            this.MinorVersionNumeric.Value = new decimal(new int[] {
            1381,
            0,
            0,
            0});
            // 
            // MajorVersionNumeric
            // 
            this.MajorVersionNumeric.Location = new System.Drawing.Point(245, 210);
            this.MajorVersionNumeric.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.MajorVersionNumeric.Name = "MajorVersionNumeric";
            this.MajorVersionNumeric.Size = new System.Drawing.Size(120, 20);
            this.MajorVersionNumeric.TabIndex = 1;
            this.MajorVersionNumeric.Value = new decimal(new int[] {
            511,
            0,
            0,
            0});
            // 
            // UpdateProgressBar
            // 
            this.UpdateProgressBar.Location = new System.Drawing.Point(107, 286);
            this.UpdateProgressBar.MarqueeAnimationSpeed = 50;
            this.UpdateProgressBar.Name = "UpdateProgressBar";
            this.UpdateProgressBar.Size = new System.Drawing.Size(650, 31);
            this.UpdateProgressBar.TabIndex = 0;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(868, 366);
            this.tabPage2.TabIndex = 2;
            this.tabPage2.Text = "Server";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(868, 366);
            this.tabPage3.TabIndex = 3;
            this.tabPage3.Text = "Logs";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // RepoBGW
            // 
            this.RepoBGW.WorkerReportsProgress = true;
            this.RepoBGW.WorkerSupportsCancellation = true;
            // 
            // BYONDTimer
            // 
            this.BYONDTimer.Tick += new System.EventHandler(this.BYONDTimer_Tick);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(40)))), ((int)(((byte)(34)))));
            this.ClientSize = new System.Drawing.Size(900, 415);
            this.Controls.Add(this.Panels);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "/tg/station 13 Server Control Panel";
            this.Panels.ResumeLayout(false);
            this.RepoPanel.ResumeLayout(false);
            this.RepoPanel.PerformLayout();
            this.BYONDPanel.ResumeLayout(false);
            this.BYONDPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MinorVersionNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MajorVersionNumeric)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.TabControl Panels;
		private System.Windows.Forms.TabPage RepoPanel;
		private System.Windows.Forms.Label RepoProgressBarLabel;
		private System.Windows.Forms.ProgressBar RepoProgressBar;
		private System.Windows.Forms.Button CloneRepositoryButton;
		private System.ComponentModel.BackgroundWorker RepoBGW;
		private System.Windows.Forms.Label CurrentRevisionLabel;
		private System.Windows.Forms.TextBox RepoEmailTextBox;
		private System.Windows.Forms.TextBox RepoCommitterNameTextBox;
		private System.Windows.Forms.Button RepoApplyButton;
		private System.Windows.Forms.TextBox RepoBranchTextBox;
		private System.Windows.Forms.TextBox RepoRemoteTextBox;
		private System.Windows.Forms.Button HardReset;
		private System.Windows.Forms.Button UpdateRepoButton;
		private System.Windows.Forms.Button TestMergeButton;
		private System.Windows.Forms.Label CommitterEmailTitle;
		private System.Windows.Forms.Label CommiterNameTitle;
		private System.Windows.Forms.Label IdentityLabel;
		private System.Windows.Forms.Label TestMergeListTitle;
		private System.Windows.Forms.Label RemoteNameTitle;
		private System.Windows.Forms.Label BranchNameTitle;
		private System.Windows.Forms.Label CurrentRevisionTitle;
		private System.Windows.Forms.TabPage BYONDPanel;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.TextBox TestMergeListLabel;
		private System.Windows.Forms.Button UpdateButton;
		private System.Windows.Forms.NumericUpDown MinorVersionNumeric;
		private System.Windows.Forms.NumericUpDown MajorVersionNumeric;
		private System.Windows.Forms.ProgressBar UpdateProgressBar;
		private System.Windows.Forms.Label MajorVersionLabel;
		private System.Windows.Forms.Label MinorVersionLabel;
		private System.Windows.Forms.Label VersionLabel;
		private System.Windows.Forms.Label VersionTitle;
		private System.Windows.Forms.Label StatusLabel;
		private System.Windows.Forms.Timer BYONDTimer;
		private System.Windows.Forms.Label StagedVersionLabel;
		private System.Windows.Forms.Label StagedVersionTitle;
	}
}

