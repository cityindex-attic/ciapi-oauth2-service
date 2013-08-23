using System;

namespace CIAuth.ResourceServer
{
    public class TokenItem
    {
        public DateTime Expires { get; set; }
        public string Token { get; set; }

        public string Username { get; set; }

        public string Session { get; set; }
    }
}