using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{

    [Export]
    public class ClassWithDependecy
    {
        public Dependency _dep;
        [ImportingConstructor]
        public ClassWithDependecy(Dependency dep)
        {
            _dep = dep;
        }
    }

    [Export]
    public class ClassWithDependecyAndSameBaseType
    {
        public IDependency _dep;
        [ImportingConstructor]
        public ClassWithDependecyAndSameBaseType(IDependency dep)
        {
            _dep = dep;
        }
    }


    [Export]
    [Export(typeof(IDependency))]
    public class Dependency :IDependency
    {
        public Dependency()
        {
        }
    }

    public class NotRealDependency : IDependency
    {
        public NotRealDependency()
        {
        }
    }


    public interface IDependency
    {
    }
}
