using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStyleLifetimeDemo.Parts
{
    [Export]
    public class InventoryCalculator
    {
        [ImportingConstructor]
        public InventoryCalculator(DatabaseConnection connection) { }
    }
}
