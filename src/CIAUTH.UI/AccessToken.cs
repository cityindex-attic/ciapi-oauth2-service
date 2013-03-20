using System;

namespace CIAUTH.UI
{
    public class AccessTokenEventArgs:EventArgs
    {
        public AccessToken AccessToken { get; set; }
        public string Message { get; set; }
    }
    public class AccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }

    }
}