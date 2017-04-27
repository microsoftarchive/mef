## Extending Microsoft.Composition with Alternative Programming Models

MEF for web and Windows Store apps (aka the Microsoft.Composition NuGet package) set out with the design goal to ship a lean core composition engine that can be richly extended. Most IoC containers follow this philosophy, so to be a bit more specific about our goals:

# Most simple extensions should be simple to write and explain 
# Extensions should provide the same performance, robustness and diagnostics as the core features
# A few broad extension points are preferable to many narrow ones

To keep complexity at a minimum it was also our intention to implement as many container features as possible as ‘extensions’, even if they ship in the core package.

In this page we’ll walk through one simple scenario, examine the container extension points that support it, and discuss a few other ways in which these extension points can be put to use.

## What is a Programming Model?

The programming model determines how parts are described for the container. Out of the box, Microsoft.Composition supports the familiar MEF Attributed Programming Model in which parts are regular .NET classes marked up with Import and Export attributes.

{{
[Export(typeof(IAmusement))](Export(typeof(IAmusement)))
public class Rollercoaster : IAmusement
{
    [Import](Import)
    public ITrack Track { get; set; }
}
}}
The Attributed Programming Model is quite flexible and provides support for custom attribute types and for rule-based conventions, but all of these build on the basic programming model foundation for using exports from class-based parts. There are many other ways that values can be supplied for composition.

## Using Web.config Settings as Exports

Let’s consider parts that need to be configured with settings from an App.config or Web.config file. Here we’ve created a custom Setting attribute that specifies the key for the setting:

{{
[Export](Export)
public class Downloader
{
    readonly string _url;

    [ImportingConstructor](ImportingConstructor)
    public Downloader([Setting("serverUrl")](Setting(_serverUrl_)) string url)
    {
        _url = url;
    }

    public void Download()
    {
        Console.WriteLine("Downloading from {0}...", _url);
    }
}
}}
This hypothetical part, in addition to whatever other imports it has, needs to be configured with the URL of a server. Our container extension is going to enable the serverUrl parameter to be read from the application configuration file:

{{
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <add key="serverUrl" value="http://mef.codeplex.com" />
  </appSettings>
</configuration>
}}
This effectively turns XML app configuration into a programming model for the container; once it is enabled, appSettings entries specify exports that other parts can consume.

Some might suggest that this is too much coupling between the importing part and the configuration file; this is a reasonable viewpoint, but the model does have some benefits, for example settings used by an application are declaratively discoverable. Like everything, the technique has pros and cons; as an example it covers a lot of the container extension API so let’s run with it here.

## Composition Contracts

Microsoft.Composition matches exports to imports based on contracts. A contract is a familiar MEF construct:

![](ProgrammingModelExtensions_Contract.png)
 
In order for an import to match an export, they must share the same contract type, have the same contract name (if any) and have compatible metadata constraints. We write out contracts in the following format:

{{
ContractType "contractName" { ConstraintName = value, ...}
}}
Whenever the container encounters an import for a new contract, it queries the available extensions to see if they can supply a matching export. We’re going to take advantage of this feature to supply a value whenever a part imports an app setting. The first step of that is to determine what the contract for a Setting import will look like.

## Constraining the Imported Contract

If we return to the constructor of the Downloader part, without our custom Setting attribute, it looks like this:

{{
    [ImportingConstructor](ImportingConstructor)
    public Downloader(string url)
}}
This makes url into a vanilla MEF import, with a contract that looks simply like:

{{
System.String
}}
This isn’t a great contract – it is so broad that any string will match, there’s no way to match this up with a specific setting value.

We want to ensure that settings are only supplied a very specific value, and the mechanism for doing that is Metadata Constraints. Each constraint comprises a name and a value:

{{
System.String { SettingKey = "serverUrl" }
}}
The rules of contracts require that constraints match exactly, so this contract will no longer be satisfied by just any old string: perfect for our purposes. To specify a metadata constraint on an import, we can use the standard ImportMetadataConstraint attribute:

{{
    [ImportingConstructor](ImportingConstructor)
    public Downloader([ImportMetadataConstraint("SettingKey", "serverUrl")](ImportMetadataConstraint(_SettingKey_,-_serverUrl_)) string url)
}}
This works but is a bit verbose to use frequently. MEF allows the creation of custom attributes to specify export metadata, and the same attributes can be applied to imports to specify metadata constraints. The Setting attribute is such an attribute:

{{
[AttributeUsage(AttributeTargets.Property ](-AttributeTargets.Parameter))
[MetadataAttribute](MetadataAttribute)
public class SettingAttribute : Attribute
{
    readonly string _key;

    public SettingAttribute(string key)
    {
        _key = key;
    }

    public string SettingKey { get { return _key; } }
}
}}
We can now use our desired syntax to import settings:

{{
    [ImportingConstructor](ImportingConstructor)
    public Downloader([Setting("serverUrl")](Setting(_serverUrl_)) string url)
}}
Using the Setting attribute on an appropriate export will match the import: 

{{
    [Export, Setting("serverUrl")](Export,-Setting(_serverUrl_))
    public string Url { get { return ... } }
}}
That isn’t our goal here however. The other side of our extension is going to match these constraints and supply exports from the configuration file.

## Plugging in an ExportDescriptorProvider

We can plug support for settings into the container as an ExportDescriptorProvider when the container is configured:

{{
var configuration = new ContainerConfiguration()
    .WithAssembly(typeof(Program).Assembly)
    .WithProvider(new AppSettingsExportDescriptorProvider());
}}
The AppSettingsExportDescriptorProvider will be queried whenever the container encounters a new contract. If the contract has a SettingKey constraint, and the value exists in the App.config file, then the provider will return an ExportDescriptor that the container can later use to retrieve the value. We’ll show the complete listing for the provider, then break down each of the steps involved:

