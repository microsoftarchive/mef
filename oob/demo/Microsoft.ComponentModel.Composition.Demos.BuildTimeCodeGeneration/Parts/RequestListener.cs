using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildTimeCodeGeneration.Parts
{
    // [Export]
    //
    public class RequestListener
    {
        Lazy<ConsoleLog> _log;
        
        // [ImportingConstructor]
        //
        public RequestListener(Lazy<ConsoleLog> log)
        {
            _log = log;
        }

        public void HandleRequest()
        {
            _log.Value.Write("Request incoming...");
        }
    }
}
