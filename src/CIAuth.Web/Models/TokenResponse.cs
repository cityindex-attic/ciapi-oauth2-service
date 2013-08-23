namespace CIAuth.Web.Models
{
    public class TokenResponse
    {
        /// <summary>
        /// invalid_request, unauthorized_client, access_denied, unsupported_response_type
        /// invalid_scope, server_error, temporarily_unavailable
        /// </summary>
        public string error { get; set; }
        public string error_description { get; set; }
        public string error_uri { get; set; }
        // #TODO: accept state param on token endpoint
        public string state { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string refresh_token { get; set; }
        public string access_token { get; set; }
        public string username { get; set; }
        public string session { get; set; }
    }
}