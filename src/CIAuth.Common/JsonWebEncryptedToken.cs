using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using CIAuth.Common.Encryption;
using CIAuth.Common.Extensions;
using Microsoft.IdentityModel.Claims;
using Newtonsoft.Json;
using Security.Cryptography;

namespace CIAuth.Core
{
    public class JsonWebEncryptedToken
    {
        private const string TYPE_HEADER = "typ";
        private const string JSON_WEB_TOKEN = "JWT";
        private const string ENCRYPTION_ALGORITHM_HEADER = "alg";
        private const string ENCRYPTION_METHOD_HEADER = "enc";
        private const string RSA_OAEP = "RSA-OAEP";
        private const string AES_256_GCM = "A256GCM";
        private const string EXPIRATION_TIME_CLAIM = "exp";
        private const string ISSUER_CLAIM = "iss";
        private const string AUDIENCE_CLAIM = "aud";



        private Dictionary<string, string> claims = new Dictionary<string, string>();

        public JsonWebEncryptedToken(DateTime expiresOn)
        {
            ExpiresOn = expiresOn;
        }

        [JsonProperty(PropertyName = TYPE_HEADER)]
        public string Type
        {
            get { return JSON_WEB_TOKEN; }
        }

        [JsonProperty(PropertyName = ENCRYPTION_ALGORITHM_HEADER)]
        public string EncryptionAlgorithm
        {
            get { return RSA_OAEP; }
        }

        [JsonProperty(PropertyName = ENCRYPTION_METHOD_HEADER)]
        public string EncryptionMethod
        {
            get { return AES_256_GCM; }
        }


        [JsonIgnore]
        public string AsymmetricKey { get; set; }

        [JsonIgnore]
        public IList<Claim> Claims
        {
            get
            {
                return claims.Keys.SelectMany(key =>
                                              claims[key].Split(',')
                                                         .Select(value => new Claim(key, value))).ToList();
            }
        }

        [JsonIgnore]
        public DateTime ExpiresOn
        {
            get
            {
                long epoch = Int64.Parse(claims[EXPIRATION_TIME_CLAIM]);
                return epoch.ToDateTimeFromEpoch();
            }
            private set
            {
                long epoch = value.ToEpochTime();
                claims.Add(EXPIRATION_TIME_CLAIM, epoch.ToString());
            }
        }

        [JsonIgnore]
        public string Issuer
        {
            get { return claims.ContainsKey(ISSUER_CLAIM) ? claims[ISSUER_CLAIM] : String.Empty; }
            set { claims.Add(ISSUER_CLAIM, value); }
        }


        [JsonIgnore]
        public string Username
        {
            get { return claims.ContainsKey("username") ? claims["username"] : String.Empty; }
            set { claims.Add("username", value); }
        }

        [JsonIgnore]
        public string Session
        {
            get { return claims.ContainsKey("session") ? claims["session"] : String.Empty; }
            set { claims.Add("session", value); }
        }

        [JsonIgnore]
        public string Audience
        {
            get { return claims.ContainsKey(AUDIENCE_CLAIM) ? claims[AUDIENCE_CLAIM] : String.Empty; }
            set { claims.Add(AUDIENCE_CLAIM, value); }
        }

        public void AddClaim(string claimType, string value)
        {
            if (claims.ContainsKey(claimType))
                claims[claimType] = claims[claimType] + "," + value;
            else
                claims.Add(claimType, value);
        }

        public override string ToString()
        {
            string header = JsonConvert.SerializeObject(this);
            string claims = JsonConvert.SerializeObject(this.claims);

            // Generate a 256 bit random Content Master Key and a 96 bit initialization vector
            var masterKey = new byte[32];
            var initVector = new byte[12];
            using (var provider = new RNGCryptoServiceProvider())
            {
                provider.GetBytes(masterKey);
                provider.GetBytes(initVector);
            }

            byte[] encryptedMasterKey = null;
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(AsymmetricKey);
                encryptedMasterKey = rsa.Encrypt(masterKey, true); // OAEP Padding
            }

            var authData = new EncryptedPayload
                {
                    Header = header,
                    EncryptedMasterKey = encryptedMasterKey,
                    InitializationVector = initVector
                };

            byte[] additionalAuthenticatedData = authData.ToAdditionalAuthenticatedData();

            byte[] tag = null;
            byte[] cipherText = null;

            using (var aes = new AuthenticatedAesCng())
            {
                aes.CngMode = CngChainingMode.Gcm; // Galois/Counter Mode
                aes.Key = masterKey;
                aes.IV = initVector;
                aes.AuthenticatedData = additionalAuthenticatedData;

                using (var ms = new MemoryStream())
                {
                    using (IAuthenticatedCryptoTransform encryptor = aes.CreateAuthenticatedEncryptor())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            // Encrypt the claims set
                            byte[] claimsSet = Encoding.UTF8.GetBytes(claims);
                            cs.Write(claimsSet, 0, claimsSet.Length);
                            cs.FlushFinalBlock();
                            tag = encryptor.GetTag();
                            cipherText = ms.ToArray();
                        }
                    }
                }
            }

            var payload = new EncryptedPayload
                {
                    Header = header,
                    EncryptedMasterKey = encryptedMasterKey,
                    InitializationVector = initVector,
                    CipherText = cipherText,
                    Tag = tag
                };

            string token = payload.ToString();

            return token;
        }

        public static JsonWebEncryptedToken Parse(string token, string privateKey)
        {
            byte[] claimSet = null;
            EncryptedPayload payload = null;

            try
            {
                payload = EncryptedPayload.Parse(token);

                byte[] masterKey = null;
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.FromXmlString(privateKey);
                    masterKey = rsa.Decrypt(payload.EncryptedMasterKey, true);
                }

                byte[] additionalAuthenticatedData = payload.ToAdditionalAuthenticatedData();
                using (var aes = new AuthenticatedAesCng())
                {
                    aes.CngMode = CngChainingMode.Gcm;
                    aes.Key = masterKey;
                    aes.IV = payload.InitializationVector;
                    aes.AuthenticatedData = additionalAuthenticatedData;
                    aes.Tag = payload.Tag;

                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(),
                                                         CryptoStreamMode.Write))
                        {
                            byte[] cipherText = payload.CipherText;
                            cs.Write(cipherText, 0, cipherText.Length);
                            cs.FlushFinalBlock();

                            claimSet = ms.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SecurityException("Invalid Token", ex);
            }

            var jwt = JsonConvert.DeserializeObject<JsonWebEncryptedToken>(payload.Header);
            jwt.AsymmetricKey = privateKey;
            jwt.claims = JsonConvert.DeserializeObject
                <Dictionary<string, string>>(Encoding.UTF8.GetString(claimSet));



            if (jwt.ExpiresOn < DateTime.UtcNow)
                throw new SecurityException("Token has expired");

            return jwt;
        }
    }
}