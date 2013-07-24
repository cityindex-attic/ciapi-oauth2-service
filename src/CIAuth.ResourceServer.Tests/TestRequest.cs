using System;
using System.Collections.Specialized;
using System.Web;

namespace CIAuth.ResourceServer.Tests
{
    public class TestRequest : HttpRequestBase
    {
        private readonly NameValueCollection _headers;
        private readonly NameValueCollection _serverVariables;
        private Uri _uri;

        public TestRequest()
        {
            _serverVariables = new NameValueCollection();
            _headers = new NameValueCollection();
        }

        public override NameValueCollection ServerVariables
        {
            get { return _serverVariables; }
        }

        public override NameValueCollection Headers
        {
            get { return _headers; }
        }

        public override Uri Url
        {
            get { return _uri; }
        }

        public static TestRequest Create(Uri requestUri)
        {
            var req = new TestRequest
                {
                    _uri = requestUri
                };
            return req;
        }
    }
}