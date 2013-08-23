using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using AttributeRouting;
using AttributeRouting.Web.Http;
using CIAPI.DTO;
using CIAuth.Common;

namespace TradingAPI.Controllers
{
    /// <summary>
    /// simple proxy
    /// #TODO: need to echo errors from API
    /// </summary>
    [TradingApiExceptionFilter]
    public class SessionController : ApiController
    {
        [POST("")]
        public ApiLogOnResponseDTO CreateSession(ApiLogOnRequestDTO apiLogOnRequest)
        {
            ApiLogOnResponseDTO response;
            using (var client = SessionManager.CreateClient())
            {
                response = client.LogIn(apiLogOnRequest.UserName, apiLogOnRequest.Password);
            }


            HttpRuntime.Cache.Add(response.Session, response, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

            return response;



        }

        [POST("DeleteSession")]
        public ApiLogOffResponseDTO DeleteSession(ApiLogOffRequestDTO logoutRequest)
        {

            HttpRuntime.Cache.Remove(logoutRequest.Session);
            using (var client = SessionManager.CreateClient())
            {
                client.Session = logoutRequest.Session;
                client.UserName = logoutRequest.UserName;
                bool response = client.LogOut();
                return new ApiLogOffResponseDTO()
                {
                    LoggedOut = response
                };
            }
        }

    }
}
