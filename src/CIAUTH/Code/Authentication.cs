using System;
using System.Web.Configuration;
using CIAPI.DTO;
using CIAPI.Rpc;

namespace CIAUTH.Code
{
    public class Authentication
    {
        public void Login(ref UserInfo userInfo)
        {
            var client = new Client(new Uri(WebConfigurationManager.AppSettings["apiUrl"]),
                                    new Uri("http://example.com"), "CIAUTH");

            if (!string.IsNullOrEmpty(userInfo.NewPassword))
            {
                client.LogIn(userInfo.UserName, userInfo.Password);
                userInfo.SessionId = client.Session;


                client.Authentication.ChangePassword(new ApiChangePasswordRequestDTO
                                                         {
                                                             Password = userInfo.Password,
                                                             UserName = userInfo.UserName,
                                                             NewPassword = userInfo.NewPassword
                                                         });
                userInfo.SessionId = client.Session;
                userInfo.PasswordChangeRequired = false;
            }
            else
            {
                ApiLogOnResponseDTO response = client.LogIn(userInfo.UserName, userInfo.Password);
                userInfo.SessionId = client.Session;

                userInfo.PasswordChangeRequired = response.PasswordChangeRequired;
            }
        }

        public string DecryptPayload(string payload)
        {
            string payloadDecrypted = new SimplerAes().Decrypt(payload);
            return payloadDecrypted;
        }

        public string BuildPayload(UserInfo userInfo)
        {
            string package = userInfo.UserName + ":" + userInfo.SessionId + ":" + userInfo.Password;
            string encrypted = new SimplerAes().Encrypt(package);
            return encrypted;
        }
    }
}