using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStyleLifetimeDemo.Parts
{
    [Export]
    public class ProductsController : IController
    {
        [ImportingConstructor]
        public ProductsController(DatabaseConnection connection, InventoryCalculator inventoryCalculator)
        {
        }

        public void Get()
        {
            Console.WriteLine("Products!");
        }
    }
}
