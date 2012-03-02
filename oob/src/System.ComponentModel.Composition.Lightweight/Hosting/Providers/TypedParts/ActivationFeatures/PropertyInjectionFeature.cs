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
        static readonly MethodInfo AddNonPrerequisiteActionMethod = typeof(CompositionOperation).GetMethod("AddNonPrerequisiteAction");

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

        public override Expression RewriteActivator(
            Type partType, 
            ParameterExpression compositionContextParameter,
            ParameterExpression operationParameter,
            Expression activatorBody, 
            IDictionary<string, object> partMetadata,
            Dependency[] dependencies)
        {
            var propertyDependencies = dependencies
                .Where(dep => dep.Site is PropertyImportSite)
                .ToDictionary(d => ((PropertyImportSite)d.Site).Property);

            if (propertyDependencies.Count == 0)
                return activatorBody;

            var temp = Expression.Variable(partType);

            var propertySetters = new List<Expression>();
            foreach (var d in propertyDependencies)
            {
                var property = d.Key;

                var assignment = Expression.Assign(
                    Expression.MakeMemberAccess(temp, property),
                    Expression.Convert(
                        Expression.Call(
                            Expression.Constant(d.Value.Target.GetDescriptor().Activator),
                            ActivatorInvokeMethod,
                            compositionContextParameter,
                            operationParameter),
                        property.PropertyType));

                propertySetters.Add(assignment);
            }

            var queuePropertySetting = Expression.Call(
                operationParameter,
                AddNonPrerequisiteActionMethod,
                Expression.Lambda<Action>(Expression.Block(propertySetters)));

            return Expression.Block(
                new [] { temp },
                Expression.Assign(temp, Expression.Convert(activatorBody, partType)),
                queuePropertySetting,
                Expression.Convert(temp, typeof(object)));
        }
    }
}
