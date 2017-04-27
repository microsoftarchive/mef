# Recomposition

Some applications are designed to dynamically change at runtime. For example, a new extension may be downloaded, or others might become unavailable for a variety of reasons. MEF is prepared to handle these kinds of scenarios by relying on what we call recomposition, which is changing values of imports after the initial composition. 

An import can inform MEF that it supports recomposition through the **{{[System.ComponentModel.Composition.ImportAttribute](System.ComponentModel.Composition.ImportAttribute)}}** using the Allowrecomposition property. See the code snippet below:

{code:c#}
[Export](Export)
public class HttpServerHealthMonitor
{
    [ImportMany(AllowRecomposition=true)](ImportMany(AllowRecomposition=true))
    public IMessageSender[]() Senders { get; set; }
{code:c#}
{code:vb.net}
<Export()>
Public Class HttpServerHealthMonitor
    <ImportMany(AllowRecomposition:=True)> 
    Public Property Senders() As IMessageSender()
{code:vb.net}

This tells MEF that your class is ready to handle recomposition, and if the availability of IMessageSender implementations changes (either a new one is available, or made unavailable), the collection should be changed to reflect it. Once a part has opted in for recomposition, it will get notified whenever there is a change to the implementations available in the catalog, or if instances have been manually added / removed from the container. 

# Caveats of Recomposition
* When recomposition occurs, we will replace the instance of the collection / array with a new instance, we will not update the existing instance. In the example above, if a new IMessageSender appears, Senders will be completely replaced with a new array. This is in order to facilitate thread-safety.
* Recomposition is valid for virtually all types of imports supported: fields, properties and collections, but it is not supported for constructor paramters.
* If your type happens to implement the interface **{{[System.ComponentModel.Composition.IPartImportsSatisifiedNotification](System.ComponentModel.Composition.IPartImportsSatisifiedNotification)}}**, be aware that ImportCompleted will also be called whenever recomposition occurs.

# Recomposition and Silverlight
In Silverlight Recomposition plays a special rule with regards to application partitioning. For more on that see the [DeploymentCatalog](DeploymentCatalog) topic.