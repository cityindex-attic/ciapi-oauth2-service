using System.Net.Http;
using System.Security.Principal;

namespace TradingAPI.Controllers
{
    public class TradingApiIdentity : GenericIdentity
    {
        public string Session { get; private set; }
        public int? ClientAccountId { get; private set; }
        public int? TradingAccountId { get; private set; }
        public int? AppKeyLogonUserId { get; private set; }

        public TradingApiIdentity(string name, string session, int? appKeyLogonUserId = null, int? clientAccountId = null, int? tradingAccountId = null)
            : base(name)
        {
            Session = session;
            ClientAccountId = clientAccountId;
            TradingAccountId = tradingAccountId;
            AppKeyLogonUserId = appKeyLogonUserId;
        }

        public override string AuthenticationType
        {
            get
            {
                return "CityIndex";
            }
        }

        public override bool IsAuthenticated
        {
            get
            {
                return Session != null;
            }
        }

        public static TradingApiIdentity GetCurrentFromRequest(HttpRequestMessage requestMessage)
        {
            if (requestMessage == null)
                return null;
            object principalObj;
            if (!requestMessage.Properties.TryGetValue(Constants.UserPrincipalKey, out principalObj))
                return null;
            var principal = principalObj as TradingApiPrincipal;
            if (principal == null) return null;
            return principal.Identity;
        }

    }
}