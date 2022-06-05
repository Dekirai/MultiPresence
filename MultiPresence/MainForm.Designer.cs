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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.label1 = new System.Windows.Forms.Label();
            this.notify = new System.Windows.Forms.NotifyIcon(this.components);
            this.cms = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.presenceSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kingdomHeartsIIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cb_KH2_GermanTranslation = new System.Windows.Forms.ToolStripMenuItem();
            this.tYTheTasmanianTigerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cb_TY_GermanTranslation = new System.Windows.Forms.ToolStripMenuItem();
            this.zeldaWindWakerHDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cb_WWHD_GermanTranslation = new System.Windows.Forms.ToolStripMenuItem();
            this.cb_DisableNotifications = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btn_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.cb_HideButtons = new System.Windows.Forms.ToolStripMenuItem();
            this.cms.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "You should not see this";
            // 
            // notify
            // 
            this.notify.ContextMenuStrip = this.cms;
            this.notify.Icon = ((System.Drawing.Icon)(resources.GetObject("notify.Icon")));
            this.notify.Text = "MultiPresence";
            this.notify.Visible = true;
            // 
            // cms
            // 
            this.cms.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.presenceSettingsToolStripMenuItem,
            this.cb_HideButtons,
            this.cb_DisableNotifications,
            this.toolStripSeparator1,
            this.btn_Exit});
            this.cms.Name = "cms";
            this.cms.Size = new System.Drawing.Size(194, 98);
            // 
            // presenceSettingsToolStripMenuItem
            // 
            this.presenceSettingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.kingdomHeartsIIToolStripMenuItem,
            this.tYTheTasmanianTigerToolStripMenuItem,
            this.zeldaWindWakerHDToolStripMenuItem});
            this.presenceSettingsToolStripMenuItem.Name = "presenceSettingsToolStripMenuItem";
            this.presenceSettingsToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.presenceSettingsToolStripMenuItem.Text = "Presence Settings";
            // 
            // kingdomHeartsIIToolStripMenuItem
            // 
            this.kingdomHeartsIIToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cb_KH2_GermanTranslation});
            this.kingdomHeartsIIToolStripMenuItem.Name = "kingdomHeartsIIToolStripMenuItem";
            this.kingdomHeartsIIToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.kingdomHeartsIIToolStripMenuItem.Text = "Kingdom Hearts II";
            // 
            // cb_KH2_GermanTranslation
            // 
            this.cb_KH2_GermanTranslation.CheckOnClick = true;
            this.cb_KH2_GermanTranslation.Name = "cb_KH2_GermanTranslation";
            this.cb_KH2_GermanTranslation.Size = new System.Drawing.Size(176, 22);
            this.cb_KH2_GermanTranslation.Text = "German Translation";
            // 
            // tYTheTasmanianTigerToolStripMenuItem
            // 
            this.tYTheTasmanianTigerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cb_TY_GermanTranslation});
            this.tYTheTasmanianTigerToolStripMenuItem.Name = "tYTheTasmanianTigerToolStripMenuItem";
            this.tYTheTasmanianTigerToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.tYTheTasmanianTigerToolStripMenuItem.Text = "TY the Tasmanian Tiger";
            // 
            // cb_TY_GermanTranslation
            // 
            this.cb_TY_GermanTranslation.CheckOnClick = true;
            this.cb_TY_GermanTranslation.Name = "cb_TY_GermanTranslation";
            this.cb_TY_GermanTranslation.Size = new System.Drawing.Size(176, 22);
            this.cb_TY_GermanTranslation.Text = "German Translation";
            // 
            // zeldaWindWakerHDToolStripMenuItem
            // 
            this.zeldaWindWakerHDToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cb_WWHD_GermanTranslation});
            this.zeldaWindWakerHDToolStripMenuItem.Name = "zeldaWindWakerHDToolStripMenuItem";
            this.zeldaWindWakerHDToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.zeldaWindWakerHDToolStripMenuItem.Text = "Zelda: Wind Waker HD";
            // 
            // cb_WWHD_GermanTranslation
            // 
            this.cb_WWHD_GermanTranslation.CheckOnClick = true;
            this.cb_WWHD_GermanTranslation.Name = "cb_WWHD_GermanTranslation";
            this.cb_WWHD_GermanTranslation.Size = new System.Drawing.Size(176, 22);
            this.cb_WWHD_GermanTranslation.Text = "German Translation";
            // 
            // cb_DisableNotifications
            // 
            this.cb_DisableNotifications.CheckOnClick = true;
            this.cb_DisableNotifications.Name = "cb_DisableNotifications";
            this.cb_DisableNotifications.Size = new System.Drawing.Size(193, 22);
            this.cb_DisableNotifications.Text = "Disable Notifications";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(190, 6);
            // 
            // btn_Exit
            // 
            this.btn_Exit.Name = "btn_Exit";
            this.btn_Exit.Size = new System.Drawing.Size(193, 22);
            this.btn_Exit.Text = "Exit";
            this.btn_Exit.Click += new System.EventHandler(this.btn_Exit_Click);
            // 
            // cb_HideButtons
            // 
            this.cb_HideButtons.CheckOnClick = true;
            this.cb_HideButtons.Name = "cb_HideButtons";
            this.cb_HideButtons.Size = new System.Drawing.Size(193, 22);
            this.cb_HideButtons.Text = "Hide Presence Buttons";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 36);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.Text = "MultiPresence";
            this.cms.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private NotifyIcon notify;
        private ContextMenuStrip cms;
        private ToolStripMenuItem btn_Exit;
        private ToolStripMenuItem cb_DisableNotifications;
        private ToolStripMenuItem presenceSettingsToolStripMenuItem;
        private ToolStripMenuItem tYTheTasmanianTigerToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem kingdomHeartsIIToolStripMenuItem;
        private ToolStripMenuItem zeldaWindWakerHDToolStripMenuItem;
        public ToolStripMenuItem cb_TY_GermanTranslation;
        public ToolStripMenuItem cb_KH2_GermanTranslation;
        public ToolStripMenuItem cb_WWHD_GermanTranslation;
        private ToolStripMenuItem cb_HideButtons;
    }
}