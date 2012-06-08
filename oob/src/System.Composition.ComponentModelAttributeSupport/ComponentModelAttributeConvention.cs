// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Composition.Convention;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Composition.ComponentModelAttributeSupport
{
    public class ComponentModelAttributeConvention : AttributedModelProvider
    {
        public override IEnumerable<Attribute> GetCustomAttributes(Type reflectedType, Reflection.MemberInfo member)
        {
            return MapAttributes(reflectedType, member, member.GetCustomAttributes(false).Cast<Attribute>());
        }

        public override IEnumerable<Attribute> GetCustomAttributes(Type reflectedType, Reflection.ParameterInfo parameter)
        {
            return MapAttributes(reflectedType, parameter, parameter.GetCustomAttributes(false).Cast<Attribute>());
        }

        static IEnumerable<Attribute> MapAttributes(Type reflectedType, object member, IEnumerable<Attribute> attributes)
        {
            // This method could be improved by logging any unsupported features that are
            // detected.

            // This method could execute much faster by indexing translations according to
            // the inspected attribute type.

            // This method could be optimized with caching.

            // This class could scan for private/field imports and exports, and create proxy
            // classes to expose them.

            var creationPolicy = System.ComponentModel.Composition.CreationPolicy.Any;
            var ti = member as TypeInfo;

            foreach (var attribute in attributes)
            {
                if (ti == null || ti.AsType() == reflectedType)
                {
                    var export = attribute as System.ComponentModel.Composition.ExportAttribute;
                    if (export != null)
                        yield return new System.Composition.ExportAttribute(export.ContractName, export.ContractType);

                    var em = attribute as System.ComponentModel.Composition.ExportMetadataAttribute;
                    if (em != null)
                        yield return new System.Composition.ExportMetadataAttribute(em.Name, em.Value);

                    if (attribute.GetType().GetCustomAttribute<System.ComponentModel.Composition.MetadataAttributeAttribute>() != null)
                    {
                        foreach (var mp in attribute.GetType().GetProperties().Where(p => p.CanRead))
                            yield return new System.Composition.ExportMetadataAttribute(mp.Name, mp.GetValue(attribute));
                    }
                }

                var ic = attribute as System.ComponentModel.Composition.ImportingConstructorAttribute;
                if (ic != null)
                    yield return new System.Composition.ImportingConstructorAttribute();

                var im = attribute as System.ComponentModel.Composition.ImportAttribute;
                if (im != null)
                    yield return new System.Composition.ImportAttribute(im.ContractName, im.ContractType) { AllowDefault = im.AllowDefault };

                var imm = attribute as System.ComponentModel.Composition.ImportManyAttribute;
                if (imm != null)
                    yield return new System.Composition.ImportManyAttribute(im.ContractName, im.ContractType);

                var mi = member as MethodInfo;
                if (mi != null && mi.Name == "OnImportsSatisfied" && mi.ReturnType == typeof(void) &&
                        mi.GetParameters().Length == 0 &&
                        typeof(System.ComponentModel.Composition.IPartImportsSatisfiedNotification).IsAssignableFrom(mi.DeclaringType))
                    yield return new System.Composition.OnImportsSatisfiedAttribute();

                var pcp = attribute as System.ComponentModel.Composition.PartCreationPolicyAttribute;
                if (pcp != null)
                    creationPolicy = pcp.CreationPolicy;

                var pnd = attribute as System.ComponentModel.Composition.PartNotDiscoverableAttribute;
                if (pnd != null)
                    yield return new System.Composition.PartNotDiscoverableAttribute();
            }
            
            if (ti != null && ti.IsClass &&
                    creationPolicy == System.ComponentModel.Composition.CreationPolicy.Any ||
                    creationPolicy == System.ComponentModel.Composition.CreationPolicy.Shared)
                yield return new SharedAttribute();
        }
    }
}
