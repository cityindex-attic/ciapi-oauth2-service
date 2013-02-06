using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CIAUTH.Code;

namespace CIAUTH.Controllers
{
    public class DecryptController : ApiController
    {
     

        // GET api/<controller>/5
        public string Get(string id)
        {
            var decryptPayload = new Encryptor().DecryptPayload(id);
            return decryptPayload;
        }

   
    }
}