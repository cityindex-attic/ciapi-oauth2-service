using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using CIAPI.DTO;
using CIAPI.Rpc;

namespace CIAUTH.Code
{
    public class Encryptor
    {
        public void Login(ref UserInfo userInfo)
        {
            var client = new Client(new Uri(WebConfigurationManager.AppSettings["apiUrl"]), new Uri("http://example.com"), "CIAUTH");
          
            if(!string.IsNullOrEmpty(userInfo.NewPassword))
            {
                client.LogIn(userInfo.UserName, userInfo.Password);
                userInfo.SessionId = client.Session;


                var response = client.Authentication.ChangePassword(new ApiChangePasswordRequestDTO()
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
                var response = client.LogIn(userInfo.UserName, userInfo.Password);
                userInfo.SessionId = client.Session;

                userInfo.PasswordChangeRequired = response.PasswordChangeRequired;
          
            }
            
        }

        public string DecryptPayload(string payload)
        {
            var payloadDecrypted = new SimplerAes().DecryptFromUrl(payload);
            return payloadDecrypted;
        }
        public string BuildPayload(UserInfo userInfo)
        {
            string package = userInfo.UserName + ":" + userInfo.SessionId;
            var encrypted = new SimplerAes().EncryptToUrl(package);
            return encrypted;

        }
    }
}