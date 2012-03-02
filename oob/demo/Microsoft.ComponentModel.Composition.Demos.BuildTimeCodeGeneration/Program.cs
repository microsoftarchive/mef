using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Lightweight.Hosting;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildTimeCodeGeneration.Generated;
using BuildTimeCodeGeneration.Parts;

namespace BuildTimeCodeGeneration
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ContainerConfiguration()
                .WithProvider(new BuildTimeCodeGeneration_ExportDescriptorProvider());

            using (var cc = configuration.CreateContainer())
            {
                var rh = cc.Value.GetExport<RequestListener>();
                rh.HandleRequest();
            }

            Console.ReadKey(true);
        }
    }
}
