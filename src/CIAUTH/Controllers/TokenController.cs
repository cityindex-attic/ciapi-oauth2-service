using System;
using System.Web.Mvc;
using CIAPI.DTO;
using CIAPI.Rpc;
using CIAUTH.Code;
using CIAUTH.Configuration;
using CIAUTH.Models;

namespace CIAUTH.Controllers
{
    // using standard MVC controller to keep the endpoint in root, e.g. authserveruri/token
    // webapi apicontroller wants it to be authserveruri/api/token
    // will figure out if it is possible to use apicontroller later
    public class TokenController : Controller
    {

        private readonly ILoginService _loginService;
        public TokenController(ILoginService loginService)
        {
            _loginService = loginService;
        }

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

                            string username;
                            string password;

                            try
                            {

                                string decryptPayload = Utilities.DecryptPayload(refreshToken, AesKey, AesVector);
                                string[] parts = decryptPayload.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                                username = parts[0];
                                password = parts[1];

                                try
                                {
                                    ApiLogOnResponseDTO result = _loginService.Login(username, password);

                                    if (result.PasswordChangeRequired)
                                    {

                                        jsonResult = Utilities.CreateErrorJson("invalid_request", "password change required", "", 400);

                                    }
                                    else
                                    {
                                        jsonResult = Utilities.BuildToken(username, result.Session, password, AesKey,
                                                                          AesVector);
                                    }
                                }
                                catch (InvalidCredentialsException ice)
                                {
                                    jsonResult = Utilities.CreateErrorJson("invalid_request", "Invalid Username or Password", "", 401);

                                }
                                catch (Exception ex)
                                {
                                    jsonResult = Utilities.CreateErrorJson("invalid_request", ex.Message, "", 400);
                                }

                            }
                            catch (Exception ex)
                            {
                                jsonResult = Utilities.CreateErrorJson("invalid_request", "invalid refresh_token", "", 400);

                            }


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