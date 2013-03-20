namespace DesktopAppTest
{
    partial class LoginForm
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
            this.authControl1 = new CIAUTH.UI.AuthControl();
            this.SuspendLayout();
            // 
            // authControl1
            // 
            this.authControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.authControl1.Location = new System.Drawing.Point(0, 0);
            this.authControl1.Name = "authControl1";
            this.authControl1.Size = new System.Drawing.Size(838, 596);
            this.authControl1.TabIndex = 0;
            this.authControl1.TokenEvent += new System.EventHandler<CIAUTH.UI.AccessTokenEventArgs>(this.authControl1_TokenEvent);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 596);
            this.Controls.Add(this.authControl1);
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            this.ResumeLayout(false);

        }

        #endregion

        private CIAUTH.UI.AuthControl authControl1;


    }
}