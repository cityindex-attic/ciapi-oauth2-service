using System;
using CIAUTH.UI;

namespace WpfAppTest
{
    public partial class LoginWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
            AuthControl.ShowLogin(authServer: "http://23.21.217.245/ciauth", clientId: "654");
        }

        public event EventHandler<AccessTokenEventArgs> TokenEvent;

        private void AuthControl_TokenEvent_1(object sender, AccessTokenEventArgs e)
        {
            EventHandler<AccessTokenEventArgs> handler = TokenEvent;
            if (handler != null) handler(this, e);

            DialogResult = e.Message == "Login complete";
        }
    }
}