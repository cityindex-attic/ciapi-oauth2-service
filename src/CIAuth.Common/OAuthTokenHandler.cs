using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Encryption;
using Microsoft.IdentityModel.Claims;

namespace Infrastructure
{
    public class OAuthTokenHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                               CancellationToken cancellationToken)
        {
            try
            {
                HttpRequestHeaders headers = request.Headers;
                if (headers.Authorization != null)
                {
                    if (headers.Authorization.Scheme.Equals("Bearer"))
                    {
                        string accessToken = request.Headers.Authorization.Parameter;
                        JsonWebToken token = JsonWebToken.Parse(accessToken, EncryptionHelper.Key);

                        // #TODO: decode token and set headers

                        var identity = new ClaimsIdentity(token.Claims, "Bearer");
                        var principal = new ClaimsPrincipal(new[] {identity});


                        throw new NotImplementedException("pull username and session from claims");
                        //Thread.CurrentPrincipal = principal;

                        //if (HttpContext.Current != null)
                        //    HttpContext.Current.User = principal;
                    }
                }

                return base.SendAsync(request, cancellationToken).ContinueWith(task =>
                    {
                        HttpResponseMessage response = task.Result;
                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Bearer",
                                                                                               "error=\"invalid_token\""));
                        }

                        return response;
                    });
            }
            catch
            {
                // this happens if our token code fails. 
                // should only happen if the Authorization header is set with a bogus token
                return Task<HttpResponseMessage>.Factory.StartNew(() =>
                    {
                        var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Bearer",
                                                                                           "error=\"invalid_token\""));
                        return response;
                    });
            }
        }
    }
}