using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using CIAPI.DTO;
using CIAPI.Rpc;
using Infrastructure.Encryption;

namespace Infrastructure.TradingAPI
{
    public class SessionManager
    {

        private readonly string _rpcUri;
        private readonly string _streamingUri;
        private readonly string _appKey;

        public SessionManager(string rpcUri, string streamingUri, string appKey)
        {
            _rpcUri = rpcUri;
            _streamingUri = streamingUri;
            _appKey = appKey;
        }

        public Client CreateClient()
        {


            var client = new Client(new Uri(_rpcUri), new Uri(_streamingUri), _appKey);
            return client;

        }

        /// <summary>
        /// use CIAPI to authenticate provided credentials, retrieve account information, compose and return
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public SessionInfo Authenticate(string username, string password)
        {
            ApiLogOnResponseDTO apiLogOnResponseDTO;
            AccountInformationResponseDTO accountInformationResponseDTO;
            using (var client = CreateClient())
            {
                apiLogOnResponseDTO = client.LogIn(username, password);
                accountInformationResponseDTO = client.AccountInformation.GetClientAndTradingAccount();
            }


            var returnValue = new SessionInfo
                {
                    //encrypt the credentials for use in offline refresh scenario (temporary stop-gap)
                    EncryptedCredentials = (username + ":" + password).ToCipherText(),
                    Session = apiLogOnResponseDTO,
                    Account = accountInformationResponseDTO
                };

            return returnValue;
        }



        public void DeleteSession(SessionInfo session)
        {
            using (var client = CreateClient())
            {
                client.UserName = session.Account.LogonUserName;
                client.Session = session.Session.Session;

                try
                {
                    client.LogOut();
                }
                catch
                {

                    // #TODO: log exception and swallow. nothing to be done.
                }
            }
        }

        public SessionInfo RefreshSession(SessionInfo session)
        {
            var credentials = session.EncryptedCredentials.ToClearText().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            string username = credentials[0];
            string password = credentials[1];

            DeleteSession(session);

            // #TODO: handle exceptions

            SessionInfo newSession = Authenticate(username, password);

            return newSession;

        }
    }
}
