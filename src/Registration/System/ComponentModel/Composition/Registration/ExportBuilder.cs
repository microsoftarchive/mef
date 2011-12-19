using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.Internal;

namespace System.ComponentModel.Composition.Registration
{
    public sealed class ExportBuilder
    {
        private bool _isInherited;
        private string _contractName;
        private Type _contractType;
        private List<Tuple<string, object>> _metadataItems;
        private List<Tuple< string, Func<Type, object>>> _metadataItemFuncs;

        public ExportBuilder() {}

        public ExportBuilder AsContractType<T>()
        {
            return AsContractType(typeof(T));
        }

        public ExportBuilder AsContractType(Type type)
        {
            this._contractType = type;
            return this;
        }

        public ExportBuilder AsContractName(string contractName)
        {
            this._contractName = contractName;
            return this;
        }

        public ExportBuilder Inherited()
        {
            this._isInherited = true;
            return this;
        }

        public ExportBuilder AddMetadata(string name, object value)
        {
            if(this._metadataItems == null)
            {
                this._metadataItems = new List<Tuple<string, object>>();
            }
            this._metadataItems.Add(Tuple.Create(name, value));
            return this;
        }

        public ExportBuilder AddMetadata(string name, Func<Type, object> itemFunc)
        {
            if(this._metadataItemFuncs == null)
            {
                this._metadataItemFuncs = new List<Tuple<string, Func<Type, object>>>();
            }
            this._metadataItemFuncs.Add(Tuple.Create(name, itemFunc));
            return this;
        }

        internal void BuildAttributes(Type type, ref List<Attribute> attributes)
        {
            if(attributes == null)
            {
                attributes = new List<Attribute>();
            }

            if (this._isInherited)
            {
                // Default export
                attributes.Add(new InheritedExportAttribute(this._contractName, this._contractType));
            }
            else
            {
                // Default export
                attributes.Add(new ExportAttribute(this._contractName, this._contractType));
            }

            //Add metadata attributes from direct specification
            if (this._metadataItems != null)
            {
                foreach (var item in this._metadataItems)
                {
                    attributes.Add(new ExportMetadataAttribute(item.Item1, item.Item2));
                }
            }

            //Add metadata attributes from func specification
            if (this._metadataItemFuncs != null)
            {
                foreach (var item in this._metadataItemFuncs)
                {
                    var name = item.Item1;
                    var value = (item.Item2 != null) ? item.Item2(type.UnderlyingSystemType) : null;
                    attributes.Add(new ExportMetadataAttribute(name, value));
                }
            }
        }
    }
}
