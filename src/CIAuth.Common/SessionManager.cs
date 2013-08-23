using System;
using CIAPI.DTO;
using CIAPI.Rpc;
using CIAuth.Common.Configuration;
using CIAuth.Common.Encryption;

namespace CIAuth.Common
{
    public static class SessionManager
    {
        public static Uri RpcUri
        {
            get { return new Uri(CIAuthSection.Instance().TradingAPI.TradingAPIRpc); }
        }

        public static Uri StreamingUri
        {
            get { return new Uri(CIAuthSection.Instance().TradingAPI.TradingAPIStreaming); }
        }

        public static string AppKey
        {
            get { return CIAuthSection.Instance().TradingAPI.TradingAPIAppKey; }
        }


        public static Client CreateClient()
        {
            var client = new Client(RpcUri, StreamingUri, AppKey);
            return client;
        }

        /// <summary>
        ///     use CIAPI to authenticate provided credentials, retrieve account information, compose and return
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static SessionInfo Authenticate(string username, string password)
        {
            ApiLogOnResponseDTO apiLogOnResponseDTO;
            AccountInformationResponseDTO accountInformationResponseDTO;
            using (Client client = CreateClient())
            {
                apiLogOnResponseDTO = client.LogIn(username, password);
                accountInformationResponseDTO = client.AccountInformation.GetClientAndTradingAccount();
            }


            var returnValue = new SessionInfo
                {
                    Session = apiLogOnResponseDTO,
                    Account = accountInformationResponseDTO
                };

            return returnValue;
        }


        public static void DeleteSession(string username, string session)
        {
            using (Client client = CreateClient())
            {
                client.UserName = username;
                client.Session = session;

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

        //public SessionInfo RefreshSession(SessionInfo session)
        //{
        //    var credentials = session.EncryptedCredentials.ToClearText().Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
        //    string username = credentials[0];
        //    string password = credentials[1];

        //    DeleteSession(username, session);

        //    // #TODO: handle exceptions

        //    SessionInfo newSession = Authenticate(username, password);

        //    return newSession;

        //}
    }
}