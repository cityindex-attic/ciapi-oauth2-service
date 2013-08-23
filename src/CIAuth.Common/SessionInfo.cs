using CIAPI.DTO;

namespace CIAuth.Common
{
    public class SessionInfo
    {
        public ApiLogOnResponseDTO Session { get; set; }
        public AccountInformationResponseDTO Account { get; set; }

        /// <summary>
        /// NOTE: this is a TEMPORARY stop-gap measure to help facilitate 'refresh' capabilities
        /// it is advised that only TEST accounts be used against this service until the API
        /// team exposes a /session/refresh method that will exhange a new session for an old (still valid) session
        /// </summary>
        public string EncryptedCredentials { get; set; }
    }
}