using System;

namespace CIAuth.Web.Controllers
{
    public class CIAuthException : Exception
    {

        public string Description { get; set; }
        public CIAuthException(string message, string description)
            : base(message)
        {
            Description = description;
        }
        public CIAuthException(string message, string description, Exception innerException)
            : base(message, innerException)
        {
            Description = description;
        }


    }
}