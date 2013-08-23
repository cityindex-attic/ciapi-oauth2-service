using System;
using System.Net;
using System.Security;
using System.Web;
using Newtonsoft.Json;

namespace CIAuth.ResourceServer
{
    /// <summary>
    /// #TODO: establish solid expiration plan so that refreshing is handled without gaps
    /// 
    /// </summary>
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



                    var tokenResult = ValidateToken(authToken, request);


                    // check for error
                    if (tokenResult.error == "invalid_token")
                    {
                        throw new SecurityException("token is invalid");
                    }

                    // check for expired token

                    if (tokenResult.error == "expired_token")
                    {
                        throw new SecurityException("token is expired");
                    }

                    tokenItem = new TokenItem
                        {
                            Token = tokenResult.access_token,
                            Expires = DateTime.UtcNow.AddSeconds(tokenResult.expires_in),
                            Username = tokenResult.username,
                            Session = tokenResult.session
                        };

                    TokenCache.Insert(tokenItem);
                }

                if (tokenItem.Expires < DateTime.UtcNow)
                {
                    TokenCache.Remove(authToken);

                    // #TODO: refresh token if access_type is offline
                    throw new SecurityException("token is expired");
                }

                request.Headers["username"] = tokenItem.Username;
                request.Headers["session"] = tokenItem.Session;

                // and off the request goes to the TradingAPI

            }
            // #TODO: check for only200 and set exception response same same as TradingAPI
            catch (SecurityException se)
            {
                //new ApiErrorResponseDTO()
                //    {
                //        ErrorCode = 4011,
                //        HttpStatus = 401,
                //        ErrorMessage = "Session is not valid"
                //    }
                // log it
                throw;
            }
            catch (Exception ex)
            {
                //log it
                throw new SecurityException("token exception");
            }
        }

        private static TokenResponse ValidateToken(string authToken, HttpRequestBase request)
        {
            var endpoint = TokenHandlingModuleConfiguration.CIAuthEndpoint;
            var client = new WebClient();
            client.Headers["Content-Type"] = "application/json";
            var jsonResult = client.UploadString(endpoint + "/token/validate",
                                                 JsonConvert.SerializeObject(
                                                 new
                                                 {
                                                     token = authToken,
                                                     userHostAddress = request.UserHostAddress,
                                                     userHostName = request.UserHostName,
                                                     userAgent=request.UserAgent
                                                 }, Formatting.Indented));
            var tokenResult = JsonConvert.DeserializeObject<TokenResponse>(jsonResult);
            return tokenResult;
        }
    }
}