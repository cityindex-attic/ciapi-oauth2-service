using System.Configuration;

namespace CIAuth.Common.Configuration
{
    public class CIAuthSection : ConfigurationSection
    {
        [ConfigurationProperty("encryptionKey", IsRequired = false)]
        public EncryptionKeyElement EncryptionKey
        {
            get { return (EncryptionKeyElement)base["encryptionKey"]; }
            set { base["encryptionKey"] = value; }
        }

        [ConfigurationProperty("tradingAPI",IsRequired = false)]
        public TradingAPIElement TradingAPI
        {
            get { return (TradingAPIElement)base["tradingAPI"]; }
            set { base["tradingAPI"] = value; }
        }

        [ConfigurationProperty("authServer")]
        public string AuthServer
        {
            get { return (string)base["authServer"]; }
            set { base["authServer"] = value; }
        }


        [ConfigurationProperty("clientId")]
        public string ClientId
        {
            get { return (string)base["clientId"]; }
            set { base["clientId"] = value; }
        }
        [ConfigurationProperty("clientSecret")]
        public string ClientSecret
        {
            get { return (string)base["clientSecret"]; }
            set { base["clientSecret"] = value; }
        }
        public static CIAuthSection Instance()
        {
            return (CIAuthSection)ConfigurationManager.GetSection("ciauth");
        }


    }
}