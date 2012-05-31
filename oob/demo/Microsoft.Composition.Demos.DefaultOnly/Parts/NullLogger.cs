using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Composition.Demos.DefaultOnly.Extension;

namespace Microsoft.Composition.Demos.DefaultOnly.Parts
{
    [DefaultExport(typeof(ILogger))]
    public class NullLogger : ILogger
    {
        public void Write(string message)
        {
        }
    }
}
