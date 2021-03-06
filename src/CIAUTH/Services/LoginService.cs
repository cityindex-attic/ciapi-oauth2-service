﻿using System;
using CIAPI.DTO;
using CIAPI.Rpc;
using CIAUTH.Code;
using CIAUTH.Configuration;

namespace CIAUTH.Services
{
    // #TODO: move config values to bootstrapper
    public class LoginService : ILoginService
    {
        private string _apiUrl;
        private string _appKey;

        public string ApiUrl
        {
            get { return _apiUrl ?? CIAUTHConfigurationSection.Instance.ApiUrl; }
            set { _apiUrl = value; }
        }

        public string AppKey
        {
            get { return _appKey ?? CIAUTHConfigurationSection.Instance.AppKey; }
            set { _appKey = value; }
        }

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

        public bool Logout(string username, string session)
        {
            bool result;
            using (Client client = BuildClient())
            {
                client.Session = session;
                client.UserName = username;
                result = client.LogOut();
            }

            return result;
        }

        private Client BuildClient()
        {
            var client = new Client(new Uri(ApiUrl),
                                    new Uri("http://example.com"), AppKey);
            return client;
        }
    }
}