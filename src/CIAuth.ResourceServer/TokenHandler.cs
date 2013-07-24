using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Web;
using CIAuth.Core;
using CIAuth.Core.Extensions;


namespace CIAuth.ResourceServer
{
    public class TokenHandler
    {
        public void ProcessRequest(HttpRequestBase request)
        {
            string authHeader = request.Headers["Authorization"];

            if (string.IsNullOrEmpty(authHeader))
            {
                // this request is not meant for us.
                return;
            }


            // ok, caller claims to be authenticated by the auth server.
            // if it all checks out, set the username and session header and let 
            // the request fall through to the trading api.
            // if not, throw a security exception

            try
            {
                authHeader = authHeader.Trim();

                if (!authHeader.ToLower().StartsWith("bearer"))
                {
                    throw new ArgumentException("Invalid Authorization header.");
                }

                string authToken = authHeader.Substring(authHeader.IndexOf(' ')).Trim();

                if (string.IsNullOrEmpty(authToken))
                {
                    throw new ArgumentException("Authorization header");
                }

                var tokenItem = TokenCache.GetGrant(authToken);


                if (tokenItem == null)
                {
                    JsonWebEncryptedToken jwt = AuthenticationServerProxy.ValidateToken(authToken, request);

                    tokenItem = new TokenItem
                        {
                            Token = authToken,
                            Expires = jwt.ExpiresOn,
                            Username = jwt.Username,
                            Session = jwt.Session
                        };

                    TokenCache.Insert(tokenItem);
                }

                if (tokenItem.Expires < DateTimeOffset.UtcNow)
                {
                    TokenCache.Remove(authToken);

                    // #TODO: refresh token if access_type is offline
                    throw new SecurityException("token is expired");
                }

                request.Headers["username"] = tokenItem.Username;
                request.Headers["session"] = tokenItem.Session;

                // and off the request goes to the TradingAPI

            }
            catch (SecurityException se)
            {
                // log it
                throw;
            }
            catch (Exception ex)
            {
                //log it
                throw new SecurityException("token exception");
            }
        }
    }
}
