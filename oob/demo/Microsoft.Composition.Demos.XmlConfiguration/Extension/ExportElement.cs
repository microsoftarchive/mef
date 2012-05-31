using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace XmlConfigurationDemo.Extension
{
    public class ExportElement : ConfigurationElement
    {
        public const string ContractTypeAttributeName = "contractType";
        public const string Key = ContractTypeAttributeName;

        [ConfigurationProperty(ContractTypeAttributeName, IsRequired=true)]
        public string ContractType { get { return (string) this[ContractTypeAttributeName]; } }
    }
}
