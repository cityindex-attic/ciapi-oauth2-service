using System;
using System.IO;
using System.Linq;
using System.Security;
using CIAuth.Core.Extensions;
using Infrastructure.Issuer;
using NUnit.Framework;

namespace CIAuth.Core.Tests
{
    [TestFixture]
    public class DateUtilityFixture
    {
        [Test]
        public void CanRoundTripDateTimeOffset()
        {
            var reference = DateTimeOffset.Parse("Wed, 24 Jul 2013 04:43:09 GMT");
            var epoch = reference.ToEpochTime();
            var date = epoch.ToDateTimeOffsetFromEpoch();
            Assert.AreEqual(reference.ToString(),date.ToString());

        }
    }
    [TestFixture]
    public class TokenIssuerFixture
    {
        private static string EncKey;
        static TokenIssuerFixture()
        {
            EncKey = File.ReadAllText("enckey");
        }

        [Test]
        public void CanCreateEncryptedJWT()
        {

            string clientId = "testClient";
            string username = "JQPublic";
            string session = "someguidvalue";
            DateTimeOffset expiresOn = DateTimeOffset.Parse(" Fri, 24 Jul 2015 04:43:09 GMT");

            var tokenIssuer = new TokenIssuer();

            tokenIssuer.ShareKeyOutofBand(clientId, EncKey);


            string token = tokenIssuer.GetEncryptedToken(clientId, username, session, expiresOn);

            var jwt = JsonWebEncryptedToken.Parse(token, EncKey);

            Assert.AreEqual(clientId, jwt.Audience);
            Assert.AreEqual(username, jwt.Username);


            var actualDate = jwt.ExpiresOn;
            Assert.AreEqual(expiresOn.ToString(), actualDate.ToString());


        }

        [Test, ExpectedException(typeof(SecurityException), ExpectedMessage = "Token has expired")]
        public void ParseWillFailOnExpiredToken()
        {
            var tokenIssuer = new TokenIssuer();
            tokenIssuer.ShareKeyOutofBand("testClient", EncKey);
            string token = tokenIssuer.GetEncryptedToken("testClient", "JQPublic", "someguidvalue", DateTimeOffset.UtcNow - TimeSpan.FromSeconds(1));
            var jwt = JsonWebEncryptedToken.Parse(token, EncKey);

        }

        [Test]
        public void CanParseEncryptedJWT()
        {

            var tokenString = File.ReadAllText("token");
            var jwt = JsonWebEncryptedToken.Parse(tokenString, EncKey);
        }


    }
}