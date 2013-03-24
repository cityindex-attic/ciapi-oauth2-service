using System;

namespace CIAUTH.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class AccessTokenEventArgs:EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public AccessToken AccessToken { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class AccessToken
    {
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SessionId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string token_type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int expires_in { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string refresh_token { get; set; }

    }
}