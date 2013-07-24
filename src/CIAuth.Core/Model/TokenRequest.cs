using Newtonsoft.Json;

namespace CIAuth.Core.Model
{
    public class TokenRequest
    {
        // ReSharper disable InconsistentNaming
        public string redirect_uri { get; set; }
        public string client_id { get; set; }
        public string code { get; set; }
        public string client_secret { get; set; }
        public string grant_type { get; set; }
        // ReSharper restore InconsistentNaming
    }
}