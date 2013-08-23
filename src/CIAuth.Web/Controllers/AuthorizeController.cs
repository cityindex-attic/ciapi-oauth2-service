using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CIAuth.Common;
using CIAuth.Common.Encryption;
using CIAuth.Core;
using CIAuth.Web.Helpers;
using CIAuth.Web.Models;

namespace CIAuth.Web.Controllers
{
    public class AuthorizeController : Controller
    {

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel login)
        {

            try
            {
                if (Request.Form["submit"] == "Cancel")
                {

                    throw new CIAuthException("access_denied", "client cancelled request");
                }


                AuthCodeRequest authRequest = GetAndSetAuthCodeRequest();
                string password = login.Password;
                string username = login.UserName;


                var authenticateResult = SessionManager.Authenticate(username, password);



                Application app = DataAccess.GetApplication(authRequest.client_id);

                DataAccess.DeleteExistingGrants(app.ApplicationId, username, authRequest.scope);


                // create and store grant using authenticateResult
                DateTime expiresOn = DateTime.UtcNow.AddDays(1);
                var jwet = new JsonWebEncryptedToken(expiresOn)
                {
                    Audience = app.ApplicationId.ToString(),
                    AsymmetricKey = app.EncryptionKey,
                    Issuer = "CIAuth",
                    Session = authenticateResult.Session.Session,
                    Username = username
                };


                jwet.AddClaim("scope", authRequest.scope);

                var scopes = (authRequest.scope + "").ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                string refreshToken = null;
                if (scopes.Contains("offline"))
                {
                    refreshToken = Guid.NewGuid().ToString("N");    
                }


                string jsonEncryptedToken = jwet.ToString();
                var token = new Token()
                {
                    AccessCode = Guid.NewGuid().ToString("N"),
                    AccessCodeExpires = DateTime.UtcNow.AddSeconds(10),
                    ApplicationId = app.ApplicationId,
                    CIAPILogonUserName = authenticateResult.Account.LogonUserName,
                    CIAPISession = authenticateResult.Session.Session,
                    CIAPIUserName = username,
                    EncryptedCIAPICredentials = (username + ":" + password).ToCipherText(),
                    Expires = expiresOn,
                    JsonEncryptedToken = jsonEncryptedToken,
                    LastAccessed = DateTime.UtcNow,
                    RefreshToken = refreshToken,
                    Scope = authRequest.scope
                };

                using (var context = new UsersContext())
                {
                    context.Tokens.Add(token);
                    context.SaveChanges();
                }

                var uri = authRequest.redirect_uri;
                uri = uri.Contains("?") ? uri + "&" : uri + "?";
                uri = uri + "code=" + token.AccessCode;
                uri = uri + "&state=" + authRequest.state;

                return new RedirectResult(uri, false);
            }
            catch (CIAPI.Rpc.InvalidCredentialsException)
            {

                ModelState.AddModelError("Authentication", "Authentication failed.");
                return View(login);
            }

        }




        [HttpPost]
        public ActionResult Index(string submit)
        {

            if (!VerifyLocalPost())
            {
                throw new CIAuthException("invalid_request", "");
            }


            // look for existing matching grant


            if (submit == "Allow")
            {
                return RedirectToAction("Login");
            }
            else
            {
                throw new CIAuthException("access_denied", "client cancelled");
            }

        }




        [HttpGet]
        public ActionResult Index(AuthCodeRequest authRequest)
        {


            // let us sort the scope string just to avoid any confusion
            var scopes = (authRequest.scope + "").ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            scopes.Sort();
            authRequest.scope = string.Join(" ", scopes);


            bool offline = scopes.Contains("offline");
            bool trading = scopes.Contains("trading");
            string module = null;
            if (authRequest.scope.Contains("module_"))
            {
                module = scopes.FirstOrDefault(s => s.StartsWith("module_")).Substring(7);
            }


            TempData["authRequest"] = authRequest;



            Application app = DataAccess.GetApplication(authRequest.client_id);

            ValidateAuthRequest(authRequest, app);

            string responseType = authRequest.response_type;


            switch (responseType)
            {
                case "code":

                    ViewBag.ClientName = app.UserProfile.UserName;
                    ViewBag.ApplicationName = app.ApplicationName + (string.IsNullOrEmpty(module) ? "" : " (module " + module + ")");
                    ViewBag.Trading = trading;
                    ViewBag.Offline = offline;
                    break;

                default:
                    throw new Exception("unsupported_response_type");
            }

            return View();


        }

        /// <summary>
        /// make sure that we only recognize this form value as a form post from this view ONLY. 
        /// e.g. accepting 'submit' field from external would allow bypass of the review and grant step.   
        /// 
        /// Antiforgery token?
        /// </summary>
        /// <returns></returns>
        private bool VerifyLocalPost()
        {
            return true;
        }


        private static void ValidateAuthRequest(AuthCodeRequest authRequest, Application client)
        {
            Uri redirectUri;
            try
            {
                redirectUri = new Uri(authRequest.redirect_uri);
            }
            catch
            {
                throw new Exception("invalid_request");
            }

            if (!client.Hosts.ToLower().Contains(redirectUri.Host.ToLower()))
            {
                throw new Exception("invalid_request");
            }
        }

        /// <summary>
        /// invalid_request, unauthorized_client, access_denied, unsupported_response_type
        /// invalid_scope, server_error, temporarily_unavailable
        /// 
        /// handling of all exceptions from that authorize endpoint should default to
        /// sending error back to callback as uri parameter (if callback is present)
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {

            string errorFile = Server.MapPath("~/App_Data/errorlog.txt");
            string errorText = filterContext.Exception.ToString();

            // #TODO: add AuthCodeRequest data to log
            Utils.WriteLog(errorFile, errorText);

            if (filterContext.HttpContext.IsCustomErrorEnabled)
            {
                filterContext.ExceptionHandled = true;

                var ex = filterContext.Exception;

                string message;
                if (ex.GetType() != typeof(CIAuthException))
                {

                    message = "server_error";
                }
                else
                {
                    message = ex.Message;
                }

                if (TempData["authRequest"] != null)
                {
                    AuthCodeRequest authRequest = GetAndSetAuthCodeRequest();
                    string state = authRequest.state;
                    string redirectUri = authRequest.redirect_uri;
                    redirectUri = redirectUri + (redirectUri.Contains("?") ? "&" : "?");

                    redirectUri = redirectUri + "error=" + HttpUtility.UrlEncode(message);
                    redirectUri = redirectUri + "&state=" + state;

                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.StatusCode = 200;  // #TODO: do rfc oauth2 clients expect an error status code?

                    filterContext.Result = new RedirectResult(redirectUri, false);

                }


            }
            else
            {
                base.OnException(filterContext);
            }
        }


        private AuthCodeRequest GetAndSetAuthCodeRequest()
        {
            AuthCodeRequest authRequest;
            authRequest = (AuthCodeRequest)TempData["authRequest"];
            // put it back in tempdata
            TempData["authRequest"] = authRequest;
            return authRequest;
        }



    }
}
