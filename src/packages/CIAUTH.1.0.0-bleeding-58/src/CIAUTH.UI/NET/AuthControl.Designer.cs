namespace CIAUTH.UI.NET
{
    partial class AuthControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.WbLogin = new System.Windows.Forms.WebBrowser();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.btnShowCert = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnShowCert);
            this.panel1.Controls.Add(this.txtUrl);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(811, 43);
            this.panel1.TabIndex = 0;
            // 
            // WbLogin
            // 
            this.WbLogin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WbLogin.Location = new System.Drawing.Point(0, 43);
            this.WbLogin.MinimumSize = new System.Drawing.Size(20, 20);
            this.WbLogin.Name = "WbLogin";
            this.WbLogin.Size = new System.Drawing.Size(811, 525);
            this.WbLogin.TabIndex = 1;
            this.WbLogin.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowser1_Navigated);
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(85, 7);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.ReadOnly = true;
            this.txtUrl.Size = new System.Drawing.Size(723, 20);
            this.txtUrl.TabIndex = 0;
            // 
            // btnShowCert
            // 
            this.btnShowCert.Location = new System.Drawing.Point(4, 4);
            this.btnShowCert.Name = "btnShowCert";
            this.btnShowCert.Size = new System.Drawing.Size(75, 23);
            this.btnShowCert.TabIndex = 1;
            this.btnShowCert.Text = "SSL";
            this.btnShowCert.UseVisualStyleBackColor = true;
            this.btnShowCert.Click += new System.EventHandler(this.btnShowCert_Click);
            // 
            // CIAUTHControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.WbLogin);
            this.Controls.Add(this.panel1);
            this.Name = "CIAUTHControl";
            this.Size = new System.Drawing.Size(811, 568);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnShowCert;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.WebBrowser WbLogin;
    }
}
