using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
 using System.Web.Http;
using CIAPI.Rpc;
using CIAUTH.Configuration;


namespace CIAUTH.Controllers.TradingApi
{
     public class TradingApiProxyController : ApiController
     {

         private string _apiUrl;
         private string _appKey;

         public string ApiUrl
         {
             get { return _apiUrl ?? CIAUTHConfigurationSection.Instance.ApiUrl; }
             set { _apiUrl = value; }
         }
         public string AppKey
         {
             get { return _appKey ?? CIAUTHConfigurationSection.Instance.AppKey; }
             set { _appKey = value; }
         }
         public Client BuildClient()
         {
             var client = new Client(new Uri(ApiUrl),
                                     new Uri("http://example.com"), AppKey);
             return client;
         }

     }
     public class AuthenticationController : TradingApiProxyController
    {

        [HttpGet]
        public bool LogOut(string username, string session)
        {
            var client = BuildClient();
            client.UserName = username;
            client.Session = session;
            bool result = client.LogOut();
            return result;
        }


    }
}
