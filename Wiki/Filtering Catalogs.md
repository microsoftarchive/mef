# Filtering Catalogs

When using child containers it may be important to filter catalogs based on some specific criteria. For example, it is common to filter based on part's creation policy. The following code snippet demonstrates how to set up this particular approach:

{code:c#}
var catalog = new AssemblyCatalog(typeof(Program).Assembly);
var parent = new CompositionContainer(catalog);

var filteredCat = new FilteredCatalog(catalog,
    def => def.Metadata.ContainsKey(CompositionConstants.PartCreationPolicyMetadataName) &&
    ((CreationPolicy)def.Metadata[CompositionConstants.PartCreationPolicyMetadataName](CompositionConstants.PartCreationPolicyMetadataName)) == CreationPolicy.NonShared);
var child = new CompositionContainer(filteredCat, parent);

var root = child.GetExportedObject<Root>();
child.Dispose();
{code:c#}
{code:vb.net}
Dim catalog = New AssemblyCatalog(GetType(Program).Assembly) 
Dim parent = New CompositionContainer(catalog) 

Dim filteredCat = New DirectoryCatalog(catalog, Sub(def) def.Metadata.ContainsKey(CompositionConstants.PartCreationPolicyMetadataName) AndAlso (CType(def.Metadata(CompositionConstants.PartCreationPolicyMetadataName), CreationPolicy)) = CreationPolicy.NonShared) 
Dim child = New CompositionContainer(filteredCat, parent) 

Dim root = child.GetExportedObject(Of Root)() 
child.Dispose()
{code:vb.net}

If CreationPolicy is not enough as a criteria to select parts you may want to use the **{{[System.ComponentModel.Composition.PartMetadataAttribute](System.ComponentModel.Composition.PartMetadataAttribute)}}** instead. It allows you to attach metadata to the part so you can use it to construct a filtering expression. For example, the following is a class that with the attribute applied:

{code:c#}
[PartMetadata("scope", "webrequest"), Export](PartMetadata(_scope_,-_webrequest_),-Export)
public class HomeController : Controller
{
} 
{code:c#}
{code:vb.net}
<PartMetadata("scope", "webrequest"), Export()>
Public Class HomeController
    Inherits Controller
End Class
{code:vb.net}

This allows you to create a child container with parts that should be scoped to a (logical) web request. Note that it is up to you to define a scope boundary, in other words, MEF doesn't know what a "webrequest" is, so you have to create some infrastructure code to create/dispose containers per web request.

{code:c#}
var catalog = new AssemblyCatalog(typeof(Program).Assembly);
var parent = new CompositionContainer(catalog);

var filteredCat = new FilteredCatalog(catalog,
    def => def.Metadata.ContainsKey("scope") &&
    def.Metadata["scope"](_scope_).ToString() == "webrequest");
var perRequest = new CompositionContainer(filteredCat, parent);

var controller = perRequest.GetExportedObject<HomeController>();
perRequest.Dispose();
{code:c#}
{code:vb.net}
Dim catalog = New AssemblyCatalog(GetType(Program).Assembly) 
Dim parent = New CompositionContainer(catalog) 

Dim filteredCat = New FilteredCatalog(catalog, Function(def) def.Metadata.ContainsKey("scope") AndAlso def.Metadata("scope").ToString() = "webrequest")
Dim perRequest = New CompositionContainer(filteredCat, parent) 

Dim controller = perRequest.GetExportedObject(Of HomeController)() 
perRequest.Dispose()
{code:vb.net}

Note that we do not provide a FilteredCatalog class. A simple implementation that illustrates what would take to build one follows:

{code:c#}
using System;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;

public class FilteredCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
{
    private readonly ComposablePartCatalog _inner;
    private readonly INotifyComposablePartCatalogChanged _innerNotifyChange;
    private readonly IQueryable<ComposablePartDefinition> _partsQuery;

    public FilteredCatalog(ComposablePartCatalog inner,
                           Expression<Func<ComposablePartDefinition, bool>> expression)
    {
        _inner = inner;
        _innerNotifyChange = inner as INotifyComposablePartCatalogChanged;
        _partsQuery = inner.Parts.Where(expression);
    }

    public override IQueryable<ComposablePartDefinition> Parts
    {
        get
        {
            return _partsQuery;
        }
    }

    public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed
    {
        add
        {
            if (_innerNotifyChange != null)
                _innerNotifyChange.Changed += value;
        }
        remove
        {
            if (_innerNotifyChange != null)
                _innerNotifyChange.Changed -= value;
        }
    }

    public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing
    {
        add
        {
            if (_innerNotifyChange != null)
                _innerNotifyChange.Changing += value;
        }
        remove
        {
            if (_innerNotifyChange != null)
                _innerNotifyChange.Changing -= value;
        }
    }
}
{code:c#}
{code:vb.net}
Imports System
Imports System.ComponentModel.Composition.Primitives
Imports System.ComponentModel.Composition.Hosting
Imports System.Linq
Imports System.Linq.Expressions

Public Class FilteredCatalog
    Inherits ComposablePartCatalog
    Implements INotifyComposablePartCatalogChanged
    Private ReadOnly _inner As ComposablePartCatalog
    Private ReadOnly _innerNotifyChange As INotifyComposablePartCatalogChanged
    Private ReadOnly _partsQuery As IQueryable(Of ComposablePartDefinition) 

    Public Sub New(ByVal inner As ComposablePartCatalog, ByVal expression As Expression(Of Func(Of ComposablePartDefinition, Boolean))) 
        _inner = inner
        _innerNotifyChange = TryCast(inner, INotifyComposablePartCatalogChanged) 
        _partsQuery = inner.Parts.Where(expression) 
    End Sub

    Public Overrides ReadOnly Property Parts() As IQueryable(Of ComposablePartDefinition) 
        Get
            Return _partsQuery
        End Get
    End Property

    Public Custom Event Changed As EventHandler(Of ComposablePartCatalogChangeEventArgs) Implements INotifyComposablePartCatalogChanged.Changed 

        AddHandler(ByVal value As EventHandler(Of ComposablePartCatalogChangeEventArgs)) 
            If _innerNotifyChange IsNot Nothing Then
                AddHandler _innerNotifyChange.Changed, value
            End If
        End AddHandler
        RemoveHandler(ByVal value As EventHandler(Of ComposablePartCatalogChangeEventArgs)) 
            If _innerNotifyChange IsNot Nothing Then
                RemoveHandler _innerNotifyChange.Changed, value
            End If
        End RemoveHandler
        RaiseEvent(ByVal sender As System.Object, ByVal e As ComposablePartCatalogChangeEventArgs) 

        End RaiseEvent
    End Event

    Public Custom Event Changing As EventHandler(Of ComposablePartCatalogChangeEventArgs) Implements INotifyComposablePartCatalogChanged.Changing
        AddHandler(ByVal value As EventHandler(Of ComposablePartCatalogChangeEventArgs)) 

            If _innerNotifyChange IsNot Nothing Then
                AddHandler _innerNotifyChange.Changing, value
            End If
        End AddHandler
        RemoveHandler(ByVal value As EventHandler(Of ComposablePartCatalogChangeEventArgs)) 
            If _innerNotifyChange IsNot Nothing Then
                RemoveHandler _innerNotifyChange.Changing, value
            End If
        End RemoveHandler
        RaiseEvent(ByVal sender As System.Object, ByVal e As ComposablePartCatalogChangeEventArgs) 

        End RaiseEvent
    End Event
End Class
{code:vb.net}

