using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Lightweight.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DesktopStyleLifetimeDemo.Parts;

namespace DesktopStyleLifetimeDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var conventions = new RegistrationBuilder();
            conventions.ForTypesMatching(t => t.Namespace == typeof(Application).Namespace)
                .Export();

            var configuration = new ContainerConfiguration()
                .WithDefaultConventions(conventions)
                .WithAssembly(typeof(Program).Assembly);

            using (var container = configuration.CreateContainer())
            {
                var application = container.Value.GetExport<Application>();
                application.Run("SuperIDE");
            }

            Console.ReadKey();
        }
    }
}
