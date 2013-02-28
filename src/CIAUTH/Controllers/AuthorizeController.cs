using System;
using System.Web;
using System.Web.Mvc;
using CIAPI.DTO;
using CIAPI.Rpc;
using CIAUTH.Code;
using CIAUTH.Configuration;
using CIAUTH.Models;
using Newtonsoft.Json;

namespace CIAUTH.Controllers
{
    public class AuthorizeController : Controller
    {
        private static readonly byte[] AesKey;
        private static readonly byte[] AesVector;

        private readonly ILoginService _loginService;

        static AuthorizeController()
        {
            AesVector = Utilities.ToByteArray(CIAUTHConfigurationSection.Instance.AesVector);
            AesKey = Utilities.ToByteArray(CIAUTHConfigurationSection.Instance.AesKey);
        }

        public AuthorizeController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        public ActionResult UserAgentLogin()
        {
            return View();
        }
        // ReSharper disable InconsistentNaming
        public ActionResult Callback(string code, string client_id, bool? cancel, string state, bool? complete)
        // ReSharper restore InconsistentNaming
        {

            if (complete.GetValueOrDefault())
            {
                if (cancel.GetValueOrDefault())
                {
                    ViewBag.Message = "Login cancelled by user";
                }
                else
                {
                    ViewBag.Message = "Login complete";
                }
                return View();
            }

            string rawUrl = Request.RawUrl + "&complete=true";
            string encodedToken = null;
            if (!cancel.GetValueOrDefault())
            {

                var token = Utilities.BuildAccessToken(code, AesKey, AesVector);
                encodedToken = "#" + HttpUtility.UrlEncode(JsonConvert.SerializeObject(token));
            }
            string redirect = rawUrl + encodedToken;
            return new RedirectResult(redirect, false);
        }

        #region AJAX Methods

        public ActionResult AjaxLogin()
        {
            return View();
        }

        // ReSharper disable InconsistentNaming
        public ActionResult RefreshToken(string refresh_token)
        // ReSharper restore InconsistentNaming
        {
            bool success;
            string reason;
            bool passwordChangeRequired;
            AccessToken token = null;

            try
            {
                string decryptPayload = Utilities.DecryptPayload(refresh_token, AesKey, AesVector);
                string[] parts = decryptPayload.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                string username = parts[0];
                string password = parts[2];
                string session = parts[1];

                try
                {
                    try
                    {
                        _loginService.Logout(username, session);
                    }
                    catch
                    {

                        // swallow
                    }

                    ApiLogOnResponseDTO result = _loginService.Login(username, password);

                    if (result.PasswordChangeRequired)
                    {
                        success = false;
                        reason = "Password change required";
                        passwordChangeRequired = true;
                    }
                    else
                    {
                        token = Utilities.BuildAccessToken(username, result.Session, password, AesKey,
                                                           AesVector);


                        success = true;
                        reason = "Password changed";
                        passwordChangeRequired = false;
                    }
                }
                catch (InvalidCredentialsException)
                {
                    success = false;
                    reason = "Invalid Username or Password";
                    passwordChangeRequired = false;
                }
            }
            catch (Exception)
            {
                success = false;
                reason = "invalid refresh_token";
                passwordChangeRequired = false;
            }

            return Json(new { success, reason, passwordChangeRequired, token });
        }

        public ActionResult Logout(string username, string session)
        {
            bool success = false;
            string reason = null;


            try
            {
                success = _loginService.Logout(username, session);
            }
            catch (Exception ex)
            {
                reason = ex.Message;
            }

            return Json(new { success, reason });
        }

