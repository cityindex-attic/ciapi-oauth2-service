using System;
using System.Linq;
using System.Web.Mvc;
using CIAUTH.Configuration;
using CIAUTH.Models;

namespace CIAUTH.Code
{
    public static class Utilities
    {
        // ReSharper disable InconsistentNaming
        public static void ValidateOAUTHParameters(string response_type, string redirect_uri, ClientElement client)
        // ReSharper restore InconsistentNaming
        {
            if (client == null)
            {
                throw new Exception("unregistered client");
            }

            if (response_type != "code")
            {
                throw new Exception("invalid response_type");
            }

            if (string.IsNullOrEmpty(redirect_uri))
            {
                throw new Exception("empty redirect_uri");
            }
        }

        public static JsonResult CreateErrorJsonResult(string error, string errorDescription, string errorUri, int status)
        {
            var jsonResult = new JsonResult
                                 {
                                     Data = CreateError(error, errorDescription, errorUri, status)
                                 };
            return jsonResult;
        }

        public static Error CreateError(string error, string errorDescription, string errorUri, int status)
        {
            return new Error
                       {
                           status = status,
                           error = error,
                           error_description = errorDescription,
                           error_uri = errorUri
                       };
        }

        public static AccessToken BuildAccessToken(string code, byte[] aesKey, byte[] aesVector)
        {
            try
            {
                string decryptPayload = DecryptPayload(code, aesKey, aesVector);
                string[] parts = decryptPayload.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                string username = parts[0];
                string session = parts[1];
                string password = parts[2];

                return BuildAccessToken(username, session, password, aesKey, aesVector);
            }
            catch (Exception ex)
            {
                throw new Exception("Malformed access code", ex);
            }
        }
        public static JsonResult BuildAccessTokenJsonResult(string code, byte[] aesKey, byte[] aesVector)
        {
            try
            {
                string decryptPayload = DecryptPayload(code, aesKey, aesVector);
                string[] parts = decryptPayload.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                string username = parts[0];
                string session = parts[1];
                string password = parts[2];

                return BuildAccessTokenJsonResult(username, session, password, aesKey, aesVector);
            }
            catch (Exception ex)
            {
                throw new Exception("Malformed access code", ex);
            }
        }

        public static JsonResult BuildAccessTokenJsonResult(string username, string session, string password,
                                                            byte[] aesKey,
                                                            byte[] aesVector)
        {
            return new JsonResult { Data = BuildAccessToken(username, session, password, aesKey, aesVector) };
        }

        public static AccessToken BuildAccessToken(string username, string session, string password, byte[] aesKey,
                                                   byte[] aesVector)
        {
            string accessToken = username + ":" + session;

            string refreshToken = BuildPayload(username, password, session, aesKey, aesVector);

            return new AccessToken
                               {
                                   access_token = accessToken,
                                   expires_in = (int)DateTime.Now.AddDays(1).ToEpoch(),
                                   refresh_token = refreshToken,
                                   token_type = "bearer"
                               };
        }

        public static string DecryptPayload(string payload, byte[] aesKey, byte[] aesVector)
        {
            return new AesEncryption(aesKey, aesVector).Decrypt(payload);
        }

        public static string BuildPayload(string username, string password, string session, byte[] aesKey, byte[] aesVector)
        {
            return new AesEncryption(aesKey, aesVector).Encrypt(username + ":" + session + ":" + password);
        }
        public static string BuildPayloadAndEncode(string username, string password, string session, byte[] aesKey, byte[] aesVector)
        {
            return new AesEncryption(aesKey, aesVector).Encode(BuildPayload(username, password, session, aesKey, aesVector));
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

        public static byte[] ToByteArray(string value)
        {
            try
            {
                // ReSharper disable RedundantExplicitArrayCreation
                byte[] bytes =
                    value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => Convert.ToByte(s)).ToArray();
                // ReSharper restore RedundantExplicitArrayCreation
                return bytes;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("error initializing aes array", ex);
            }
        }

        public static long ToEpoch(this DateTime time)
        {
            return (long)(time.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime FromEpoch(this long epoch)
        {
            var d = new DateTime(1970, 1, 1);
            d = d.AddSeconds(epoch);
            return d.ToLocalTime();
        }

    }
}