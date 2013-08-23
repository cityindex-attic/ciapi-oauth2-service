using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CIAuth.Common;
using CIAuth.Common.Encryption;
using CIAuth.Web.Models;
using WebMatrix.WebData;

namespace CIAuth.Web.Controllers
{
    [Authorize(Roles = "Client")]
    public class ApplicationsController : Controller
    {
        private UsersContext db = new UsersContext();

        //
        // GET: /Applications/

        public ActionResult Index()
        {
            var id = WebSecurity.GetUserId(User.Identity.Name);
            var applications = db.Applications.Include(a => a.UserProfile).Where(b => b.UserId == id);
            
            return View(applications.ToList());
        }


        //
        // GET: /Applications/Create

        public ActionResult Create()
        {

            return View();
        }

        //
        // POST: /Applications/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Application application)
        {
            if (ModelState.IsValid)
            {
                application.GrantType = "AuthorizationCode";
                application.Offline = true;
                application.UserId = WebSecurity.GetUserId(User.Identity.Name);
                application.EncryptionKey = KeyIssuer.GenerateAsymmetricKey().PrivateKey;
                application.ClientSecret = Guid.NewGuid().ToString("N");
                db.Applications.Add(application);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", application.UserId);
            return View(application);
        }

        //
        // GET: /Applications/Edit/5

        public ActionResult Edit(int id = 0)
        {
            // #TODO provide link to reset secret and/or key
            // either of these actions revokes all existing tokens

            Application application = db.Applications.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }

            return View(application);
        }

        //
        // POST: /Applications/Edit/5



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Application application)
        {
            if (ModelState.IsValid)
            {
                // #TODO: use refetch to keep secret and key from being used in hidden fields

                db.Entry(application).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", application.UserId);
            return View(application);
        }

        //
        // GET: /Applications/Delete/5

        public ActionResult ResetSecret(int id = 0)
        {
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            RevokeTokens(application);
            application.ClientSecret = Guid.NewGuid().ToString("N");
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id = 0)
        {
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        //
        // POST: /Applications/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Application application = db.Applications.Find(id);
            RevokeTokens(application);
            db.Applications.Remove(application);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void RevokeTokens(Application application)
        {

            try
            {
                var tokens = application.Tokens.ToArray();
                application.Tokens.Clear();
                foreach (var token in tokens)
                {
                    try
                    {
                        SessionManager.DeleteSession(token.CIAPIUserName, token.CIAPISession);
                        db.Tokens.Remove(token);
                    }
                    catch (Exception ex)
                    {
                        // log and swallow
                    }
                }
            }
            catch (Exception ex2)
            {

                throw;
            }
            finally
            {
                db.SaveChanges();
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}