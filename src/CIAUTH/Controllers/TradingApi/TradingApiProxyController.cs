using System;
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
}