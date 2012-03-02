// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.Util;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.ActivationFeatures;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Diagnostics;
using System.ComponentModel.Composition.Lightweight.Debugging.Proxies;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.Discovery
{
    [DebuggerDisplay("{PartType.Name}")]
    [DebuggerTypeProxy(typeof(DiscoveredPartDebuggerProxy))]
    class DiscoveredPart
    {
        readonly Type _partType;
        readonly IAttributeContext _attributeContext;
        readonly ICollection<DiscoveredExport> _exports = new List<DiscoveredExport>();
        readonly ActivationFeature[] _activationFeatures;
        readonly Lazy<IDictionary<string, object>> _partMetadata;

        // This is unbounded so potentially a source of memory consumption,
        // but in reality unlikely to be a problem.
        readonly IList<Type[]> _appliedArguments = new List<Type[]>();

        // Lazyily initialised among potentially many exports
        ConstructorInfo _constructor;
        CompositeActivator _partActivator;

        static readonly IDictionary<string, object> NoMetadata = new Dictionary<string, object>();
        static readonly MethodInfo ActivatorInvoke = typeof(CompositeActivator).GetMethod("Invoke");

        DiscoveredPart(
            Type partType,
            IAttributeContext attributeContext,
            ActivationFeature[] activationFeatures,
            Lazy<IDictionary<string, object>> partMetadata)
        {
            _partType = partType;
            _attributeContext = attributeContext;
            _activationFeatures = activationFeatures;
            _partMetadata = partMetadata;
        }

        public DiscoveredPart(
            Type partType, 
            IAttributeContext attributeContext,
            ActivationFeature[] activationFeatures)
        {
            _partType = partType;
            _attributeContext = attributeContext;
            _activationFeatures = activationFeatures;
            _partMetadata = new Lazy<IDictionary<string, object>>(() => GetPartMetadata(partType));
        }

        public Type PartType { get { return _partType; } }

        public bool IsShared { get { return ContractHelpers.IsShared(_partMetadata.Value); } }

        public void AddDiscoveredExport(DiscoveredExport export)
        {
            _exports.Add(export);
            export.Part = this;
        }

        public Dependency[] GetDependencies(DependencyAccessor definitionAccessor)
        {
            return GetPartActivatorDependencies(definitionAccessor)
                .Concat(_activationFeatures
                    .SelectMany(feature => feature.GetDependencies(_partType, definitionAccessor)))
                .Where(a => a != null)
                .ToArray();
        }

        IEnumerable<Dependency> GetPartActivatorDependencies(DependencyAccessor definitionAccessor)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance; 
            
            foreach (var c in _partType.GetConstructors(bindingFlags))
            {
                if (_attributeContext.GetDeclaredAttribute<ImportingConstructorAttribute>(c) != null)
                {
                    if (_constructor != null)
                    {
                        var message = string.Format("Multiple importing constructors were found on type '{0}'.", _partType);
                        throw new LightweightCompositionException(message);
                    }

                    _constructor = c;
                }
            }

            if (_constructor == null)
                _constructor = _partType.GetConstructor(bindingFlags, null, new Type[0], null);

            if (_constructor == null)
            {
                var message = string.Format("No importing constructor was found on type '{0}'.", _partType);
                throw new LightweightCompositionException(message);
            }

            var cps = _constructor.GetParameters();

            for (var i = 0; i < cps.Length; ++i)
            {
                var pi = cps[i];
                var site = new ParameterImportSite(pi);

                var importInfo = ContractHelpers.GetImportInfo(pi.ParameterType, _attributeContext.GetDeclaredAttributes(pi));
                if (!importInfo.AllowDefault)
                {
                    yield return definitionAccessor.ResolveRequiredDependency(site, importInfo.Contract, true);
                }
                else
                {
                    Dependency optional;
                    if (definitionAccessor.TryResolveOptionalDependency(site, importInfo.Contract, true, out optional))
                        yield return optional;
                }
            }
        }

        public CompositeActivator GetActivator(DependencyAccessor definitionAccessor, Dependency[] dependencies)
        {
            if (_partActivator != null) return _partActivator;

            var contextParam = Expression.Parameter(typeof(LifetimeContext), "cc");
            var operationParm = Expression.Parameter(typeof(CompositionOperation), "op");

            var cps = _constructor.GetParameters();
            Expression[] paramActivatorCalls = new Expression[cps.Length];

            var partActivatorDependencies = dependencies
                .Where(dep => dep.Site is ParameterImportSite)
                .ToDictionary(d => ((ParameterImportSite)d.Site).Parameter);

            for (var i = 0; i < cps.Length; ++i)
            {
                var pi = cps[i];
                Dependency dep;

                if (partActivatorDependencies.TryGetValue(pi, out dep)) 
                {
                    var a = dep.Target.GetDescriptor().Activator;
                    paramActivatorCalls[i] =
                        Expression.Convert(Expression.Call(Expression.Constant(a), ActivatorInvoke, contextParam, operationParm), pi.ParameterType);
                }
                else
                {
                    paramActivatorCalls[i] = Expression.Default(pi.ParameterType);
                }
            }

            Expression body = Expression.Convert(Expression.New(_constructor, paramActivatorCalls), typeof(object));

            foreach (var feature in _activationFeatures)
                body = feature.RewriteActivator(_partType, contextParam, operationParm, body, _partMetadata.Value, dependencies);

            var activator = Expression.Lambda<CompositeActivator>(body, contextParam, operationParm);

            _partActivator = activator.Compile();
            return _partActivator;
        }

        IDictionary<string, object> GetPartMetadata(Type partType)
        {
            var partMetadata = new Dictionary<string, object>();

            foreach (var attr in _attributeContext.GetDeclaredAttributes(partType))
            {
                if (attr is PartCreationPolicyAttribute)
                {
                    var creationPolicy = ((PartCreationPolicyAttribute)attr).CreationPolicy;
                    partMetadata.Add(Constants.CreationPolicyPartMetadataName, creationPolicy);
                }
                else if (attr is PartMetadataAttribute)
                {
                    var ma = (PartMetadataAttribute)attr;
                    partMetadata.Add(ma.Name, ma.Value);
                }
                else if (attr is IPartMetadataAttribute)
                {
                    var ma = (IPartMetadataAttribute)attr;
                    foreach (var mkv in ma.Metadata)
                        partMetadata.Add(mkv.Key, mkv.Value);
                }
            }

            return partMetadata.Count == 0 ? NoMetadata : partMetadata;
        }

        public bool TryCloseGenericPart(Type[] typeArguments, out DiscoveredPart closed)
        {
            if (_appliedArguments.Any(args => Enumerable.SequenceEqual(args, typeArguments)))
            {
                closed = null;
                return false;
            }

            _appliedArguments.Add(typeArguments);

            var closedType = _partType.MakeGenericType(typeArguments);

            var result = new DiscoveredPart(closedType, _attributeContext, _activationFeatures, _partMetadata);

            foreach (var export in _exports)
            {
                var closedExport = export.CloseGenericExport(closedType, typeArguments);
                result.AddDiscoveredExport(closedExport);
            }

            closed = result;
            return true;
        }

        public IEnumerable<DiscoveredExport> DiscoveredExports { get { return _exports; } }
    }
}
