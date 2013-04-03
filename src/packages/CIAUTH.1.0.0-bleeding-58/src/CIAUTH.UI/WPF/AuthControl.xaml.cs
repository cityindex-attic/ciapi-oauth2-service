using System;
using System.Windows;
using System.Windows.Navigation;

namespace CIAUTH.UI.WPF
{
    /// <summary>
    /// </summary>
    public partial class AuthControl
    {
        /// <summary>
        /// </summary>
        public AuthControl()
        {
            InitializeComponent();
            txtUrl.Text = "Loading City Index Authorization....";
        }


        /// <summary>
        /// </summary>
        public event EventHandler<AccessTokenEventArgs> TokenEvent;

        /// <summary>
        /// </summary>
        /// <param name="e"></param>
        public void OnTokenEvent(AccessTokenEventArgs e)
        {
            EventHandler<AccessTokenEventArgs> handler = TokenEvent;
            if (handler != null) handler(this, e);
        }

        /// <summary>
        /// </summary>
        /// <param name="authServer"></param>
        /// <param name="clientId"></param>
        public void ShowLogin(string authServer, string clientId)
        {
            Uri authUri = CommonLogic.CreateAuthUri(authServer, clientId);
            WbLogin.Navigate(authUri);
        }


        private void WbLogin_Navigated(object sender, NavigationEventArgs e)
        {
            SetUrlText(e.Uri);

            if (e.Uri.AbsoluteUri.Contains("complete=true"))
            {
                if (e.Uri.AbsoluteUri.Contains("#"))
                {
                    AccessToken token = CommonLogic.DeserializeAccessToken(e.Uri.AbsoluteUri);

                    var ea = new AccessTokenEventArgs { AccessToken = token, Message = "Login complete" };
                    OnTokenEvent(ea);
                }
                else
                {
                    var ea = new AccessTokenEventArgs { AccessToken = null, Message = "Login failed" };
                    OnTokenEvent(ea);
                }
            }
        }

        private void BtnShowCert_OnClick(object sender, RoutedEventArgs e)
        {
            CommonLogic.ShowCert(WbLogin.Source.AbsoluteUri);
        }

        private void SetUrlText(Uri uri)
        {
            txtUrl.Text = uri.AbsoluteUri;
        }
    }
}