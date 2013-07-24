using System;
using Microsoft.IdentityModel.Claims;

namespace CIAuth.ResourceServer
{
    public class TokenItem
    {
        public DateTimeOffset Expires { get; set; }
        public string Token { get; set; }

        public string Username { get; set; }

        public string Session { get; set; }
    }
}