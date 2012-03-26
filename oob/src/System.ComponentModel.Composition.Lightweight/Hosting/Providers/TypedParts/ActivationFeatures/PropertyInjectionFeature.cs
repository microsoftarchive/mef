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

namespace System.ComponentModel.Composition.Lightweight.Hosting.Providers.TypedParts.ActivationFeatures
{
    /// <summary>
    /// Modifies activators of parts with property imports so that the properties
    /// are set appropriately.
    /// </summary>
    class PropertyInjectionFeature : ActivationFeature
    {
        readonly IAttributeContext _attributeContext;
        static readonly MethodInfo ActivatorInvokeMethod = typeof(CompositeActivator).GetMethod("Invoke");

        public PropertyInjectionFeature(IAttributeContext attributeContext)
        {
            _attributeContext = attributeContext;
        }

        public override IEnumerable<Dependency> GetDependencies(Type partType, DependencyAccessor definitionAccessor)
        {
            var imports = (from pi in partType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(pi => pi.CanWrite)
                           let attrs = _attributeContext
                             .GetDeclaredAttributes(pi)
                             .ToArray()
                           where attrs.Any(a => a is ImportAttribute || a is ImportManyAttribute)
                           select new { Property = pi, ImportInfo = ContractHelpers.GetImportInfo(pi.PropertyType, attrs) }).ToArray();

            if (imports.Length == 0)
                return NoDependencies;

            var result = new List<Dependency>();

            foreach (var i in imports)
            {
                var site = new PropertyImportSite(i.Property);

                if (!i.ImportInfo.AllowDefault)
                {
                    result.Add(definitionAccessor.ResolveRequiredDependency(site, i.ImportInfo.Contract, false));
                }
                else
                {
                    Dependency optional;
                    if (definitionAccessor.TryResolveOptionalDependency(site, i.ImportInfo.Contract, false, out optional))
                        result.Add(optional);

                    // Variation from CompositionContainer behaviour: we don't have to support recomposition
                    // so we don't require that defaultable imports be set to null.
                }
            }

            return result;
        }

        public override CompositeActivator RewriteActivator(
            Type partType,
            CompositeActivator activator,
            IDictionary<string, object> partMetadata,
            Dependency[] dependencies)
        {
            var propertyDependencies = dependencies
                .Where(dep => dep.Site is PropertyImportSite)
                .ToDictionary(d => ((PropertyImportSite)d.Site).Property);

            if (propertyDependencies.Count == 0)
                return activator;

            var lc = Expression.Parameter(typeof(LifetimeContext));
            var op = Expression.Parameter(typeof(CompositionOperation));
            var inst = Expression.Parameter(typeof(object));
            var typed = Expression.Variable(partType);

            var statements = new List<Expression>();
            var assignTyped = Expression.Assign(typed, Expression.Convert(inst, partType));
            statements.Add(assignTyped);

            foreach (var d in propertyDependencies)
            {
                var property = d.Key;

                var assignment = Expression.Assign(
                    Expression.MakeMemberAccess(typed, property),
                    Expression.Convert(
                        Expression.Call(
                            Expression.Constant(d.Value.Target.GetDescriptor().Activator),
                            ActivatorInvokeMethod,
                            lc,
                            op),
                        property.PropertyType));

                statements.Add(assignment);
            }

            statements.Add(inst);

            var setAll = Expression.Block(new[] { typed }, statements);
            var setAction = Expression.Lambda<Func<object, LifetimeContext, CompositionOperation, object>>(
                setAll, inst, lc, op).Compile();

            return (c, o) =>
            {
                var i = activator(c, o);
                o.AddNonPrerequisiteAction(() => setAction(i, c, o));
                return i;
            };
        }
    }
}
