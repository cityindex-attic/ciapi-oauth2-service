using System.Configuration;
using System.IO;

namespace CIAuth.ResourceServer
{
    public static class TokenHandlingModuleConfiguration
    {
        public static string CIAuthEndpoint
        {
            get { return ConfigurationManager.AppSettings["CIAUTH_ENDPOINT"]; }
        }
        public static string EncryptionKey
        {
            get { return File.ReadAllText("enckey"); }
        }
    }
}