using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace TradingAPI.Controllers
{
    public class AuthenticationHandler : DelegatingHandler
    {


        protected override System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var queryString = GetQueryString(request);
            var username = GetHeaderOrQueryStringValue("username", request, queryString);
            var session = GetHeaderOrQueryStringValue("session", request, queryString);

            if (string.IsNullOrEmpty(session))
            {
                Thread.CurrentPrincipal = TradingApiPrincipal.AnonymousPrincipal;
            }
            else
            {
                var cache = System.Web.HttpRuntime.Cache;
                var item = cache.Get(session);
                if (item == null)
                {
                    var responseTcs = new TaskCompletionSource<HttpResponseMessage>();
                    responseTcs.SetResult(ApiErrorResponseDTO.Unauthorized.ToHttpResponseMessage());
                    return ApiErrorResponseDTO.Unauthorized.ToHttpResponseMessage().ToTask();
                }

                Thread.CurrentPrincipal = new TradingApiPrincipal(username, session, 0, 0, 0, null);

            }
            return base.SendAsync(request, cancellationToken);
        }

        //protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        //                                                       CancellationToken cancellationToken)
        //{


        //    var queryString = GetQueryString(request);
        //    var username = GetHeaderOrQueryStringValue("username", request, queryString);
        //    var session = GetHeaderOrQueryStringValue("session", request, queryString);

        //    if (string.IsNullOrEmpty(session))
        //    {
        //        AddPrincipalToSession(request, TradingApiPrincipal.AnonymousPrincipal);

        //        return base.SendAsync(request, cancellationToken);
        //    }

        //    username = string.IsNullOrEmpty(username) ? "anonymous" : username.ToUpperInvariant();

        //    var tradingAccountId = GetQueryStringValueAsInteger("tradingAccountId", queryString);
        //    var clientAccountId = GetQueryStringValueAsInteger("clientAccountId", queryString);


        //    var authenticationRequest = new ValidateSessionAuthenticationRequestDTO
        //    {
        //        UserName = username,
        //        Session = session,
        //        TradingAccountId = tradingAccountId,
        //        ClientAccountId = clientAccountId
        //    };


        //}

        //private const int CacheSuccessfulSessionValidationTimeInSeconds = 60;

        //private ValidateSessionResponse AuthenticateUser(ValidateSessionAuthenticationRequestDTO authenticationRequest, int requestId, string session)
        //{
        //    //if (CacheSuccessfulSessionValidationTimeInSeconds <= 0)
        //    //{
        //    //    return _sessionValidator.ValidateSession(authenticationRequest, requestId);
        //    //}

        //    // Note this has a potential race condition, however the pay off from not locking access to the cache object is worth it.
        //    // Most client access to the method is not parallel for the first authentication call.
        //    //      Clients first make a single logon request, then multiple parallel authenticated requests
        //    var cache = System.Web.HttpRuntime.Cache;
        //    var item = cache.Get(session);
        //    if (item != null)
        //    {
        //        return (ValidateSessionResponse)item;
        //    }

        //    ValidateSessionResponse authenticationResponse = _sessionValidator.ValidateSession(authenticationRequest, requestId);
        //    if (authenticationResponse.IsValid)
        //    {
        //        cache.Add(session, authenticationResponse, null, DateTime.UtcNow.AddSeconds(CacheSuccessfulSessionValidationTimeInSeconds), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
        //    }

        //    return authenticationResponse;
        //}
        //private void AddPrincipalToSession(HttpRequestMessage request, TradingApiPrincipal principal)
        //{
        //    request.Properties[Constants.UserPrincipalKey] = principal;
        //}



        public static int? GetQueryStringValueAsInteger(string name, NameValueCollection queryString)
        {
            var stringValue = GetQueryStringValue(name, queryString);
            if (stringValue == null) return null;
            int value;
            if (!int.TryParse(stringValue, out value))
            {
                throw new Exception();
                //throw new TradingApiException(ApiErrorResponseDTO.InvalidParameterValue, "Query string argument '" + name + "' expected to be an integer but was " + value);
            }
            return value;
        }

        public static string GetHeaderOrQueryStringValue(string name, HttpRequestMessage request, NameValueCollection queryString)
        {
            var headerValue = GetHeader(name, request);
            return string.IsNullOrEmpty(headerValue) ? GetQueryStringValue(name, queryString) : headerValue;
        }

        public static string GetQueryStringValue(string name, NameValueCollection queryString)
        {
            var queryStringValues = queryString.GetValues(name);

            if (queryStringValues == null || queryStringValues.Length == 0) return null;

            if (queryStringValues.Length > 1)
            {
                throw new Exception();
                //throw new TradingApiException(ApiErrorResponseDTO.InvalidParameterValue, "Query string argument '" + name + "' has multiple values but a single value was expected.");
            }

            return queryStringValues[0];
        }

        public static string GetHeader(string header, HttpRequestMessage request)
        {
            IEnumerable<string> headerValues;

            if (!request.Headers.TryGetValues(header, out headerValues)) return null;

            var headerValueList = headerValues.ToList();

            if (headerValueList.Count > 1)
            {
                throw new Exception();
                //throw new ApiErrorResponseDTO(ErrorCode.InvalidParameterValue, "Header '" + header + "' has multiple values but a single value was expected.");
            }

            if (!request.Headers.Contains(header)) return null;

            return headerValueList.First();
        }

        public static NameValueCollection GetQueryString(HttpRequestMessage request)
        {
            var query = request.RequestUri.Query;
            return HttpUtility.ParseQueryString(query);
        }
    }
}