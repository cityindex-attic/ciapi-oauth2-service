using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CIAPI.DTO;
using CIAPI.Rpc;

namespace CIAUTH.Code
{
    public static class Utilities
    {
        public static byte[] ToByteArray(string value)
        {
            try
            {
// ReSharper disable RedundantExplicitArrayCreation
                byte[] bytes =
                    value.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(s => Convert.ToByte(s)).
                        ToArray();
// ReSharper restore RedundantExplicitArrayCreation
                return bytes;
            }
            catch (Exception ex)
            {
                throw new Exception("error initializing aes array", ex);
            }
        }

        public static JsonResult CreateErrorJson(string error, string errorDescription, string errorUri, int status)
        {
            var jsonResult = new JsonResult
                                 {
                                     Data = new Error
                                                {
                                                    status = status,
                                                    error = error,
                                                    error_description = errorDescription,
                                                    error_uri = errorUri
                                                }
                                 };
            return jsonResult;
        }

     

        public static JsonResult BuildToken(string code, byte[] aesKey, byte[] aesVector)
        {
            string session;
            string password;
            string username;

            try
            {
                string decryptPayload = DecryptPayload(code, aesKey, aesVector);
                string[] parts = decryptPayload.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);

                username = parts[0];
                session = parts[1];
                password = parts[2];
            }
            catch (Exception ex)
            {
                throw new Exception("Malformed access code", ex);
            }


            return BuildToken(username, session, password, aesKey, aesVector);
        }

        public static JsonResult BuildToken(string username, string session, string password, byte[] aesKey,
                                            byte[] aesVector)
        {
            string accessToken = username + ":" + session;
            // #TODO: expose un encoded encrypt/decrypt methods
            string encrypted = new SimplerAes(aesKey, aesVector).Encrypt(username + ":" + password);
            string refreshToken = HttpUtility.UrlDecode(encrypted);
            var tokenObj = new AccessToken
                               {
                                   access_token = accessToken,
                                   expires_in = (int) DateTime.Now.AddDays(1).ToEpoch(),
                                   refresh_token = refreshToken,
                                   token_type = "bearer"
                               };

            var jsonResult = new JsonResult {Data = tokenObj};
            return jsonResult;
        }

        public static string DecryptPayload(string payload, byte[] aesKey, byte[] aesVector)
        {
            string payloadDecrypted = new SimplerAes(aesKey, aesVector).Decrypt(payload);
            return payloadDecrypted;
        }

        public static string BuildPayload(string username, string password, string session, byte[] aesKey,
                                          byte[] aesVector)
        {
            string package = username + ":" + session + ":" + password;

            string encrypted = new SimplerAes(aesKey, aesVector).Encrypt(package);
            return encrypted;
        }


        public static string ComposeUrl(string baseUrl, string query)
        {
            string url = baseUrl;
            if (baseUrl.IndexOf("?", StringComparison.Ordinal) > -1)
            {
                url = url + "&";
            }
            else
            {
                url = url + "?";
            }
            return url + query;
        }
    }
}