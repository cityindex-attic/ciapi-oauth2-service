using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using CIAUTH;
using CIAUTH.Code;
using CIAUTH.Controllers;
using Moq;
using NUnit.Framework;

namespace CIAUTH.Tests.Controllers
{
    [TestFixture]
    public class  TokenControllerFixture
    {
        [Test]
        public void InvalidRefreshTokenReturnsOAUTHError()
        {


            var formCollection = new FormCollection
                                     {
                                         {"client_id", "123"},
                                         {"client_secret", "456"},
                                         {"grant_type", "refresh_token"},
                                         {"refresh_token", "fooo"},
                                         {"code", ""}
                                     };
            var context = Mocking.FakeHttpContext();
            var controller = new TokenController();
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            var result = (JsonResult)controller.Index(formCollection);

            Assert.AreEqual(400, context.Response.StatusCode);

            Assert.IsInstanceOf(typeof(Error), result.Data);

            var error = (Error)result.Data;

            Assert.AreEqual(400, error.status);
            Assert.AreEqual("", error.error_uri);
            Assert.AreEqual("invalid refresh token", error.error_description);
            Assert.AreEqual("invalid_request", error.error);
        }
        [Test]
        public void InvalidCodeReturnsOAUTHError()
        {


            var formCollection = new FormCollection
                                     {
                                         {"client_id", "123"},
                                         {"client_secret", "456"},
                                         {"grant_type", "authorization_code"},
                                         {"refresh_token", ""},
                                         {"code", "fasfafd"}
                                     };
            var context = Mocking.FakeHttpContext();
            var controller = new TokenController();
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            var result = (JsonResult)controller.Index(formCollection);

            Assert.AreEqual(400, context.Response.StatusCode);

            Assert.IsInstanceOf(typeof(Error), result.Data);

            var error = (Error)result.Data;

            Assert.AreEqual(400, error.status);
            Assert.AreEqual("", error.error_uri);
            Assert.AreEqual("Malformed access code", error.error_description);
            Assert.AreEqual("invalid_request", error.error);
        }

        [Test]
        public void InvalidGrantTypeReturnsOAUTHError()
        {


            var formCollection = new FormCollection
                                     {
                                         {"client_id", "123"},
                                         {"client_secret", "456"},
                                         {"grant_type", "foo"},
                                         {"refresh_token", ""},
                                         {"code", ""}
                                     };
            var context = Mocking.FakeHttpContext();
            var controller = new TokenController();
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            var result = (JsonResult)controller.Index(formCollection);

            Assert.AreEqual(400, context.Response.StatusCode);

            Assert.IsInstanceOf(typeof(Error), result.Data);

            var error = (Error)result.Data;

            Assert.AreEqual(400, error.status);
            Assert.AreEqual("", error.error_uri);
            Assert.AreEqual("", error.error_description);
            Assert.AreEqual("unsupported_grant_type", error.error);
        }

        [Test]
        public void InvalidSecretReturnsOAUTHError()
        {


            var formCollection = new FormCollection
                                     {
                                         {"client_id", "123"},
                                         {"client_secret", ""},
                                         {"grant_type", ""},
                                         {"refresh_token", ""},
                                         {"code", ""}
                                     };
            var context = Mocking.FakeHttpContext();
            var controller = new TokenController();
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            var result = (JsonResult)controller.Index(formCollection);

            Assert.AreEqual(400, context.Response.StatusCode);

            Assert.IsInstanceOf(typeof(Error), result.Data);

            var error = (Error)result.Data;

            Assert.AreEqual(400, error.status);
            Assert.AreEqual("", error.error_uri);
            Assert.AreEqual("invalid client secret", error.error_description);
            Assert.AreEqual("invalid_client", error.error);
        }


        [Test]
        public void InvalidClientIdReturnsOAUTHError()
        {

             
            var formCollection = new FormCollection
                                     {
                                         {"client_id", ""},
                                         {"client_secret", ""},
                                         {"grant_type", ""},
                                         {"refresh_token", ""},
                                         {"code", ""}
                                     };
            var context = Mocking.FakeHttpContext();
            var controller = new TokenController()  ;
            controller.ControllerContext = new ControllerContext(context,new RouteData(),  controller);

            var result = (JsonResult) controller.Index(formCollection);

            Assert.AreEqual(400,context.Response.StatusCode);
            
            Assert.IsInstanceOf(typeof(Error), result.Data);
            
            var error = (Error) result.Data;
            
            Assert.AreEqual(400,error.status);
            Assert.AreEqual("", error.error_uri);
            Assert.AreEqual("client is not registered", error.error_description);
            Assert.AreEqual("invalid_client", error.error);
        }






     
    }
}
