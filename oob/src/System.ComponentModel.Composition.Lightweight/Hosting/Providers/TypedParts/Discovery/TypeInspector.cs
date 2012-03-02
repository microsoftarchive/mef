// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.ActivationFeatures;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.ComponentModel.Composition.Lightweight.Util;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.Discovery
{
    class TypeInspector
    {
        static readonly IDictionary<string, object> NoMetadata = new Dictionary<string, object>();

        readonly ActivationFeature[] _activationFeatures;
        readonly IAttributeContext _attributeContext;
       
        public TypeInspector(IAttributeContext attributeContext, ActivationFeature[] activationFeatures)
        {
            _attributeContext = attributeContext;
            _activationFeatures = activationFeatures;
        }

        public bool InspectTypeForPart(Type type, out DiscoveredPart part)
        {
            part = null;

            if (type.IsAbstract || !type.IsClass || _attributeContext.GetDeclaredAttribute<PartNotDiscoverableAttribute>(type) != null)
                return false;

            foreach (var export in DiscoverExports(type))
            {
                part = part ?? new DiscoveredPart(type, _attributeContext, _activationFeatures);
                part.AddDiscoveredExport(export);
            }

            return part != null;
        }

        IEnumerable<DiscoveredExport> DiscoverExports(Type partType)
        {
            foreach (var export in DiscoverInstanceExports(partType))
                yield return export;

            foreach (var export in DiscoverPropertyExports(partType))
                yield return export;
        }

        IEnumerable<DiscoveredExport> DiscoverInstanceExports(Type partType)
        {
            foreach (var export in _attributeContext.GetDeclaredAttributes<ExportAttribute>(partType))
            {
                IDictionary<string, object> metadata = new Dictionary<string, object>();
                ReadMetadataAttribute(export, metadata);

                var applied = _attributeContext.GetDeclaredAttributes(partType);
                ReadLooseMetadata(applied, metadata);

                var contractType = export.ContractType ?? partType;
                CheckInstanceExportCompatibility(partType, contractType);

                var exportKey = new Contract(contractType, export.ContractName);

                if (metadata.Count == 0)
                    metadata = NoMetadata;

                yield return new DiscoveredInstanceExport(exportKey, metadata);
            }
        }

        IEnumerable<DiscoveredExport> DiscoverPropertyExports(Type partType)
        {
            foreach (var property in partType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.CanRead))
            {
                foreach (var export in _attributeContext.GetDeclaredAttributes<ExportAttribute>(property))
                {
                    IDictionary<string, object> metadata = new Dictionary<string, object>();
                    ReadMetadataAttribute(export, metadata);

                    var applied = _attributeContext.GetDeclaredAttributes(property);
                    ReadLooseMetadata(applied, metadata);

                    var contractType = export.ContractType ?? property.PropertyType;
                    CheckPropertyExportCompatibility(partType, property, contractType);

                    var exportKey = new Contract(export.ContractType ?? property.PropertyType, export.ContractName);

                    if (metadata.Count == 0)
                        metadata = NoMetadata;

                    yield return new DiscoveredPropertyExport(exportKey, metadata, property);
                }
            }
        }
        void ReadLooseMetadata(object[] appliedAttributes, IDictionary<string, object> metadata)
        {
            foreach (var attribute in appliedAttributes)
            {
                if (attribute is ExportAttribute)
                    continue;

                var ema = attribute as ExportMetadataAttribute;
                if (ema != null)
                {
                    AddMetadata(metadata, ema.Name, ema.Value);
                }
                else
                {
                    ReadMetadataAttribute((Attribute)attribute, metadata);
                }
            }
        }

        void AddMetadata(IDictionary<string, object> metadata, string name, object value)
        {
            object existingValue;
            if (!metadata.TryGetValue(name, out existingValue))
            {
                metadata.Add(name, value);
                return;
            }

            var valueType = existingValue.GetType();
            if (valueType.IsArray)
            {
                var existingArray = (Array)existingValue;
                var newArray = Array.CreateInstance(value.GetType(), existingArray.Length + 1);
                Array.Copy(existingArray, newArray, existingArray.Length);
                newArray.SetValue(value, existingArray.Length);
                metadata[name] = newArray;
            }
            else
            {
                var newArray = Array.CreateInstance(value.GetType(), 2);
                newArray.SetValue(existingValue, 0);
                newArray.SetValue(value, 1);
                metadata[name] = newArray;
            }
        }

        void ReadMetadataAttribute(Attribute attribute, IDictionary<string, object> metadata)
        {
            var attrType = attribute.GetType();

            // Note, we don't support ReflectionContext in this scenario as
            if (attrType.GetCustomAttribute<MetadataAttributeAttribute>(false) == null)
                return;

            foreach (var prop in attrType
                .GetProperties()
                .Where(p => p.DeclaringType == attrType && p.CanRead))
            {
                AddMetadata(metadata, prop.Name, prop.GetValue(attribute, null));
            }
        }

        static void CheckPropertyExportCompatibility(Type partType, PropertyInfo property, Type contractType)
        {
            if (partType.IsGenericTypeDefinition)
            {
                CheckGenericContractCompatibility(partType, property.PropertyType, contractType);
            }
            else if (!contractType.IsAssignableFrom(property.PropertyType))
            {
                var message = string.Format("Exported contract type '{0}' is not assignable from property '{1}' of part '{2}'.", contractType.Name, property.Name, partType.Name);
                throw new LightweightCompositionException(message);
            }
        }

        static void CheckGenericContractCompatibility(Type partType, Type exportingMemberType, Type contractType)
        {
            if (!contractType.IsGenericTypeDefinition)
            {
                var message = string.Format("Open generic part '{0}' cannot export non-generic contract '{1}'.", partType.Name, contractType.Name);
                throw new LightweightCompositionException(message);
            }

            var compatible = false;

            foreach (var ifce in GetAssignableTypes(exportingMemberType))
            {
                if (ifce == contractType || (ifce.IsGenericType && ifce.GetGenericTypeDefinition() == contractType))
                {
                    var mappedType = ifce;
                    if (!mappedType.GetGenericArguments().SequenceEqual(partType.GetGenericArguments()))
                    {
                        var message = string.Format("Exported contract '{0}' of open generic part '{1}' does not match the generic arguments of the class.", contractType.Name, partType.Name);
                        throw new LightweightCompositionException(message);
                    }

                    compatible = true;
                    break;
                }
            }

            if (!compatible)
            {
                var message = string.Format("The open generic export '{0}' on part '{1}' is not compatible with the contract '{2}'.", exportingMemberType.Name, partType.Name, contractType.Name);
                throw new LightweightCompositionException(message);
            }
        }

        static IEnumerable<Type> GetAssignableTypes(Type exportingMemberType)
        {
            foreach (var ifce in exportingMemberType.GetInterfaces())
                yield return ifce;

            var b = exportingMemberType;
            while (b != null)
            {
                yield return b;
                b = b.BaseType;
            }
        }

        static void CheckInstanceExportCompatibility(Type partType, Type contractType)
        {
            if (partType.IsGenericTypeDefinition)
            {
                CheckGenericContractCompatibility(partType, partType, contractType);
            }
            else if (!contractType.IsAssignableFrom(partType))
            {
                var message = string.Format("Exported contract type '{0}' is not assignable from part '{1}'.", contractType.Name, partType.Name);
                throw new LightweightCompositionException(message);
            }
        }
    }
}
