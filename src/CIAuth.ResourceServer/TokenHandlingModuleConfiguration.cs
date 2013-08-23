using System.Configuration;

namespace CIAuth.ResourceServer
{
    public static class TokenHandlingModuleConfiguration
    {
        public static string CIAuthEndpoint
        {
            get { return ConfigurationManager.AppSettings["CIAUTH_ENDPOINT"]; }
        }

    }
}