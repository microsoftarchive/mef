using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Composition.Demos.ExportUnrecognizedConcreteTypes.Parts
{
    [Export]
    class MainForm
    {
        [Import]
        public CustomersView Customers { get; set; }
    }
}
