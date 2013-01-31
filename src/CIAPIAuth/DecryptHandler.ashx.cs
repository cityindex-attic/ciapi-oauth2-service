using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CIAPIAuthWeb;

namespace CIAPIAuth
{
    /// <summary>
    /// Summary description for DecryptHandler
    /// </summary>
    public class DecryptHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            var payloadIn = context.Request["auth"];
            var payloadDecrypted = new SimplerAes().DecryptFromUrl(payloadIn);
            context.Response.Write(payloadDecrypted);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}