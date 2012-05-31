using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestLibrary
{

    [Export]
    public class ClassWithDependecy
    {
        public TestDependency _dep;
        [ImportingConstructor]
        public ClassWithDependecy(TestDependency dep)
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
    public class TestDependency :IDependency
    {
        public TestDependency()
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
