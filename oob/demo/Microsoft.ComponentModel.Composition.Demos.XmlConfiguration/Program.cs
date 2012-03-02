using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Lightweight.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlConfigurationDemo.Extension;
using XmlConfigurationDemo.Parts;

namespace XmlConfigurationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ContainerConfiguration()
                .WithPartsFromXml();

            using (var container = configuration.CreateContainer())
            {
                var w = container.Value.GetExport<Window>();
                w.Show();
            }

            Console.ReadKey(true);
        }
    }
}
