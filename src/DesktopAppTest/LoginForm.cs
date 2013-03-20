using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Newtonsoft.Json;
using CIAUTH.UI;
namespace DesktopAppTest
{
    public partial class LoginForm : Form
    {
        public event EventHandler<AccessTokenEventArgs> TokenEvent;

        public void OnTokenEvent(AccessTokenEventArgs e)
        {
            EventHandler<AccessTokenEventArgs> handler = TokenEvent;
            if (handler != null) handler(this, e);
            if (e.Message == "Login complete")
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }

        }

        public LoginForm()
        {
            InitializeComponent();
        }

        public void ShowLogin()
        {
            authControl1.ShowLogin();
        }


        private void authControl1_TokenEvent(object sender, CIAUTH.UI.AccessTokenEventArgs e)
        {
            OnTokenEvent(e);
        }
    }
}
