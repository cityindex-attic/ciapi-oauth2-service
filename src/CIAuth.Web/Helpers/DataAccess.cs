using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CIAuth.Common;
using CIAuth.Web.Controllers;
using CIAuth.Web.Models;

namespace CIAuth.Web.Helpers
{
    public static class DataAccess
    {
        public static Application GetApplication(int id)
        {
            using (var context = new UsersContext())
            {
                Application user = context.Applications.Include("UserProfile").FirstOrDefault(u => u.ApplicationId == id);

                if (user == null)
                {
                    throw new CIAuthException("unauthorized_client", "");
                }
                return user;
            }

        }

        public static void DeleteExistingGrants(int clientId, string username, string scope)
        {
            using (var context = new UsersContext())
            {
                List<Token> existingGrants = context.Tokens.Where(g =>
                                                                  g.Application.ApplicationId == clientId &&
                                                                  g.CIAPIUserName.ToLower() == username.ToLower() &&
                                                                  g.Scope == scope).ToList();

                foreach (var existingGrant in existingGrants)
                {
                    SessionManager.DeleteSession(existingGrant.CIAPIUserName, existingGrant.CIAPISession);
                    context.Tokens.Remove(existingGrant);
                    context.SaveChanges();
                }


            }
        }

        
    }
}