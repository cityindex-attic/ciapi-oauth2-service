using System.Web.Http;
using System.Web.Mvc;
using CIAUTH.Code;
using CIAUTH.Services;
using Microsoft.Practices.Unity;
using Unity.Mvc3;

namespace CIAUTH.Tests
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
  
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<ILoginService, LoginService>();            
        

            return container;
        }
    }
}