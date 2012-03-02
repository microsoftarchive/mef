using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ComponentModel.Composition.Demos.DefaultOnly.Extension;
using Microsoft.ComponentModel.Composition.Demos.DefaultOnly.Parts;

namespace Microsoft.ComponentModel.Composition.Demos.DefaultOnly.Parts
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ContainerConfiguration()
                .WithAssembly(typeof(Program).Assembly)
                .WithProvider(new DefaultExportDescriptorProvider());

            using (var container = configuration.CreateContainer())
            {
                var greeter = container.Value.GetExport<Greeter>();
                greeter.Greet();
            }

            Console.ReadKey();
        }
    }
}
