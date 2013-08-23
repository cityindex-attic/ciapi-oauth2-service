using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CIAuth.Common;
using CIAuth.Web.Filters;
using CIAuth.Web.Models;
using WebMatrix.WebData;

namespace CIAuth.Web.Controllers
{
    [ConsentAuthorize(Roles = "User")]
    public class ConsentController : Controller
    {
        private UsersContext db = new UsersContext();

        //
        // GET: /Consent/

        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            try
            {
                // #TODO: trap trading api exceptions
                var response = SessionManager.Authenticate(model.UserName, model.Password);
                SessionManager.DeleteSession(model.UserName, response.Session.Session);

                // #prefix username to prevent collisions
                string userName = "CityIndex_" + model.UserName;

                int userId = WebSecurity.GetUserId(userName);

                if (userId == -1)
                {
                    WebSecurity.CreateUserAndAccount(userName, model.Password);
                    if (!Roles.GetRolesForUser(userName).Contains("User"))
                    {
                        Roles.AddUsersToRoles(new[] { userName }, new[] { "User" });
                    }


                    

                }

                WebSecurity.Login(userName, model.Password);
                return RedirectToAction("Index", "Consent");


            }
            catch (MembershipCreateUserException e)
            {
                ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
            }


            return View(model);

        }


        public ActionResult Index()
        {
            var tokens = db.Tokens.Include(t => t.Application);
            return View(tokens.ToList());
        }


        //
        // GET: /Consent/Details/5

        public ActionResult Details(int id = 0)
        {
            Token token = db.Tokens.Find(id);
            if (token == null)
            {
                return HttpNotFound();
            }
            return View(token);
        }

        

        //
        // GET: /Consent/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Token token = db.Tokens.Find(id);
            if (token == null)
            {
                return HttpNotFound();
            }
            return View(token);
        }

        //
        // POST: /Consent/Delete/5

        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Token token = db.Tokens.Find(id);
            db.Tokens.Remove(token);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}