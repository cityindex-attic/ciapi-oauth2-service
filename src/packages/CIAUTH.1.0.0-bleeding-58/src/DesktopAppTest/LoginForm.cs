using System;
using System.Windows.Forms;
using CIAUTH.UI;

namespace DesktopAppTest
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            authControl1.ShowLogin(authServer: "http://23.21.217.245/ciauth", clientId: "654");
        }

        public event EventHandler<AccessTokenEventArgs> TokenEvent;


        private void authControl1_TokenEvent(object sender, AccessTokenEventArgs e)
        {
            EventHandler<AccessTokenEventArgs> handler = TokenEvent;
            if (handler != null) handler(this, e);


            DialogResult = e.Message == "Login complete" ? DialogResult.OK : DialogResult.Cancel;
        }
    }
}