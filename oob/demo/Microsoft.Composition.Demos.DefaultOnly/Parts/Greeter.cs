using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Composition.Demos.DefaultOnly.Parts
{
    [Export]
    public class Greeter
    {
        IMessage _message;
        ILogger _logger;

        [ImportingConstructor]
        public Greeter(IMessage message, ILogger logger)
        {
            _message = message;
            _logger = logger;
        }

        public void Greet()
        {
            _logger.Write(_message.Current);
        }
    }
}
