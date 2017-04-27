# Strongly Typed Metadata

[Exports and Metadata](Exports-and-Metadata) went over MEF’s ability to attach metadata to exports and have imports constraining what can be used to satisfy the imports based on the same metadata. It is a very useful feature, but one might argue that it is too loose to rely on dictionary entries and there ought to be a better way. 
Fortunately, there is. With MEF you can define strongly typed metadata and attach them to exports and constrain and/or access it on the import level. The following describes required to get it working.

## On the export side

The export side is where the metadata is attached, describing among other things, the capabilities of the implementation of a particular contract. In order to do it more strongly typed than using the ExportMetadataAttribute, you need to create your own attribute and decorate it with **{{[System.ComponentModel.Composition.MetadataAttribute](System.ComponentModel.Composition.MetadataAttribute)}}**. Using our previous example of IMessageSender, we would create an attribute like the following:

{code:c#}
[MetadataAttribute](MetadataAttribute)
[AttributeUsage(AttributeTargets.Class)](AttributeUsage(AttributeTargets.Class))
public class MessageSenderCapabilitiesAttribute : Attribute
{
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

Note the MetadataAttribute applied to our custom attribute.
The next step is to apply our attribute to our IMessageSender implementations:

{code:c#}
[Export(typeof(IMessageSender))](Export(typeof(IMessageSender)))
[MessageSenderCapabilities(Transport=MessageTransport.Smtp)](MessageSenderCapabilities(Transport=MessageTransport.Smtp))
public class EmailSender : IMessageSender
{
    public void Send(string message)
    {
        Console.WriteLine(message);
    }
}

[Export(typeof(IMessageSender))](Export(typeof(IMessageSender)))
[MessageSenderCapabilities(Transport=MessageTransport.Smtp, IsSecure=true)](MessageSenderCapabilities(Transport=MessageTransport.Smtp,-IsSecure=true))
public class SecureEmailSender : IMessageSender
{
    public void Send(string message)
    {
        Console.WriteLine(message);
    }
}

[Export(typeof(IMessageSender))](Export(typeof(IMessageSender)))
[MessageSenderCapabilities(Transport=MessageTransport.PhoneNetwork)](MessageSenderCapabilities(Transport=MessageTransport.PhoneNetwork))
public class SMSSender : IMessageSender
{
    public void Send(string message)
    {
        Console.WriteLine(message);
    }
}
{code:c#}

That’s all that is required on the export side. Under the hood, MEF is still populating a dictionary, but this fact becomes invisible to you.

## On the import side

There is nothing preventing us from using the metadata dictionary on our import side. In fact, our small sample will continue to work:

{code:c#}
[Export](Export)
public class HttpServerHealthMonitor
{
    [Import](Import)
    public ExportCollection<IMessageSender> Senders { get; set; }

    public void SendNotification()
    {
        foreach (var sender in Senders)
        {
            if (((MessageTransport)sender.Metadata["Transport"](_Transport_)) == MessageTransport.Smtp && 
                sender.Metadata.ContainsKey("IsSecure") &&
                ((bool)sender.Metadata["IsSecure"](_IsSecure_)))
            {
                var messageSender = sender.GetExportedObject();
                messageSender.Send("Server is fine");
                
                break;
            }
        }
    }
}
{code:c#} 

However, it doesn’t look so good. Thus we should use a strongly typed metadata view on the import side too. All you’ll need is to define an interface with matching members (names and types). For our sample it would be an interface like the following:

{code:c#}
public interface IMessageSenderCapabilities
{
    MessageTransport Transport { get; }
    bool IsSecure { get; }
}
{code:c#}

Then you can start using the types **{{[System.ComponentModel.Composition.Export<T, TMetadataView>](System.ComponentModel.Composition.Export_T,-TMetadataView_)}}** and **{{[System.ComponentModel.Composition.ExportCollection<T, TMetadataView> ](System.ComponentModel.Composition.ExportCollection_T,-TMetadataView_-)}}** where T is the contract type and TMetadataView is the interface you’ve created. 
Now, applying that to our import side sample and you will notice how it reduced the amount of code and got rid of the literals:

{code:c#}
[Export](Export)
public class HttpServerHealthMonitor
{
    [Import](Import)
    public ExportCollection<IMessageSender, IMessageSenderCapabilities> Senders { get; set; }

    public void SendNotification()
    {
        foreach(var sender in Senders)
        {
            if (sender.MetadataView.Transport == MessageTransport.Smtp && 
                sender.MetadataView.IsSecure)
            {
                var messageSender = sender.GetExportedObject();
                messageSender.Send("Server is fine");
                
                break;
            }
        }
    }
}
{code:c#}
 

