using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStyleLifetimeDemo.Parts
{
    [Export]
    public class HomeController : IController
    {
        public void Get()
        {
            Console.WriteLine("Home!");
        }
    }
}
