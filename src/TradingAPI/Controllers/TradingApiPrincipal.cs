using System.Security.Principal;

namespace TradingAPI.Controllers
{
    public class TradingApiPrincipal : GenericPrincipal
    {
        public readonly static TradingApiPrincipal AnonymousPrincipal = new TradingApiPrincipal("anonymous", null, null, null, null, new[] { "Everyone" });
        public TradingApiPrincipal(string name, string session, int? appKeyLogonUserId, int? clientAccountId = null, int? tradingAccountId = null, string[] roles = null) :
            base(new TradingApiIdentity(name, session, appKeyLogonUserId, clientAccountId, tradingAccountId), roles)
        {

        }

        public new TradingApiIdentity Identity
        {
            get
            {
                return (TradingApiIdentity)base.Identity;
            }
        }
    }
}