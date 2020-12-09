namespace dboxSyncer
{
    partial class FormFolderAdd
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
            this.labelLocalFolder = new System.Windows.Forms.Label();
            this.labelNetFolder = new System.Windows.Forms.Label();
            this.textBoxLocalFolderPath = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.textBoxNetFolderPath = new System.Windows.Forms.TextBox();
            this.textBoxNetFolderId = new System.Windows.Forms.TextBox();
            this.treeViewNetFolder = new System.Windows.Forms.TreeView();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.labelLoadingTips = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.labelType = new System.Windows.Forms.Label();
            this.radioButtonTypeSync = new System.Windows.Forms.RadioButton();
            this.radioButtonTypeUpload = new System.Windows.Forms.RadioButton();
            this.radioButtonTypeDownload = new System.Windows.Forms.RadioButton();
            this.labelTypeTips = new System.Windows.Forms.Label();
            this.labelSubfolderTips = new System.Windows.Forms.Label();
            this.panelLocalFolderPathBox = new System.Windows.Forms.Panel();
            this.panelNetFolderPathBox = new System.Windows.Forms.Panel();
            this.panelLocalFolderPathBox.SuspendLayout();
            this.panelNetFolderPathBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelLocalFolder
            // 
            this.labelLocalFolder.AutoSize = true;
            this.labelLocalFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelLocalFolder.ForeColor = System.Drawing.Color.Black;
            this.labelLocalFolder.Location = new System.Drawing.Point(16, 16);
            this.labelLocalFolder.Name = "labelLocalFolder";
            this.labelLocalFolder.Size = new System.Drawing.Size(71, 12);
            this.labelLocalFolder.TabIndex = 0;
            this.labelLocalFolder.Text = "LocalFolder";
            // 
            // labelNetFolder
            // 
            this.labelNetFolder.AutoSize = true;
            this.labelNetFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelNetFolder.ForeColor = System.Drawing.Color.Black;
            this.labelNetFolder.Location = new System.Drawing.Point(16, 98);
            this.labelNetFolder.Name = "labelNetFolder";
            this.labelNetFolder.Size = new System.Drawing.Size(59, 12);
            this.labelNetFolder.TabIndex = 3;
            this.labelNetFolder.Text = "NetFolder";
            // 
            // textBoxLocalFolderPath
            // 
            this.textBoxLocalFolderPath.BackColor = System.Drawing.Color.White;
            this.textBoxLocalFolderPath.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxLocalFolderPath.Font = new System.Drawing.Font("宋体", 9F);
            this.textBoxLocalFolderPath.ForeColor = System.Drawing.Color.Black;
            this.textBoxLocalFolderPath.Location = new System.Drawing.Point(3, 7);
            this.textBoxLocalFolderPath.Name = "textBoxLocalFolderPath";
            this.textBoxLocalFolderPath.ReadOnly = true;
            this.textBoxLocalFolderPath.Size = new System.Drawing.Size(442, 14);
            this.textBoxLocalFolderPath.TabIndex = 1;
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.BackColor = System.Drawing.Color.WhiteSmoke;
            this.buttonBrowse.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.buttonBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonBrowse.Font = new System.Drawing.Font("宋体", 9F);
            this.buttonBrowse.Location = new System.Drawing.Point(480, 40);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(80, 28);
            this.buttonBrowse.TabIndex = 2;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = false;
            this.buttonBrowse.Click += new System.EventHandler(this.ButtonBrowse_Click);
            // 
            // textBoxNetFolderPath
            // 
            this.textBoxNetFolderPath.BackColor = System.Drawing.Color.White;
            this.textBoxNetFolderPath.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxNetFolderPath.Font = new System.Drawing.Font("宋体", 9F);
            this.textBoxNetFolderPath.ForeColor = System.Drawing.Color.Black;
            this.textBoxNetFolderPath.Location = new System.Drawing.Point(3, 7);
            this.textBoxNetFolderPath.Name = "textBoxNetFolderPath";
            this.textBoxNetFolderPath.ReadOnly = true;
            this.textBoxNetFolderPath.Size = new System.Drawing.Size(442, 14);
            this.textBoxNetFolderPath.TabIndex = 4;
            // 
            // textBoxNetFolderId
            // 
            this.textBoxNetFolderId.BackColor = System.Drawing.Color.White;
            this.textBoxNetFolderId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxNetFolderId.Font = new System.Drawing.Font("宋体", 9F);
            this.textBoxNetFolderId.ForeColor = System.Drawing.Color.Black;
            this.textBoxNetFolderId.Location = new System.Drawing.Point(480, 122);
            this.textBoxNetFolderId.Name = "textBoxNetFolderId";
            this.textBoxNetFolderId.Size = new System.Drawing.Size(21, 21);
            this.textBoxNetFolderId.TabIndex = 5;
            this.textBoxNetFolderId.Visible = false;
            // 
            // treeViewNetFolder
            // 
            this.treeViewNetFolder.BackColor = System.Drawing.Color.WhiteSmoke;
            this.treeViewNetFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeViewNetFolder.Font = new System.Drawing.Font("宋体", 9F);
            this.treeViewNetFolder.ForeColor = System.Drawing.Color.Black;
            this.treeViewNetFolder.FullRowSelect = true;
            this.treeViewNetFolder.ItemHeight = 20;
            this.treeViewNetFolder.Location = new System.Drawing.Point(24, 156);
            this.treeViewNetFolder.Name = "treeViewNetFolder";
            this.treeViewNetFolder.Size = new System.Drawing.Size(452, 240);
            this.treeViewNetFolder.TabIndex = 6;
            this.treeViewNetFolder.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeViewNetFolder_AfterSelect);
            // 
            // buttonAdd
            // 
            this.buttonAdd.BackColor = System.Drawing.Color.WhiteSmoke;
            this.buttonAdd.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAdd.Font = new System.Drawing.Font("宋体", 9F);
            this.buttonAdd.Location = new System.Drawing.Point(16, 512);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(100, 32);
            this.buttonAdd.TabIndex = 13;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = false;
            this.buttonAdd.Click += new System.EventHandler(this.ButtonAdd_Click);
            // 
            // labelLoadingTips
            // 
            this.labelLoadingTips.AutoSize = true;
            this.labelLoadingTips.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelLoadingTips.Location = new System.Drawing.Point(40, 172);
            this.labelLoadingTips.Name = "labelLoadingTips";
            this.labelLoadingTips.Size = new System.Drawing.Size(71, 12);
            this.labelLoadingTips.TabIndex = 7;
            this.labelLoadingTips.Text = "LoadingTips";
            // 
            // labelType
            // 
            this.labelType.AutoSize = true;
            this.labelType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelType.Location = new System.Drawing.Point(16, 428);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(29, 12);
            this.labelType.TabIndex = 8;
            this.labelType.Text = "Type";
            // 
            // radioButtonTypeSync
            // 
            this.radioButtonTypeSync.AutoSize = true;
            this.radioButtonTypeSync.Checked = true;
            this.radioButtonTypeSync.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButtonTypeSync.Location = new System.Drawing.Point(24, 452);
            this.radioButtonTypeSync.Name = "radioButtonTypeSync";
            this.radioButtonTypeSync.Size = new System.Drawing.Size(46, 16);
            this.radioButtonTypeSync.TabIndex = 10;
            this.radioButtonTypeSync.TabStop = true;
            this.radioButtonTypeSync.Text = "Sync";
            this.radioButtonTypeSync.UseVisualStyleBackColor = true;
            // 
            // radioButtonTypeUpload
            // 
            this.radioButtonTypeUpload.AutoSize = true;
            this.radioButtonTypeUpload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButtonTypeUpload.Location = new System.Drawing.Point(140, 452);
            this.radioButtonTypeUpload.Name = "radioButtonTypeUpload";
            this.radioButtonTypeUpload.Size = new System.Drawing.Size(58, 16);
            this.radioButtonTypeUpload.TabIndex = 11;
            this.radioButtonTypeUpload.TabStop = true;
            this.radioButtonTypeUpload.Text = "Upload";
            this.radioButtonTypeUpload.UseVisualStyleBackColor = true;
            // 
            // radioButtonTypeDownload
            // 
            this.radioButtonTypeDownload.AutoSize = true;
            this.radioButtonTypeDownload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.radioButtonTypeDownload.Location = new System.Drawing.Point(286, 452);
            this.radioButtonTypeDownload.Name = "radioButtonTypeDownload";
            this.radioButtonTypeDownload.Size = new System.Drawing.Size(70, 16);
            this.radioButtonTypeDownload.TabIndex = 12;
            this.radioButtonTypeDownload.TabStop = true;
            this.radioButtonTypeDownload.Text = "Download";
            this.radioButtonTypeDownload.UseVisualStyleBackColor = true;
            // 
            // labelTypeTips
            // 
            this.labelTypeTips.AutoSize = true;
            this.labelTypeTips.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelTypeTips.ForeColor = System.Drawing.Color.Gray;
            this.labelTypeTips.Location = new System.Drawing.Point(59, 428);
            this.labelTypeTips.Name = "labelTypeTips";
            this.labelTypeTips.Size = new System.Drawing.Size(53, 12);
            this.labelTypeTips.TabIndex = 9;
            this.labelTypeTips.Text = "TypeTips";
            // 
            // labelSubfolderTips
            // 
            this.labelSubfolderTips.AutoSize = true;
            this.labelSubfolderTips.ForeColor = System.Drawing.Color.Gray;
            this.labelSubfolderTips.Location = new System.Drawing.Point(128, 530);
            this.labelSubfolderTips.Name = "labelSubfolderTips";
            this.labelSubfolderTips.Size = new System.Drawing.Size(83, 12);
            this.labelSubfolderTips.TabIndex = 14;
            this.labelSubfolderTips.Text = "SubfolderTips";
            // 
            // panelLocalFolderPathBox
            // 
            this.panelLocalFolderPathBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLocalFolderPathBox.Controls.Add(this.textBoxLocalFolderPath);
            this.panelLocalFolderPathBox.Location = new System.Drawing.Point(24, 40);
            this.panelLocalFolderPathBox.Name = "panelLocalFolderPathBox";
            this.panelLocalFolderPathBox.Size = new System.Drawing.Size(452, 28);
            this.panelLocalFolderPathBox.TabIndex = 1;
            // 
            // panelNetFolderPathBox
            // 
            this.panelNetFolderPathBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelNetFolderPathBox.Controls.Add(this.textBoxNetFolderPath);
            this.panelNetFolderPathBox.Location = new System.Drawing.Point(24, 122);
            this.panelNetFolderPathBox.Name = "panelNetFolderPathBox";
            this.panelNetFolderPathBox.Size = new System.Drawing.Size(452, 28);
            this.panelNetFolderPathBox.TabIndex = 4;
            // 
            // FormFolderAdd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(584, 561);
            this.Controls.Add(this.labelSubfolderTips);
            this.Controls.Add(this.labelTypeTips);
            this.Controls.Add(this.radioButtonTypeDownload);
            this.Controls.Add(this.radioButtonTypeUpload);
            this.Controls.Add(this.radioButtonTypeSync);
            this.Controls.Add(this.labelType);
            this.Controls.Add(this.labelLoadingTips);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.treeViewNetFolder);
            this.Controls.Add(this.textBoxNetFolderId);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.labelNetFolder);
            this.Controls.Add(this.labelLocalFolder);
            this.Controls.Add(this.panelLocalFolderPathBox);
            this.Controls.Add(this.panelNetFolderPathBox);
            this.Font = new System.Drawing.Font("宋体", 9F);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFolderAdd";
            this.Text = "dboxShare Syncer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormFolderAdd_FormClosing);
            this.Load += new System.EventHandler(this.FormFolderAdd_Load);
            this.Shown += new System.EventHandler(this.FormFolderAdd_Shown);
            this.panelLocalFolderPathBox.ResumeLayout(false);
            this.panelLocalFolderPathBox.PerformLayout();
            this.panelNetFolderPathBox.ResumeLayout(false);
            this.panelNetFolderPathBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelLocalFolder;
        private System.Windows.Forms.Label labelNetFolder;
        private System.Windows.Forms.TextBox textBoxLocalFolderPath;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.TextBox textBoxNetFolderPath;
        private System.Windows.Forms.TextBox textBoxNetFolderId;
        private System.Windows.Forms.TreeView treeViewNetFolder;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Label labelLoadingTips;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.RadioButton radioButtonTypeSync;
        private System.Windows.Forms.RadioButton radioButtonTypeUpload;
        private System.Windows.Forms.RadioButton radioButtonTypeDownload;
        private System.Windows.Forms.Label labelTypeTips;
        private System.Windows.Forms.Label labelSubfolderTips;
        private System.Windows.Forms.Panel panelLocalFolderPathBox;
        private System.Windows.Forms.Panel panelNetFolderPathBox;
    }
}