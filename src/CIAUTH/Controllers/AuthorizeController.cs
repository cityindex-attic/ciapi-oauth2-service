﻿using System;
using System.Web.Mvc;
using System.Web.Routing;
using CIAPI.DTO;
using CIAPI.Rpc;
using CIAUTH.Code;
using CIAUTH.Configuration;

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


        public ActionResult ChangePassword(string client_id, string response_type, string redirect_uri, string state)
        {
            return View();
        }



        // ReSharper disable InconsistentNaming
        public ActionResult Index(string client_id, string response_type, string redirect_uri, string state)
        // ReSharper restore InconsistentNaming
        {
            ClientElement client = CIAUTHConfigurationSection.Instance.Clients[client_id];
            ValidateOAUTHParameters(response_type, redirect_uri, client);
            ViewBag.SiteName = client.Name;
            return View();
        }

        // ReSharper disable InconsistentNaming
        private static void ValidateOAUTHParameters(string response_type, string redirect_uri, ClientElement client)
        // ReSharper restore InconsistentNaming
        {
            if (client == null)
            {
                throw new Exception("unregistered client");
            }

            if (response_type != "code")
            {
                throw new Exception("invalid response_type");
            }

            if (string.IsNullOrEmpty(redirect_uri))
            {
                throw new Exception("empty redirect_uri");
            }
        }


        [HttpPost]
        // ReSharper disable InconsistentNaming
        public ActionResult Index(string username, string password, string login, string cancel, string client_id,
                                  string response_type, string redirect_uri, string state)
        // ReSharper restore InconsistentNaming
        {
            if (!string.IsNullOrEmpty(cancel))
            {
                return new RedirectResult(Utilities.ComposeUrl(redirect_uri, "cancel=true&state=" + state));
            }


            ClientElement client = CIAUTHConfigurationSection.Instance.Clients[client_id];

            ValidateOAUTHParameters(response_type, redirect_uri, client);


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
                                Utilities.ComposeUrl(redirect_uri, "code=" + payload + "&state=" + state), false);
                    }
                }
                catch (InvalidCredentialsException )
                {
                    ViewBag.ErrorMessage = "Invalid Username or Password";
                }
                
            }

            return redirectResult;
        }
    }
}