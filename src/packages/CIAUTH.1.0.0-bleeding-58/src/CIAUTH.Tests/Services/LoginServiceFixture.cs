using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using CIAUTH.Services;

using NUnit.Framework;

namespace CIAUTH.Tests.Services
{
    [TestFixture,Ignore]
    public class LoginServiceFixture
    {

        [Test]
        public void CanChangeUrlAndKey()
        {

            var login = new LoginService();
            string url = login.ApiUrl;
            Assert.IsNotNullOrEmpty(url);
            login.ApiUrl = "foo";
            url = login.ApiUrl;
            Assert.AreEqual("foo",url);

            string key = login.AppKey;
            Assert.IsNotNullOrEmpty(key);
            login.AppKey = "bar";
            key = login.AppKey;
            Assert.AreEqual("bar",key);
        }

        [Test]
        public void EnsureChangePassword()
        {

            var login = new LoginService();
            login.ChangePassword("XX658109", "password", "password");

        }

        [Test]
        public void EnsureLogin()
        {

            var login = new LoginService ();
            login.Login("XX658109", "password");
             
        }
    }
}
