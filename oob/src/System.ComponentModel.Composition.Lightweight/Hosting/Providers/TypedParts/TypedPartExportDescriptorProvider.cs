// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.Util;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.Discovery;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.ActivationFeatures;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts
{
    class TypedPartExportDescriptorProvider : ExportDescriptorProvider
    {
        readonly IDictionary<Contract, ICollection<DiscoveredExport>> _discoveredParts = new Dictionary<Contract, ICollection<DiscoveredExport>>();

        public TypedPartExportDescriptorProvider(IEnumerable<Type> types, IAttributeContext attributeContext)
        {
            var activationFeatures = CreateActivationFeatures(attributeContext);
            var typeInspector = new TypeInspector(attributeContext, activationFeatures);

            foreach (var type in types)
            {
                DiscoveredPart part;
                if (typeInspector.InspectTypeForPart(type, out part))
                {
                    AddDiscoveredPart(part);
                }
            }
        }

        void AddDiscoveredPart(DiscoveredPart part)
        {
            foreach (var export in part.DiscoveredExports)
            {
                AddDiscoveredExport(export);
            }
        }

        void AddDiscoveredExport(DiscoveredExport export, Contract contract = null)
        {
            var actualContract = contract ?? export.Contract;

            ICollection<DiscoveredExport> forKey;
            if (!_discoveredParts.TryGetValue(actualContract, out forKey))
            {
                forKey = new List<DiscoveredExport>();
                _discoveredParts.Add(actualContract, forKey);
            }

            forKey.Add(export);
        }

        public override ExportDescriptorPromise[] GetExportDescriptors(Contract contract, DependencyAccessor definitionAccessor)
        {
            DiscoverGenericParts(contract);
            DiscoverConstrainedParts(contract);

            ICollection<DiscoveredExport> forKey;
            if (!_discoveredParts.TryGetValue(contract, out forKey))
                return NoExportDescriptors;

            // Allow some garbage to be collected - important
            _discoveredParts.Remove(contract);

            return forKey.Select(de => de.GetExportDescriptorPromise(contract, definitionAccessor)).ToArray();
        }

        // Currently just attempting to prove that this is logically possible - rough implementation
        // that can probably be revisited if successful.
        // As with elsewhere in this class, identity issues complicate matters a bit.
        void DiscoverConstrainedParts(Contract contract)
        {
            var constrainedDiscriminator = contract.Discriminator as MetadataConstrainedDiscriminator;
            if (constrainedDiscriminator != null)
            {
                var unwrapped = new Contract(contract.ContractType, constrainedDiscriminator.InnerDiscriminator);
                DiscoverGenericParts(unwrapped);

                ICollection<DiscoveredExport> forKey;
                if (_discoveredParts.TryGetValue(unwrapped, out forKey))
                {
                    foreach (var export in forKey)
                    {
                        foreach (var constraint in constrainedDiscriminator.Metadata)
                        {
                            object value;
                            if (export.Metadata.TryGetValue(constraint.Key, out value) &&
                                (constraint.Value == null ? value == null : constraint.Value.Equals(value)))
                            {
                                AddDiscoveredExport(export, contract);
                            }
                        }
                    }
                }
            }
        }

        void DiscoverGenericParts(Contract contract)
        {
            if (!contract.ContractType.IsGenericType)
                return;

            var gtd = contract.ContractType.GetGenericTypeDefinition();
            var openGenericContract = new Contract(gtd, contract.Discriminator);
            ICollection<DiscoveredExport> openGenericParts;
            if (!_discoveredParts.TryGetValue(openGenericContract, out openGenericParts))
                return;

            var typeArguments = contract.ContractType.GetGenericArguments();
            foreach (var open in openGenericParts)
            {
                DiscoveredPart closed;
                if (open.Part.TryCloseGenericPart(typeArguments, out closed))
                    AddDiscoveredPart(closed);
            }
        }

        static ActivationFeature[] CreateActivationFeatures(IAttributeContext attributeContext)
        {
            return new ActivationFeature[] {
                new DisposalFeature(),
                new PropertyInjectionFeature(attributeContext),
                new PisnFeature(),
                new LifetimeFeature(),
            };
        }

        internal static ActivationFeature[] DebugGetActivationFeatures(IAttributeContext attributeContext)
        {
            return CreateActivationFeatures(attributeContext);
        }
    }
}
