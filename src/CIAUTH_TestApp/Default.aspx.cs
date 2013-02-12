using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using CIAPI.DTO;
using CIAPI.Rpc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CIAUTH_TestApp
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string state = Request["state"];
            string code = Request["code"];

            if (code != null)
            {

                string authServer = WebConfigurationManager.AppSettings["authServer"];

                var grant_type = "authorization_code";
                var client_id = "123";
                var client_secret = "456";

                var client = new WebClient();
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                string upload =
                    string.Format("client_id={0}&client_secret={1}&grant_type={2}&code={3}", HttpUtility.UrlEncode(client_id), HttpUtility.UrlEncode(client_secret), HttpUtility.UrlEncode(grant_type), HttpUtility.UrlEncode(code));
                var payload = client.UploadString(authServer + "/Token", upload);
                // payload is in form "username:session" because WebApi return 'json' strings. 
                // we can return text/plain but involves custom content formatters. !?
                // so we just leave it as is and munge the return

                JObject payloadobj = (JObject) JsonConvert.DeserializeObject(payload);

                string refresh_token = payloadobj["refresh_token"].Value<string>();

                string access_token = payloadobj["access_token"].Value<string>();

                // #TODO: clarify expire date

                var pair = access_token.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                var username = pair[0];
                var session = pair[1];

                // save it for later user
                Session["CIAPI_SESSION"] = session;
                Session["CIAPI_USERNAME"] = username;
                
                Session["CIAPI_REFRESHTOKEN"] = refresh_token;
            }


            // fancy app code


            if (Session["CIAPI_SESSION"] == null)
            {
                // not authorized, show button

                AuthPanel.Visible = true;
                CIAPIPanel.Visible = false;
            }
            else
            {
                // authorized. do something with session

                CIAPIPanel.Visible = true;
                AuthPanel.Visible = false;



                string savedSession = (string)Session["CIAPI_SESSION"];
                string savedUsername = (string)Session["CIAPI_USERNAME"];


                AccountInformationResponseDTO result;

                using (var client = new Client(new Uri("https://ciapi.cityindex.com/tradingapi"), new Uri("http://foo.com"), "CIAPIAuthConsumer"))
                {

                    client.LogInUsingSession(savedUsername, savedSession);

                    result = client.AccountInformation.GetClientAndTradingAccount();
                }

                LogonUserNameLabel.Text = result.LogonUserName;


            }
        }

        protected void AuthButton_Click(object sender, EventArgs e)
        {

            string authServer = WebConfigurationManager.AppSettings["authServer"];
            Response.Redirect(authServer + "/Authorize?response_type=code&client_id=12345&redirect_uri=" + HttpUtility.UrlEncode(Request.Url.ToString()));
        }
    }
}