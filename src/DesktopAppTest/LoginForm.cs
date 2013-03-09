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

namespace DesktopAppTest
{
    public partial class LoginForm : Form
    {
        public event EventHandler<AccessTokenEventArgs> TokenEvent;

        public void OnTokenEvent(AccessTokenEventArgs e)
        {
            EventHandler<AccessTokenEventArgs> handler = TokenEvent;
            if (handler != null) handler(this, e);
        }

        public LoginForm()
        {
            InitializeComponent();
        }

        public void ShowLogin()
        {
            string authServer = ConfigurationManager.AppSettings["auth_server"];
            webBrowser1.Navigate(authServer + "/Authorize?response_type=code&client_id=654&redirect_uri=" + HttpUtility.UrlEncode(authServer + "/authorize/callback") + "&state=statevalue");
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (e.Url.AbsoluteUri.Contains("complete=true"))
            {
                if (e.Url.AbsoluteUri.Contains("#"))
                {
                    var tokenText = e.Url.AbsoluteUri.Substring(e.Url.AbsoluteUri.IndexOf("#") + 1);
                    tokenText = HttpUtility.UrlDecode(tokenText);
                    AccessToken token = JsonConvert.DeserializeObject<AccessToken>(tokenText);
                    
                    AccessTokenEventArgs ea = new AccessTokenEventArgs(){AccessToken = token,Message = "Login complete"};
                    OnTokenEvent(ea);
                    this.DialogResult=DialogResult.OK;
                }
                else
                {
                    AccessTokenEventArgs ea = new AccessTokenEventArgs() { AccessToken = null, Message = "Login failed" };
                    OnTokenEvent(ea);
                    this.DialogResult=DialogResult.Cancel;
                }
            }
        }
    }
}
