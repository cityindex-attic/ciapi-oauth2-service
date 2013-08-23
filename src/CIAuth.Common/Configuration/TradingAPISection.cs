using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace CIAuth.Common.Configuration
{
    public class TradingAPIElement:ConfigurationElement
    {
        [ConfigurationProperty("tradingAPI_rpc")]
        public string TradingAPIRpc
        {
            get { return (string)base["tradingAPI_rpc"]; }
            set { base["tradingAPI_rpc"] = value; }
        }
        [ConfigurationProperty("tradingAPI_streaming")]
        public string TradingAPIStreaming
        {
            get { return (string)base["tradingAPI_streaming"]; }
            set { base["tradingAPI_streaming"] = value; }
        }
        [ConfigurationProperty("tradingAPI_appKey")]
        public string TradingAPIAppKey
        {
            get { return (string)base["tradingAPI_appKey"]; }
            set { base["tradingAPI_appKey"] = value; }
        }

    }
}
