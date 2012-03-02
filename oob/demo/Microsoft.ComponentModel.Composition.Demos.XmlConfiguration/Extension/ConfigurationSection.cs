using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlConfigurationDemo.Extension
{
    public class ConfigurationSection : System.Configuration.ConfigurationSection
    {
        public const string PartsPropertyName = "parts";

        [ConfigurationProperty(PartsPropertyName, IsRequired=false)]
        public PartsCollection Parts { get { return (PartsCollection)this[PartsPropertyName]; } }
    }
}
