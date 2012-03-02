using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace System.ComponentModel.Composition.Registration
{
    // This class exists to enable configuration of PartBuilder<T>
    public class ParameterImportBuilder
    {
        public T Import<T>()
        {
            return default(T);
        }
        
        public T Import<T>(Action<ImportBuilder> configure) 
        {
            return default(T);
        }
    }
}
