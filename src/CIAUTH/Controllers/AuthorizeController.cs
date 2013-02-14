using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using CIAPI.Rpc;
using CIAUTH.Code;
using CIAUTH.Configuration;

namespace CIAUTH.Controllers
{
    public class AuthorizeController : Controller
    {


// ReSharper disable InconsistentNaming
        public ActionResult Index(string client_id, string response_type, string redirect_uri, string state)
// ReSharper restore InconsistentNaming
        {
            var client = CIAUTHConfigurationSection.Instance.Clients[client_id];

            // #TODO: validate client and show error page if bad
            if (client == null)
            {
                throw new Exception("unregistered client");
            }

            ViewBag.SiteName = client.Name;


            return View();
        }


        [HttpPost]
// ReSharper disable InconsistentNaming
        public ActionResult Index(string username, string password, string login, string cancel, string client_id, string response_type, string redirect_uri, string state)
// ReSharper restore InconsistentNaming
        {
            if (!string.IsNullOrEmpty(cancel))
            {
                Response.Redirect(Utilities.ComposeUrl(redirect_uri, "cancel=true&state=" + state));
            }


            var client = CIAUTHConfigurationSection.Instance.Clients[client_id];

            // #TODO: validate client and show error page if bad
            if (client == null)
            {
                throw new Exception("unregistered client");
            }

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
                    var rpcClient = Utilities.BuildClient();
                    var result = rpcClient.LogIn(username, password);

                    if (result.PasswordChangeRequired)
                    {
                        ViewBag.ErrorMessage = "Password change required";
                        // #TODO: implement dedicated change password view and then redirect back to authorize
                    }
                    else
                    {

                        var payload = Utilities.BuildPayload(username, password, result.Session);
                        redirectResult = new RedirectResult(Utilities.ComposeUrl(redirect_uri, "code=" + payload + "&state=" + state), false);

                    }
                }
                catch (InvalidCredentialsException ice)
                {
                    ViewBag.ErrorMessage = "Invalid Username or Password";
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                }
            }

            return redirectResult;
        }

    }
}
