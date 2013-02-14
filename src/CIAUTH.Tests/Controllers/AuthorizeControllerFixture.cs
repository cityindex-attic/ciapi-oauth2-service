using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CIAPI.DTO;
using CIAPI.Rpc;
using CIAUTH.Code;
using CIAUTH.Controllers;
using Moq;
using NUnit.Framework;

namespace CIAUTH.Tests.Controllers
{
    [TestFixture]
    public class AuthorizeControllerFixture
    {
        public AuthorizeControllerFixture()
        {
            Bootstrapper.Initialise();
        }

        [Test]
        public void ValidCredentialsReturnsRedirect()
        {
            var loginServiceMock = new Mock<ILoginService>();
            loginServiceMock.Setup(m => m.Login(It.IsAny<string>(), It.IsAny<string>())).Returns(
                new ApiLogOnResponseDTO()
                    {
                        AllowedAccountOperator = true,
                        PasswordChangeRequired = false,
                        Session = "session"

                    });

            var controller = new AuthorizeController(loginServiceMock.Object);

            HttpContextBase context = Mocking.FakeHttpContext();
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            string client_id = "123";
            string response_type = "code";
            string redirect_uri = "http://foo.bar.com";
            string state = "state";
            string username = "foo";
            string password = "bar";
            string login = "Login";
            string cancel = "";

            var result =

                controller.Index(username, password, login, cancel, client_id, response_type, redirect_uri, state);
            Assert.IsInstanceOf<RedirectResult>(result);

            var redirectResult = (RedirectResult)result;
            Assert.IsFalse(redirectResult.Permanent);
            Assert.AreEqual("http://foo.bar.com?code=JpstAC9GbwGop5FiEqfs3Q%3d%3d&state=state", redirectResult.Url);
        }

        [Test]
        public void InvalidCredentialsShowError()
        {
            var loginServiceMock = new Mock<ILoginService>();
            loginServiceMock.Setup(m => m.Login(It.IsAny<string>(), It.IsAny<string>())).Throws(
                new InvalidCredentialsException("Invalid"));

            var controller = new AuthorizeController(loginServiceMock.Object);

            HttpContextBase context = Mocking.FakeHttpContext();
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            string client_id = "123";
            string response_type = "code";
            string redirect_uri = "http://foo.bar.com";
            string state = "";
            string username = "foo";
            string password = "bar";
            string login = "Login";
            string cancel = "";

            var result =
                (ViewResult)
                controller.Index(username, password, login, cancel, client_id, response_type, redirect_uri, state);
            Assert.AreEqual("Demo App", result.ViewBag.SiteName);


            Assert.AreEqual("Invalid Username or Password", result.ViewBag.ErrorMessage);
        }

        [Test]
        public void EmptyRedirectUriThrows()
        {
            HttpContextBase context = Mocking.FakeHttpContext();

            var loginServiceMock = new Mock<ILoginService>();

            var controller = new AuthorizeController(loginServiceMock.Object);
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            string client_id = "123";
            string response_type = "code";
            string redirect_uri = "";
            string state = "";
            Assert.Throws(typeof(Exception), () => controller.Index(client_id, response_type, redirect_uri, state),
                          "invalid response_type");
        }

        [Test]
        public void FormSubmitShowsRequiredFieldErrors()
        {
            HttpContextBase context = Mocking.FakeHttpContext();
            var loginServiceMock = new Mock<ILoginService>();

            var controller = new AuthorizeController(loginServiceMock.Object);
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            string client_id = "123";
            string response_type = "code";
            string redirect_uri = "http://foo.bar.com";
            string state = "";
            string username = "";
            string password = "";
            string login = "Login";
            string cancel = "";

            var result =
                (ViewResult)
                controller.Index(username, password, login, cancel, client_id, response_type, redirect_uri, state);
            Assert.AreEqual("Demo App", result.ViewBag.SiteName);

            Assert.AreEqual("Required", result.ViewBag.UidLabel);
            Assert.AreEqual("Required", result.ViewBag.PwdLabel);

            Assert.AreEqual("Required fields missing", result.ViewBag.ErrorMessage);
            //ViewBag.ErrorMessage = "Invalid Username or Password";
        }

        [Test]
        public void InvalidClientIdThrows()
        {
            HttpContextBase context = Mocking.FakeHttpContext();

            var loginServiceMock = new Mock<ILoginService>();

            var controller = new AuthorizeController(loginServiceMock.Object);
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            string client_id = "";
            string response_type = "";
            string redirect_uri = "";
            string state = "";
            Assert.Throws(typeof(Exception), () => controller.Index(client_id, response_type, redirect_uri, state),
                          "unregistered client");
        }



        [Test]
        public void InvalidResponseTypeThrows()
        {
            HttpContextBase context = Mocking.FakeHttpContext();

            var loginServiceMock = new Mock<ILoginService>();

            var controller = new AuthorizeController(loginServiceMock.Object);
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            string client_id = "123";
            string response_type = "";
            string redirect_uri = "";
            string state = "";
            Assert.Throws(typeof(Exception), () => controller.Index(client_id, response_type, redirect_uri, state),
                          "invalid response_type");
        }

        [Test]
        public void ValidClientIdShowsClientName()
        {
            HttpContextBase context = Mocking.FakeHttpContext();

            var loginServiceMock = new Mock<ILoginService>();

            var controller = new AuthorizeController(loginServiceMock.Object);
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            string client_id = "123";
            string response_type = "code";
            string redirect_uri = "http://foo.bar.com";
            string state = "";
            var result = (ViewResult)controller.Index(client_id, response_type, redirect_uri, state);
            Assert.AreEqual("Demo App", result.ViewBag.SiteName);
        }
    }
}