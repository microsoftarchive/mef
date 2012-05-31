// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Composition.Hosting.Core;
using System.Composition.Hosting.Util;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Composition.Hosting.Providers.Metadata
{
    /// <summary>
    /// Creates components for contracts like:
    ///     Func&lt;IDictionary&lt;string,object&gt;,TMetadata&gt; "MetadataViewProvider"
    /// Where TMetadata is a concrete class with a parameterless or dictionary constructor.
    /// </summary>
    class MetadataViewProviderExportDescriptorProvider : ExportDescriptorProvider
    {
        public const string MetadataViewProviderContractName = "MetadataViewProvider";
        static readonly MethodInfo GetMetadataViewProviderMethod = typeof(MetadataViewProviderExportDescriptorProvider).GetTypeInfo().GetDeclaredMethod("GetMetadataViewProvider");
        static readonly MethodInfo GetMetadataValueMethod = typeof(MetadataViewProviderExportDescriptorProvider).GetTypeInfo().GetDeclaredMethod("GetMetadataValue");

        public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(
            CompositionContract contract,
            DependencyAccessor descriptorAccessor)
        {
            if (contract.ContractName != MetadataViewProviderContractName ||
                !contract.ContractType.IsConstructedGenericType ||
                contract.ContractType.GetGenericTypeDefinition() != typeof(Func<,>) ||
                contract.MetadataConstraints != null)
                return NoExportDescriptors;

            var providerArgs = contract.ContractType.GenericTypeArguments;
            var argType = providerArgs[0];
            var viewType = providerArgs[1];
            if (argType != typeof(IDictionary<string, object>))
                return NoExportDescriptors;

            if (viewType == typeof(IDictionary<string, object>))
            {
                Func<IDictionary<string,object>, IDictionary<string,object>> rawProvider = d => d;
                return new[] { new ExportDescriptorPromise(
                    contract,
                    "Metadata View Provider",
                    true,
                    NoDependencies,
                    _ => ExportDescriptor.Create((c, o) => rawProvider, NoMetadata)) };
            }

            if (viewType.GetTypeInfo().IsAbstract)
                return NoExportDescriptors;

            var getViewMethod = GetMetadataViewProviderMethod.MakeGenericMethod(viewType);
            var gvm = getViewMethod.CreateStaticDelegate<Func<object>>();
            var viewProvider = gvm();

            return new[] { new ExportDescriptorPromise(
                contract,
                "Metadata View Provider",
                true,
                NoDependencies,
                _ => ExportDescriptor.Create((c, o) => viewProvider, NoMetadata)) };
        }

        static Func<IDictionary<string, object>, TMetadata> GetMetadataViewProvider<TMetadata>()
        {
            var ti = typeof(TMetadata).GetTypeInfo();
            var dictionaryConstructor = ti.DeclaredConstructors.SingleOrDefault(ci =>
            {
                var ps = ci.GetParameters();
                return ci.IsPublic && ps.Length == 1 && ps[0].ParameterType == typeof(IDictionary<string, object>);
            });

            if (dictionaryConstructor != null)
            {
                var providerArg = Expression.Parameter(typeof(IDictionary<string,object>), "metadata");
                return Expression.Lambda<Func<IDictionary<string, object>, TMetadata>>(
                        Expression.New(dictionaryConstructor, providerArg),
                        providerArg)
                    .Compile();
            }

            var parameterlessConstructor = ti.DeclaredConstructors.SingleOrDefault(ci => ci.IsPublic && ci.GetParameters().Length == 0);
            if (parameterlessConstructor != null)
            {
                var providerArg = Expression.Parameter(typeof(IDictionary<string, object>), "metadata");
                var resultVar = Expression.Variable(typeof(TMetadata), "result");

                var blockExprs = new List<Expression>();
                blockExprs.Add(Expression.Assign(resultVar, Expression.New(parameterlessConstructor)));

                foreach (var prop in typeof(TMetadata).GetTypeInfo().DeclaredProperties
                    .Where(prop => prop.GetMethod.IsPublic && !prop.GetMethod.IsStatic && prop.SetMethod.IsPublic && !prop.SetMethod.IsStatic))
                {
                    var dva = Expression.Constant(prop.GetCustomAttribute<DefaultValueAttribute>(false), typeof(DefaultValueAttribute));
                    var name = Expression.Constant(prop.Name, typeof(string));
                    var m = GetMetadataValueMethod.MakeGenericMethod(prop.PropertyType);
                    var assign = Expression.Assign(
                        Expression.Property(resultVar, prop),
                        Expression.Call(null, m, providerArg, name, dva));
                    blockExprs.Add(assign);
                }

                blockExprs.Add(resultVar);

                return Expression.Lambda<Func<IDictionary<string, object>, TMetadata>>(
                        Expression.Block(new[] { resultVar }, blockExprs), providerArg)
                    .Compile();
            }

            var noConstructorsMessage = string.Format("The type '{0}' cannot be used as a metadata view as it does not have a suitable (parameterless or dictionary) constructor.", typeof(TMetadata).Name);
            throw new CompositionFailedException(noConstructorsMessage);
        }

        static TValue GetMetadataValue<TValue>(IDictionary<string, object> metadata, string name, DefaultValueAttribute defaultValue)
        {
            object result;
            if (metadata.TryGetValue(name, out result))
                return (TValue)result;

            if (defaultValue != null)
                return (TValue)defaultValue.Value;
            
            // This could be significantly improved by describing the target metadata property.
            var message = string.Format("Export metadata for '{0}' is missing and no default value was supplied.", name);
            throw new CompositionFailedException(message);
        }
    }
}
