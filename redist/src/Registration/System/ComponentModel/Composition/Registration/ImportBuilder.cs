using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.ComponentModel.Composition.Registration
{
    public sealed class ImportBuilder
    {
        private static readonly Type StringType = typeof(string);
        private string _contractName;
        private Type _contractType;
        private bool _asMany;
        private bool _asManySpecified = false;
        private bool _allowDefault;
        private bool _allowRecomposition;
        private CreationPolicy _requiredCreationPolicy;
        private ImportSource  _source;

        public ImportBuilder() {}

        public ImportBuilder AsContractType<T>()
        {
            return AsContractType(typeof(T));
        }

        public ImportBuilder AsContractType(Type type)
        {
            this._contractType = type;
            return this;
        }

        public ImportBuilder AsContractName(string contractName)
        {
            this._contractName = contractName;
            return this;
        }

        public ImportBuilder AsMany (bool isMany = true)
        {
            this._asMany = isMany;
            this._asManySpecified = true;
            return this;
        }

        public ImportBuilder AllowDefault()
        {
            this._allowDefault = true;
            return this;
        }

        public ImportBuilder AllowRecomposition()
        {
            this._allowRecomposition = true;
            return this;
        }

        public ImportBuilder RequiredCreationPolicy(CreationPolicy requiredCreationPolicy)
        {
            this._requiredCreationPolicy = requiredCreationPolicy;
            return this;
        }

        public ImportBuilder Source(ImportSource source)
        {
            this._source = source;
            return this;
        }

        internal void BuildAttributes(Type type, ref List<Attribute> attributes)
        {
            Attribute importAttribute;
            
            // Infer from Type when not explicitly set.
            bool asMany = (!this._asManySpecified) ? type != StringType && typeof(IEnumerable).IsAssignableFrom(type) : this._asMany;
            if(!asMany)
            {
                importAttribute = new ImportAttribute(this._contractName, this._contractType) 
                                    {
                                        AllowDefault = this._allowDefault,
                                        AllowRecomposition = this._allowRecomposition,
                                        RequiredCreationPolicy = this._requiredCreationPolicy,
                                        Source = this._source
                                    };
            }
            else
            {
                importAttribute = new ImportManyAttribute(this._contractName, this._contractType) 
                                    {
                                        AllowRecomposition = this._allowRecomposition,
                                        RequiredCreationPolicy = this._requiredCreationPolicy,
                                        Source = this._source
                                    };
            }
            if(attributes == null)
            {
                attributes = new List<Attribute>();
            }
            attributes.Add(importAttribute);
            return;
        }
    }
}
