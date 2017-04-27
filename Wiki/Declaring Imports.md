# Declaring Imports
Composable Parts declare imports **{{[System.ComponentModel.Composition.ImportAttribute](System.ComponentModel.Composition.ImportAttribute)}}** attribute. Similar to exports, there are several different methods namely through Fields, Properties and Constructor Parameters.

## Property Imports
To import a value to a property, decorate the property with the **{{[System.ComponentModel.Composition.ImportAttribute](System.ComponentModel.Composition.ImportAttribute)}}**. For example the snippet below imports an IMessageSender

{code:c#}
  class Program
  {
    [Import](Import)
    public IMessageSender MessageSender { get; set; }
  }
{code:c#}
{code:vb.net}
Class Program
    <Import()>
    Public Property MessageSender() As IMessageSender
End Class
{code:vb.net}


## Constructor Parameters
You can also specify imports through constructor parameters. This means that instead of adding properties for each import, you add parameters to a constructor for each import. To use this, follow the following steps.

1. Add a **{{[System.ComponentModel.Composition.ImportingConstructorAttribute](System.ComponentModel.Composition.ImportingConstructorAttribute)}}** attribute to the constructor that should be used by MEF. 
2. Add parameters to the constructor for each import.

For example the code below imports a message sender in the constructor of the Program class.

{code:c#}
  class Program
  {
    [ImportingConstructor](ImportingConstructor)
    public Program(IMessageSender messageSender) 
    {
       ...
    }
  }
{code:c#}
{code:vb.net}
Class Program
    <ImportingConstructor()>
    Public Sub New(ByVal messageSender As IMessageSender) 
      ... 
    End Sub
End Class
{code:vb.net}

**Parameter imports**

There are several different different ways to define imports on the constructor.

1. Implied import - By default the container will use the type of the parameter to identify the contract. For example in the code below, the IMessageSender contract will be used.

{code:c#}
  class Program
  {
    [ImportingConstructor](ImportingConstructor)
    public Program(IMessageSender messageSender) 
    {
    }
  }
{code:c#}
{code:vb.net}
Class Program
    <ImportingConstructor()>
    Public Sub New(ByVal messageSender As IMessageSender) 
    End Sub
End Class
{code:vb.net}

2. Explicit import - If you want to specify the contract to be imported add an **{{[System.ComponentModel.Composition.ImportAttribute](System.ComponentModel.Composition.ImportAttribute)}}** attribute to the parameter. 

## Field Imports
MEF also supports importing values directly to fields.

{code:c#}
  class Program
  {
    [Import](Import)
    private IMessageSender _messageSender;
  }
{code:c#}
{code:vb.net}
Class Program
    <Import()>
    Private _messageSender As IMessageSender
End Class
{code:vb.net}

_Note: note that importing or exporting private members (fields, properties and methods) while supported in full trust is likely to be problematic on medium/partial trust._

## Optional imports
MEF allows you to specify that an import is optional. When you enable this, the container will provide an export if one is available otherwise it will set the import to {{Default(T)}}. To make an import optional, set {{AllowDefault=true}} on the import as below.

{code:c#}
[Export](Export)
public class OrderController {
  private ILogger _logger;

  [ImportingConstructor](ImportingConstructor)
  public OrderController([Import(AllowDefault=true)](Import(AllowDefault=true)) ILogger logger) {
    if(logger == null)
      logger = new DefaultLogger();
    _logger = logger;
  }
}
{code:c#}
{code:vb.net}
<Export()>
Public Class OrderController
    Private _logger As ILogger

    <ImportingConstructor()>
    Public Sub New(<Import(AllowDefault:=True)> ByVal logger As ILogger) 
        If logger Is Nothing Then
            logger = New DefaultLogger()
        End If
        _logger = logger
    End Sub
End Class
{code:vb.net}

OrderController optionally imports a logger. If the logger is not present, it will set it's private {{_logger}} to a new {{DefaultLogger}} instance otherwise it will use the imported logger.
## Importing collections
In addition to single imports, you can import collections with the ImportMany attribute. This means that all instances of the specific contract will be imported from the container.

MEF parts can also support recomposition. This means that as new exports become available in the container, collections are automatically updated with the new set. For example below the Notifier class imports a collection of IMessageSender. This means if there are 3 exports of IMessageSender available in the container, they will be pushed in to the Senders property during compositon. 

{code:c#}
 public class Notifier 
 {
    [ImportMany(AllowRecomposition=true)](ImportMany(AllowRecomposition=true))
    public IEnumerable<IMessageSender> Senders {get; set;}

    public void Notify(string message) 
    {
      foreach(IMessageSender sender in Senders)
      {
        sender.Send(message);
      }
    } 
  }
{code:c#}
{code:vb.net}
Public Class Notifier
    <ImportMany(AllowRecomposition:=True)> 
    Public Property Senders() As IEnumerable(Of IMessageSender) 

    Public Sub Notify(ByVal message As String) 
        For Each sender As IMessageSender In Senders
            sender.Send(message) 
        Next sender
    End Sub
End Class
{code:vb.net}

## IPartImportsSatisfiedNotification
In some situations it may be important for your class to be notified when MEF is done with the import process for your class instance. If that's the case implement the **{{[System.ComponentModel.Composition.IPartImportsSatisfiedNotification](System.ComponentModel.Composition.IPartImportsSatisfiedNotification)}}** interface. This interface has only a single method: OnImportsSatisfied, which is called when all imports that could be satisfied have been satisfied.

{code:c#}
 public class Program : IPartImportsSatisfiedNotification
 {
    [ImportMany](ImportMany)
    public IEnumerable<IMessageSender> Senders {get; set;}

    public void OnImportsSatisfied() 
    {
      // when this is called, all imports that could be satisfied have been satisfied.
    } 
  }
{code:c#}
{code:vb.net}
Public Class Program
    Implements IPartImportsSatisfiedNotification
    <ImportMany()>
    Public Property Senders() As IEnumerable(Of IMessageSender) 

    Public Sub OnImportsSatisfied() Implements IPartImportsSatisfiedNotification.OnImportsSatisfied
        ' when this is called, all imports that could be satisfied have been satisfied. 
    End Sub
End Class
{code:vb.net}

