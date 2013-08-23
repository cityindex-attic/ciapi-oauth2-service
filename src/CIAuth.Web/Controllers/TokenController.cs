using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CIAuth.Common;
using CIAuth.Common.Encryption;
using CIAuth.Core;
using CIAuth.Web.Helpers;
using CIAuth.Web.Models;

namespace CIAuth.Web.Controllers
{
    public class TokenController : Controller
    {


        [HttpPost]
        public ActionResult Validate(string token)
        {
            // #TODO: verify validate_token is only ever called by a resource server (TradingAPI)
            // via IP?

            using (var context = new UsersContext())
            {

                // #TODO: verify token is issued to client app
                var grant = context.Tokens.FirstOrDefault(g => g.JsonEncryptedToken == token);

                if (grant == null)
                {
                    return Json(new TokenResponse()
                    {
                        error = "invalid_token"
                    });
                }
                else
                {
                    if (grant.Expires <= DateTime.UtcNow)
                    {
                        return Json(new TokenResponse()
                        {
                            error = "expired_token"
                        });
                    }

                    return Json(new TokenResponse()
                    {
                        token_type = "bearer",
                        expires_in = (int)grant.Expires.ToUniversalTime().Subtract(DateTime.UtcNow).TotalSeconds,
                        scope = grant.Scope,
                        refresh_token = grant.RefreshToken,
                        access_token = grant.JsonEncryptedToken,
                        username = grant.CIAPIUserName,
                        session = grant.CIAPISession
                    });
                }
            }


        }


        [HttpPost]
        public ActionResult Index(TokenRequest tokenRequest)
        {

            switch (tokenRequest.grant_type)
            {
                case "revoke_token":
                    using (var context = new UsersContext())
                    {
                        var application =
                            context.Applications.FirstOrDefault(
                                c => c.ApplicationId == tokenRequest.client_id);

                        if (application == null)
                        {
                            throw new CIAuthException("invalid_request", "");
                        }


                        if (application.ClientSecret != tokenRequest.client_secret)
                        {
                            throw new CIAuthException("invalid_request", "");
                        }

                        string tokenToRevoke = tokenRequest.token;

                        var token = context.Tokens.FirstOrDefault(g => g.JsonEncryptedToken == tokenToRevoke && g.Application.UserId == tokenRequest.client_id);

                        if (token == null)
                        {
                            throw new CIAuthException("invalid_request", "");
                        }

                        // replace grant with new
                        string[] credentials = token.EncryptedCIAPICredentials.ToClearText().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        string username = credentials[0];
                        string password = credentials[1];


                        SessionManager.DeleteSession(token.CIAPIUserName, token.CIAPISession);
                        context.Tokens.Remove(token);
                        context.SaveChanges();

                        return Json(new TokenResponse()
                        {
                        });

                    }

                    break;
                case "refresh_token":
                    using (var context = new UsersContext())
                    {
                        var application =
                            context.Applications.FirstOrDefault(
                                c => c.ApplicationId == tokenRequest.client_id);

                        if (application == null)
                        {
                            throw new CIAuthException("invalid_request", "");
                        }


                        if (application.ClientSecret != tokenRequest.client_secret)
                        {
                            throw new CIAuthException("invalid_request", "");
                        }

                        string refreshToken = tokenRequest.refresh_token;

                        var token = context.Tokens.FirstOrDefault(g => g.RefreshToken == refreshToken);

                        if (token == null)
                        {
                            throw new CIAuthException("invalid_request", "");
                        }

                        // replace grant with new
                        string[] credentials = token.EncryptedCIAPICredentials.ToClearText().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        string username = credentials[0];
                        string password = credentials[1];


                        var authenticateResult = SessionManager.Authenticate(username, password);

                        DateTime expiresOn = DateTime.UtcNow.AddDays(1);
                        var jsonWebEncryptedToken = new JsonWebEncryptedToken(expiresOn)
                        {
                            Audience = application.ApplicationId.ToString(),
                            AsymmetricKey = application.EncryptionKey,
                            Issuer = "CIAuth",
                            Session = authenticateResult.Session.Session,
                            Username = username
                        };

                        jsonWebEncryptedToken.AddClaim("scope", token.Scope);


                        token.Expires = expiresOn;
                        string jsonEncryptedToken = jsonWebEncryptedToken.ToString();
                        token.JsonEncryptedToken = jsonEncryptedToken;
                        token.RefreshToken = Guid.NewGuid().ToString("N");

                        context.SaveChanges();

                        return Json(new TokenResponse()
                        {
                            token_type = "bearer",
                            expires_in = (int)token.Expires.ToUniversalTime().Subtract(DateTime.UtcNow).TotalSeconds,
                            scope = token.Scope,
                            refresh_token = token.RefreshToken,
                            access_token = token.JsonEncryptedToken,
                            username = token.CIAPIUserName
                        });

                    }
                    break;
                case "authorization_code":
                    using (var context = new UsersContext())
                    {
                        var client =
                            context.Applications.FirstOrDefault(
                                c => c.ApplicationId == tokenRequest.client_id);

                        if (client == null)
                        {
                            throw new CIAuthException("invalid_request", "");
                        }


                        if (client.ClientSecret != tokenRequest.client_secret)
                        {
                            throw new CIAuthException("invalid_request", "");
                        }


                        Token token =
                            context.Tokens.FirstOrDefault(g => g.AccessCode == tokenRequest.code && g.Application.ApplicationId == tokenRequest.client_id);


                        if (token == null)
                        {
                            throw new CIAuthException("invalid_request", "");
                        }

                        // null out the access_code - it is one use only
                        token.AccessCode = null;


                        var scopes = (token.Scope + "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        scopes.Sort();


                        if (scopes.Contains("offline"))
                        {
                            token.RefreshToken = Guid.NewGuid().ToString("N");

                        }

                        context.SaveChanges();

                        return Json(new TokenResponse()
                        {
                            token_type = "bearer",
                            expires_in = (int)token.Expires.ToUniversalTime().Subtract(DateTime.UtcNow).TotalSeconds,
                            scope = token.Scope,
                            refresh_token = token.RefreshToken,
                            access_token = token.JsonEncryptedToken,
                            username = token.CIAPIUserName
                        });
                    }



                default:
                    throw new CIAuthException("invalid_request", "invalid grant type");
            }






        }
        /// <summary>
        /// invalid_request, unauthorized_client, access_denied, unsupported_response_type
        /// invalid_scope, server_error, temporarily_unavailable
        /// 
        ///  all exceptions from token endpoint must return json representation of TokenResponse with error
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;


            string errorFile = Server.MapPath("~/App_Data/errorlog.txt");
            string errorText = filterContext.Exception.ToString();
            Utils.WriteLog(errorFile, errorText);


            var ex = filterContext.Exception;
            string message;

            if (ex.GetType() != typeof(CIAuthException))
            {

                message = "server_error";
            }
            else
            {
                message = ex.Message;
            }


            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 200;  // #TODO: do rfc oauth2 clients expect an error status code?


            filterContext.Result = Json(new TokenResponse()
            {
                error = message
            });


        }

    }
}
