using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIAuth.Core.Encryption;
using NUnit.Framework;

namespace CIAuth.Core.Tests
{
    [TestFixture]
    public class KeyIssuerFixture
    {
        [Test]
        public void CanGenerateAsymmetricKey()
        {
            EncryptionKey key = KeyIssuer.GenerateAsymmetricKey();

        }
    }
}
