using System;
using System.Windows.Forms;

namespace CIAUTH.UI.NET
{
    /// <summary>
    /// </summary>
    public partial class AuthControl : UserControl
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

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            SetUrlText(e.Url);

            if (e.Url.AbsoluteUri.Contains("complete=true"))
            {
                if (e.Url.AbsoluteUri.Contains("#"))
                {
                    AccessToken token = CommonLogic.DeserializeAccessToken(e.Url.AbsoluteUri);

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

        private void btnShowCert_Click(object sender, EventArgs e)
        {
            CommonLogic.ShowCert(WbLogin.Url.AbsoluteUri);
        }


        private void SetUrlText(Uri uri)
        {
            txtUrl.Text = uri.AbsoluteUri;
            txtUrl.DeselectAll();
        }
    }
}