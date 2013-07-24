using System;
using System.IO;
using System.Web;
using CIAuth.Core;

namespace CIAuth.ResourceServer
{
    public class AuthenticationServerProxy
    {
        public static JsonWebEncryptedToken ValidateToken(string authToken, HttpRequestBase request)
        {
            JsonWebEncryptedToken token = JsonWebEncryptedToken.Parse(authToken, TokenHandlingModuleConfiguration.EncryptionKey);
            token.AsymmetricKey = null;
            return token;
        }
    }
}