using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ComponentModel.Composition.Demos.ExportUnrecognizedConcreteTypes.Parts
{
    [Export]
    class MainForm
    {
        [Import]
        public CustomersView Customers { get; set; }
    }
}
