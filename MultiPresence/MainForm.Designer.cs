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
            cms.Items.AddRange(new ToolStripItem[] { cb_DisableNotifications, btn_Exit });
            cms.Name = "cms";
            cms.Size = new Size(225, 70);
            // 
            // cb_DisableNotifications
            // 
            cb_DisableNotifications.CheckOnClick = true;
            cb_DisableNotifications.Name = "cb_DisableNotifications";
            cb_DisableNotifications.Size = new Size(224, 22);
            cb_DisableNotifications.Text = "Disable System Notifications";
            // 
            // btn_Exit
            // 
            btn_Exit.Name = "btn_Exit";
            btn_Exit.Size = new Size(224, 22);
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
    }
}