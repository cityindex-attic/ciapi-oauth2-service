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
            this.ciauthControl1 = new DesktopAppTest.CIAUTHControl();
            this.SuspendLayout();
            // 
            // ciauthControl1
            // 
            this.ciauthControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ciauthControl1.Location = new System.Drawing.Point(0, 0);
            this.ciauthControl1.Name = "ciauthControl1";
            this.ciauthControl1.Size = new System.Drawing.Size(838, 596);
            this.ciauthControl1.TabIndex = 0;
            this.ciauthControl1.TokenEvent += new System.EventHandler<DesktopAppTest.AccessTokenEventArgs>(this.ciauthControl1_TokenEvent);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(838, 596);
            this.Controls.Add(this.ciauthControl1);
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            this.ResumeLayout(false);

        }

        #endregion

        private CIAUTHControl ciauthControl1;

    }
}