namespace CIAUTH.Models
{
    public class Error
    {
        // ReSharper disable InconsistentNaming
        public int status;
        public string error { get; set; }
        public string error_description { get; set; }
        public string error_uri { get; set; }
        // ReSharper restore InconsistentNaming
    }
}