namespace CIAuth.Core.Model
{
    public class AuthzCodeRequest
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// describes the requested permission
        /// </summary>
        public string scope { get; set; }

        /// <summary>
        /// arbitrary client value. typically used to store a destination url
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// registered redirect endpoint that will parse auth response
        /// </summary>
        public string redirect_uri { get; set; }

        /// <summary>
        /// desired payload: code, access code to be exchanged, token, one leg user agent flow - demand https
        /// </summary>
        public string response_type { get; set; }

        public string client_id { get; set; }

        /// <summary>
        /// describes the desired access type: online (interactive), offline (autonomous application usage)
        /// </summary>
        public string access_type { get; set; }
        
        // ReSharper restore InconsistentNaming
    }
}