using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Lightweight.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ComponentModel.Composition.Demos.ExportUnrecognizedConcreteTypes.Extension;
using Microsoft.ComponentModel.Composition.Demos.ExportUnrecognizedConcreteTypes.Parts;

namespace Microsoft.ComponentModel.Composition.Demos.ExportUnrecognizedConcreteTypes
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ContainerConfiguration()
                .WithPart<MainForm>()
                .WithProvider(new UnrecognizedConcreteTypeSource());

            using (var container = configuration.CreateContainer())
            {
                var form = container.Value.GetExport<MainForm>();
                form.Customers.Render();
            }

            Console.ReadKey();
        }
    }
}
