using System;
using System.Text;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Composition.Hosting.Providers;
using System.Composition.Convention;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Composition.Demos.ComponentModelAttributeSupport;
using System.Composition.Hosting.Providers.Metadata;
using System.Composition;

namespace Microsoft.Composition.Demos.ComponentModelAttributeSupport.UnitTests
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
    }
}
