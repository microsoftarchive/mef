// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting.Core;
using System.ComponentModel.Composition.Lightweight.ProgrammingModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.Discovery
{
    class DiscoveredPropertyExport : DiscoveredExport
    {
        static readonly MethodInfo ActivatorInvoke = typeof(CompositeActivator).GetMethod("Invoke");

        readonly PropertyInfo _property;

        public DiscoveredPropertyExport(Contract contract, IDictionary<string, object> metadata, PropertyInfo property)
            : base(contract, metadata)
        {
            _property = property;
        }

        protected override ExportDescriptor GetExportDescriptor(CompositeActivator partActivator)
        {
            var args = new[] { Expression.Parameter(typeof(LifetimeContext)), Expression.Parameter(typeof(CompositionOperation)) };

            var activator = Expression.Lambda<CompositeActivator>(
                Expression.Property(
                    Expression.Convert(Expression.Call(Expression.Constant(partActivator), ActivatorInvoke, args), _property.DeclaringType),
                    _property),
                args);

            return ExportDescriptor.Create(activator.Compile(), Metadata);
        }

        public override DiscoveredExport CloseGenericExport(Type closedPartType, Type[] genericArguments)
        {
            var contract = Contract.ContractType.MakeGenericType(genericArguments);
            var newContract = new Contract(contract, Contract.Discriminator);
            var property = closedPartType.GetProperty(_property.Name);
            return new DiscoveredPropertyExport(newContract, Metadata, property);
        }
    }
}
