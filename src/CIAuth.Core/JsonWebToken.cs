using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Infrastructure.Encryption;
using Microsoft.IdentityModel.Claims;
using Newtonsoft.Json;

namespace Infrastructure
{
    public class JsonWebToken
    {
        private const string TYPE_HEADER = "typ";
        private const string JSON_WEB_TOKEN = "JWT";
        private const string SIGNING_ALGORITHM_HEADER = "alg";
        private const string HMAC_SHA256 = "HS256";
        private const string EXPIRATION_TIME_CLAIM = "exp";
        private const string ISSUER_CLAIM = "iss";
        private const string AUDIENCE_CLAIM = "aud";

        private static readonly TimeSpan lifeTime = new TimeSpan(0, 2, 0);
        private static readonly DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);

        private Dictionary<string, string> claims = new Dictionary<string, string>();
        private byte[] keyBytes;

        public JsonWebToken()
        {
            TimeSpan ts = DateTime.UtcNow - epochStart + lifeTime;
            ExpiresOn = Convert.ToUInt64(ts.TotalSeconds);
        }

        [JsonProperty(PropertyName = TYPE_HEADER)]
        public string Type
        {
            get { return JSON_WEB_TOKEN; }
        }

        [JsonProperty(PropertyName = SIGNING_ALGORITHM_HEADER)]
        public string SignatureAlgorithm
        {
            get { return HMAC_SHA256; }
        }


        [JsonIgnore]
        public string SymmetricKey
        {
            get { return Convert.ToBase64String(keyBytes); }
            set { keyBytes = Convert.FromBase64String(value); }
        }

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
        public ulong ExpiresOn
        {
            get { return UInt64.Parse(claims[EXPIRATION_TIME_CLAIM]); }
            private set { claims.Add(EXPIRATION_TIME_CLAIM, value.ToString()); }
        }

        [JsonIgnore]
        public string Issuer
        {
            get { return claims.ContainsKey(ISSUER_CLAIM) ? claims[ISSUER_CLAIM] : String.Empty; }
            set { claims.Add(ISSUER_CLAIM, value); }
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
            string header = JsonConvert.SerializeObject(this).ToBase64String();
            string claims = JsonConvert.SerializeObject(this.claims).ToBase64String();
            string signature = String.Empty;

            using (var hmac = new HMACSHA256(keyBytes))
            {
                string data = String.Format("{0}.{1}", header, claims);
                byte[] signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                signature = signatureBytes.ToBase64String();
            }

            return String.Format("{0}.{1}.{2}", header, claims, signature);
        }

        public static JsonWebToken Parse(string token, string secretKey)
        {
            string[] parts = token.Split('.');
            if (parts.Length != 3)
                throw new SecurityException("Bad token");

            string header = Encoding.UTF8.GetString(parts[0].ToByteArray());
            string claims = Encoding.UTF8.GetString(parts[1].ToByteArray());
            byte[] incomingSignature = parts[2].ToByteArray();
            string computedSignature = String.Empty;

            var jwt = JsonConvert.DeserializeObject<JsonWebToken>(header);
            jwt.SymmetricKey = secretKey;
            jwt.claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(claims);

            using (var hmac = new HMACSHA256(Convert.FromBase64String(secretKey)))
            {
                string data = String.Format("{0}.{1}", parts[0], parts[1]);
                byte[] signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                computedSignature = signatureBytes.ToBase64String();
            }

            if (!computedSignature.Equals(incomingSignature.ToBase64String(), StringComparison.Ordinal))
                throw new SecurityException("Signature is invalid");

            TimeSpan ts = DateTime.UtcNow - epochStart;

            if (jwt.ExpiresOn < Convert.ToUInt64(ts.TotalSeconds))
                throw new SecurityException("Token has expired");

            return jwt;
        }
    }
}