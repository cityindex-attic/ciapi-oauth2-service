using System;
using System.Security.Cryptography;

namespace CIAuth.Core.Encryption
{
    public class KeyIssuer
    {
        public static string GenerateSharedSymmetricKey()
        {
            // 256-bit key
            using (var provider = new RNGCryptoServiceProvider())
            {
                var secretKeyBytes = new Byte[32];
                provider.GetBytes(secretKeyBytes);

                return Convert.ToBase64String(secretKeyBytes);
            }
        }

        public static EncryptionKey GenerateAsymmetricKey()
        {
            string publicKey;
            string privateKey;

            using (var rsa = new RSACryptoServiceProvider())
            {
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
                
            }

            return new EncryptionKey(publicKey, privateKey);
        }

    }
}