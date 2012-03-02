using System;

namespace System.Reflection.Context.Projection
{
    // TODO: Temporary interface until we can add ReflectionContext to Type/Assembly
    internal interface IProjectable
    {
        Projector Projector
        {
            get;
        }
    }
}
