using System;
using System.Net;
using System.Windows.Navigation;
using Newtonsoft.Json;

namespace CIAUTH.UI
{
    public partial class AuthControl
    {
        public AuthControl()
        {
            InitializeComponent();
        }

        public string AuthServer { get; set; }
        public event EventHandler<AccessTokenEventArgs> TokenEvent;

        public void OnTokenEvent(AccessTokenEventArgs e)
        {
            EventHandler<AccessTokenEventArgs> handler = TokenEvent;
            if (handler != null) handler(this, e);
        }

        public void ShowLogin()
        {
            WbLogin.Navigate(
                new Uri(AuthServer + "/Authorize?response_type=code&client_id=654&redirect_uri=" +
                        HttpUtility.UrlEncode(AuthServer + "/authorize/callback") + "&state=statevalue"));
        }

        private void WbLogin_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Uri.AbsoluteUri.Contains("complete=true"))
            {
                if (e.Uri.AbsoluteUri.Contains("#"))
                {
                    string tokenText = e.Uri.AbsoluteUri.Substring(e.Uri.AbsoluteUri.IndexOf("#", StringComparison.Ordinal) + 1);
                    tokenText = HttpUtility.UrlDecode(tokenText);
                    var token = JsonConvert.DeserializeObject<AccessToken>(tokenText);

                    var ea = new AccessTokenEventArgs {AccessToken = token, Message = "Login complete"};
                    OnTokenEvent(ea);
                }
                else
                {
                    var ea = new AccessTokenEventArgs {AccessToken = null, Message = "Login failed"};
                    OnTokenEvent(ea);
                }
            }
        }
    }
}