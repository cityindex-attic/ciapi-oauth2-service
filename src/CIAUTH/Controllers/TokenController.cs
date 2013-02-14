using System;
using System.Web.Mvc;
using CIAUTH.Code;
using CIAUTH.Configuration;

namespace CIAUTH.Controllers
{
    // using standard MVC controller to keep the endpoint in root, e.g. authserveruri/token
    // webapi apicontroller wants it to be authserveruri/api/token
    // will figure out if it is possible to use apicontroller later
    public class TokenController : Controller
    {
        private static readonly byte[] AesKey;
        private static readonly byte[] AesVector;


        static TokenController()
        {
            AesVector = Utilities.ToByteArray(CIAUTHConfigurationSection.Instance.AesVector);
            AesKey = Utilities.ToByteArray(CIAUTHConfigurationSection.Instance.AesKey);
        }

        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
        {
            JsonResult jsonResult;

            string clientId = formCollection["client_id"];
            string clientSecret = formCollection["client_secret"];

            ClientElement client = CIAUTHConfigurationSection.Instance.Clients[clientId];

            if (client == null)
            {
                jsonResult = Utilities.CreateErrorJson("invalid_client", "client is not registered", "", 400);
            }

            else if (clientSecret != client.Secret)
            {
                jsonResult = Utilities.CreateErrorJson("invalid_client", "invalid client secret", "", 400);
            }
            else
            {
                string grantType = formCollection["grant_type"];

                try
                {
                    switch (grantType.ToLower())
                    {
                        case "refresh_token":
                            string refreshToken = formCollection["refresh_token"];
                            jsonResult = Utilities.RefreshToken(refreshToken, AesKey, AesVector);
                            break;

                        case "authorization_code":
                            string code = formCollection["code"];
                            jsonResult = Utilities.BuildToken(code, AesKey, AesVector);
                            break;

                        default:
                            jsonResult = Utilities.CreateErrorJson("unsupported_grant_type", "", "", 400);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    jsonResult = Utilities.CreateErrorJson("invalid_request", ex.Message, "", 400);
                }
            }

            var error = jsonResult.Data as Error;

            if (error != null)
            {
                Response.StatusCode = error.status;
            }

            return jsonResult;
        }
    }
}