# Lazy Exports
During composition of a part, an import will trigger the instantiation of a part (or parts) that expose the necessary exports required for the original requested part. For some applications delaying this instantiation – and preventing the recursive composition down the graph – may be an important factor to consider as creation a long and complex graph of objects can be expensive and unnecessary. 

This is the motivation for MEF to support what we call lazy exports. In order to use it all you need to do is to import **{{[System.Lazy<T>](System.Lazy_T_)}}** instead of **{{[T](T)}}** directly. For example, consider the code snippet bellow:

{code:c#}
public class HttpServerHealthMonitor
{
    [Import](Import)
    public IMessageSender Sender { get; set; }
{code:c#}
{code:vb.net}
Public Class HttpServerHealthMonitor
    <Import()>
    Public Property Sender() As IMessageSender
{code:vb.net}

The code above imports states that it depends on a contract (IMessageSender) implementation. When MEF supply this dependency it will need to also create the IMessageSender selected and recursively the dependencies that the implementation might have. 
In order to turn this import to be lazy, you just need to replace it by and Lazy<IMessageSender>:

{code:c#}
[Export](Export)
public class HttpServerHealthMonitor
{
    [Import](Import)
    public Lazy<IMessageSender> Sender { get; set; }
{code:c#}
{code:vb.net}
<Export()>
Public Class HttpServerHealthMonitor
    <Import()>
    Public Property Sender() As Lazy(Of IMessageSender) 
{code:vb.net}

In this case you are opt-in for delaying this instantiation until you actually need the implementation instance. In order to request the instance, use the property **{{[Lazy<T>.Value](Lazy_T_.Value)}}**.

