// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection.Context.Projection;

namespace System.Reflection.Context.Custom
{
    internal class CustomModule : ProjectingModule
    {
        private readonly CustomReflectionContext _context;

        public CustomModule(Module template, CustomReflectionContext context)
            : base(template, context.Projector)
        {
            _context = context;
        }

        public CustomReflectionContext ReflectionContext
        {
            get { return _context; }
        }
    }
}
