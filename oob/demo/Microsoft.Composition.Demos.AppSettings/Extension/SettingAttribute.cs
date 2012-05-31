using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettingsExtensionDemo.Extension
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    [MetadataAttribute]
    public class SettingAttribute : Attribute
    {
        readonly string _key;

        public SettingAttribute(string key)
        {
            _key = key;
        }

        public string SettingKey { get { return _key; } }
    }
}
