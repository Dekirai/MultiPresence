namespace MultiPresenceGame
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            label1 = new Label();
            notify = new NotifyIcon(components);
            cms = new ContextMenuStrip(components);
            languageToolStripMenuItem = new ToolStripMenuItem();
            cb_english = new ToolStripMenuItem();
            cb_german = new ToolStripMenuItem();
            cb_DisableNotifications = new ToolStripMenuItem();
            btn_Config = new ToolStripMenuItem();
            btn_Blacklist = new ToolStripMenuItem();
            cb_LaunchWithWindows = new ToolStripMenuItem();
            cb_LaunchWithWindowsAdmin = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            btn_Exit = new ToolStripMenuItem();
            cms.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(129, 15);
            label1.TabIndex = 1;
            label1.Text = "You should not see this";
            // 
            // notify
            // 
            notify.ContextMenuStrip = cms;
            notify.Icon = (Icon)resources.GetObject("notify.Icon");
            notify.Text = "MultiPresence";
            notify.Visible = true;
            // 
            // cms
            // 
            cms.Items.AddRange(new ToolStripItem[] { languageToolStripMenuItem, cb_DisableNotifications, btn_Config, btn_Blacklist, cb_LaunchWithWindows, cb_LaunchWithWindowsAdmin, toolStripSeparator1, btn_Exit });
            cms.Name = "cms";
            cms.Size = new Size(239, 164);
            // 
            // languageToolStripMenuItem
            // 
            languageToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cb_english, cb_german });
            languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            languageToolStripMenuItem.Size = new Size(238, 22);
            languageToolStripMenuItem.Text = "Language";
            // 
            // cb_english
            // 
            cb_english.Checked = true;
            cb_english.CheckOnClick = true;
            cb_english.CheckState = CheckState.Checked;
            cb_english.Name = "cb_english";
            cb_english.Size = new Size(116, 22);
            cb_english.Text = "English";
            // 
            // cb_german
            // 
            cb_german.CheckOnClick = true;
            cb_german.Name = "cb_german";
            cb_german.Size = new Size(116, 22);
            cb_german.Text = "German";
            // 
            // cb_DisableNotifications
            // 
            cb_DisableNotifications.CheckOnClick = true;
            cb_DisableNotifications.Name = "cb_DisableNotifications";
            cb_DisableNotifications.Size = new Size(238, 22);
            cb_DisableNotifications.Text = "Disable System Notifications";
            // 
            // btn_Config
            // 
            btn_Config.Name = "btn_Config";
            btn_Config.Size = new Size(238, 22);
            btn_Config.Text = "Open config";
            // 
            // btn_Blacklist
            // 
            btn_Blacklist.Name = "btn_Blacklist";
            btn_Blacklist.Size = new Size(238, 22);
            btn_Blacklist.Text = "Open blacklist";
            // 
            // cb_LaunchWithWindows
            // 
            cb_LaunchWithWindows.CheckOnClick = true;
            cb_LaunchWithWindows.Name = "cb_LaunchWithWindows";
            cb_LaunchWithWindows.Size = new Size(238, 22);
            cb_LaunchWithWindows.Text = "Launch with Windows";
            // 
            // cb_LaunchWithWindowsAdmin
            // 
            cb_LaunchWithWindowsAdmin.CheckOnClick = true;
            cb_LaunchWithWindowsAdmin.Name = "cb_LaunchWithWindowsAdmin";
            cb_LaunchWithWindowsAdmin.Size = new Size(238, 22);
            cb_LaunchWithWindowsAdmin.Text = "Launch with Windows (Admin)";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(235, 6);
            // 
            // btn_Exit
            // 
            btn_Exit.Name = "btn_Exit";
            btn_Exit.Size = new Size(238, 22);
            btn_Exit.Text = "Exit";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(329, 36);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MainForm";
            Text = "MainForm";
            cms.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private NotifyIcon notify;
        private ContextMenuStrip cms;
        private ToolStripMenuItem languageToolStripMenuItem;
        private ToolStripMenuItem cb_english;
        private ToolStripMenuItem cb_german;
        private ToolStripMenuItem cb_DisableNotifications;
        private ToolStripMenuItem btn_Config;
        private ToolStripMenuItem btn_Blacklist;
        private ToolStripMenuItem cb_LaunchWithWindows;
        private ToolStripMenuItem cb_LaunchWithWindowsAdmin;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem btn_Exit;
    }
}