        // ReSharper disable InconsistentNaming
        public ActionResult Login(string username, string password, string new_password)
        // ReSharper restore InconsistentNaming
        {
            // #TODO: restrict calls to same origin, we don't want native code clients collecting credentials and calling this

            bool success = true;
            string reason;
            bool passwordChangeRequired = false;
            AccessToken token;

            try
            {
                ApiLogOnResponseDTO result = _loginService.Login(username, password);
                passwordChangeRequired = result.PasswordChangeRequired;
                reason = "Logged in";
                token = Utilities.BuildAccessToken(username, result.Session, password, AesKey, AesVector);
                if (result.PasswordChangeRequired)
                {
                    if (!string.IsNullOrEmpty(new_password))
                    {
                        ApiChangePasswordResponseDTO pwdChangeResult = _loginService.ChangePassword(username, password,
                                                                                                    new_password);
                        if (pwdChangeResult.IsPasswordChanged)
                        {
                            passwordChangeRequired = false;
                            reason = "Password changed";
                            token = Utilities.BuildAccessToken(username, result.Session, new_password, AesKey, AesVector);
                        }
                        else
                        {
                            passwordChangeRequired = true;
                            success = false;
                            reason = "Password was not changed";
                            token = null;
                            //#TODO: ask API team if they could be less informative ;-)
                        }
                    }
                    else
                    {
                        passwordChangeRequired = true;
                        success = false;
                        reason = "Password change required";
                        token = null;
                    }
                }
            }
            catch (InvalidCredentialsException)
            {
                success = false;
                reason = "Invalid credentials";
                token = null;
            }
            catch (Exception ex)
            {
                success = false;
                reason = ex.Message;
                token = null;
            }

            return Json(new { success, reason, passwordChangeRequired, token });
        }

        #endregion

        // ReSharper disable InconsistentNaming
        public ActionResult ChangePassword(string client_id, string response_type, string redirect_uri, string state)
        // ReSharper restore InconsistentNaming
        {
            return View();
        }


        // ReSharper disable InconsistentNaming
        public ActionResult Index(string client_id, string response_type, string redirect_uri, string state)
        // ReSharper restore InconsistentNaming
        {
            ClientElement client = CIAUTHConfigurationSection.Instance.Clients[client_id];
            Utilities.ValidateOAUTHParameters(response_type, redirect_uri, client);
            ViewBag.SiteName = client.Name;
            return View();
        }


        [HttpPost]
        // ReSharper disable InconsistentNaming
        public ActionResult Index(string username, string password, string login, string cancel, string client_id,
                                  string response_type, string redirect_uri, string state)
        // ReSharper restore InconsistentNaming
        {
            if (string.IsNullOrEmpty(redirect_uri))
            {
                //#TODO: validate that serverside redirect allowed for client
                redirect_uri = "Callback";
            }
            if (!string.IsNullOrEmpty(cancel))
            {
                return new RedirectResult(Utilities.ComposeUrl(redirect_uri, "cancel=true&state=" + state));
            }


            ClientElement client = CIAUTHConfigurationSection.Instance.Clients[client_id];

            Utilities.ValidateOAUTHParameters(response_type, redirect_uri, client);


            ViewBag.SiteName = client.Name;
            ViewBag.ErrorMessage = "";
            ViewBag.PwdLabel = "";
            ViewBag.UidLabel = "";

            ActionResult redirectResult = View();

            bool error = false;

            if (string.IsNullOrEmpty(username))
            {
                error = true;
                ViewBag.UidLabel = "Required";
            }

            if (string.IsNullOrEmpty(password))
            {
                error = true;
                ViewBag.PwdLabel = "Required";
            }


            if (error)
            {
                ViewBag.ErrorMessage = "Required fields missing";
            }
            else
            {
                try
                {
                    ApiLogOnResponseDTO result = _loginService.Login(username, password);

                    if (result.PasswordChangeRequired)
                    {
                        redirectResult = RedirectToAction("ChangePassword", "Authorize", new
                                                                                             {
                                                                                                 client_id,
                                                                                                 response_type,
                                                                                                 redirect_uri,
                                                                                                 state
                                                                                             });
                    }
                    else
                    {
                        string payload = Utilities.BuildPayloadAndEncode(username, password, result.Session, AesKey, AesVector);
                        redirectResult =
                            new RedirectResult(
                                Utilities.ComposeUrl(redirect_uri,
                                                     "code=" + payload + "&state=" + state + "&client_id=" + client_id),
                                false);
                    }
                }
                catch (InvalidCredentialsException)
                {
                    ViewBag.ErrorMessage = "Invalid Username or Password";
                }
            }

            return redirectResult;
        }
    }
}