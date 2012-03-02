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
    interface IAttributeContext
    {
        TAttribute GetDeclaredAttribute<TAttribute>(MemberInfo member) where TAttribute : Attribute;
        object[] GetDeclaredAttributes(MemberInfo member);
        TAttribute[] GetDeclaredAttributes<TAttribute>(MemberInfo member) where TAttribute : Attribute;

        TAttribute GetDeclaredAttribute<TAttribute>(ParameterInfo parameter) where TAttribute : Attribute;
        object[] GetDeclaredAttributes(ParameterInfo parameter);
        TAttribute[] GetDeclaredAttributes<TAttribute>(ParameterInfo parameter) where TAttribute : Attribute;
    }
}
