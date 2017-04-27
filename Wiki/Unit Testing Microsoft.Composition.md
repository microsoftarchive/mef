# Unit Testing Microsoft.Composition

When using composition heavily in an application it can be useful to verify that composition succeeds via unit tests.

The challenge of unit testing composition code is to ensure that parts don't execute inappropriately during this testing. Unit tests for the parts themselves verify their behavior using mocks and other standard techniques that isolate the parts at test time.

The goal of composition testing is to ensure that a part _can_ be created using the exports provided by other parts at run-time. For each part of interest, we want to verify that its imports can be satisfied by other parts, that there are no circular dependencies, and that no contracts are over-supplied.

This page provides some guidance and sample code for writing these kinds of tests.

## The {{CompositionAssert}} class

The {{CompositionAssert}} class below provides two overloads of the {{CanExportSingle()}} assertion.

{{
public static class CompositionAssert
{
    public static void CanExportSingle(CompositionContext context, Type contractType, string contractName = null, IDictionary<string, object> metadataConstraints = null)
    {
        var lazyType = typeof(Lazy<>).MakeGenericType(contractType);
        var lazyContract = new CompositionContract(lazyType, contractName, metadataConstraints);
        context.GetExport(lazyContract);
    }

    public static void CanExportSingle<TContract>(CompositionContext context, string contractName = null, IDictionary<string, object> metadataConstraints = null)
    {
        CanExportSingle(context, typeof(TContract), contractName, metadataConstraints);
    }
}
}}

These assertions are used like so:

{{
[TestMethod](TestMethod)
public void WorkItemsControllerCanBeComposed()
{
    var host = CreateCompositionHost();
    CompositionAssert.CanExportSingle<WorkItemsController>(host);
}
}}

The {{CanExportSingle()}} method takes a contract type and checks that it can be composed. It does this by transforming the contract {{T}} into {{Lazy<T>}} under the hood before requesting an instance from the container. When lazy dependencies are used, the container won't actually create an instance of the requested export, so in the example, no {{WorkItemsController}} is created for the test. However, the container **does** validate lazy dependencies, so the test will only pass if {{WorkItemsController}} and all of its dependencies are correctly configured.

In this example {{CreateCompositionHost()}} stubs out whatever is necessary to create the container in exactly the state that is used at runtime.

## Which contracts should I test?

{{CanExportSingle()}} does a **deep** check of dependencies, traversing all relationships including {{Lazy<T>}}, {{ExportFactory<T>}} and {{[ImportMany](ImportMany)}}.

Therefore, it is only necessary to check exported contracts that are requested through either:

* Direct calls to {{CompositionContext.GetExport()}}, {{CompositionContext.GetExports()}} and {{CompositionContext.TryGetExport()}}, or
* Imported properties on objects passed to {{CompositionContext.SatisfyImports()}}

These are the _Composition Roots_ of the application. They will generally be few, for example, the controllers in an ASP.NET MVC or Web API application, or ViewModels in an MVVM application.

## What problems are not caught?

Currently, this technique will not reveal lifetime problems, for example requesting an instance of a part outside of a sharing boundary that it is constrained by.
