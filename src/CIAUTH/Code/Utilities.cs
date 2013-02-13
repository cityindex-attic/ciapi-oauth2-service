using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CIAUTH.Code
{
    public static class Utilities
    {
        public static string ComposeUrl(string baseUrl,string query)
        {
            string url = baseUrl;
            if(baseUrl.IndexOf("?")>-1)
            {
                url = url + "&";
            }
            else
            {
                url = url + "?";
            }
            return url + query;
        }
    }
}