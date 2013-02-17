using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CIAPI.DTO;
using CIAPI.Rpc;
using CIAUTH.Code;
using CIAUTH.Controllers;
using CIAUTH.Models;
using Moq;
using NUnit.Framework;

namespace CIAUTH.Tests.Controllers
{
    [TestFixture]
    public class TokenControllerFixture
    {
        public const string _client_id = "123";
        public const string _client_secret = "456";
        public const string _refresh_token = "JpstAC9GbwGop5FiEqfs3Q==";

        public const string _code = "JpstAC9GbwGop5FiEqfs3Q==";
        public const string _username = "foo";
        public const string _password = "bar";

        public const string _session = "session";

        public TokenControllerFixture()
        {
            Bootstrapper.Initialise();
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
            HttpContextBase context = Mocking.FakeHttpContext();
            var loginServiceMock = new Mock<ILoginService>();
            var controller = new TokenController(loginServiceMock.Object);
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            var result = (JsonResult)controller.Index(formCollection);

            Assert.AreEqual(400, context.Response.StatusCode);

            Assert.IsInstanceOf(typeof(Error), result.Data);

            var error = (Error)result.Data;

            Assert.AreEqual(400, error.status);
            Assert.AreEqual("", error.error_uri);
            Assert.AreEqual("client is not registered", error.error_description);
            Assert.AreEqual("invalid_client", error.error);
        }

        [Test]
        public void InvalidCodeReturnsOAUTHError()
        {
            var formCollection = new FormCollection
                                     {
                                         {"client_id", _client_id},
                                         {"client_secret", _client_secret},
                                         {"grant_type", "authorization_code"},
                                         {"refresh_token", ""},
                                         {"code", "fasfafd"}
                                     };
            HttpContextBase context = Mocking.FakeHttpContext();
            var loginServiceMock = new Mock<ILoginService>();
            var controller = new TokenController(loginServiceMock.Object);
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
                                         {"client_id", _client_id},
                                         {"client_secret", _client_secret},
                                         {"grant_type", "foo"},
                                         {"refresh_token", ""},
                                         {"code", ""}
                                     };
            HttpContextBase context = Mocking.FakeHttpContext();
            var loginServiceMock = new Mock<ILoginService>();
            var controller = new TokenController(loginServiceMock.Object);
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
        public void InvalidRefreshTokenReturnsOAUTHError()
        {
            var formCollection = new FormCollection
                                     {
                                         {"client_id", _client_id},
                                         {"client_secret", _client_secret},
                                         {"grant_type", "refresh_token"},
                                         {"refresh_token", "fooo"},
                                         {"code", ""}
                                     };
            HttpContextBase context = Mocking.FakeHttpContext();
            var loginServiceMock = new Mock<ILoginService>();
            var controller = new TokenController(loginServiceMock.Object);
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            var result = (JsonResult)controller.Index(formCollection);

            Assert.AreEqual(400, context.Response.StatusCode);

            Assert.IsInstanceOf(typeof(Error), result.Data);

            var error = (Error)result.Data;

            Assert.AreEqual(400, error.status);
            Assert.AreEqual("", error.error_uri);
            Assert.AreEqual("invalid refresh_token", error.error_description);
            Assert.AreEqual("invalid_request", error.error);
        }

