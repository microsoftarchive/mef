using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using TestLibrary;
using System.ComponentModel.Composition.Lightweight.Hosting;
using System.ComponentModel.Composition.Lightweight.Hosting.Providers;
using System.ComponentModel.Composition.Registration;

namespace System.ComponentModel.Composition.Lightweight.UnitTests
{
    public class ContainerTests
    {
        protected static IExportProvider CreateContainer(params Type[] types)
        {
            return new ContainerConfiguration()
                .WithParts(types)
                .CreateContainer()
                .Value;
        }

        protected static IExportProvider CreateContainer(RegistrationBuilder rb, params Type[] types)
        {
            return new ContainerConfiguration()
                .WithParts(types)
                .WithDefaultConventions(rb)
                .CreateContainer()
                .Value;
        }
    }
}
