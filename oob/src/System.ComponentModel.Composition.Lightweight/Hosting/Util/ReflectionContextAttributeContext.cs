// -----------------------------------------------------------------------
// Copyright © 2012 Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Lightweight.Util
{
    class ReflectionContextAttributeContext : IAttributeContext
    {
        ReflectionContext _reflectionContext;

        public ReflectionContextAttributeContext(ReflectionContext reflectionContext)
        {
            _reflectionContext = reflectionContext;
        }

        Type MapType(Type type)
        {
            return _reflectionContext.MapType(type.GetTypeInfo());
        }

        ConstructorInfo MapConstructor(ConstructorInfo constructor)
        {
            return MapType(constructor.DeclaringType).GetConstructor(constructor.GetParameters().Select(pi => pi.ParameterType).ToArray());
        }

        T MapMemberOrDefault<T>(MemberInfo member, Func<MemberInfo, T> mapping)
        {
            const BindingFlags instanceBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            const BindingFlags declaredInstanceBindingFlags = instanceBindingFlags | BindingFlags.DeclaredOnly;

            MemberInfo mapped = null;
            if (member.MemberType == MemberTypes.TypeInfo || member.MemberType == MemberTypes.NestedType)
            {
                mapped = MapType((Type)member);
            }
            else
            {
                var mappedType = MapType(member.DeclaringType);
                if (member.MemberType == MemberTypes.Constructor)
                {
                    mapped = mappedType.GetConstructor(
                        instanceBindingFlags,
                        null,
                        ((ConstructorInfo)member).GetParameters().Select(pi => pi.ParameterType).ToArray(),
                        null);
                }
                else if (member.MemberType == MemberTypes.Property)
                {
                    var inspectedType = mappedType;
                    while (mapped == null && inspectedType != null)
                    {
                        mapped = inspectedType.GetProperty(member.Name, declaredInstanceBindingFlags);
                        inspectedType = inspectedType.BaseType;
                    }
                }
            }

            if (mapped == null)
                return default(T);

            return mapping(mapped);
        }

        public TAttribute GetDeclaredAttribute<TAttribute>(MemberInfo member) where TAttribute : Attribute
        {
            return MapMemberOrDefault(member, m => m.GetCustomAttribute<TAttribute>());
        }

        public object[] GetDeclaredAttributes(MemberInfo member)
        {
            return MapMemberOrDefault(member, m => m.GetCustomAttributes(false));
        }

        public TAttribute[] GetDeclaredAttributes<TAttribute>(MemberInfo member) where TAttribute : Attribute
        {
            return MapMemberOrDefault(member, m => m.GetCustomAttributes<TAttribute>().ToArray());
        }

        T MapParameterOrDefault<T>(ParameterInfo parameter, Func<ParameterInfo, T> mapping)
        {
            return MapMemberOrDefault(parameter.Member, m => mapping(((MethodBase)m).GetParameters()[parameter.Position]));
        }

        public TAttribute GetDeclaredAttribute<TAttribute>(ParameterInfo parameter) where TAttribute : Attribute
        {
            return MapParameterOrDefault(parameter, m => m.GetCustomAttribute<TAttribute>());
        }

        public object[] GetDeclaredAttributes(ParameterInfo parameter)
        {
            return MapParameterOrDefault(parameter, m => m.GetCustomAttributes(false));
        }

        public TAttribute[] GetDeclaredAttributes<TAttribute>(ParameterInfo parameter) where TAttribute : Attribute
        {
            return MapParameterOrDefault(parameter, m => m.GetCustomAttributes<TAttribute>().ToArray());
        }
    }
}
