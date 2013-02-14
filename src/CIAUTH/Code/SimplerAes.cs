using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CIAUTH.Code
{
    /// <summary>
    /// http://stackoverflow.com/a/5518092/242897 - modified
    /// </summary>
    public class SimplerAes
    {
        private readonly ICryptoTransform _decryptor;
        private readonly UTF8Encoding _encoder;
        private readonly ICryptoTransform _encryptor;

        public SimplerAes(byte[] key, byte[] vector)
        {
            var rm = new RijndaelManaged();
            _encryptor = rm.CreateEncryptor(key, vector);
            _decryptor = rm.CreateDecryptor(key, vector);
            _encoder = new UTF8Encoding();
        }


        public string Encrypt(string unencrypted)
        {
            string value = Convert.ToBase64String(Encrypt(_encoder.GetBytes(unencrypted)));

            value = value.Replace("/", "$");
            value = value.Replace("+", "#");
            value = HttpUtility.UrlEncode(value);
            return value;
        }

        public string Decrypt(string encrypted)
        {
            string value = encrypted;
            value = value.Replace("$", "/");

            value = value.Replace("#", "+");
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
    }
}