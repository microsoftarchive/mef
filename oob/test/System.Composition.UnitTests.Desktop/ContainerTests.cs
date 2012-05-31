using System;
using System.Text;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Composition.Hosting.Providers;
using System.Composition.Convention;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestLibrary;
using System.Composition.ComponentModelAttributeSupport;

namespace System.Composition.UnitTests
{
    public class ContainerTests
    {
        protected static CompositionContext CreateContainer(params Type[] types)
        {
            return new ContainerConfiguration()
                .WithParts(types)
                .WithProvider(new ComponentModelMetadataViewProvider())
                .CreateContainer();
        }

        protected static CompositionContext CreateContainer(ConventionBuilder rb, params Type[] types)
        {
            return new ContainerConfiguration()
                .WithParts(types)
                .WithDefaultConventions(rb)
                .WithProvider(new ComponentModelMetadataViewProvider())
                .CreateContainer();
        }
    }
}
