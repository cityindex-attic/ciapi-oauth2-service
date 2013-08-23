using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CIAuth.Web.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class ConsentAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {
        private string _roles;
        private string[] _rolesSplit = new string[0];


        public string Roles
        {
            get { return (_roles ?? string.Empty); }
            set
            {
                _roles = value;
                _rolesSplit = value.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
            }
        }


        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.ActionName.ToLower() != "login" && !AuthorizeCore(filterContext.HttpContext))
            {
                // Redirect to login page
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                        {
                            // #TODO: get these values from attribute ctor
                            {"controller", "Consent"},
                            {"action", "Login"}
                        });
            }
        }

        // Methods
        protected virtual bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            IPrincipal user = httpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            if ((_rolesSplit.Length > 0))
            {
                foreach (string s in _rolesSplit)
                {
                    if (!user.IsInRole(s))
                    {
                        return false;
                    }
                }

                return true;
            }
            return true;
        }
    }
}