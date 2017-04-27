# Exports and Metadata
[Declaring Exports](Declaring-Exports) explained the basics of parts exporting services and values. In some cases it’s necessary to associate information with exports for a variety of reasons. Commonly it’s used to explain about the capabilities of an specific implementation of a common contract. This is useful to allow imports to either constraint the export that can satisfy it, or to import all available implementations at the time and check their capabilities in runtime before using the export.

## Attaching Metadata to an Export
Consider the IMessageSender service introduced earlier. Suppose we have a few implementations, and they have differences that may be relevant to the consumer of the implementations. For our example the transport of the message and whether is secure are important information for a consumer (importer).

### Using ExportMetadataAttribute
All we have to do to attach this information is to use the **{{[System.ComponentModel.Composition.ExportMetadataAttribute](System.ComponentModel.Composition.ExportMetadataAttribute)}}**:

{code:c#}
public interface IMessageSender
{
    void Send(string message);
}

[Export(typeof(IMessageSender))](Export(typeof(IMessageSender)))
[ExportMetadata("transport", "smtp")](ExportMetadata(_transport_,-_smtp_))
public class EmailSender : IMessageSender
{
    public void Send(string message)
    {
        Console.WriteLine(message);
    }
}

[Export(typeof(IMessageSender))](Export(typeof(IMessageSender)))
[ExportMetadata("transport", "smtp")](ExportMetadata(_transport_,-_smtp_))
[ExportMetadata("secure", null)](ExportMetadata(_secure_,-null))
public class SecureEmailSender : IMessageSender
{
    public void Send(string message)
    {
        Console.WriteLine(message);
    }
}

[Export(typeof(IMessageSender))](Export(typeof(IMessageSender)))
[ExportMetadata("transport", "phone_network")](ExportMetadata(_transport_,-_phone_network_))
public class SMSSender : IMessageSender
{
    public void Send(string message)
    {
        Console.WriteLine(message);
    }
}
{code:c#}
{code:vb.net}
Public Interface IMessageSender
    Sub Send(ByVal message As String) 
End Interface

<Export(GetType(IMessageSender)), ExportMetadata("transport", "smtp")>
Public Class EmailSender
    Implements IMessageSender
    Public Sub Send(ByVal message As String) Implements IMessageSender.Send
        Console.WriteLine(message) 
    End Sub
End Class

<Export(GetType(IMessageSender)), ExportMetadata("transport", "smtp"), ExportMetadata("secure", Nothing)> 
Public Class SecureEmailSender
    Implements IMessageSender
    Public Sub Send(ByVal message As String) Implements IMessageSender.Send
        Console.WriteLine(message) 
    End Sub
End Class

<Export(GetType(IMessageSender)), ExportMetadata("transport", "phone_network")>
Public Class SMSSender
    Implements IMessageSender
    Public Sub Send(ByVal message As String) Implements IMessageSender.Send
        Console.WriteLine(message) 
    End Sub
End Class
{code:vb.net}

### Using a Custom Export Attribute
In order to do it more strongly typed than using the ExportMetadataAttribute, you need to create your own attribute and decorate it with **{{[System.ComponentModel.Composition.MetadataAttribute](System.ComponentModel.Composition.MetadataAttribute)}}**. In this example we also derive from ExportAttribute, thus creating a custom Export attribute that also specifies metadata. 

{code:c#}
[MetadataAttribute](MetadataAttribute)
[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)](AttributeUsage(AttributeTargets.Class,-AllowMultiple=false))
public class MessageSenderAttribute : ExportAttribute
{
    public MessageSenderAttribute() : base(typeof(IMessageSender)) { }
    public MessageTransport Transport { get; set; }
    public bool IsSecure { get; set; }
}

public enum MessageTransport
{
    Undefined,
    Smtp,
    PhoneNetwork,
    Other
}
{code:c#}
{code:vb.net}
<MetadataAttribute(), AttributeUsage(AttributeTargets.Class, AllowMultiple:=False)> 
Public Class MessageSenderAttribute
    Inherits ExportAttribute
    Public Sub New()
        MyBase.New(GetType(IMessageSender)) 
    End Sub
    Public Property Transport() As MessageTransport
    Public Property IsSecure() As Boolean
End Class

Public Enum MessageTransport
    Undefined
    Smtp
    PhoneNetwork
    Other
End Enum
{code:vb.net}

Above, the MetadataAttribute is applied to our custom export attribute. The next step is to apply the attribute to our IMessageSender implementations:

{code:c#}
[MessageSender(Transport=MessageTransport.Smtp)](MessageSender(Transport=MessageTransport.Smtp))
public class EmailSender : IMessageSender
{
    public void Send(string message)
    {
        Console.WriteLine(message);
    }
}

[MessageSender(Transport=MessageTransport.Smtp, IsSecure=true)](MessageSender(Transport=MessageTransport.Smtp,-IsSecure=true))
public class SecureEmailSender : IMessageSender
{
    public void Send(string message)
    {
        Console.WriteLine(message);
    }
}

[MessageSender(Transport=MessageTransport.PhoneNetwork)](MessageSender(Transport=MessageTransport.PhoneNetwork))
public class SMSSender : IMessageSender
{
    public void Send(string message)
    {
        Console.WriteLine(message);
    }
}
{code:c#}

{code:vb.net}
<MessageSender(Transport:=MessageTransport.Smtp)> 
Public Class EmailSender
    Implements IMessageSender
    Public Sub Send(ByVal message As String) Implements IMessageSender.Send
        Console.WriteLine(message) 
    End Sub
End Class

<MessageSender(Transport:=MessageTransport.Smtp, IsSecure:=True)> 
Public Class SecureEmailSender
    Implements IMessageSender
    Public Sub Send(ByVal message As String) Implements IMessageSender.Send
        Console.WriteLine(message) 
    End Sub
End Class

<MessageSender(Transport:=MessageTransport.PhoneNetwork)> 
Public Class SMSSender
    Implements IMessageSender
    Public Sub Send(ByVal message As String) Implements IMessageSender.Send
        Console.WriteLine(message) 
    End Sub
End Class
{code:vb.net}

That’s all that is required on the export side. Under the hood, MEF is still populating a dictionary, but this fact becomes invisible to you.

**Note:** You can also create metadata attributes that are not themselves exports, by creating an attribute which is decorated with MetadataAttributeAttribute. In these cases the metadata will be added to Exports on the same member where the custom metadata attribute was applied.

## Importing Metadata
Importers can access the metadata attached to the exports.
### Using Strongly-typed Metadata
To access metadata in a strongly-typed fashion created a metadata view by definining an interface with matching read only properties (names and types). For our sample it would be an interface like the following:

{code:c#}
public interface IMessageSenderCapabilities
{
    MessageTransport Transport { get; }
    bool IsSecure { get; }
}
{code:c#}
{code:vb.net}
Public Interface IMessageSenderCapabilities
    ReadOnly Property Transport() As MessageTransport
    ReadOnly Property IsSecure() As Boolean
End Interface
{code:vb.net}

Then you can start importing using the type **{{System.Lazy<T, TMetadata>}}** where T is the contract type and TMetadata is the interface you’ve created. 

{code:c#}
[Export](Export)
public class HttpServerHealthMonitor
{
    [ImportMany](ImportMany)
    public Lazy<IMessageSender, IMessageSenderCapabilities>[]() Senders { get; set; }

    public void SendNotification()
    {
        foreach(var sender in Senders)
        {
            if (sender.Metadata.Transport == MessageTransport.Smtp && 
                sender.Metadata.IsSecure)
            {
                var messageSender = sender.Value;
                messageSender.Send("Server is fine");
                
                break;
            }
        }
    }
}
{code:c#}
{code:vb.net}
<Export()>
Public Class HttpServerHealthMonitor
    <ImportMany()>
    Public Property Senders() As Lazy(Of IMessageSender, IMessageSenderCapabilities)() 

    Public Sub SendNotification()
        For Each sender In Senders
            If sender.Metadata.Transport = MessageTransport.Smtp AndAlso sender.Metadata.IsSecure Then
                Dim messageSender = sender.Value
                messageSender.Send("Server is fine")

                Exit For
            End If
        Next sender
    End Sub
End Class
{code:vb.net}

### Using Weakly-typed metadata
To access metadata in a weakly-typed fashion, you import uisng the type **{{System.Lazy<T, TMetadata>}}** passing IDictionary<string,object> for the metadata. You can then access the metadata through the Metadata property which will be a dictionary.

**Note:** In general we recommend the strongly-typed method for accessing metadata, however there are systems that need to access the metadata in a dynamic fashion, which this allows.

{code:c#}
[Export](Export)
public class HttpServerHealthMonitor
{
    [ImportMany](ImportMany)
    public Lazy<IMessageSender, IDictionary<string,object>>[]() Senders { get; set; }

    public void SendNotification()
    {
        foreach(var sender in Senders)
        {
            if (sender.Metadata.ContainsKey("Transport") && sender.Metadata["Transport"](_Transport_) == MessageTransport.Smtp && 
                sender.Metadata.ContainsKey("Issecure") && Metadata["IsSecure"](_IsSecure_) == true)
            {
                var messageSender = sender.Value;
                messageSender.Send("Server is fine");
                
                break;
            }
        }
    }
}
{code:c#}
{code:vb.net}
<Export()>
Public Class HttpServerHealthMonitor
    <ImportMany()>
    Public Property Senders() As Lazy(Of IMessageSender, IDictionary(Of String, Object))() 

    Public Sub SendNotification()
        For Each sender In Senders
            If sender.Metadata.ContainsKey("Transport") AndAlso sender.Metadata("Transport") = MessageTransport.Smtp AndAlso sender.Metadata.ContainsKey("Issecure") AndAlso sender.Metadata("IsSecure") = True Then
                Dim messageSender = sender.Value
                messageSender.Send("Server is fine")

                Exit For
            End If
        Next sender
    End Sub
End Class
{code:vb.net}

### Metadata filtering and DefaultValueAttribute
When you specifiy a metadata view, an implicit filtering will occur to match **only** those exports which contain the metadata properties defined in the view. You can specify on the metadata view that a property is not required, by using the {{System.ComponentModel.DefaultValueAttribute}}. Below you can see where we have specified a default value of false on IsSecure. This means if a part exports IMessageSender, but does not supply IsSecure metadata, then it will still be matched.

{code:c#}
public interface IMessageSenderCapabilities
{
    MessageTransport Transport { get; }
    [DefaultValue(false)](DefaultValue(false));
    bool IsSecure { get; }
}
{code:c#}
{code:vb.net}
Public Interface IMessageSenderCapabilities
    ReadOnly Property Transport() As MessageTransport
    <DefaultValue(False)> 
    ReadOnly Property IsSecure() As Boolean
End Interface
{code:vb.net}
