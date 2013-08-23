using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CIAuth.Common.Configuration
{
    public class EncryptionKeyElement : ConfigurationElement
    {
        public XDocument ToXml()
        {
            XDocument x = new XDocument(
                new XElement("RSAKeyValue",
                    new XElement("Modulus", Modulus),
                    new XElement("Exponent", Exponent),
                    new XElement("P", P),
                    new XElement("Q", Q),
                    new XElement("DP", DP),
                    new XElement("DQ", DQ),
                    new XElement("InverseQ", InverseQ),
                    new XElement("D", D))
                );
            return x;
        }

        [ConfigurationProperty("modulus")]
        public string Modulus
        {
            get { return (string)base["modulus"]; }
            set { base["modulus"] = value; }
        }

        [ConfigurationProperty("exponent")]
        public string Exponent
        {
            get { return (string)base["exponent"]; }
            set { base["exponent"] = value; }
        }

        [ConfigurationProperty("p")]
        public string P
        {
            get { return (string)base["p"]; }
            set { base["p"] = value; }
        }

        [ConfigurationProperty("q")]
        public string Q
        {
            get { return (string)base["q"]; }
            set { base["q"] = value; }
        }

        [ConfigurationProperty("dp")]
        public string DP
        {
            get { return (string)base["dp"]; }
            set { base["dp"] = value; }
        }

        [ConfigurationProperty("dq")]
        public string DQ
        {
            get { return (string)base["dq"]; }
            set { base["dq"] = value; }
        }

        [ConfigurationProperty("inverseQ")]
        public string InverseQ
        {
            get { return (string)base["inverseQ"]; }
            set { base["inverseQ"] = value; }
        }

        [ConfigurationProperty("d")]
        public string D
        {
            get { return (string)base["d"]; }
            set { base["d"] = value; }
        }

    }
}
