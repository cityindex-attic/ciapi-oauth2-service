﻿using System.Configuration;

namespace CIAUTH.Configuration
{
    // <clients>
    // <client id="" secret="" name="" about_url="" ip="" /> 
    // </clients>

    // <client 
    // id=""
    // secret=""
    // name=""
    // about_url=""
    // ip=""
    // /> 
    public class ClientElement : ConfigurationElement
    {
 
        [ConfigurationProperty("logo")]
        public string Logo
        {
            get { return (string)base["logo"]; }
            //set { base["id"] = value; }
        }
        [ConfigurationProperty("id")]
        public string Id
        {
            get { return (string) base["id"]; }
            //set { base["id"] = value; }
        }

        [ConfigurationProperty("secret")]
        public string Secret
        {
            get { return (string) base["secret"]; }
            //set { base["secret"] = value; }
        }

        [ConfigurationProperty("name")]
        public string Name
        {
            get { return (string) base["name"]; }
            //set { base["name"] = value; }
        }

        [ConfigurationProperty("about_url")]
        public string AboutURL
        {
            get { return (string) base["about_url"]; }
            //set { base["about_url"] = value; }
        }

        [ConfigurationProperty("hosts")]
        public string Hosts
        {
            get { return (string)base["hosts"]; }
        }
         


    }
}