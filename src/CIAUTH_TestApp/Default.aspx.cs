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

namespace CIAUTH_TestApp
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string auth = Request["auth"];
            if (auth != null)
            {
 

                string authServer = WebConfigurationManager.AppSettings["authServer"];

                var client = new WebClient();
                var payload =
                    client.DownloadString(authServer + "/api/Decrypt/" + HttpUtility.UrlEncode(auth));

                // payload is in form "username:session" because WebApi return 'json' strings. 
                // we can return text/plain but involves custom content formatters. !?
                // so we just leave it as is and munge the return

                payload=payload.Trim('"');

                var pair = payload.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                var username = pair[0];
                var session = pair[1];

                // save it for later user
                Session["CIAPI_SESSION"] = session;
                Session["CIAPI_USERNAME"] = username;
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
            Response.Redirect(authServer + "?returnUrl=" + HttpUtility.UrlEncode(Request.Url.ToString()));
        }
    }
}