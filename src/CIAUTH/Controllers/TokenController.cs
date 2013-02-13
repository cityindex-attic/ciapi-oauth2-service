using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using CIAUTH.Code;

namespace CIAUTH.Controllers
{
    // using standard MVC controller to keep the endpoint in root, e.g. authserveruri/token
    // webapi apicontroller wants it to be authserveruri/api/token
    // will figure out if it is possible to use apicontroller later
    public class TokenController : Controller
    {
        [HttpPost]
        public JsonResult Index()
        {

            string client_id = Request.Form["client_id"];
            string client_secret = Request.Form["client_secret"];
            string grant_type = Request.Form["grant_type"];


            //  grant_type=refresh_token&refresh_token=tGzv3JOkF0XG5Qx2TlKWIA&client_id=s6BhdRkqt3&client_secret=7Fjfp0ZBr1KtDRbnfVdmIw



            // client_id=12345
            // client_secret=secret
            // grant_type=authorization_code
            // code=f5205cc9-a207-46a7-9888-906a40a3582e

            JsonResult jsonResult = null;
            switch (grant_type.ToLower())
            {
                case "refresh_token":

                    var refresh_token = Request.Form["refresh_token"];


                    break;
                case "authorization_code":


                    string code = Request.Form["code"];

                    jsonResult = BuildToken(code);
                    break;
                default:
                    throw new Exception("invalid grant type");
            }

            return jsonResult;

        }

        private JsonResult BuildToken(string code)
        {

            string username;
            string session;
            string password;
            try
            {
                var decryptPayload = new Authentication().DecryptPayload(code);
                var parts = decryptPayload.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                username = parts[0];
                session = parts[1];
                password = parts[2];

            }
            catch (Exception ex)
            {

                throw;
            }


            string accessToken = username + ":" + session;
            // #TODO: expose un encoded encrypt/decrypt methods
            string refreshToken = HttpUtility.UrlDecode(new SimplerAes().Encrypt(username + ":" + password));
            var tokenObj = new AccessToken()
            {
                access_token = accessToken,
                expires_in = (int)DateTime.Now.AddDays(1).ToEpoch(),
                refresh_token = refreshToken,
                token_type = "bearer"
            };

            var jsonResult = new JsonResult() { Data = tokenObj };
            return jsonResult;
        }

    }
}
