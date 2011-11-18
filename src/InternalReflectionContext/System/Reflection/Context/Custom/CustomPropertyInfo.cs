// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection.Context.Projection;

namespace System.Reflection.Context.Custom
{
    internal class CustomPropertyInfo : ProjectingPropertyInfo
    {
        private readonly CustomReflectionContext _context;

        public CustomPropertyInfo(PropertyInfo template, CustomReflectionContext context)
            : base(template, context.Projector)
        {
            _context = context;
        }

        public CustomReflectionContext ReflectionContext
        {
            get { return _context; }
        }

        // Currently only the results of GetCustomAttributes can be customizaed.
        // We don't need to override GetCustomAttributesData.
        #region ICustomAttributeProvider implementation
        public override object[] GetCustomAttributes(bool inherit)
        {
            return GetCustomAttributes(typeof(object), inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return AttributeUtils.GetCustomAttributes(ReflectionContext, this, attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return AttributeUtils.IsDefined(this, attributeType, inherit);
        }
        #endregion
    }
}
