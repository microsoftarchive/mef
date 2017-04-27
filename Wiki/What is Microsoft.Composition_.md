[Microsoft.Composition](http://nuget.org/packages/microsoft.composition) is an update for MEF that is optimized for web and Windows 8 Store apps. It is distributed as a NuGet package compatible with the .NET 4.5 Framework and .NET for Windows Store apps.

These environments do not emphasize in-place third party extensibility, and so composition in these environments can be simplified and streamlined.

Microsoft.Composition includes a lifetime model oriented towards the 'unit of work' patterns that appear in server-side and MVVM-style applications.

Throughput under server workloads is several orders of magnitude higher than what can be achieved with CompositionContainer, and with almost zero contention.

## Getting Started

First, install the [Microsoft.Composition NuGet package](http://nuget.org/packages/microsoft.composition) using the _Manage NuGet Packages_ dialog in Visual Studio, or from the _Package Manager Console_:

{{
PM> Install-Package Microsoft.Composition -Pre
}}

This will add a number of assemblies to the project.

**Parts** for Microsoft.Composition use the familiar MEF attributes, found in the {{System.Composition}} namespace.

{{
[Export(typeof(IMessageHandler))](Export(typeof(IMessageHandler)))
public class MessageHandler : IMessageHandler
{
    [ImportingConstructor](ImportingConstructor)
    public MessageHandler(IDatabaseConnection connection)
    {
    }
}
}}

The **container** is created using a simple {{ContainerConfiguration}} class.

{{
var configuration = new ContainerConfiguration()
    .WithAssembly(typeof(MessageHandler).Assembly);

using (var container = configuration.CreateContainer())
{
    var greeter = container.GetExport<IMessageHandler>();
    greeter.Greet();
}
}}

{{ContainerConfiguration}} provides the methods {{WithPart()}} and {{WithAssembly()}} as well as the {{WithProvider()}} method for plugging in container extensions.

**Conventions** are defined in Microsoft.Composition using {{ConventionBuilder}}:

{{
var conventions = new ConventionBuilder();
conventions.ForTypesDerivedFrom<IMessageHandler>().Export();
}}

The {{WithPart()}} and {{WithAssembly()}} configuration methods accept conventions as parameters:

{{
configuration.WithPart<MessageHandler>(conventions);
}}

{{ConventionBuilder}} supports the same syntax and rules as {{RegistrationBuilder}} does in the full-framework version of MEF. The BCL Team Blog discusses how to [define](http://blogs.msdn.com/b/bclteam/archive/2011/11/01/getting-started-with-convention-based-part-registration-in-mef-version-2.aspx) and [override](http://blogs.msdn.com/b/bclteam/archive/2011/11/03/overriding-part-registration-conventions-with-the-mef-attributes-nick.aspx) conventions.

If you are familiar with MEF in the .NET Framework 4.0 and onwards, see the [migration guide](MetroChanges) for further information.