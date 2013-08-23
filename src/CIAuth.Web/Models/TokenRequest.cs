namespace CIAuth.Web.Models
{
    public class TokenRequest
    {
        // ReSharper disable InconsistentNaming
        public string redirect_uri { get; set; }
        public int client_id { get; set; }
        public string code { get; set; }
        public string client_secret { get; set; }
        public string grant_type { get; set; }
        public string token { get; set; }
        public string refresh_token { get; set; }
        
        // ReSharper restore InconsistentNaming
    }
}