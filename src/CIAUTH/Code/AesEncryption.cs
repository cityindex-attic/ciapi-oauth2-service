using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CIAUTH.Code
{
    public class AesEncryption
    {
        private readonly ICryptoTransform _decryptor;
        private readonly UTF8Encoding _encoder;
        private readonly ICryptoTransform _encryptor;

        public AesEncryption(byte[] key, byte[] vector)
        {
            var rm = new RijndaelManaged();
            _encryptor = rm.CreateEncryptor(key, vector);
            _decryptor = rm.CreateDecryptor(key, vector);
            _encoder = new UTF8Encoding();
        }

        public string Encode(string value)
        {
            return HttpUtility.UrlEncode(value);
        }
        public string EncryptAndEncode(string value)
        {
            return Encode(Encrypt(value));
        }

        public string Encrypt(string unencrypted)
        {
            return Base64Out(Convert.ToBase64String(Encrypt(_encoder.GetBytes(unencrypted))));
        }

        
        public string Decrypt(string encrypted)
        {
            string value = encrypted;
            value = Base64In(value);
            return _encoder.GetString(Decrypt(Convert.FromBase64String(value)));
        }


        private byte[] Encrypt(byte[] buffer)
        {
            return Transform(buffer, _encryptor);
        }

        private byte[] Decrypt(byte[] buffer)
        {
            return Transform(buffer, _decryptor);
        }

        private byte[] Transform(byte[] buffer, ICryptoTransform transform)
        {
            var stream = new MemoryStream();
            using (var cs = new CryptoStream(stream, transform, CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
            }
            return stream.ToArray();
        }

        private static string Base64Out(string value)
        {
            return value.Replace("/", "$").Replace("+", "#");
        }

        private static string Base64In(string value)
        {
            return value.Replace("$", "/").Replace("#", "+");
        }
    }
}