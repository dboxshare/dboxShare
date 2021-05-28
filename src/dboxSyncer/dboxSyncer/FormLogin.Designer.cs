namespace dboxSyncer
{
    partial class FormLogin
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
            this.labelServerUrl = new System.Windows.Forms.Label();
            this.labelLoginId = new System.Windows.Forms.Label();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxServerUrl = new System.Windows.Forms.TextBox();
            this.textBoxLoginId = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.checkBoxAutoLogin = new System.Windows.Forms.CheckBox();
            this.buttonLogin = new System.Windows.Forms.Button();
            this.labelLoginTips = new System.Windows.Forms.Label();
            this.panelServerUrlBox = new System.Windows.Forms.Panel();
            this.panelLoginIdBox = new System.Windows.Forms.Panel();
            this.panelPasswordBox = new System.Windows.Forms.Panel();
            this.panelServerUrlBox.SuspendLayout();
            this.panelLoginIdBox.SuspendLayout();
            this.panelPasswordBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelServerUrl
            // 
            this.labelServerUrl.AutoSize = true;
            this.labelServerUrl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelServerUrl.ForeColor = System.Drawing.Color.Black;
            this.labelServerUrl.Location = new System.Drawing.Point(16, 16);
            this.labelServerUrl.Name = "labelServerUrl";
            this.labelServerUrl.Size = new System.Drawing.Size(65, 12);
            this.labelServerUrl.TabIndex = 0;
            this.labelServerUrl.Text = "ServerUrl";
            // 
            // labelLoginId
            // 
            this.labelLoginId.AutoSize = true;
            this.labelLoginId.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelLoginId.ForeColor = System.Drawing.Color.Black;
            this.labelLoginId.Location = new System.Drawing.Point(16, 98);
            this.labelLoginId.Name = "labelLoginId";
            this.labelLoginId.Size = new System.Drawing.Size(47, 12);
            this.labelLoginId.TabIndex = 2;
            this.labelLoginId.Text = "LoginId";
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelPassword.ForeColor = System.Drawing.Color.Black;
            this.labelPassword.Location = new System.Drawing.Point(16, 180);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(53, 12);
            this.labelPassword.TabIndex = 4;
            this.labelPassword.Text = "Password";
            // 
            // textBoxServerUrl
            // 
            this.textBoxServerUrl.BackColor = System.Drawing.Color.White;
            this.textBoxServerUrl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxServerUrl.Font = new System.Drawing.Font("宋体", 9F);
            this.textBoxServerUrl.ForeColor = System.Drawing.Color.Black;
            this.textBoxServerUrl.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.textBoxServerUrl.Location = new System.Drawing.Point(3, 7);
            this.textBoxServerUrl.MaxLength = 50;
            this.textBoxServerUrl.Name = "textBoxServerUrl";
            this.textBoxServerUrl.Size = new System.Drawing.Size(275, 14);
            this.textBoxServerUrl.TabIndex = 1;
            // 
            // textBoxLoginId
            // 
            this.textBoxLoginId.BackColor = System.Drawing.Color.White;
            this.textBoxLoginId.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxLoginId.Font = new System.Drawing.Font("宋体", 9F);
            this.textBoxLoginId.ForeColor = System.Drawing.Color.Black;
            this.textBoxLoginId.Location = new System.Drawing.Point(3, 7);
            this.textBoxLoginId.MaxLength = 16;
            this.textBoxLoginId.Name = "textBoxLoginId";
            this.textBoxLoginId.Size = new System.Drawing.Size(275, 14);
            this.textBoxLoginId.TabIndex = 3;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.BackColor = System.Drawing.Color.White;
            this.textBoxPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxPassword.Font = new System.Drawing.Font("宋体", 9F);
            this.textBoxPassword.ForeColor = System.Drawing.Color.Black;
            this.textBoxPassword.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.textBoxPassword.Location = new System.Drawing.Point(3, 7);
            this.textBoxPassword.MaxLength = 16;
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(275, 14);
            this.textBoxPassword.TabIndex = 5;
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // checkBoxAutoLogin
            // 
            this.checkBoxAutoLogin.AutoSize = true;
            this.checkBoxAutoLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxAutoLogin.ForeColor = System.Drawing.Color.Black;
            this.checkBoxAutoLogin.Location = new System.Drawing.Point(24, 262);
            this.checkBoxAutoLogin.Name = "checkBoxAutoLogin";
            this.checkBoxAutoLogin.Size = new System.Drawing.Size(75, 16);
            this.checkBoxAutoLogin.TabIndex = 6;
            this.checkBoxAutoLogin.Text = "AutoLogin";
            this.checkBoxAutoLogin.UseVisualStyleBackColor = true;
            // 
            // buttonLogin
            // 
            this.buttonLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(168)))), ((int)(((byte)(188)))));
            this.buttonLogin.FlatAppearance.BorderSize = 0;
            this.buttonLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLogin.Font = new System.Drawing.Font("宋体", 9F);
            this.buttonLogin.ForeColor = System.Drawing.Color.White;
            this.buttonLogin.Location = new System.Drawing.Point(24, 294);
            this.buttonLogin.Name = "buttonLogin";
            this.buttonLogin.Size = new System.Drawing.Size(286, 32);
            this.buttonLogin.TabIndex = 7;
            this.buttonLogin.Text = "Login";
            this.buttonLogin.UseVisualStyleBackColor = false;
            this.buttonLogin.Click += new System.EventHandler(this.ButtonLogin_Click);
            // 
            // labelLoginTips
            // 
            this.labelLoginTips.AutoSize = true;
            this.labelLoginTips.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelLoginTips.Font = new System.Drawing.Font("宋体", 9F);
            this.labelLoginTips.ForeColor = System.Drawing.Color.Red;
            this.labelLoginTips.Location = new System.Drawing.Point(16, 358);
            this.labelLoginTips.Name = "labelLoginTips";
            this.labelLoginTips.Size = new System.Drawing.Size(59, 12);
            this.labelLoginTips.TabIndex = 8;
            this.labelLoginTips.Text = "LoginTips";
            // 
            // panelServerUrlBox
            // 
            this.panelServerUrlBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelServerUrlBox.Controls.Add(this.textBoxServerUrl);
            this.panelServerUrlBox.Location = new System.Drawing.Point(24, 40);
            this.panelServerUrlBox.Name = "panelServerUrlBox";
            this.panelServerUrlBox.Size = new System.Drawing.Size(285, 28);
            this.panelServerUrlBox.TabIndex = 1;
            // 
            // panelLoginIdBox
            // 
            this.panelLoginIdBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelLoginIdBox.Controls.Add(this.textBoxLoginId);
            this.panelLoginIdBox.Location = new System.Drawing.Point(24, 122);
            this.panelLoginIdBox.Name = "panelLoginIdBox";
            this.panelLoginIdBox.Size = new System.Drawing.Size(285, 28);
            this.panelLoginIdBox.TabIndex = 3;
            // 
            // panelPasswordBox
            // 
            this.panelPasswordBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPasswordBox.Controls.Add(this.textBoxPassword);
            this.panelPasswordBox.Location = new System.Drawing.Point(24, 204);
            this.panelPasswordBox.Name = "panelPasswordBox";
            this.panelPasswordBox.Size = new System.Drawing.Size(285, 28);
            this.panelPasswordBox.TabIndex = 5;
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(334, 411);
            this.Controls.Add(this.labelLoginTips);
            this.Controls.Add(this.checkBoxAutoLogin);
            this.Controls.Add(this.buttonLogin);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.labelLoginId);
            this.Controls.Add(this.labelServerUrl);
            this.Controls.Add(this.panelServerUrlBox);
            this.Controls.Add(this.panelLoginIdBox);
            this.Controls.Add(this.panelPasswordBox);
            this.Font = new System.Drawing.Font("宋体", 9F);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLogin";
            this.Text = "dboxShare Syncer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLogin_FormClosing);
            this.Load += new System.EventHandler(this.FormLogin_Load);
            this.panelServerUrlBox.ResumeLayout(false);
            this.panelServerUrlBox.PerformLayout();
            this.panelLoginIdBox.ResumeLayout(false);
            this.panelLoginIdBox.PerformLayout();
            this.panelPasswordBox.ResumeLayout(false);
            this.panelPasswordBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelServerUrl;
        private System.Windows.Forms.Label labelLoginId;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox textBoxServerUrl;
        private System.Windows.Forms.TextBox textBoxLoginId;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.CheckBox checkBoxAutoLogin;
        private System.Windows.Forms.Button buttonLogin;
        private System.Windows.Forms.Label labelLoginTips;
        private System.Windows.Forms.Panel panelServerUrlBox;
        private System.Windows.Forms.Panel panelLoginIdBox;
        private System.Windows.Forms.Panel panelPasswordBox;
    }
}