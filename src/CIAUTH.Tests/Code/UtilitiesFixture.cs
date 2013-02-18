using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CIAUTH.Code;
using CIAUTH.Configuration;
using CIAUTH.Models;
using NUnit.Framework;

namespace CIAUTH.Tests.Code
{
    [TestFixture]
    public class UtilitiesFixture
    {
        private static readonly byte[] AesKey;
        private static readonly byte[] AesVector;
        static UtilitiesFixture()
        {
            //#TODO: does accessing config in here smell? i think it does...
            AesVector = Utilities.ToByteArray(CIAUTHConfigurationSection.Instance.AesVector);
            AesKey = Utilities.ToByteArray(CIAUTHConfigurationSection.Instance.AesKey);
        }
        //foo,bar,session
        //code=JpstAC9GbwGop5FiEqfs3Q==
        [Test]
        public void DecryptPayload()
        {
            string result = Utilities.DecryptPayload("JpstAC9GbwGop5FiEqfs3Q==", AesKey, AesVector);
            Assert.AreEqual("foo:session:bar", result);


        }

        [Test]
        public void BuildPayload()
        {
            string result = Utilities.BuildPayloadAndEncode("foo", "bar", "session" , AesKey, AesVector);
            Assert.AreEqual("JpstAC9GbwGop5FiEqfs3Q%3d%3d", result);



        }

        [Test]
        public void BuildTokenFromCode()
        {
            JsonResult result = Utilities.BuildAccessTokenJsonResult("JpstAC9GbwGop5FiEqfs3Q==", AesKey, AesVector);
            Assert.IsInstanceOf<AccessToken>(result.Data);
            var data = (AccessToken)result.Data;
            Assert.AreEqual("foo:session", data.access_token);
            Assert.AreEqual("JpstAC9GbwGop5FiEqfs3Q==", data.refresh_token);
            Assert.AreEqual("bearer", data.token_type);
        }

        [Test]
        public void BuildTokenFromText()
        {
            JsonResult result = Utilities.BuildAccessTokenJsonResult("foo", "session", "bar", AesKey, AesVector);
            Assert.IsInstanceOf<AccessToken>(result.Data);
            var data = (AccessToken)result.Data;
            Assert.AreEqual("foo:session", data.access_token);
            Assert.AreEqual("JpstAC9GbwGop5FiEqfs3Q==", data.refresh_token);
            Assert.AreEqual("bearer", data.token_type);


        }

        [Test]
        public void ComposeUrl()
        {
            string result;
            result = Utilities.ComposeUrl("http://foo.bar.com", "a=b");
            Assert.AreEqual("http://foo.bar.com?a=b", result);
            result = Utilities.ComposeUrl("http://foo.bar.com?d=f&g=h", "a=b");
            Assert.AreEqual("http://foo.bar.com?d=f&g=h&a=b", result);

            
        }

        [Test]
        public void CreateErrorJson()
        {
            JsonResult result = Utilities.CreateErrorJsonResult("error","description","uri",400);

            Assert.IsInstanceOf<Error>(result.Data);
            var data = (Error) result.Data;
            Assert.AreEqual("error", data.error);
            Assert.AreEqual("description", data.error_description);
            Assert.AreEqual("uri", data.error_uri);
            Assert.AreEqual(400, data.status);
             
        }
        [Test]
        public void ToByteArrayFails()
        {
            Assert.Throws<ArgumentException>(()=>Utilities.ToByteArray("abc"));

        }
        [Test]
        public void ToByteArray()
        {
            var result = Utilities.ToByteArray("1,2, 3 ,4");
            Assert.AreEqual(4, result.Length);
            Assert.AreEqual(1, result[0]);
            Assert.AreEqual(2, result[1]);
            Assert.AreEqual(3, result[2]);
            Assert.AreEqual(4, result[3]);
 
        }

        [Test]
        public void EpochDate()
        {
            var date = new DateTime(2010, 1, 1);
            long result;
            result = date.ToEpoch();
            
            var result2 = result.FromEpoch();
            Assert.AreEqual(2010, result2.Year);
            Assert.AreEqual(1, result2.Day);
            Assert.AreEqual(1, result2.Month);
        }
    }
}
