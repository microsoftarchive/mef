using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebStyleLifetimeDemo.Parts;

namespace WebStyleLifetimeDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ContainerConfiguration()
                .WithAssembly(typeof(Program).Assembly);

            using (var container = configuration.CreateContainer())
            {
                var ws = container.GetExport<WebServer>();
                ws.Get("products");
                ws.Get("home");
            }

            Console.ReadKey();
        }
    }
}
