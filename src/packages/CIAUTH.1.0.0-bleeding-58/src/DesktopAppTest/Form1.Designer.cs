namespace DesktopAppTest
{
    partial class Form1
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
            this.btn_login = new System.Windows.Forms.Button();
            this.btn_logout = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbl_username = new System.Windows.Forms.Label();
            this.lbl_session = new System.Windows.Forms.Label();
            this.lbl_message = new System.Windows.Forms.Label();
            this.btn_account_info = new System.Windows.Forms.Button();
            this.tb__account_info = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btn_login
            // 
            this.btn_login.Location = new System.Drawing.Point(12, 12);
            this.btn_login.Name = "btn_login";
            this.btn_login.Size = new System.Drawing.Size(75, 23);
            this.btn_login.TabIndex = 0;
            this.btn_login.Text = "Log In";
            this.btn_login.UseVisualStyleBackColor = true;
            this.btn_login.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_logout
            // 
            this.btn_logout.Enabled = false;
            this.btn_logout.Location = new System.Drawing.Point(93, 12);
            this.btn_logout.Name = "btn_logout";
            this.btn_logout.Size = new System.Drawing.Size(75, 23);
            this.btn_logout.TabIndex = 1;
            this.btn_logout.Text = "Log Out";
            this.btn_logout.UseVisualStyleBackColor = true;
            this.btn_logout.Click += new System.EventHandler(this.btn_logout_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Username:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(103, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Session ID:";
            // 
            // lbl_username
            // 
            this.lbl_username.AutoSize = true;
            this.lbl_username.Location = new System.Drawing.Point(16, 69);
            this.lbl_username.Name = "lbl_username";
            this.lbl_username.Size = new System.Drawing.Size(0, 13);
            this.lbl_username.TabIndex = 5;
            // 
            // lbl_session
            // 
            this.lbl_session.AutoSize = true;
            this.lbl_session.Location = new System.Drawing.Point(106, 68);
            this.lbl_session.Name = "lbl_session";
            this.lbl_session.Size = new System.Drawing.Size(0, 13);
            this.lbl_session.TabIndex = 6;
            // 
            // lbl_message
            // 
            this.lbl_message.AutoSize = true;
            this.lbl_message.Location = new System.Drawing.Point(13, 86);
            this.lbl_message.Name = "lbl_message";
            this.lbl_message.Size = new System.Drawing.Size(0, 13);
            this.lbl_message.TabIndex = 7;
            // 
            // btn_account_info
            // 
            this.btn_account_info.Enabled = false;
            this.btn_account_info.Location = new System.Drawing.Point(19, 107);
            this.btn_account_info.Name = "btn_account_info";
            this.btn_account_info.Size = new System.Drawing.Size(149, 23);
            this.btn_account_info.TabIndex = 8;
            this.btn_account_info.Text = "Account Info";
            this.btn_account_info.UseVisualStyleBackColor = true;
            this.btn_account_info.Click += new System.EventHandler(this.btn_account_info_Click);
            // 
            // tb__account_info
            // 
            this.tb__account_info.Location = new System.Drawing.Point(19, 136);
            this.tb__account_info.Multiline = true;
            this.tb__account_info.Name = "tb__account_info";
            this.tb__account_info.Size = new System.Drawing.Size(388, 114);
            this.tb__account_info.TabIndex = 9;
            this.tb__account_info.WordWrap = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 262);
            this.Controls.Add(this.tb__account_info);
            this.Controls.Add(this.btn_account_info);
            this.Controls.Add(this.lbl_message);
            this.Controls.Add(this.lbl_session);
            this.Controls.Add(this.lbl_username);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_logout);
            this.Controls.Add(this.btn_login);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_login;
        private System.Windows.Forms.Button btn_logout;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lbl_username;
        private System.Windows.Forms.Label lbl_session;
        private System.Windows.Forms.Label lbl_message;
        private System.Windows.Forms.Button btn_account_info;
        private System.Windows.Forms.TextBox tb__account_info;
    }
}