        [Test]
        public void InvalidSecretReturnsOAUTHError()
        {
            var formCollection = new FormCollection
                                     {
                                         {"client_id", _client_id},
                                         {"client_secret", ""},
                                         {"grant_type", ""},
                                         {"refresh_token", ""},
                                         {"code", ""}
                                     };
            HttpContextBase context = Mocking.FakeHttpContext();
            var loginServiceMock = new Mock<ILoginService>();
            var controller = new TokenController(loginServiceMock.Object);
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
        public void ValidCodeReturnsToken()
        {
            var formCollection = new FormCollection
                                     {
                                         {"client_id", _client_id},
                                         {"client_secret", _client_secret},
                                         {"grant_type", "authorization_code"},
                                         {"code", "JpstAC9GbwGop5FiEqfs3Q=="}
                                     };
            HttpContextBase context = Mocking.FakeHttpContext();
            var loginServiceMock = new Mock<ILoginService>();
            var controller = new TokenController(loginServiceMock.Object);
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            var result = (JsonResult)controller.Index(formCollection);


            Assert.IsInstanceOf(typeof(AccessToken), result.Data);

            var token = (AccessToken)result.Data;

            Assert.AreEqual("foo:session", token.access_token);
            Assert.AreEqual(_refresh_token, token.refresh_token);
            Assert.AreEqual("bearer", token.token_type);
        }

        [Test]
        public void ValidRefreshCodeButBadCredentialsReturnsError()
        {
            var formCollection = new FormCollection
                                     {
                                         {"client_id", _client_id},
                                         {"client_secret", _client_secret},
                                         {"grant_type", "refresh_token"},
                                         {"refresh_token", _refresh_token}
                                     };
            HttpContextBase context = Mocking.FakeHttpContext();
            var loginServiceMock = new Mock<ILoginService>();
            loginServiceMock.Setup(m => m.Login(It.IsAny<string>(), It.IsAny<string>())).Throws(
                new InvalidCredentialsException("Invalid"));
            var controller = new TokenController(loginServiceMock.Object);
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            var result = (JsonResult)controller.Index(formCollection);


            Assert.AreEqual(401, context.Response.StatusCode);

            Assert.IsInstanceOf(typeof(Error), result.Data);

            var error = (Error)result.Data;

            Assert.AreEqual(401, error.status);
            Assert.AreEqual("", error.error_uri);
            Assert.AreEqual("Invalid Username or Password", error.error_description);
            Assert.AreEqual("invalid_request", error.error);
        }

        [Test]
        public void ValidRefreshCodeButPasswordChangeRequiredReturnsError()
        {
            var formCollection = new FormCollection
                                     {
                                         {"client_id", _client_id},
                                         {"client_secret", _client_secret},
                                         {"grant_type", "refresh_token"},
                                         {"refresh_token", _refresh_token}
                                     };
            HttpContextBase context = Mocking.FakeHttpContext();
            var loginServiceMock = new Mock<ILoginService>();
            loginServiceMock.Setup(m => m.Login(It.IsAny<string>(), It.IsAny<string>())).Returns(
                new ApiLogOnResponseDTO
                    {
                        AllowedAccountOperator = true,
                        PasswordChangeRequired = true,
                        Session = _session
                    });
            var controller = new TokenController(loginServiceMock.Object);
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            var result = (JsonResult)controller.Index(formCollection);


            Assert.AreEqual(400, context.Response.StatusCode);

            Assert.IsInstanceOf(typeof(Error), result.Data);

            var error = (Error)result.Data;

            Assert.AreEqual(400, error.status);
            Assert.AreEqual("", error.error_uri);
            Assert.AreEqual("password change required", error.error_description);
            Assert.AreEqual("invalid_request", error.error);
        }

        [Test]
        public void ValidRefreshCodeReturnsToken()
        {
            var formCollection = new FormCollection
                                     {
                                         {"client_id", _client_id},
                                         {"client_secret", _client_secret},
                                         {"grant_type", "refresh_token"},
                                         {"refresh_token", _refresh_token}
                                     };
            HttpContextBase context = Mocking.FakeHttpContext();
            var loginServiceMock = new Mock<ILoginService>();
            loginServiceMock.Setup(m => m.Login(It.IsAny<string>(), It.IsAny<string>())).Returns(
                new ApiLogOnResponseDTO
                    {
                        AllowedAccountOperator = true,
                        PasswordChangeRequired = false,
                        Session = _session
                    });
            var controller = new TokenController(loginServiceMock.Object);
            controller.ControllerContext = new ControllerContext(context, new RouteData(), controller);

            var result = (JsonResult)controller.Index(formCollection);


            Assert.IsInstanceOf(typeof(AccessToken), result.Data);

            var token = (AccessToken)result.Data;

            Assert.AreEqual(string.Format("{0}:{1}", _username, _session), token.access_token);
            Assert.AreEqual(_refresh_token, token.refresh_token);
            Assert.AreEqual("bearer", token.token_type);
        }
    }
}