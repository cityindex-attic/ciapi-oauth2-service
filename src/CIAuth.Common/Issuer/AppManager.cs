using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CIAuth.DataAccess.Model;
using Infrastructure.Encryption;

namespace Infrastructure.Issuer
{
    public class AppManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"> key = KeyIssuer.GenerateAsymmetricKey();</param>
        /// <param name="clientId"></param>
        /// <param name="callbackUris"></param>
        public Client CreateApp(EncryptionKey key, string clientId, string callbackUris)
        {
            
            byte[] result;
            string clientSecret = Guid.NewGuid().ToString("N");


            var client = new Client()
                {
                    Allowed = true,
                    ClientId = clientId,
                    
                    ClientSecret = Guid.NewGuid().ToString("N"),
                    EncryptionPublicKey = key.PublicKey,
                    EncryptionPrivate = key.PrivateKey,
                    CallbackUris = callbackUris
                };

            return client;
        }
    }
}
