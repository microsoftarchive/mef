using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ComponentModel.Composition.Demos.DefaultOnly.Extension;

namespace Microsoft.ComponentModel.Composition.Demos.DefaultOnly.Parts
{
    [DefaultExport(typeof(IMessage))]
    public class DefaultMessage : IMessage
    {
        public string Current
        {
            get { return "Hello, World"; }
        }
    }
}
