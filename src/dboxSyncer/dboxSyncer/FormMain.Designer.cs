namespace dboxSyncer
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemMain = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemSync = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemWeb = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.listViewData = new System.Windows.Forms.ListView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.groupBoxSync = new System.Windows.Forms.GroupBox();
            this.labelDownloadSpeedTips = new System.Windows.Forms.Label();
            this.labelUploadSpeedTips = new System.Windows.Forms.Label();
            this.labelIntervalMinute = new System.Windows.Forms.Label();
            this.buttonSetting = new System.Windows.Forms.Button();
            this.labelUploadKBS = new System.Windows.Forms.Label();
            this.labelDownloadKBS = new System.Windows.Forms.Label();
            this.labelDownloadSpeed = new System.Windows.Forms.Label();
            this.labelUploadSpeed = new System.Windows.Forms.Label();
            this.labelIntervalTime = new System.Windows.Forms.Label();
            this.panelUploadSpeedBox = new System.Windows.Forms.Panel();
            this.textBoxUploadSpeed = new System.Windows.Forms.TextBox();
            this.panelDownloadSpeedBox = new System.Windows.Forms.Panel();
            this.textBoxDownloadSpeed = new System.Windows.Forms.TextBox();
            this.panelIntervalTimeBox = new System.Windows.Forms.Panel();
            this.listBoxIntervalTime = new System.Windows.Forms.ListBox();
            this.buttonSync = new System.Windows.Forms.Button();
            this.buttonWeb = new System.Windows.Forms.Button();
            this.buttonHide = new System.Windows.Forms.Button();
            this.contextMenuStrip.SuspendLayout();
            this.groupBoxSync.SuspendLayout();
            this.panelUploadSpeedBox.SuspendLayout();
            this.panelDownloadSpeedBox.SuspendLayout();
            this.panelIntervalTimeBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;
            this.notifyIcon.Text = "notifyIcon";
            this.notifyIcon.Visible = true;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(36, 36);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemMain,
            this.toolStripMenuItemSync,
            this.toolStripMenuItemWeb,
            this.toolStripMenuItemQuit});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(106, 92);
            // 
            // toolStripMenuItemMain
            // 
            this.toolStripMenuItemMain.Name = "toolStripMenuItemMain";
            this.toolStripMenuItemMain.Size = new System.Drawing.Size(105, 22);
            this.toolStripMenuItemMain.Text = "Main";
            this.toolStripMenuItemMain.Click += new System.EventHandler(this.ToolStripMenuItemMain_Click);
            // 
            // toolStripMenuItemSync
            // 
            this.toolStripMenuItemSync.Name = "toolStripMenuItemSync";
            this.toolStripMenuItemSync.Size = new System.Drawing.Size(105, 22);
            this.toolStripMenuItemSync.Text = "Sync";
            this.toolStripMenuItemSync.Click += new System.EventHandler(this.ToolStripMenuItemSync_Click);
            // 
            // toolStripMenuItemWeb
            // 
            this.toolStripMenuItemWeb.Name = "toolStripMenuItemWeb";
            this.toolStripMenuItemWeb.Size = new System.Drawing.Size(105, 22);
            this.toolStripMenuItemWeb.Text = "Web";
            this.toolStripMenuItemWeb.Click += new System.EventHandler(this.ToolStripMenuItemWeb_Click);
            // 
            // toolStripMenuItemQuit
            // 
            this.toolStripMenuItemQuit.Name = "toolStripMenuItemQuit";
            this.toolStripMenuItemQuit.Size = new System.Drawing.Size(105, 22);
            this.toolStripMenuItemQuit.Text = "Quit";
            this.toolStripMenuItemQuit.Click += new System.EventHandler(this.ToolStripMenuItemQuit_Click);
            // 
            // listViewData
            // 
            this.listViewData.BackColor = System.Drawing.Color.WhiteSmoke;
            this.listViewData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewData.ForeColor = System.Drawing.Color.Black;
            this.listViewData.FullRowSelect = true;
            this.listViewData.HideSelection = false;
            this.listViewData.Location = new System.Drawing.Point(16, 16);
            this.listViewData.Name = "listViewData";
            this.listViewData.Size = new System.Drawing.Size(752, 260);
            this.listViewData.SmallImageList = this.imageList;
            this.listViewData.TabIndex = 0;
            this.listViewData.UseCompatibleStateImageBehavior = false;
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList.ImageSize = new System.Drawing.Size(1, 20);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // buttonAdd
            // 
            this.buttonAdd.BackColor = System.Drawing.Color.WhiteSmoke;
            this.buttonAdd.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAdd.Location = new System.Drawing.Point(560, 284);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(120, 24);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = false;
            this.buttonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.BackColor = System.Drawing.Color.WhiteSmoke;
            this.buttonDelete.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.buttonDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDelete.Location = new System.Drawing.Point(688, 284);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(80, 24);
            this.buttonDelete.TabIndex = 2;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = false;
            this.buttonDelete.Click += new System.EventHandler(this.ButtonDelete_Click);
            // 
            // groupBoxSync
            // 
            this.groupBoxSync.Controls.Add(this.labelDownloadSpeedTips);
            this.groupBoxSync.Controls.Add(this.labelUploadSpeedTips);
            this.groupBoxSync.Controls.Add(this.labelIntervalMinute);
            this.groupBoxSync.Controls.Add(this.buttonSetting);
            this.groupBoxSync.Controls.Add(this.labelUploadKBS);
            this.groupBoxSync.Controls.Add(this.labelDownloadKBS);
            this.groupBoxSync.Controls.Add(this.labelDownloadSpeed);
            this.groupBoxSync.Controls.Add(this.labelUploadSpeed);
            this.groupBoxSync.Controls.Add(this.labelIntervalTime);
            this.groupBoxSync.Controls.Add(this.panelUploadSpeedBox);
            this.groupBoxSync.Controls.Add(this.panelDownloadSpeedBox);
            this.groupBoxSync.Controls.Add(this.panelIntervalTimeBox);
            this.groupBoxSync.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBoxSync.Location = new System.Drawing.Point(16, 364);
            this.groupBoxSync.Name = "groupBoxSync";
            this.groupBoxSync.Size = new System.Drawing.Size(600, 180);
            this.groupBoxSync.TabIndex = 3;
            this.groupBoxSync.TabStop = false;
            this.groupBoxSync.Text = "Sync";
            // 
            // labelDownloadSpeedTips
            // 
            this.labelDownloadSpeedTips.AutoSize = true;
            this.labelDownloadSpeedTips.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelDownloadSpeedTips.Font = new System.Drawing.Font("宋体", 9F);
            this.labelDownloadSpeedTips.ForeColor = System.Drawing.Color.Gray;
            this.labelDownloadSpeedTips.Location = new System.Drawing.Point(408, 98);
            this.labelDownloadSpeedTips.Name = "labelDownloadSpeedTips";
            this.labelDownloadSpeedTips.Size = new System.Drawing.Size(41, 12);
            this.labelDownloadSpeedTips.TabIndex = 10;
            this.labelDownloadSpeedTips.Text = "0-1000";
            // 
            // labelUploadSpeedTips
            // 
            this.labelUploadSpeedTips.AutoSize = true;
            this.labelUploadSpeedTips.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelUploadSpeedTips.Font = new System.Drawing.Font("宋体", 9F);
            this.labelUploadSpeedTips.ForeColor = System.Drawing.Color.Gray;
            this.labelUploadSpeedTips.Location = new System.Drawing.Point(208, 98);
            this.labelUploadSpeedTips.Name = "labelUploadSpeedTips";
            this.labelUploadSpeedTips.Size = new System.Drawing.Size(41, 12);
            this.labelUploadSpeedTips.TabIndex = 6;
            this.labelUploadSpeedTips.Text = "0-1000";
            // 
            // labelIntervalMinute
            // 
            this.labelIntervalMinute.AutoSize = true;
            this.labelIntervalMinute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelIntervalMinute.Location = new System.Drawing.Point(100, 80);
            this.labelIntervalMinute.Name = "labelIntervalMinute";
            this.labelIntervalMinute.Size = new System.Drawing.Size(47, 12);
            this.labelIntervalMinute.TabIndex = 2;
            this.labelIntervalMinute.Text = "Minutes";
            // 
            // buttonSetting
            // 
            this.buttonSetting.BackColor = System.Drawing.Color.WhiteSmoke;
            this.buttonSetting.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.buttonSetting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSetting.Font = new System.Drawing.Font("宋体", 9F);
            this.buttonSetting.ForeColor = System.Drawing.Color.Black;
            this.buttonSetting.Location = new System.Drawing.Point(40, 122);
            this.buttonSetting.Name = "buttonSetting";
            this.buttonSetting.Size = new System.Drawing.Size(75, 28);
            this.buttonSetting.TabIndex = 11;
            this.buttonSetting.Text = "设置";
            this.buttonSetting.UseVisualStyleBackColor = false;
            this.buttonSetting.Click += new System.EventHandler(this.ButtonSetting_Click);
            // 
            // labelUploadKBS
            // 
            this.labelUploadKBS.AutoSize = true;
            this.labelUploadKBS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelUploadKBS.ForeColor = System.Drawing.Color.Black;
            this.labelUploadKBS.Location = new System.Drawing.Point(476, 72);
            this.labelUploadKBS.Name = "labelUploadKBS";
            this.labelUploadKBS.Size = new System.Drawing.Size(29, 12);
            this.labelUploadKBS.TabIndex = 9;
            this.labelUploadKBS.Text = "KB/S";
            // 
            // labelDownloadKBS
            // 
            this.labelDownloadKBS.AutoSize = true;
            this.labelDownloadKBS.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelDownloadKBS.ForeColor = System.Drawing.Color.Black;
            this.labelDownloadKBS.Location = new System.Drawing.Point(276, 72);
            this.labelDownloadKBS.Name = "labelDownloadKBS";
            this.labelDownloadKBS.Size = new System.Drawing.Size(29, 12);
            this.labelDownloadKBS.TabIndex = 5;
            this.labelDownloadKBS.Text = "KB/S";
            // 
            // labelDownloadSpeed
            // 
            this.labelDownloadSpeed.AutoSize = true;
            this.labelDownloadSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelDownloadSpeed.ForeColor = System.Drawing.Color.Black;
            this.labelDownloadSpeed.Location = new System.Drawing.Point(400, 40);
            this.labelDownloadSpeed.Name = "labelDownloadSpeed";
            this.labelDownloadSpeed.Size = new System.Drawing.Size(83, 12);
            this.labelDownloadSpeed.TabIndex = 7;
            this.labelDownloadSpeed.Text = "DownloadSpeed";
            // 
            // labelUploadSpeed
            // 
            this.labelUploadSpeed.AutoSize = true;
            this.labelUploadSpeed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelUploadSpeed.ForeColor = System.Drawing.Color.Black;
            this.labelUploadSpeed.Location = new System.Drawing.Point(200, 40);
            this.labelUploadSpeed.Name = "labelUploadSpeed";
            this.labelUploadSpeed.Size = new System.Drawing.Size(71, 12);
            this.labelUploadSpeed.TabIndex = 3;
            this.labelUploadSpeed.Text = "UploadSpeed";
            // 
            // labelIntervalTime
            // 
            this.labelIntervalTime.AutoSize = true;
            this.labelIntervalTime.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelIntervalTime.ForeColor = System.Drawing.Color.Black;
            this.labelIntervalTime.Location = new System.Drawing.Point(40, 40);
            this.labelIntervalTime.Name = "labelIntervalTime";
            this.labelIntervalTime.Size = new System.Drawing.Size(77, 12);
            this.labelIntervalTime.TabIndex = 0;
            this.labelIntervalTime.Text = "IntervalTime";
            // 
            // panelUploadSpeedBox
            // 
            this.panelUploadSpeedBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelUploadSpeedBox.Controls.Add(this.textBoxUploadSpeed);
            this.panelUploadSpeedBox.Location = new System.Drawing.Point(208, 64);
            this.panelUploadSpeedBox.Name = "panelUploadSpeedBox";
            this.panelUploadSpeedBox.Size = new System.Drawing.Size(64, 28);
            this.panelUploadSpeedBox.TabIndex = 4;
            // 
            // textBoxUploadSpeed
            // 
            this.textBoxUploadSpeed.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxUploadSpeed.Font = new System.Drawing.Font("宋体", 9F);
            this.textBoxUploadSpeed.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.textBoxUploadSpeed.Location = new System.Drawing.Point(3, 7);
            this.textBoxUploadSpeed.MaxLength = 4;
            this.textBoxUploadSpeed.Name = "textBoxUploadSpeed";
            this.textBoxUploadSpeed.Size = new System.Drawing.Size(54, 14);
            this.textBoxUploadSpeed.TabIndex = 4;
            // 
            // panelDownloadSpeedBox
            // 
            this.panelDownloadSpeedBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelDownloadSpeedBox.Controls.Add(this.textBoxDownloadSpeed);
            this.panelDownloadSpeedBox.Location = new System.Drawing.Point(408, 64);
            this.panelDownloadSpeedBox.Name = "panelDownloadSpeedBox";
            this.panelDownloadSpeedBox.Size = new System.Drawing.Size(64, 28);
            this.panelDownloadSpeedBox.TabIndex = 8;
            // 
            // textBoxDownloadSpeed
            // 
            this.textBoxDownloadSpeed.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxDownloadSpeed.Font = new System.Drawing.Font("宋体", 9F);
            this.textBoxDownloadSpeed.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.textBoxDownloadSpeed.Location = new System.Drawing.Point(3, 7);
            this.textBoxDownloadSpeed.MaxLength = 4;
            this.textBoxDownloadSpeed.Name = "textBoxDownloadSpeed";
            this.textBoxDownloadSpeed.Size = new System.Drawing.Size(54, 14);
            this.textBoxDownloadSpeed.TabIndex = 8;
            // 
            // panelIntervalTimeBox
            // 
            this.panelIntervalTimeBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelIntervalTimeBox.Controls.Add(this.listBoxIntervalTime);
            this.panelIntervalTimeBox.Location = new System.Drawing.Point(48, 64);
            this.panelIntervalTimeBox.Name = "panelIntervalTimeBox";
            this.panelIntervalTimeBox.Size = new System.Drawing.Size(48, 44);
            this.panelIntervalTimeBox.TabIndex = 1;
            // 
            // listBoxIntervalTime
            // 
            this.listBoxIntervalTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBoxIntervalTime.Font = new System.Drawing.Font("宋体", 9F);
            this.listBoxIntervalTime.ForeColor = System.Drawing.Color.Black;
            this.listBoxIntervalTime.FormattingEnabled = true;
            this.listBoxIntervalTime.ItemHeight = 12;
            this.listBoxIntervalTime.Items.AddRange(new object[] {
            "20",
            "30",
            "60",
            "90",
            "120",
            "180",
            "240",
            "300",
            "360"});
            this.listBoxIntervalTime.Location = new System.Drawing.Point(3, 3);
            this.listBoxIntervalTime.Name = "listBoxIntervalTime";
            this.listBoxIntervalTime.Size = new System.Drawing.Size(40, 36);
            this.listBoxIntervalTime.TabIndex = 1;
            // 
            // buttonSync
            // 
            this.buttonSync.BackColor = System.Drawing.Color.WhiteSmoke;
            this.buttonSync.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.buttonSync.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSync.Font = new System.Drawing.Font("宋体", 9F);
            this.buttonSync.ForeColor = System.Drawing.Color.Black;
            this.buttonSync.Location = new System.Drawing.Point(648, 372);
            this.buttonSync.Name = "buttonSync";
            this.buttonSync.Size = new System.Drawing.Size(120, 32);
            this.buttonSync.TabIndex = 4;
            this.buttonSync.Text = "Sync";
            this.buttonSync.UseVisualStyleBackColor = false;
            this.buttonSync.Click += new System.EventHandler(this.ButtonSync_Click);
            // 
            // buttonWeb
            // 
            this.buttonWeb.BackColor = System.Drawing.Color.WhiteSmoke;
            this.buttonWeb.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.buttonWeb.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonWeb.Font = new System.Drawing.Font("宋体", 9F);
            this.buttonWeb.ForeColor = System.Drawing.Color.Black;
            this.buttonWeb.Location = new System.Drawing.Point(648, 412);
            this.buttonWeb.Name = "buttonWeb";
            this.buttonWeb.Size = new System.Drawing.Size(120, 32);
            this.buttonWeb.TabIndex = 5;
            this.buttonWeb.Text = "Web";
            this.buttonWeb.UseVisualStyleBackColor = false;
            this.buttonWeb.Click += new System.EventHandler(this.ButtonWeb_Click);
            // 
            // buttonHide
            // 
            this.buttonHide.BackColor = System.Drawing.Color.WhiteSmoke;
            this.buttonHide.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.buttonHide.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonHide.Font = new System.Drawing.Font("宋体", 9F);
            this.buttonHide.Location = new System.Drawing.Point(648, 512);
            this.buttonHide.Name = "buttonHide";
            this.buttonHide.Size = new System.Drawing.Size(120, 32);
            this.buttonHide.TabIndex = 7;
            this.buttonHide.Text = "Hide";
            this.buttonHide.UseVisualStyleBackColor = false;
            this.buttonHide.Click += new System.EventHandler(this.ButtonHide_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.listViewData);
            this.Controls.Add(this.buttonHide);
            this.Controls.Add(this.buttonWeb);
            this.Controls.Add(this.buttonSync);
            this.Controls.Add(this.groupBoxSync);
            this.Font = new System.Drawing.Font("宋体", 9F);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.Text = "dboxShare Syncer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.contextMenuStrip.ResumeLayout(false);
            this.groupBoxSync.ResumeLayout(false);
            this.groupBoxSync.PerformLayout();
            this.panelUploadSpeedBox.ResumeLayout(false);
            this.panelUploadSpeedBox.PerformLayout();
            this.panelDownloadSpeedBox.ResumeLayout(false);
            this.panelDownloadSpeedBox.PerformLayout();
            this.panelIntervalTimeBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemMain;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSync;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemWeb;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemQuit;
        private System.Windows.Forms.ListView listViewData;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.GroupBox groupBoxSync;
        private System.Windows.Forms.Label labelUploadKBS;
        private System.Windows.Forms.Label labelDownloadKBS;
        private System.Windows.Forms.Label labelDownloadSpeed;
        private System.Windows.Forms.Label labelUploadSpeed;
        private System.Windows.Forms.Label labelIntervalTime;
        private System.Windows.Forms.TextBox textBoxDownloadSpeed;
        private System.Windows.Forms.TextBox textBoxUploadSpeed;
        private System.Windows.Forms.ListBox listBoxIntervalTime;
        private System.Windows.Forms.Button buttonSetting;
        private System.Windows.Forms.Button buttonSync;
        private System.Windows.Forms.Button buttonWeb;
        private System.Windows.Forms.Button buttonHide;
        private System.Windows.Forms.Label labelIntervalMinute;
        public System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Label labelDownloadSpeedTips;
        private System.Windows.Forms.Label labelUploadSpeedTips;
        private System.Windows.Forms.Panel panelUploadSpeedBox;
        private System.Windows.Forms.Panel panelDownloadSpeedBox;
        private System.Windows.Forms.Panel panelIntervalTimeBox;
        private System.Windows.Forms.ImageList imageList;
    }
}

