using System.Configuration;
using System.Web.Configuration;

namespace CIAUTH.Configuration
{
    public class CIAUTHConfigurationSection : ConfigurationSection
    {
        public static CIAUTHConfigurationSection Instance
        {
            get
            {
                var section = WebConfigurationManager.GetSection("ciauth") as CIAUTHConfigurationSection;
                return section;
            }
        }

        [ConfigurationProperty("aes_vector")]
        public string AesVector
        {
            get { return (string) base["aes_vector"]; }

            set { base["aes_vector"] = value; }
        }

        [ConfigurationProperty("aes_key")]
        public string AesKey
        {
            get { return (string) base["aes_key"]; }
            set { base["aes_key"] = value; }
        }

        [ConfigurationProperty("app_key")]
        public string AppKey
        {
            get { return (string) base["app_key"]; }
            set { base["app_key"] = value; }
        }

        [ConfigurationProperty("api_url")]
        public string ApiUrl
        {
            get { return (string) base["api_url"]; }
            set { base["api_url"] = value; }
        }

        [ConfigurationProperty("clients")]
        public ClientElementCollection Clients
        {
            get { return (ClientElementCollection) base["clients"]; }
            set { base["clients"] = value; }
        }
    }
}