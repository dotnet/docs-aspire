---
title: .NET Aspire diagnostics overview
description: Learn about the diagnostics tools and features available in .NET Aspire.
ms.topic: overview
ms.date: 04/15/2025
ai-usage: ai-assisted
---

# .NET Aspire diagnostics overview

Several APIs of .NET Aspire are decorated with the <xref:System.Diagnostics.CodeAnalysis.ExperimentalAttribute>. This attribute indicates that the API is experimental and might be removed or changed in future versions of .NET Aspire. The attribute is used to identify APIs that aren't yet stable and might not be suitable for production use.

## ASPIREACADOMAINS001

<span id="ASPIREACADOMAINS001"></span>

.NET Aspire 9.0 introduces the ability to customize container app resources using any of the following extension methods:

- <xref:Aspire.Hosting.AzureContainerAppProjectExtensions.PublishAsAzureContainerApp*>
- <xref:Aspire.Hosting.AzureContainerAppContainerExtensions.PublishAsAzureContainerApp*>
- <xref:Aspire.Hosting.AzureContainerAppExecutableExtensions.PublishAsAzureContainerApp*>

When you use one of these methods, the Azure Developer CLI (`azd`) can no longer preserve custom domains. Instead use the <xref:Aspire.Hosting.ContainerAppExtensions.ConfigureCustomDomain(Azure.Provisioning.AppContainers.ContainerApp,Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.ApplicationModel.ParameterResource},Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.ApplicationModel.ParameterResource})> method to configure a custom domain within the .NET Aspire app host. The `ConfigureCustomDomain(...)` extension method is experimental. To suppress the compiler error/warning, use the following code:

To suppress the `ASPIREACADOMAINS001` diagnostic, select the **Copy** button from the following diagnostic ID, and see [Suppress diagnostic](#suppress-diagnostic):

```plaintext
ASPIREACADOMAINS001
```

## ASPIREHOSTINGPYTHON001

<span id="ASPIREHOSTINGPYTHON001"></span>

.NET Aspire provides a way to add Python executables or applications to the .NET Aspire app host. Since the shape of this API is expected to change in the future, it's experimental (<xref:System.Diagnostics.CodeAnalysis.ExperimentalAttribute>). To suppress the compiler error/warning, use the following code:

To suppress the `ASPIREHOSTINGPYTHON001` diagnostic, select the **Copy** button from the following diagnostic ID, and see [Suppress diagnostic](#suppress-diagnostic):

```plaintext
ASPIREAZURE001
```

## ASPIRECOSMOSDB001

<span id="ASPIRECOSMOSDB001"></span>

.NET Aspire provides a way to use the Cosmos DB Linux-based (preview) emulator. Since this emulator is in preview and the shape of this API is expected to change in the future, it's experimental (<xref:System.Diagnostics.CodeAnalysis.ExperimentalAttribute>). To suppress the compiler error/warning, use the following code:

To suppress the `ASPIRECOSMOSDB001` diagnostic, select the **Copy** button from the following diagnostic ID, and see [Suppress diagnostic](#suppress-diagnostic):

```plaintext
ASPIRECOSMOSDB001
```

## ASPIREPUBLISHERS001

<span id="ASPIREPUBLISHERS001"></span>

.NET Aspire introduced the concept of _Publishers_ starting in version 9.2. Publishers play a pivotal role in the deployment process, enabling the transformation of your distributed app into deployable assets. This alleviates the intermediate step of producing the publishing [manifest](../deployment/manifest-format.md) for tools to act on, instead empowering the developer to express their intent directly in C#.

Publishers are currently in preview and the APIs are experimental (<xref:System.Diagnostics.CodeAnalysis.ExperimentalAttribute>). To suppress the compiler error/warning, use the following code:

To suppress the `ASPIREPUBLISHERS001` diagnostic, select the **Copy** button from the following diagnostic ID, and see [Suppress diagnostic](#suppress-diagnostic):

```plaintext
ASPIREPUBLISHERS001
```

## ASPIREAZURE001

<span id="ASPIREAZURE001"></span>

The .NET Aspire Azure hosting integration now ships with a publisher. If you're using any of the <xref:Aspire.Hosting.AzurePublisherExtensions.AddAzurePublisher*> APIs, you might see a compiler error/warning indicating that the API is experimental. This is expected behavior, as the API is still in preview and the shape of this API is expected to change in the future.

To suppress the `ASPIREAZURE001` diagnostic, select the **Copy** button from the following diagnostic ID, and see [Suppress diagnostic](#suppress-diagnostic):

```plaintext
ASPIREAZURE001
```

## Suppress diagnostic

You can suppress any diagnostic in this document using one of the following methods:

- Add the <xref:System.Diagnostics.CodeAnalysis.SuppressMessageAttribute> at the assembly, class, method, line, etc.
- Include the diagnostic ID in the `NoWarn` property of your project file.
- Use preprocessor directives in your code.
- Configure diagnostic severity in an _.editorconfig_ file.

There are some common patterns for suppressing diagnostics in .NET projects. The best method depends on your context and the specific diagnostic. Here's a quick guide to help you choose:

### Suppress with the suppress message attribute

The <xref:System.Diagnostics.CodeAnalysis.SuppressMessageAttribute> is ideal when you need targeted, documented suppression tied directly to a specific code element like a class or method. It shines when you’re making a deliberate exception to a rule that is valid in most other places. This attribute keeps the suppression close to the code it affects, which helps reviewers and future maintainers understand the rationale. While it's a clean solution for isolated cases, it can clutter the code if overused, and it’s not the best choice for widespread or bulk suppressions.

To suppress the `ASPIREEX007` diagnostic with the <xref:System.Diagnostics.CodeAnalysis.SuppressMessageAttribute>, consider the following examples:

**Assembly-level suppression:**

```csharp
// Typically placed in AssemblyInfo.cs, GlobalUsings.cs, or any global file.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Aspire.Example",                      // Category
    "ASPIREEX007:AspireExampleDiagnostic", // CheckId and optional rule title
    Justification = "This warning is not applicable to our context.")]
```

**Class-level suppression:**

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Aspire.Example",
    "ASPIREEX007:AspireExampleDiagnostic",
    Justification = "This warning is not applicable to our context.")]
public class ExampleClassThatConsumesTheOffendingAPI
{
    // Class implementation
}
```

**Method-level suppression:**

```csharp
public class ExampleClass
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Aspire.Example",
        "ASPIREEX007:AspireExampleDiagnostic",
        Justification = "This warning is not applicable to our context.")]
    public void ExampleMethodThatConsumesTheOffendingAPI()
    {
        // Method implementation
    }
}
```

> [!IMPORTANT]
> If the diagnostic is triggered on a property, field, or parameter, you can also apply it directly there with the <xref:System.Diagnostics.CodeAnalysis.SuppressMessageAttribute>.

### Suppress within the project file

Using the `NoWarn` property in the project file is great when you need to suppress a diagnostic across the entire project. This is helpful in cases where a diagnostic is irrelevant to your scenario or when dealing with known false positives. It’s a simple, central way to silence a diagnostic without touching your code. However, it lacks visibility—developers won’t see that a diagnostic is suppressed unless they inspect the project file—so it can make it harder to track intentional exceptions or identify the reason behind suppressions.

To suppress the `ASPIREEX007` diagnostic in the project file, add the following code to your _.csproj_ file:

```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);ASPIREEX007</NoWarn>
</PropertyGroup>
```

> [!TIP]
> The `$(NoWarn)` property in the preceding XML is used to append the diagnostic ID to the existing list of suppressed warnings. This ensures that you don't accidentally remove other suppressions already in place.

### Suppress with preprocessor directives

Preprocessor directives like `#pragma warning disable` offer fine-grained suppression within a specific scope, such as a method body or block of code. They're especially handy when you need a temporary suppression during refactoring, or when a rule incorrectly flags a particular line that you can't or don't want to change. The ability to tightly wrap just the affected code makes this approach powerful, but it can also make the code harder to read and maintain, especially if the directives are scattered or forgotten over time.

