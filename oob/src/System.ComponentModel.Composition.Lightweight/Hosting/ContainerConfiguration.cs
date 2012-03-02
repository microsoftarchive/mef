// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.ActivationFeatures;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers.Lazy;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers.ExportFactory;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers.ImportMany;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers.CurrentScope;
using System.ComponentModel.Composition.Lightweight.Util;
using System.Reflection;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers.Dictionaries;
using System.ComponentModel.Composition.Registration;
using System.Diagnostics;
using System.ComponentModel.Composition.Lightweight.Debugging.Proxies;

namespace System.ComponentModel.Composition.Lightweight.Hosting
{
    /// <summary>
    /// Configures and constructs a lightweight container.
    /// </summary>
    [DebuggerTypeProxy(typeof(ContainerConfigurationDebuggerProxy))]
    public class ContainerConfiguration
    {
        IAttributeContext _defaultAttributeContext;
        readonly IList<ExportDescriptorProvider> _addedSources = new List<ExportDescriptorProvider>();
        readonly IList<Tuple<IEnumerable<Type>, IAttributeContext>> _types = new List<Tuple<IEnumerable<Type>, IAttributeContext>>();

        static readonly string[] NoBoundaries = new string[0];

        /// <summary>
        /// Create the container. The value returned from this method provides
        /// the exports in the container, as well as a means to dispose the container.
        /// </summary>
        /// <returns>The container wrapped in an <see cref="ExportLifetimeContext{T}"/>.</returns>
        public ExportLifetimeContext<IExportProvider> CreateContainer()
        {
            var sources = new ExportDescriptorProvider[] {
                new LazyExportDescriptorProvider(),
                new ExportFactoryExportDescriptorProvider(),
                new ImportManyExportDescriptorProvider(),
                new LazyWithMetadataExportDescriptorProvider(),
                new CurrentScopeExportDescriptorProvider(),
                new ExportFactoryWithMetadataExportDescriptorProvider(),
                new DictionaryExportDescriptorProvider()
            }
            .Concat(_addedSources)
            .ToList();

            foreach (var typeSet in _types)
            {
                var ac = typeSet.Item2 ?? _defaultAttributeContext ?? new DirectAttributeContext();

                sources.Add(new TypedPartExportDescriptorProvider(typeSet.Item1, ac));
            }

            var container = new LifetimeContext(new ExportDescriptorRegistry(sources.ToArray()), NoBoundaries);
            return new ExportLifetimeContext<IExportProvider>(container, container.Dispose);
        }

        /// <summary>
        /// Add an export descriptor provider to the container.
        /// </summary>
        /// <param name="exportDescriptorProvider">An export descriptor provider.</param>
        /// <returns>A configuration object allowing configuration to continue.</returns>
        public ContainerConfiguration WithProvider(ExportDescriptorProvider exportDescriptorProvider)
        {
            if (exportDescriptorProvider == null) throw new ArgumentNullException("ExportDescriptorProvider");
            _addedSources.Add(exportDescriptorProvider);
            return this;
        }

        /// <summary>
        /// Add conventions defined using a <see cref="ReflectionContext"/> to the container.
        /// These will be used as the default conventions; types and assemblies added with a
        /// specific convention will use their own.
        /// </summary>
        /// <param name="reflectionContext"></param>
        /// <returns>A configuration object allowing configuration to continue.</returns>
        public ContainerConfiguration WithDefaultConventions(ReflectionContext reflectionContext)
        {
            if (reflectionContext == null) throw new ArgumentNullException("reflectionContext");

            if (_defaultAttributeContext != null)
                throw new InvalidOperationException("The default conventions for the container configuration have already been set.");

            _defaultAttributeContext = new ReflectionContextAttributeContext(reflectionContext);
            return this;
        }

        /// <summary>
        /// Add a part type to the container. If the part type does not have any exports it
        /// will be ignored.
        /// </summary>
        /// <param name="partType">The part type.</param>
        /// <param name="conventions">Conventions represented by a <see cref="ReflectionContext"/>, or null.</param>
        /// <returns>A configuration object allowing configuration to continue.</returns>
        public ContainerConfiguration WithPart(Type partType, ReflectionContext conventions = null)
        {
            return WithParts(new[] { partType }, conventions);
        }

        /// <summary>
        /// Add a part type to the container. If the part type does not have any exports it
        /// will be ignored.
        /// </summary>
        /// <typeparam name="TPart">The part type.</typeparam>
        /// <param name="conventions">Conventions represented by a <see cref="ReflectionContext"/>, or null.</param>
        /// <returns>A configuration object allowing configuration to continue.</returns>
        public ContainerConfiguration WithPart<TPart>(ReflectionContext conventions = null)
        {
            return WithPart(typeof(TPart), conventions);
        }

        /// <summary>
        /// Add part types to the container. If a part type does not have any exports it
        /// will be ignored.
        /// </summary>
        /// <param name="partTypes">The part types.</param>
        /// <returns>A configuration object allowing configuration to continue.</returns>
        public ContainerConfiguration WithParts(params Type[] partTypes)
        {
            return WithParts((IEnumerable<Type>)partTypes);
        }

        /// <summary>
        /// Add part types to the container. If a part type does not have any exports it
        /// will be ignored.
        /// </summary>
        /// <param name="partTypes">The part types.</param>
        /// <param name="conventions">Conventions represented by a <see cref="ReflectionContext"/>, or null.</param>
        /// <returns>A configuration object allowing configuration to continue.</returns>
        public ContainerConfiguration WithParts(IEnumerable<Type> partTypes, ReflectionContext conventions = null)
        {
            IAttributeContext ac = conventions != null ?
                new ReflectionContextAttributeContext(conventions) :
                null;

            _types.Add(Tuple.Create(partTypes, ac));
            return this;
        }

        /// <summary>
        /// Add part types from an assembly to the container. If a part type does not have any exports it
        /// will be ignored.
        /// </summary>
        /// <param name="assembly">The assembly from which to add part types.</param>
        /// <param name="conventions">Conventions represented by a <see cref="ReflectionContext"/>, or null.</param>
        /// <returns>A configuration object allowing configuration to continue.</returns>
        public ContainerConfiguration WithAssembly(Assembly assembly, ReflectionContext conventions = null)
        {
            return WithAssemblies(new[] { assembly }, conventions);
        }

        /// <summary>
        /// Add part types from a list of assemblies to the container. If a part type does not have any exports it
        /// will be ignored.
        /// </summary>
        /// <param name="assemblies">Assemblies containing part types.</param>
        /// <param name="conventions">Conventions represented by a <see cref="ReflectionContext"/>, or null.</param>
        /// <returns>A configuration object allowing configuration to continue.</returns>
        public ContainerConfiguration WithAssemblies(IEnumerable<Assembly> assemblies, ReflectionContext conventions = null)
        {
            return WithParts(assemblies.SelectMany(a => a.GetTypes()), conventions);
        }

        internal ExportDescriptorProvider[] DebugGetAddedExportDescriptorProviders()
        {
            return _addedSources.ToArray();
        }

        internal Tuple<IEnumerable<Type>, IAttributeContext>[] DebugGetRegisteredTypes()
        {
            return _types.ToArray();
        }

        internal IAttributeContext DebugGetDefaultAttributeContext()
        {
            return _defaultAttributeContext;
        }
    }
}
