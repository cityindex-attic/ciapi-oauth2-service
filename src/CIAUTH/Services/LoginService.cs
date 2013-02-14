using System;
using CIAPI.DTO;
using CIAPI.Rpc;
using CIAUTH.Code;
using CIAUTH.Configuration;

namespace CIAUTH.Services
{
    // #TODO: move config values to bootstrapper
    public class LoginService : ILoginService
    {
        #region ILoginService Members

        public ApiChangePasswordResponseDTO ChangePassword(string username, string password, string newPassword)
        {
            ApiChangePasswordResponseDTO result;
            using (Client client = BuildClient())
            {
                result = client.Authentication.ChangePassword(new ApiChangePasswordRequestDTO
                                                                  {
                                                                      UserName = username,
                                                                      Password = password,
                                                                      NewPassword = newPassword
                                                                  });
            }

            return result;
        }

        public ApiLogOnResponseDTO Login(string username, string password)
        {
            ApiLogOnResponseDTO result;
            using (Client client = BuildClient())
            {
                result = client.LogIn(username, password);
            }

            return result;
        }

        #endregion

        private static Client BuildClient()
        {
            var client = new Client(new Uri(CIAUTHConfigurationSection.Instance.ApiUrl),
                                    new Uri("http://example.com"), CIAUTHConfigurationSection.Instance.AppKey);
            return client;
        }
    }
}