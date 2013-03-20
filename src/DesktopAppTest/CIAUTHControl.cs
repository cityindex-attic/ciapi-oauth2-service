using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.IO;

using System.Net;
using System.Web;
using Newtonsoft.Json;
using System.Configuration;

namespace DesktopAppTest
{
    public partial class CIAUTHControl : UserControl
    {
        public CIAUTHControl()
        {
            InitializeComponent();
            txtUrl.Text = "Loading City Index Authorization....";
        }


        public void ShowCert()
        {
            //Do webrequest to get info on secure site
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(txtUrl.Text);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();

            //retrieve the ssl cert and assign it to an X509Certificate object
            X509Certificate cert = request.ServicePoint.Certificate;

            if (cert == null) {
                MessageBox.Show("Site has no SSL certificate");
                return;
            }

            //convert the X509Certificate to an X509Certificate2 object by passing it into the constructor
            X509Certificate2 cert2 = new X509Certificate2(cert);

            //display the cert dialog box
            X509Certificate2UI.DisplayCertificate(cert2);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
        public event EventHandler<AccessTokenEventArgs> TokenEvent;

        public void OnTokenEvent(AccessTokenEventArgs e)
        {
            EventHandler<AccessTokenEventArgs> handler = TokenEvent;
            if (handler != null) handler(this, e);
        }
        public void ShowLogin()
        {
            string authServer = ConfigurationManager.AppSettings["auth_server"];
            webBrowser1.Navigate(authServer + "/Authorize?response_type=code&client_id=654&redirect_uri=" + HttpUtility.UrlEncode(authServer + "/authorize/callback") + "&state=statevalue");
        }
        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            txtUrl.Text = e.Url.AbsoluteUri;
            txtUrl.DeselectAll();
            if (e.Url.AbsoluteUri.Contains("complete=true"))
            {
                if (e.Url.AbsoluteUri.Contains("#"))
                {
                    var tokenText = e.Url.AbsoluteUri.Substring(e.Url.AbsoluteUri.IndexOf("#") + 1);
                    tokenText = HttpUtility.UrlDecode(tokenText);
                    AccessToken token = JsonConvert.DeserializeObject<AccessToken>(tokenText);

                    AccessTokenEventArgs ea = new AccessTokenEventArgs() { AccessToken = token, Message = "Login complete" };
                    OnTokenEvent(ea);
                    
                }
                else
                {
                    AccessTokenEventArgs ea = new AccessTokenEventArgs() { AccessToken = null, Message = "Login failed" };
                    OnTokenEvent(ea);
                    
                }
            }
        }

        private void btnShowCert_Click(object sender, EventArgs e)
        {
            ShowCert();
        }
    }
}
