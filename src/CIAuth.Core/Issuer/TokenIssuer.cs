using System;
using System.Collections.Generic;
using CIAuth.Core;
using CIAuth.Core.Extensions;
using Microsoft.IdentityModel.Claims;

namespace Infrastructure.Issuer
{
    public class TokenIssuer
    {
        private Dictionary<string, string> audienceKeys = new Dictionary<string, string>();

        /// <summary>
        /// This method is called to register a key with the token issuer against an audience or a RP
        /// </summary>
        /// <param name="audience">the AppKey</param>
        /// <param name="key">the public encryption key</param>
        public void ShareKeyOutofBand(string audience, string key)
        {
            if (!audienceKeys.ContainsKey(audience))
                audienceKeys.Add(audience, key);
            else
                audienceKeys[audience] = key;
        }



        public string GetEncryptedToken(string audience, string username, string session, DateTimeOffset expiresOn)
        {

            var token = new JsonWebEncryptedToken(expiresOn)
                {
                    AsymmetricKey = audienceKeys[audience],
                    Issuer = "CIAuth",
                    Audience = audience
                };

            token.AddClaim("username", username);
            token.AddClaim("session", session);


            return token.ToString();
        }

    }
}