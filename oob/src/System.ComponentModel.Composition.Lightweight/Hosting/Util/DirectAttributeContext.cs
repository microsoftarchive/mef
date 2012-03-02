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
    class DirectAttributeContext : IAttributeContext
    {
        public TAttribute GetDeclaredAttribute<TAttribute>(MemberInfo member) where TAttribute : Attribute
        {
            return member.GetCustomAttribute<TAttribute>();
        }

        public object[] GetDeclaredAttributes(MemberInfo member)
        {
            return member.GetCustomAttributes(false);
        }

        public TAttribute[] GetDeclaredAttributes<TAttribute>(MemberInfo member) where TAttribute : Attribute
        {
            return member.GetCustomAttributes<TAttribute>().ToArray();
        }

        public TAttribute GetDeclaredAttribute<TAttribute>(ParameterInfo parameter) where TAttribute : Attribute
        {
            return parameter.GetCustomAttribute<TAttribute>();
        }

        public object[] GetDeclaredAttributes(ParameterInfo parameter)
        {
            return parameter.GetCustomAttributes(false);
        }

        public TAttribute[] GetDeclaredAttributes<TAttribute>(ParameterInfo parameter) where TAttribute : Attribute
        {
            return parameter.GetCustomAttributes<TAttribute>().ToArray();
        }
    }
}
