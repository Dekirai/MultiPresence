namespace MultiPresence
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
            cb_DisableNotifications = new ToolStripMenuItem();
            cb_DisableAutoUpdates = new ToolStripMenuItem();
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
            label1.TabIndex = 0;
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
            cms.Items.AddRange(new ToolStripItem[] { cb_DisableNotifications, cb_DisableAutoUpdates, btn_Config, btn_Blacklist, cb_LaunchWithWindows, cb_LaunchWithWindowsAdmin, toolStripSeparator1, btn_Exit });
            cms.Name = "cms";
            cms.Size = new Size(239, 186);
            // 
            // cb_DisableNotifications
            // 
            cb_DisableNotifications.CheckOnClick = true;
            cb_DisableNotifications.Name = "cb_DisableNotifications";
            cb_DisableNotifications.Size = new Size(238, 22);
            cb_DisableNotifications.Text = "Disable System Notifications";
            cb_DisableNotifications.CheckedChanged += cb_DisableNotifications_CheckedChanged;
            // 
            // cb_DisableAutoUpdates
            // 
            cb_DisableAutoUpdates.CheckOnClick = true;
            cb_DisableAutoUpdates.Name = "cb_DisableAutoUpdates";
            cb_DisableAutoUpdates.Size = new Size(238, 22);
            cb_DisableAutoUpdates.Text = "Disable Auto-Updates";
            cb_DisableAutoUpdates.CheckedChanged += cb_DisableAutoUpdates_CheckedChanged;
            // 
            // btn_Config
            // 
            btn_Config.Name = "btn_Config";
            btn_Config.Size = new Size(238, 22);
            btn_Config.Text = "Open config folder";
            btn_Config.Click += btn_Config_Click;
            // 
            // btn_Blacklist
            // 
            btn_Blacklist.Name = "btn_Blacklist";
            btn_Blacklist.Size = new Size(238, 22);
            btn_Blacklist.Text = "Open blacklist";
            btn_Blacklist.Click += btn_Blacklist_Click;
            // 
            // cb_LaunchWithWindows
            // 
            cb_LaunchWithWindows.CheckOnClick = true;
            cb_LaunchWithWindows.Name = "cb_LaunchWithWindows";
            cb_LaunchWithWindows.Size = new Size(238, 22);
            cb_LaunchWithWindows.Text = "Launch with Windows";
            cb_LaunchWithWindows.Click += cb_LaunchWithWindows_Click;
            // 
            // cb_LaunchWithWindowsAdmin
            // 
            cb_LaunchWithWindowsAdmin.CheckOnClick = true;
            cb_LaunchWithWindowsAdmin.Name = "cb_LaunchWithWindowsAdmin";
            cb_LaunchWithWindowsAdmin.Size = new Size(238, 22);
            cb_LaunchWithWindowsAdmin.Text = "Launch with Windows (Admin)";
            cb_LaunchWithWindowsAdmin.Click += cb_LaunchWithWindowsAdmin_Click;
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
            btn_Exit.Click += btn_Exit_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(329, 36);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MainForm";
            ShowInTaskbar = false;
            Text = "MultiPresence";
            cms.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private NotifyIcon notify;
        private ContextMenuStrip cms;
        private ToolStripMenuItem btn_Exit;
        private ToolStripMenuItem cb_DisableNotifications;
        private ToolStripMenuItem btn_Config;
        private ToolStripMenuItem btn_Blacklist;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem cb_LaunchWithWindowsAdmin;
        private ToolStripMenuItem cb_LaunchWithWindows;
        private ToolStripMenuItem cb_DisableAutoUpdates;
    }
}