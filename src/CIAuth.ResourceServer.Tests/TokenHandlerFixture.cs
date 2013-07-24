using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using CIAuth.Core;
using Infrastructure.Issuer;
using NUnit.Framework;

namespace CIAuth.ResourceServer.Tests
{
    [TestFixture]
    public class TokenHandlerFixture
    {
        private static string EncKey;

        static TokenHandlerFixture()
        {
            EncKey = File.ReadAllText("enckey");
        }
        [Test]
        public void HandlerWillValidate()
        {
            string clientId = "testClient";
            string username = "JQPublic";
            string session = "someguidvalue";
            DateTimeOffset expiresOn = DateTimeOffset.Parse(" Fri, 24 Jul 2015 04:43:09 GMT");

            var tokenIssuer = new TokenIssuer();

            tokenIssuer.ShareKeyOutofBand(clientId, EncKey);


            string token = tokenIssuer.GetEncryptedToken(clientId, username, session, expiresOn);

            var jwt = JsonWebEncryptedToken.Parse(token, EncKey);

            var request = TestRequest.Create(new Uri("http://foo.com"));
            request.ServerVariables["REMOTE_HOST"] = "https:/host.com";
            request.Headers["Authorization"] = "Bearer " + token;
            var handler = new TokenHandler();
            handler.ProcessRequest(request);



        }

    }
}
