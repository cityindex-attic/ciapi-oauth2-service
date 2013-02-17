﻿using System;
using System.Web.Mvc;
using CIAPI.DTO;
using CIAPI.Rpc;
using CIAUTH.Code;
using CIAUTH.Configuration;
using CIAUTH.Models;

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

        public ActionResult Login(string username, string password, string new_password)
        {
            // #TODO: restrict calls to same origin, we don't want native code clients collecting credentials and calling this

            JsonResult jsonResult;
            ApiLogOnResponseDTO result;
            bool success = true;
            string reason = null;
            bool passwordChangeRequired = false;
            AccessToken token = null;

            try
            {
                result = _loginService.Login(username, password);
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
                            success = true;
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
            catch (InvalidCredentialsException ice)
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

            return Json(new {success, reason, passwordChangeRequired, token});
        }


        public ActionResult ChangePassword(string client_id, string response_type, string redirect_uri, string state)
        {
            return View();
        }

        public ActionResult AjaxLogin()
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
                        string payload = Utilities.BuildPayload(username, password, result.Session, AesKey, AesVector);
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