using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIAUTH.Configuration;
using NUnit.Framework;

namespace CIAUTH.Tests.Configuration
{
    [TestFixture]
    public class ConfigurationFixture
    {
        [Test]
        public void Noid()
        {
            var section = CIAUTHConfigurationSection.Instance;

            Assert.AreEqual("123, 217, 19, 11, 24, 26, 85, 45, 114, 184, 27, 162, 37, 112, 222, 209, 241, 24, 175, 144, 173, 53, 196, 29, 24, 26, 17, 218, 131, 236, 53, 209", section.AesKey);
            Assert.AreEqual("146, 64, 191, 111, 23, 3, 113, 119, 231, 121, 221, 112, 79, 32, 114, 156", section.AesVector);
            Assert.AreEqual("https://ciapi.cityindex.com/tradingapi", section.ApiUrl);
            Assert.AreEqual("CIAUTH", section.AppKey);
           // Assert.AreEqual(2,section.Clients.Count);


            var ordinalindex = section.Clients[0];
            var index = section.Clients["123"];
            Assert.AreSame(ordinalindex,index);

            Assert.AreEqual("456", index.Secret);
            Assert.AreEqual("Demo App", index.Name);
            Assert.AreEqual("123", index.Id);
            Assert.AreEqual("*", index.Hosts);
            Assert.AreEqual("http://foo.bar.com", index.AboutURL);
        }
    }
}
