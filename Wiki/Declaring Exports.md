# Declaring Exports
Composable Parts declare exports through the **{{[System.ComponentModel.Composition.ExportAttribute](System.ComponentModel.Composition.ExportAttribute)}}** attribute. In MEF there are several different ways to declare exports including at the Part level, and through Properties and Methods. 

## Composable Part exports
A Composable Part level export is used when a Composable Part needs to export itself. In order for a Composable Part to export itself, simply decorate the Composable Part with a **{{[System.ComponentModel.Composition.ExportAttribute](System.ComponentModel.Composition.ExportAttribute)}}** attribute as is shown in the code snippet below.

{code:c#}
[Export](Export)
public class SomeComposablePart {
  ...
}
{code:c#}
{code:vb.net}
<Export()>
Public Class SomeComposablePart
    ... 
End Class
{code:vb.net}

## Property exports

Parts can also export properties. Property exports are advantageous for several reasons. 
* They allow exporting sealed types such as the core CLR types, or other third party types. 
* They allow decoupling the export from how the export is created. For example exporting the existing HttpContext which the runtime creates for you.
* They allow having a family of related exports in the same Composable Part, such as a DefaultSendersRegistry Composable Part that exports a default set of senders as properties.
For example you might have a Configuration class that exports an integer with a "Timeout" contract as in the example below.

{code:c#}
  public class Configuration
  {
    [Export("Timeout")](Export(_Timeout_))
    public int Timeout
    {
      get { return int.Parse(ConfigurationManager.AppSettings["Timeout"](_Timeout_)); }
    }
  }
  [Export](Export)
  public class UsesTimeout
  {
    [Import("Timeout")](Import(_Timeout_))
    public int Timeout { get; set; }
  }
{code:c#}
{code:vb.net}
Public Class Configuration
    <Export("Timeout")>
    Public ReadOnly Property Timeout() As Integer
        Get
            Return Integer.Parse(ConfigurationManager.AppSettings("Timeout"))
        End Get
    End Property
End Class
<Export()>
Public Class UsesTimeout
    <Import("Timeout")>
    Public Property Timeout() As Integer
End Class
{code:vb.net}

## Method exports
A method export is where a Part exports one its methods. Methods are exported as delegates which are specified in the Export contract. Method exports have several benefits including the following.
* They allow finer grained control as to what is exported. For example, a rules engine might import a set of pluggable method exports.
* They shield the caller from any knowledge of the type.
* They can be generated through light code gen, which you cannot do with the other exports.
_Note: Method exports may have no more than 4 arguments due to a framework limitation._

In the example below, the **{{MessageSender}}** class exports its **{{Send}}** method as an **{{Action<string>}}** delegate. The Processor imports the same delegate.

{code:c#}
  public class MessageSender
  {
    [Export(typeof(Action<string>))](Export(typeof(Action_string_)))
    public void Send(string message)
    {
      Console.WriteLine(message);
    }
  }

  [Export](Export)
  public class Processor
  {
    [Import(typeof(Action<string>))](Import(typeof(Action_string_)))
    public Action<string> MessageSender { get; set; }

    public void Send()
    {
      MessageSender("Processed");
    }
  }
{code:c#}

{code:vb.net}
Public Class MessageSender
    <Export(GetType(Action(Of String)))> 
    Public Sub Send(ByVal message As String) 
        Console.WriteLine(message) 
    End Sub
End Class

<Export()>
Public Class Processor
    <Import(GetType(Action(Of String)))> 
    Public Property MessageSender() As Action(Of String) 

    Public Sub Send()
        MessageSender()("Processed")
    End Sub
End Class
{code:vb.net}

You can also export and import methods by using a simple string contract. For example below the "Sender" contract is used. 

{code:c#}
  public class MessageSender
  {
    [Export("MessageSender")](Export(_MessageSender_))
    public void Send(string message)
    {
      Console.WriteLine(message);
    }
  }

  [Export](Export)
  public class Processor
  {
    [Import("MessageSender")](Import(_MessageSender_))
    public Action<string> MessageSender { get; set; }

    public void Send()
    {
      MessageSender("Processed");
    }
  }
{code:c#}
{code:vb.net}
Public Class MessageSender
    <Export("MessageSender")>
    Public Sub Send(ByVal message As String) 
        Console.WriteLine(message) 
    End Sub
End Class

<Export()>
Public Class Processor
    <Import("MessageSender")>
    Public Property MessageSender() As Action(Of String) 

    Public Sub Send()
        MessageSender()("Processed")
    End Sub
End Class
{code:vb.net}

_Note: When doing method exports, you are required to either provide a type or a string contract name, and cannot leave it blank._

## Inherited Exports
MEF supports the ability for a base class / interface to define exports which are automatically inherited by implementers. This is ideal for integration with legacy frameworks which want to take advantage of MEF for discovery but do not want to require modifying existing customer code. In order to provide this capability use the {{System.ComponentModel.Composition.InheritedExportAttribute}}. For example below {{ILogger}} has an {{InheritedExport}}. Logger implements ILogger thus it automatically exports ILogger.

{code:c#}
[InheritedExport](InheritedExport)
public interface ILogger {
  void Log(string message);
}

public class Logger : ILogger {
  public void Log(string message);
}
{code:c#}
{code:vb.net}
<InheritedExport()>
Public Interface ILogger
    Sub Log(ByVal message As String) 
End Interface

Public Class Logger
    Implements ILogger
    Public Sub Log(ByVal message As String) Implements ILogger.Log

    End Sub

End Class
{code:vb.net}

## Discovering non-public Composable Parts
MEF supports discovery of public and non-public Parts. You don't need to do anything to enable this behavior. Please note that in medium/partial trust environments (including Silverlight) non-public composition will not be supported.
