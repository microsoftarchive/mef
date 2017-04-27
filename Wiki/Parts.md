# Defining Composable Parts and Contracts
{anchor:Parts}
## Composable Parts
A Composable Part is a composable unit within MEF. Composable Parts export services that other Composable Parts need, and import services from other Composable Parts. In the MEF programming model, Composable Parts are attributed with the **{{System.ComponentModel.Composition.Import}}** and **{{[System.ComponentModel.Composition.Export](System.ComponentModel.Composition.Export)}}** attribute in order to declare their exports and imports. A Composable Part should contain at least one export. Composable Parts are either added to the container explicity or created through the use of catalogs. The default catalogs that MEF ship with identify Composable Parts through the presence of an export attribute.
{anchor:Contracts}
## Contracts
Composable Parts do not directly depend on one another, instead they depend on a contract, which is a string identifier. Every export has a contract, and every import declares the contract it needs. The container uses the contract information to match up imports to exports. If no contract is specified, MEF will implicitly use the fully qualified name of the type as the contract. If a type is passed, it will also use the fully qualified name. 

_Note: By default a type should be passed for a contract, and not a string. Although contracts can be an arbitrary string this can lead to ambiguity. For example "Sender" might overlap with another implementation of "Sender" in a different library. For this reason if you do need to specify a string constract, it is recommend that contract names should be qualified with a namespace that includes the Company Name for example "Contoso.Exports.Sender"._

In the code snippet below, all export contracts are equivalent.

{code:c#}
namespace MEFSample 
{
  [Export](Export)
  public class Exporter {...}

  [Export(typeof(Exporter))](Export(typeof(Exporter)))
  public class Exporter1 {...}

  [Export("MEFSample.Exporter")](Export(_MEFSample.Exporter_))
  public class Exporter2 {...}
}
{code:c#}
{code:vb.net}
Namespace MEFSample
    <Export()>
    Public Class Exporter
        ... 
    End Class
    <Export(GetType(Exporter))> 
    Public Class Exporter1
        ... 
    End Class
    <Export("MEFSample.Exporter")>
    Public Class Exporter2
        ... 
    End Class
End Namespace
{code:vb.net}

**Interface / Abstract contracts**

A common pattern is for a Composable Part to export an interface or an abstract type contract rather than a concrete type. This allows the importer to be completely decoupled from the specific implementation of the export it is importing resulting in a separation of concerns. For example below you can see there are two sender implementations that both export **{{IMessageSender}}**. The **{{Notifier}}** class imports a collection of **{{IMessageSender}}** which it invokes in its **{{Send()}}** method. New message senders can now easily be added to the system.

{code:c#}
  [Export(typeof(IMessageSender))](Export(typeof(IMessageSender)))
  public class EmailSender : IMessageSender {
    ...
  }

  [Export(typeof(IMessageSender))](Export(typeof(IMessageSender)))  
  public class TCPSender : IMessageSender {
    ...
  }

  public class Notifier {
    [ImportMany](ImportMany)
    public IEnumerable<IMessageSender> Senders {get; set;}
    public void Notify(string message) {
      foreach(IMessageSender sender in Senders) 
        sender.Send(message);
    } 
  }
{code:c#}

{code:vb.net}
<Export(GetType(IMessageSender))> 
Public Class EmailSender
    Implements IMessageSender
    ... 
End Class

<Export(GetType(IMessageSender))> 
Public Class TCPSender
    Implements IMessageSender
    ... 
End Class

Public Class Notifier
    <ImportMany()>
    Public Property Senders() As IEnumerable(Of IMessageSender) 
    Public Sub Notify(ByVal message As String) 
        For Each sender As IMessageSender In Senders
            sender.Send(message) 
        Next sender
    End Sub
End Class
{code:vb.net}

## Contract Assemblies
A common pattern when building extensible applications with MEF is to deploy a contract assembly. A contract assembly is simply an assembly which contains contract types that extenders can use for extending your app. Commonly these will be interfaces, but they may be abstract classes. Additonally contract assemblies will likely contain metadata view interfaces that importers will use, as well as any custom MEF export attributes.
_Note: You must specify the specific interface type (**{{IMessageSender}}**) being exported otherwise the type (**{{EmailSender}}**) itself will be exported._