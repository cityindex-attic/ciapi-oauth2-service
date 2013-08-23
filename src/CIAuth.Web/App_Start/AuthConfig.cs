using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using CIAuth.Web.Models;
using WebMatrix.WebData;

namespace CIAuth.Web
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            Database.SetInitializer<UsersContext>(null);

            try
            {
                using (var context = new UsersContext())
                {
                    if (!context.Database.Exists())
                    {
                        // Create the SimpleMembership database without Entity Framework migration schema
                        ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                    }
                }

                WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
            }
        }
    }
}
