using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CIAUTH.Services;
using NUnit.Framework;

namespace CIAUTH.Tests.Services
{
    [TestFixture]
    public class LoginServiceFixture
    {
        [Test]
        public void noid()
        {
            LoginService service = new LoginService();
            
        }
    }
}
