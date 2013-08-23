using System.Web.Http;
using AttributeRouting;
using AttributeRouting.Web.Http;
using CIAPI.DTO;
using CIAuth.Common;

namespace TradingAPI.Controllers
{
    [Authorize]
    [TradingApiExceptionFilter]
    public class UserAccountController : ApiController
    {
        public partial class AccountInformationResponseDTOProxy
        {
            /// <summary>
            /// Logon user name.
            /// </summary>
            public string LogonUserName { get; set; }
            /// <summary>
            /// Client account ID.
            /// </summary>
            public int ClientAccountId { get; set; }
            /// <summary>
            /// Base currency of the client account.
            /// </summary>
            public string ClientAccountCurrency { get; set; }
            /// <summary>
            /// Account Operator ID.
            /// </summary>
            public int AccountOperatorId { get; set; }
            /// <summary>
            /// A list of trading accounts.
            /// </summary>
            public ApiTradingAccountDTO[] TradingAccounts { get; set; }
            /// <summary>
            /// The user's personal email address.
            /// </summary>
            public string PersonalEmailAddress { get; set; }
            /// <summary>
            /// Flag indicating whether the user has more than one email address configured.
            /// </summary>
            public bool HasMultipleEmailAddresses { get; set; }
            /// <summary>
            /// Information about account holders.
            /// </summary>
            public ApiAccountHolderDTO[] AccountHolders { get; set; }
        }

        [HttpGet]
        public AccountInformationResponseDTOProxy ClientAndTradingAccount()
        {
            var queryString = AuthenticationHandler.GetQueryString(this.Request);
            var username = AuthenticationHandler.GetHeaderOrQueryStringValue("username", this.Request, queryString);
            var session = AuthenticationHandler.GetHeaderOrQueryStringValue("session", this.Request, queryString);
            using (var client = SessionManager.CreateClient())
            {
                client.Session = session;
                client.UserName = username;
                AccountInformationResponseDTO response = client.AccountInformation.GetClientAndTradingAccount();
                var responseProxied = new AccountInformationResponseDTOProxy
                    {
                        AccountHolders = response.AccountHolders, AccountOperatorId = response.AccountOperatorId, ClientAccountCurrency = response.ClientAccountCurrency, HasMultipleEmailAddresses = response.HasMultipleEmailAddresses, ClientAccountId = response.ClientAccountId, LogonUserName = response.LogonUserName, PersonalEmailAddress = response.PersonalEmailAddress, TradingAccounts = response.TradingAccounts
                    };
                return responseProxied;

            }

        }
    }
}