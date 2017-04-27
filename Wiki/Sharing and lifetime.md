## Sharing, lifetime and ExportFactory<T>

Sharing and part lifetime in Microsoft.Composition are controlled using a concept called _Sharing Boundaries_. If a part is shared, exports from the same part instance may satisfy the imports of multiple other parts.

A part instance may need to be shared across the entire application or container, (commonly called a _Singleton_). Or, instances of the part may be created and shared locally between a group of part instances participating in a unit of work, for example responding to an HTTP request, processing a message from a queue, or displaying a particular page in a navigation-based app.

The diagram below shows how an UndoBuffer part might be shared on a per-page basis in a navigation-driven app:

![](Sharing and lifetime_SharingBoundaries.png)
 
Sharing boundaries in Microsoft.Composition are given string names, here the name of the sharing boundary is “Page”.

## Creating a sharing boundary

Sharing boundaries are created using _ExportFactory<T>_. The _SharingBoundary_ attribute names the boundary that will be created by the factory.

_PageController_ uses _ExporFactory<Page>_ to create and control the bounded parts:

{{
[Export](Export)
public class PageController
{
    [Import, SharingBoundary("Page")](Import,-SharingBoundary(_Page_))
    public ExportFactory<Page> PageFactory { get; set; }
}}
Although here the contract type _Page_ and the boundary name coincide, this does not have to be the case.

The _ExportFactory<Page>.CreateExport()_ method returns an _Export<Page>_ object with the _Page_ value:

{{
    void ShowPage()
    {
        var page = PageFactory.CreateExport();
        page.Value.Show();
    }
}}
This example lets the _page_ variable go out of scope enabling the sharing boundary and all of the parts in it to be garbage collected when possible. Alternatively, disposing the _Export<Page>_ object would cause all of the parts in the sharing boundary that implement _IDisposable_ to be disposed. In server apps that manage resources like transactions and connections, it is good practice to always explicitly dispose sharing boundaries.

## Marking a part as shared

The _UndoBuffer_ part is shared within the _“Page”_ sharing boundary:

{{
[Shared("Page")](Shared(_Page_))
public class UndoBuffer { }
}}
If a part is marked as _Shared_ but does not specify a sharing boundary, it will be a globally shared singleton.

In the absence of an explicit sharing attribute, a part is non-shared.