To suppress the `ASPIREEX007` diagnostic with preprocessor directives, consider the following examples:

**File-level suppression:**

```csharp
#pragma warning disable ASPIREEX007

public class ExampleClassThatConsumesTheOffendingAPI
{
    // Class implementation
}
```

**Granular suppression:**

```csharp
public class ExampleClass
{
    public void ExampleMethodThatConsumesTheOffendingAPI()
    {
        #pragma warning disable ASPIREEX007
        // Code that triggers the diagnostic
        #pragma warning restore ASPIREEX007
    }
}
```

You can use preprocessor directives to suppress the diagnostic for a specific block of code. This is useful when you want to limit the scope of the suppression to a particular section of your code.

For more information, see [C# preprocessor directives](/dotnet/csharp/language-reference/preprocessor-directives).

### Suppress with editor configuration

Suppressing diagnostics using an _.editorconfig_ file is ideal for enforcing or adjusting analyzer behavior at scale. It allows teams to standardize severity levels (or disable rules) across an entire solution or per directory/file pattern. This method keeps suppression cleanly out of the codebase, and it works well for communicating team-wide conventions. However, it can be a bit opaque—developers need to know to look in the config file—and it doesn't offer the precision of an attribute or pragma when dealing with one-off cases.

To suppress the `ASPIREEX007` diagnostic in an _.editorconfig_ file, add the following code:

```ini
[*.cs]
dotnet_diagnostic.ASPIREEX007.severity = none
```

This configuration applies to all C# files in the project. You can also scope it to specific files or directories by adjusting the section header.

## See also

- [.NET docs: How to suppress code analysis warnings](/dotnet/fundamentals/code-analysis/suppress-warnings)
- [Visual Studio docs: Suppress code analysis violations](/visualstudio/code-quality/in-source-suppression-overview)
