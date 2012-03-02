using System;
using System.Reflection.Context.Custom;
using System.Collections.Generic;
using System.Reflection.Context.Virtual;
using System.Reflection.Context.Projection;
using System.Diagnostics.Contracts;
using System.Reflection.Context;

namespace System.Reflection.Context
{
    public abstract partial class CustomReflectionContext : ReflectionContext
    {
        private ReflectionContext _sourceContext;
        private ReflectionContextProjector _projector;

        #region constructors
        protected CustomReflectionContext()
        {
            _projector = new ReflectionContextProjector(this);
        }

        protected CustomReflectionContext(ReflectionContext source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            _sourceContext = source;
            _projector = new ReflectionContextProjector(this);
        }
        #endregion

        #region ReflectionContext overrides
        public override Assembly MapAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            return _projector.ProjectAssemblyIfNeeded(assembly);
        }

        public override Type MapType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return _projector.ProjectTypeIfNeeded(type);
        }
        #endregion

        #region public APIs
        protected virtual IEnumerable<object> GetCustomAttributes(MemberInfo member, IEnumerable<object> declaredAttributes)
        {
            return declaredAttributes;
        }

        protected virtual IEnumerable<object> GetCustomAttributes(ParameterInfo parameter, IEnumerable<object> declaredAttributes)
        {
            return declaredAttributes;
        }

        // The default implementation of GetProperties: just return an empty list.
        protected virtual IEnumerable<PropertyInfo> AddProperties(Type type)
        {
            // return an empty enumeration
            yield break;
        }

        protected PropertyInfo CreateProperty(
            Type propertyType,
            string name,
            Func<object, object> getter,
            Action<object, object> setter)
        {
            return new VirtualPropertyInfo(
                name,
                propertyType,
                getter,
                setter,
                null,
                null,
                null,
                this);
        }

        protected PropertyInfo CreateProperty(
            Type propertyType,
            string name,
            Func<object, object> getter,
            Action<object, object> setter,
            IEnumerable<Attribute> propertyCustomAttributes,
            IEnumerable<Attribute> getterCustomAttributes,
            IEnumerable<Attribute> setterCustomAttributes)
        {
            return new VirtualPropertyInfo(
                name,
                propertyType,
                getter,
                setter,
                propertyCustomAttributes,
                getterCustomAttributes,
                setterCustomAttributes,
                this);
        }
        #endregion

        #region internal/private APIs
        internal IEnumerable<PropertyInfo> GetNewPropertiesForType(CustomType type)
        {
            // We don't support adding properties on these types.
            if (type.IsInterface || type.IsGenericParameter || type.HasElementType)
                yield break;

            // Passing in the underlying type.
            IEnumerable<PropertyInfo> newProperties = AddProperties(type.UnderlyingType);

            // Setting DeclaringType on the user provided virtual properties.
            foreach (PropertyInfo prop in newProperties)
            {
                if (prop == null)
                    throw new InvalidOperationException(SR.GetString(SR.InvalidOperation_AddNullProperty));

                VirtualPropertyBase vp = prop as VirtualPropertyBase;
                if (vp == null || vp.ReflectionContext != this)
                    throw new InvalidOperationException(SR.GetString(SR.InvalidOperation_AddPropertyDifferentContext));

                if (vp.DeclaringType == null)
                    vp.SetDeclaringType(type);
                else if (!vp.DeclaringType.Equals(type))
                    throw new InvalidOperationException(SR.GetString(SR.InvalidOperation_AddPropertyDifferentType));

                yield return prop;
            }
        }

        internal IEnumerable<object> GetCustomAttributesOnMember(MemberInfo member, IEnumerable<object> declaredAttributes, Type attributeFilterType)
        {
            IEnumerable<object> attributes = GetCustomAttributes(member, declaredAttributes);
            return AttributeUtils.FilterCustomAttributes(attributes, attributeFilterType);
        }

        internal IEnumerable<object> GetCustomAttributesOnParameter(ParameterInfo parameter, IEnumerable<object> declaredAttributes, Type attributeFilterType)
        {
            IEnumerable<object> attributes = GetCustomAttributes(parameter, declaredAttributes);
            return AttributeUtils.FilterCustomAttributes(attributes, attributeFilterType);
        }

        internal Projector Projector
        {
            get { return _projector; }
        }

        internal ReflectionContext SourceContext
        {
            get { return _sourceContext; }
        }
        #endregion
    }
}
