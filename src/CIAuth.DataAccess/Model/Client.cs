using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace CIAuth.DataAccess.Model
{
    public class Client
    {
        /// <summary>
        /// Display to client upon creation and discard
        /// </summary>
        [NotMapped]
        public string ClientSecret { get; set; }

        [Key]
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }

        /// <summary>
        /// hashed client secret
        /// </summary>
        public string ClientSecretHash { get; set; }
        public string EncryptionKey { get; set; }
        public bool Allowed { get; set; }
        public string CallbackUris { get; set; }
        public string Hosts { get; set; }
        public virtual List<Grant> Grants { get; set; }
    }
}
