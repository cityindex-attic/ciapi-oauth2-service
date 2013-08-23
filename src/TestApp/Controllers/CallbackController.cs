using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using CIAPI.Rpc;
using CIAuth.Common.Configuration;
using Newtonsoft.Json;
using Salient.ReflectiveLoggingAdapter;

namespace TestApp.Controllers
{
    public class CallbackController : Controller
    {
        static CallbackController()
        {
            LogManager.CreateInnerLogger = (logName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat)
                => new SimpleTraceAppender(logName, logLevel, showLevel, showDateTime, showLogName, dateTimeFormat);
        }
        //
        // GET: /Callback/

        public ActionResult Index(string code, string error, string error_description, string error_uri, string state)
        {
            ViewBag.code = code;
            ViewBag.error = error;
            ViewBag.error_description = error_description;
            ViewBag.error_uri = error_uri;
            ViewBag.state = state;
            if (string.IsNullOrEmpty(error))
            {
                string authServer = CIAuthSection.Instance().AuthServer;

                string client_id = CIAuthSection.Instance().ClientId;
                string client_secret = CIAuthSection.Instance().ClientSecret;
                string grant_type = "authorization_code";

                string parameters = "grant_type=" + grant_type + "&code=" + code + "&client_id=" + client_id + "&client_secret=" + client_secret;



                using (WebClient wc = new WebClient())
                {
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    string jsonResult = wc.UploadString(authServer + "/token", parameters);

                    var tokenResult = JsonConvert.DeserializeObject<TokenResponse>(jsonResult);
                    ViewBag.token_type = tokenResult.token_type;
                    ViewBag.access_token = tokenResult.access_token;
                    ViewBag.username = tokenResult.username;

                    ViewBag.expires_in = tokenResult.expires_in;
                    ViewBag.scope = tokenResult.scope;
                    ViewBag.refresh_token = tokenResult.refresh_token;
                    ViewBag.tokenError = tokenResult.error;
                    ViewBag.tokenError_description = tokenResult.error_description;
                    ViewBag.tokenError_uri = tokenResult.error_uri;
                    ViewBag.tokenState = tokenResult.state;



                    // refresh token

                    using (WebClient wc2 = new WebClient())
                    {
                        grant_type = "refresh_token";
                        string refresh_token = tokenResult.refresh_token;

                        parameters = "grant_type=" + grant_type + "&refresh_token=" + refresh_token + "&client_id=" + client_id + "&client_secret=" + client_secret;

                        wc2.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        jsonResult = wc2.UploadString(authServer + "/token", parameters);
                        tokenResult = JsonConvert.DeserializeObject<TokenResponse>(jsonResult);


                        ViewBag.refreshed_token_type = tokenResult.token_type;
                        ViewBag.refreshed_access_token = tokenResult.access_token;
                        ViewBag.refreshed_username = tokenResult.username;

                        ViewBag.refreshed_expires_in = tokenResult.expires_in;
                        ViewBag.refreshed_scope = tokenResult.scope;
                        ViewBag.refreshed_refresh_token = tokenResult.refresh_token;
                        ViewBag.refreshed_tokenError = tokenResult.error;
                        ViewBag.refreshed_tokenError_description = tokenResult.error_description;
                        ViewBag.refreshed_tokenError_uri = tokenResult.error_uri;
                        ViewBag.refreshed_tokenState = tokenResult.state;
                    }


                    if (string.IsNullOrEmpty(tokenResult.error))
                    {
                        // create a CIAPI.CS Rpc.Client, set the OAuthToken field and make a call
                        var rpcUri = CIAuthSection.Instance().TradingAPI.TradingAPIRpc;
                        var appKey = CIAuthSection.Instance().TradingAPI.TradingAPIAppKey;
                        var client = new Client(new Uri(rpcUri), new Uri(rpcUri), appKey);
                        client.OAuthToken = tokenResult.access_token;
                        var result = client.AccountInformation.GetClientAndTradingAccount();

                        ViewBag.result_LogonUserName = result.LogonUserName;

                    }
                }
            }



            return View();

        }



    }
}
