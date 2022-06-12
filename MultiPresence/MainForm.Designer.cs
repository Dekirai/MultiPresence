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
            this.cb_DisableNotifications = new System.Windows.Forms.ToolStripMenuItem();
            this.btn_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.cb_DisableInfoNotifications = new System.Windows.Forms.ToolStripMenuItem();
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
            this.cb_DisableInfoNotifications,
            this.cb_DisableNotifications,
            this.btn_Exit});
            this.cms.Name = "cms";
            this.cms.Size = new System.Drawing.Size(225, 92);
            // 
            // cb_DisableNotifications
            // 
            this.cb_DisableNotifications.CheckOnClick = true;
            this.cb_DisableNotifications.Name = "cb_DisableNotifications";
            this.cb_DisableNotifications.Size = new System.Drawing.Size(224, 22);
            this.cb_DisableNotifications.Text = "Disable System Notifications";
            // 
            // btn_Exit
            // 
            this.btn_Exit.Name = "btn_Exit";
            this.btn_Exit.Size = new System.Drawing.Size(224, 22);
            this.btn_Exit.Text = "Exit";
            this.btn_Exit.Click += new System.EventHandler(this.btn_Exit_Click);
            // 
            // cb_DisableInfoNotifications
            // 
            this.cb_DisableInfoNotifications.CheckOnClick = true;
            this.cb_DisableInfoNotifications.Name = "cb_DisableInfoNotifications";
            this.cb_DisableInfoNotifications.Size = new System.Drawing.Size(224, 22);
            this.cb_DisableInfoNotifications.Text = "Disable Info Notifications";
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
        private ToolStripMenuItem cb_DisableInfoNotifications;
    }
}