using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel.Composition.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ApplicationSharedAttribute : Attribute
    {
    }
}
