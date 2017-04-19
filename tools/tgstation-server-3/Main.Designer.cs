namespace TGStationServer3
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
				TrayIcon.Dispose();
				TrayMenu.Dispose();
				DisposeRepo();
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
            this.CloseButton = new System.Windows.Forms.Button();
            this.MinimizeButton = new System.Windows.Forms.Button();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.RepoPanel = new System.Windows.Forms.TabPage();
            this.CloneRepositoryButton = new System.Windows.Forms.Button();
            this.RepoProgressBarLabel = new System.Windows.Forms.Label();
            this.RepoProgressBar = new System.Windows.Forms.ProgressBar();
            this.RepoBGW = new System.ComponentModel.BackgroundWorker();
            this.CurrentRevisionTitle = new System.Windows.Forms.Label();
            this.BranchNameTitle = new System.Windows.Forms.Label();
            this.RemoteNameTitle = new System.Windows.Forms.Label();
            this.TestMergeListTitle = new System.Windows.Forms.Label();
            this.TestMergeListLabel = new System.Windows.Forms.Label();
            this.IdentityLabel = new System.Windows.Forms.Label();
            this.CommiterNameTitle = new System.Windows.Forms.Label();
            this.CommitterEmailTitle = new System.Windows.Forms.Label();
            this.TestMergeButton = new System.Windows.Forms.Button();
            this.UpdateRepoButton = new System.Windows.Forms.Button();
            this.UpdateToShaButton = new System.Windows.Forms.Button();
            this.RepoRemoteTextBox = new System.Windows.Forms.TextBox();
            this.RepoBranchTextBox = new System.Windows.Forms.TextBox();
            this.RepoApplyButton = new System.Windows.Forms.Button();
            this.RepoCommitterNameTextBox = new System.Windows.Forms.TextBox();
            this.RepoEmailTextBox = new System.Windows.Forms.TextBox();
            this.CurrentRevisionLabel = new System.Windows.Forms.Label();
            this.TabControl.SuspendLayout();
            this.RepoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // CloseButton
            // 
            this.CloseButton.Location = new System.Drawing.Point(866, 12);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(22, 22);
            this.CloseButton.TabIndex = 0;
            this.CloseButton.Text = "X";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // MinimizeButton
            // 
            this.MinimizeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimizeButton.Location = new System.Drawing.Point(838, 12);
            this.MinimizeButton.Name = "MinimizeButton";
            this.MinimizeButton.Size = new System.Drawing.Size(22, 22);
            this.MinimizeButton.TabIndex = 1;
            this.MinimizeButton.Text = "-";
            this.MinimizeButton.UseVisualStyleBackColor = true;
            this.MinimizeButton.Click += new System.EventHandler(this.MinimizeButton_Click);
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.TitleLabel.Location = new System.Drawing.Point(12, 9);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(544, 32);
            this.TitleLabel.TabIndex = 2;
            this.TitleLabel.Text = "/tg/station 13 Server Contol Panel";
            // 
            // TabControl
            // 
            this.TabControl.Controls.Add(this.RepoPanel);
            this.TabControl.Location = new System.Drawing.Point(12, 60);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(876, 392);
            this.TabControl.TabIndex = 3;
            // 
            // RepoPanel
            // 
            this.RepoPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(40)))), ((int)(((byte)(34)))));
            this.RepoPanel.Controls.Add(this.CurrentRevisionLabel);
            this.RepoPanel.Controls.Add(this.RepoEmailTextBox);
            this.RepoPanel.Controls.Add(this.RepoCommitterNameTextBox);
            this.RepoPanel.Controls.Add(this.RepoApplyButton);
            this.RepoPanel.Controls.Add(this.RepoBranchTextBox);
            this.RepoPanel.Controls.Add(this.RepoRemoteTextBox);
            this.RepoPanel.Controls.Add(this.UpdateToShaButton);
            this.RepoPanel.Controls.Add(this.UpdateRepoButton);
            this.RepoPanel.Controls.Add(this.TestMergeButton);
            this.RepoPanel.Controls.Add(this.CommitterEmailTitle);
            this.RepoPanel.Controls.Add(this.CommiterNameTitle);
            this.RepoPanel.Controls.Add(this.IdentityLabel);
            this.RepoPanel.Controls.Add(this.TestMergeListLabel);
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
            this.RepoProgressBarLabel.AutoSize = true;
            this.RepoProgressBarLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RepoProgressBarLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.RepoProgressBarLabel.Location = new System.Drawing.Point(315, 152);
            this.RepoProgressBarLabel.Name = "RepoProgressBarLabel";
            this.RepoProgressBarLabel.Size = new System.Drawing.Size(225, 18);
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
            // RepoBGW
            // 
            this.RepoBGW.WorkerReportsProgress = true;
            this.RepoBGW.WorkerSupportsCancellation = true;
            this.RepoBGW.DoWork += new System.ComponentModel.DoWorkEventHandler(this.RepoBGW_DoWork);
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
            // BranchNameTitle
            // 
            this.BranchNameTitle.AutoSize = true;
            this.BranchNameTitle.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BranchNameTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.BranchNameTitle.Location = new System.Drawing.Point(6, 89);
            this.BranchNameTitle.Name = "BranchNameTitle";
            this.BranchNameTitle.Size = new System.Drawing.Size(70, 18);
            this.BranchNameTitle.TabIndex = 4;
            this.BranchNameTitle.Text = "Branch:";
            this.BranchNameTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.BranchNameTitle.Visible = false;
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
            // TestMergeListLabel
            // 
            this.TestMergeListLabel.AutoSize = true;
            this.TestMergeListLabel.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TestMergeListLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(248)))), ((int)(((byte)(242)))));
            this.TestMergeListLabel.Location = new System.Drawing.Point(181, 266);
            this.TestMergeListLabel.Name = "TestMergeListLabel";
            this.TestMergeListLabel.Size = new System.Drawing.Size(50, 18);
            this.TestMergeListLabel.TabIndex = 7;
            this.TestMergeListLabel.Text = "None";
            this.TestMergeListLabel.Visible = false;
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
            // TestMergeButton
            // 
            this.TestMergeButton.Location = new System.Drawing.Point(722, 46);
            this.TestMergeButton.Name = "TestMergeButton";
            this.TestMergeButton.Size = new System.Drawing.Size(140, 29);
            this.TestMergeButton.TabIndex = 11;
            this.TestMergeButton.Text = "Test Merge PR";
            this.TestMergeButton.UseVisualStyleBackColor = true;
            this.TestMergeButton.Visible = false;
            // 
            // UpdateRepoButton
            // 
            this.UpdateRepoButton.Location = new System.Drawing.Point(722, 11);
            this.UpdateRepoButton.Name = "UpdateRepoButton";
            this.UpdateRepoButton.Size = new System.Drawing.Size(140, 29);
            this.UpdateRepoButton.TabIndex = 12;
            this.UpdateRepoButton.Text = "Update to Remote";
            this.UpdateRepoButton.UseVisualStyleBackColor = true;
            this.UpdateRepoButton.Visible = false;
            // 
            // UpdateToShaButton
            // 
            this.UpdateToShaButton.Location = new System.Drawing.Point(722, 81);
            this.UpdateToShaButton.Name = "UpdateToShaButton";
            this.UpdateToShaButton.Size = new System.Drawing.Size(140, 29);
            this.UpdateToShaButton.TabIndex = 13;
            this.UpdateToShaButton.Text = "Checkout Commit";
            this.UpdateToShaButton.UseVisualStyleBackColor = true;
            this.UpdateToShaButton.Visible = false;
            // 
            // RepoRemoteTextBox
            // 
            this.RepoRemoteTextBox.Location = new System.Drawing.Point(90, 58);
            this.RepoRemoteTextBox.Name = "RepoRemoteTextBox";
            this.RepoRemoteTextBox.Size = new System.Drawing.Size(535, 20);
            this.RepoRemoteTextBox.TabIndex = 14;
            this.RepoRemoteTextBox.Visible = false;
            // 
            // RepoBranchTextBox
            // 
            this.RepoBranchTextBox.Location = new System.Drawing.Point(90, 89);
            this.RepoBranchTextBox.Name = "RepoBranchTextBox";
            this.RepoBranchTextBox.Size = new System.Drawing.Size(535, 20);
            this.RepoBranchTextBox.TabIndex = 15;
            this.RepoBranchTextBox.Visible = false;
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
            // 
            // RepoCommitterNameTextBox
            // 
            this.RepoCommitterNameTextBox.Location = new System.Drawing.Point(90, 191);
            this.RepoCommitterNameTextBox.Name = "RepoCommitterNameTextBox";
            this.RepoCommitterNameTextBox.Size = new System.Drawing.Size(535, 20);
            this.RepoCommitterNameTextBox.TabIndex = 18;
            this.RepoCommitterNameTextBox.Visible = false;
            // 
            // RepoEmailTextBox
            // 
            this.RepoEmailTextBox.Location = new System.Drawing.Point(90, 217);
            this.RepoEmailTextBox.Name = "RepoEmailTextBox";
            this.RepoEmailTextBox.Size = new System.Drawing.Size(535, 20);
            this.RepoEmailTextBox.TabIndex = 19;
            this.RepoEmailTextBox.Visible = false;
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
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(40)))), ((int)(((byte)(34)))));
            this.ClientSize = new System.Drawing.Size(900, 500);
            this.Controls.Add(this.TabControl);
            this.Controls.Add(this.TitleLabel);
            this.Controls.Add(this.MinimizeButton);
            this.Controls.Add(this.CloseButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "/tg/station 13 Server";
            this.TabControl.ResumeLayout(false);
            this.RepoPanel.ResumeLayout(false);
            this.RepoPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button CloseButton;
		private System.Windows.Forms.Button MinimizeButton;
		private System.Windows.Forms.Label TitleLabel;
		private System.Windows.Forms.TabControl TabControl;
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
		private System.Windows.Forms.Button UpdateToShaButton;
		private System.Windows.Forms.Button UpdateRepoButton;
		private System.Windows.Forms.Button TestMergeButton;
		private System.Windows.Forms.Label CommitterEmailTitle;
		private System.Windows.Forms.Label CommiterNameTitle;
		private System.Windows.Forms.Label IdentityLabel;
		private System.Windows.Forms.Label TestMergeListLabel;
		private System.Windows.Forms.Label TestMergeListTitle;
		private System.Windows.Forms.Label RemoteNameTitle;
		private System.Windows.Forms.Label BranchNameTitle;
		private System.Windows.Forms.Label CurrentRevisionTitle;
	}
}