{{
public class AppSettingsExportDescriptorProvider : ExportDescriptorProvider
{
    static readonly Type[]()() SupportedSettingTypes = new[]()() { typeof(string), typeof(int), typeof(double), typeof(DateTime), typeof(TimeSpan) };


    public override IEnumerable<ExportDescriptorPromise> GetExportDescriptors(
        CompositionContract contract, DependencyAccessor dependencyAccessor)
    {
        string key;
        CompositionContract unwrapped;

        if (!contract.TryUnwrapMetadataConstraint("SettingKey", out key, out unwrapped))
            return NoExportDescriptors;

        if (!unwrapped.Equals(new CompositionContract(unwrapped.ContractType)))
            return NoExportDescriptors;

        if (!SupportedSettingTypes.Contains(unwrapped.ContractType))
            return NoExportDescriptors;

        var value = ConfigurationManager.AppSettings.Get(key);
        if (value == null)
            return NoExportDescriptors;

        var converted = Convert.ChangeType(value, contract.ContractType);

        return new[]() {
            new ExportDescriptorPromise(
                contract,
                "Application Configuration",
                true,
                NoDependencies,
                _ => ExportDescriptor.Create((c, o) => converted, NoMetadata)) };
    }
}
}}
The provider implements one method, GetExportDescriptors(), that takes a CompositionContract and returns a list of ExportDescriptorPromises. The DependencyAccessor parameter isn’t used in this example.

The first step in implementing the provider is determining whether the contract has a SettingKey constraint applied. If not, then the request isn’t for an app setting, so the provider returns an empty list:

{{
        string key;
        CompositionContract unwrapped;

        if (!contract.TryUnwrapMetadataConstraint("SettingKey", out key, out unwrapped))
            return NoExportDescriptors;
}}
If the check succeeds, then key will hold the setting key, for example “serverUrl”, and unwrapped will contain the remainder of the contract with the constraint removed. So, in the downloader example, the contract is:

{{
System.String { SettingKey = "serverUrl" }
}}
This will be unwrapped into:

{{
System.String
}}
The next check in the provider makes sure that no name or additional constraints are applied to the contract:

{{
        if (!unwrapped.Equals(new CompositionContract(unwrapped.ContractType)))
            return NoExportDescriptors;
}}
If there are additional constraints that this provider does not understand, or if there’s a contract name applied, then we return nothing. It is very important that providers only match contracts that they understand completely.

For the same reason, the provider checks that the type is a supported setting type. Doing so ensures that the code will properly support Lazy<T> and other relationship types for settings:

{{
        if (!SupportedSettingTypes.Contains(unwrapped.ContractType))
            return NoExportDescriptors;
}}

The next step checks to see if there is a matching value in the application configuration store. If not, perhaps the contract will be supplied another way, so the provider returns an empty list:

{{
        var value = ConfigurationManager.AppSettings.Get(key);
        if (value == null)
            return NoExportDescriptors;
}}
Now that we’re sure the contract is for an app setting, and the key exists in the configuration file, we change the type of the value to match the requested contract type.

{{
        var converted = Convert.ChangeType(value, contract.ContractType);
}}
In this implementation we simply throw an exception if the conversion fails, a better implementation would test for assignability and return an empty list if the types could not be matched.

Now that we have a setting value to match the requested contract, the final step provides the composition engine with all the information it needs to:

* Validate the dependency graph
* Supply good error messages to the user if necessary
* Quickly access the exported value during composition

This information is all wrapped up in the ExportDescriptorPromise that we return:

{{
        return new[]() {
            new ExportDescriptorPromise(
                contract,
                "Application Configuration",
                true,
                NoDependencies,
                _ => ExportDescriptor.Create((c, o) => converted, NoMetadata)) };
}}
The “Application Configuration” string describes where the value came from, in case it is needed in an error message. An example could be the case where the same setting is provided elsewhere in the composition:

{{
Only one export for the contract 'String { SettingKey = "serverUrl" }' is allowed, but the following parts: 'Application Configuration', 'SomeOtherPart' export it.
 -> required by import 'serverUrl' of part 'Downloader'
}}
The other more mysterious parameter here is simply ‘true’. This indicates to the composition engine that the resulting instance is shared, and in conjunction with the dependency information returned can be used to check for dependency cycles and so on.

## What else is Possible?
There are a number of sample extensions in the MEF CodePlex source repository built using plugged-in ExportDescriptorProviders. These can be found in the /oob/demo subfolder.

|| Extension || Description ||
| App Settings | The topic of this post. Use <appSetting> values as exports. |
| Instances | Use an existing instance as an export. |
| Delegates | Use a supplied delegate to create exported instances on the fly. |
| Resolve any Concrete Type | “Automatically” supply any concrete type with a parameterless constructor as an export. |
|Dictionary Imports | Import a dictionary keyed by an export metadata value. |
|Ordered ImportMany | Order an imported collection using a value from metadata. |
| Default-only Exports | Specify that an export is a default that is used only when no other export exists for the same contract. |

Within the container itself, the Attributed Programming Model is plugged in this way via the TypedPartExportDescriptorProvider, and the technique is used so that the container can supply the current CompositionContext to parts as an import as well. Support for ImportMany, Lazy<T>, ExportFactory<T>, metadata view providers are all plugged in this way.

It is feasible that this extension point could be used to implement an “auto-mocking” container, where the exports are generated on the fly by a mocking framework. It is also conceivable that this extension point might be used to implement a completely different type-based programming model from the attributed one supplied in the box, perhaps mimicking that of another composition framework.
