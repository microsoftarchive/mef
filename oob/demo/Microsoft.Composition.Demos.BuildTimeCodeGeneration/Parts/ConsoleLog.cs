using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildTimeCodeGeneration.Parts
{
    // [Export, Shared]
    //
    public class ConsoleLog : IDisposable
    {
        public void Write(string message)
        {
            Console.WriteLine(DateTime.Now + ": " + message);
        }

        public void Dispose()
        {
            Console.WriteLine("Disposing console log.");
        }
    }
}
