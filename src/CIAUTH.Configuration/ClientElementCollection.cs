using System.Configuration;

namespace CIAUTH.Configuration
{
    [ConfigurationCollection(typeof(ClientElement), AddItemName = "client", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ClientElementCollection:ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ClientElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ClientElement)element).Id;
        }

        public ClientElement this[int index]
        {
            get { return (ClientElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public ClientElement this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    return null;
                }
                return (ClientElement)base.BaseGet(name);
            }
        }
    }
}