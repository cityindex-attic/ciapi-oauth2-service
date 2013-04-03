using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
 using System.Web.Http;


namespace CIAUTH.Controllers.TradingApi
{
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
