using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CIAuth.DataAccess.Model
{
    public class Grant
    {

        
        
        [Key,Column(Order=1)]
        public string Username { get; set; }
        [Key, Column(Order = 2)]
        public string Scope { get; set; }

        public string Session { get; set; }

        [MaxLength(2048)]
        public string Token { get; set; }

        public DateTimeOffset Expires { get; set; }
        /// <summary>
        /// This is a stop gap measure until  /tradingapi/session/refresh can be implemented
        /// to exchange a valid session, before it expires, for a new session
        /// </summary>
        [MaxLength(2048)]
        public string EncryptedCredentials { get; set; }
        /// <summary>
        /// valid values: offline (app can act on behalf of user and refresh token autonomously), online (valid only for this web session)
        /// </summary>
        public string AccessType { get; set; }
        public string ClientId { get; set; }
        public virtual Client Client { get; set; }
    }
}