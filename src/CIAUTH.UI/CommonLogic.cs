using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CIAUTH.UI
{
    /// <summary>
    /// </summary>
    public static class CommonLogic
    {
        /// <summary>
        /// </summary>
        /// <param name="url"></param>
        public static void ShowCert(string url)
        {
            //Do webrequest to get info on secure site
            var request = (HttpWebRequest) WebRequest.Create(url);
            var response = (HttpWebResponse) request.GetResponse();
            response.Close();

            //retrieve the ssl cert and assign it to an X509Certificate object
            X509Certificate cert = request.ServicePoint.Certificate;

            if (cert == null)
            {
                MessageBox.Show("Site has no SSL certificate");
                return;
            }

            //convert the X509Certificate to an X509Certificate2 object by passing it into the constructor
            var cert2 = new X509Certificate2(cert);

            //display the cert dialog box
            X509Certificate2UI.DisplayCertificate(cert2);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public static AccessToken DeserializeAccessToken(string absoluteUri)
        {
            string tokenText = absoluteUri.Substring(absoluteUri.IndexOf("#", StringComparison.Ordinal) + 1);
            tokenText = HttpUtility.UrlDecode(tokenText);
            var token = JsonConvert.DeserializeObject<AccessToken>(tokenText);
            token.UserName = token.access_token.Substring(0, token.access_token.IndexOf(":", StringComparison.Ordinal));
            token.SessionId = token.access_token.Substring(token.access_token.IndexOf(":", StringComparison.Ordinal) + 1);
            return token;
        }

        /// <summary>
        /// </summary>
        /// <param name="authServer"></param>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public static Uri CreateAuthUri(string authServer, string clientId)
        {
            string callback = HttpUtility.UrlEncode(authServer + "/authorize/callback");
            string uriString = String.Format(
                "{0}/Authorize?response_type=code&client_id={1}&redirect_uri={2}&state=statevalue", authServer, clientId,
                callback);
            var authUri = new Uri(uriString);
            return authUri;
        }
    }
}