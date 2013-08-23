using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
namespace CIAuth.ResourceServer
{
    /// <summary>
    /// abstracts access to HttpRuntime.Cache
    /// </summary>
    public static class TokenCache
    {
        private static readonly Cache Cache;

        static TokenCache()
        {
            Cache = HttpRuntime.Cache;
        }

        public static TokenItem GetGrant(string token)
        {
            var grant = (TokenItem)Cache.Get(token);
            return grant;
        }

        public static void Insert(TokenItem grant)
        {
            DateTime timeToRemoveGrantFromCache = grant.Expires.ToUniversalTime().Subtract(TimeSpan.FromMinutes(1));
            Cache.Insert(grant.Token, grant, null, timeToRemoveGrantFromCache, Cache.NoSlidingExpiration);
            // NOTE: could use removed callback to refresh token
        }

        public static void Remove(string authToken)
        {
            Cache.Remove(authToken);
        }
    }
}
