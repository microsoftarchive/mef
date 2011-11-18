using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web;
using System.ComponentModel.Composition.Web.Mvc;

[assembly: PreApplicationStartMethod(typeof(RequestCompositionScopeModule), "Register")]

