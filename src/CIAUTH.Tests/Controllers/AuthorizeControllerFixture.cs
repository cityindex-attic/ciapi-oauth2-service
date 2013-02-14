using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CIAUTH.Controllers;
using NUnit.Framework;

namespace CIAUTH.Tests.Controllers
{
    [TestFixture]
    public class AuthorizeControllerFixture
    {


        #region Show Form

        [Test]
        public void EmptyRedirectUriThrows()
        {

            HttpContextBase context = Mocking.FakeHttpContext();
            var controller = new AuthorizeController();
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            string client_id = "123";
            string response_type = "code";
            string redirect_uri = "";
            string state = "";
            Assert.Throws(typeof(Exception), () => controller.Index(client_id, response_type, redirect_uri, state), "invalid response_type");
        }

        [Test]
        public void InvalidResponseTypeThrows()
        {

            HttpContextBase context = Mocking.FakeHttpContext();
            var controller = new AuthorizeController();
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            string client_id = "123";
            string response_type = "";
            string redirect_uri = "";
            string state = "";
            Assert.Throws(typeof(Exception), () => controller.Index(client_id, response_type, redirect_uri, state), "invalid response_type");
        }

        [Test]
        public void InvalidClientIdThrows()
        {

            HttpContextBase context = Mocking.FakeHttpContext();
            var controller = new AuthorizeController();
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            string client_id = "";
            string response_type = "";
            string redirect_uri = "";
            string state = "";
            Assert.Throws(typeof(Exception), () => controller.Index(client_id, response_type, redirect_uri, state), "unregistered client");
        }

        [Test]
        public void ValidClientIdShowsClientName()
        {

            HttpContextBase context = Mocking.FakeHttpContext();
            var controller = new AuthorizeController();
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            string client_id = "123";
            string response_type = "code";
            string redirect_uri = "http://foo.bar.com";
            string state = "";
            ViewResult result = (ViewResult)controller.Index(client_id, response_type, redirect_uri, state);
            Assert.AreEqual("Demo App", result.ViewBag.SiteName);
        } 

        #endregion


    }
}