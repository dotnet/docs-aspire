---
title: .NET Aspire diagnostics overview
description: Learn about the diagnostics tools and features available in .NET Aspire.
ms.topic: overview
ms.date: 07/22/2025
---

# .NET Aspire diagnostics list

The following table lists the possible MSBuild and .NET Analyzer warnings and errors you might encounter with .NET Aspire:

| Diagnostic ID | Type | Description |
|--|--|--|
| [`ASPIRE001`](aspire001.md) | Warning | <span id="ASPIRE001"></span> The code language isn't fully supported by Aspire, some code generation targets will not run. |
| [`ASPIRE002`](aspire002.md) | Warning | <span id="ASPIRE002"></span> Project is an Aspire AppHost project but necessary dependencies aren't present. Are you missing an Aspire.Hosting.AppHost PackageReference? |
| [`ASPIRE003`](aspire003.md) | Warning | <span id="ASPIRE003"></span> 'Project' is a .NET Aspire AppHost project that requires Visual Studio version 17.10 or above to work correctly. |
| [`ASPIRE004`](aspire004.md) | Warning | <span id="ASPIRE004"></span> 'Project' is referenced by an Aspire Host project, but it is not an executable. |
| [`ASPIRE006`](aspire006.md) | (Experimental) Error | <span id="ASPIRE006"></span> Application model items must have valid names. |
| [`ASPIRE007`](aspire007.md) | Error | <span id="ASPIRE007"></span> 'Project' requires a reference to "Aspire.AppHost.Sdk" with version "9.0.0" or greater to work correctly. |
| [`ASPIRE008`](aspire008.md) | Error | <span id="ASPIRE008"></span> The Aspire workload that this project depends on is now deprecated. |
| [`ASPIREACADOMAINS001`](aspireacadomains001.md) | (Experimental) Error | <span id="ASPIREACADOMAINS001"></span> `ConfigureCustomDomain` is for evaluation purposes only and is subject to change or removal in future updates. |
| [`ASPIREAZURE001`](aspireazure001.md) | (Experimental) Error | <span id="ASPIREAZURE001"></span> Publishers are for evaluation purposes only and are subject to change or removal in future updates. |
| [`ASPIRECOMPUTE001`](aspirecompute001.md) | (Experimental) Error | <span id="ASPIRECOMPUTE001"></span> Compute related types and members are for evaluation purposes only and is subject to change or removal in future updates. |
| [`ASPIRECOSMOSDB001`](aspirecosmosdb001.md) | (Experimental) Error | <span id="ASPIRECOSMOSDB001"></span> `RunAsPreviewEmulator` is for evaluation purposes only and is subject to change or removal in future updates. |
| [`ASPIREHOSTINGPYTHON001`](aspirehostingpython001.md) | (Experimental) Error | <span id="ASPIREHOSTINGPYTHON001"></span> `AddPythonApp` is for evaluation purposes only and is subject to change or removal in future updates. |
| [`ASPIREPROXYENDPOINTS001`](aspireproxyendpoints001.md) | (Experimental) Error | <span id="ASPIREPROXYENDPOINTS001"></span> ProxyEndpoint members are for evaluation purposes only and are subject to change or removal in future updates. |
| [`ASPIREPUBLISHERS001`](aspirepublishers001.md) | Error | <span id="ASPIREPUBLISHERS001"></span> Publishers are for evaluation purposes only and are subject to change or removal in future updates. |

## Suppress diagnostic

You can suppress any diagnostic in this document using one of the following methods:

- Add the <xref:System.Diagnostics.CodeAnalysis.SuppressMessageAttribute> at the assembly, class, method, line, etc.
- Include the diagnostic ID in the `NoWarn` property of your project file.
- Use preprocessor directives in your code.
- Configure diagnostic severity in an _.editorconfig_ file.

There are some common patterns for suppressing diagnostics in .NET projects. The best method depends on your context and the specific diagnostic. Here's a quick guide to help you choose:

> [!IMPORTANT]
> The following sections provide examples for suppressing the `ASPIREE000` diagnostic, which is a placeholder for any diagnostic ID you might encounter. Replace `ASPIREE000` with the actual diagnostic ID you want to suppress.

### Suppress with the suppress message attribute

The <xref:System.Diagnostics.CodeAnalysis.SuppressMessageAttribute> is ideal when you need targeted, documented suppression tied directly to a specific code element like a class or method. It shines when you’re making a deliberate exception to a rule that is valid in most other places. This attribute keeps the suppression close to the code it affects, which helps reviewers and future maintainers understand the rationale. While it's a clean solution for isolated cases, it can clutter the code if overused, and it’s not the best choice for widespread or bulk suppressions.

To suppress the `ASPIREE000` diagnostic with the <xref:System.Diagnostics.CodeAnalysis.SuppressMessageAttribute>, consider the following examples:

**Assembly-level suppression:**

```csharp
// Typically placed in AssemblyInfo.cs, GlobalUsings.cs, or any global file.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Aspire.Example",                      // Category
    "ASPIREE000:AspireExampleDiagnostic", // CheckId and optional rule title
    Justification = "This warning is not applicable to our context.")]
```

**Class-level suppression:**

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Aspire.Example",
    "ASPIREE000:AspireExampleDiagnostic",
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
        "ASPIREE000:AspireExampleDiagnostic",
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

To suppress the `ASPIREE000` diagnostic in the project file, add the following code to your _.csproj_ file:

```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);ASPIREE000</NoWarn>
</PropertyGroup>
```

> [!TIP]
> The `$(NoWarn)` property in the preceding XML is used to append the diagnostic ID to the existing list of suppressed warnings. This ensures that you don't accidentally remove other suppressions already in place.

### Suppress with preprocessor directives

Preprocessor directives like `#pragma warning disable` offer fine-grained suppression within a specific scope, such as a method body or block of code. They're especially handy when you need a temporary suppression during refactoring, or when a rule incorrectly flags a particular line that you can't or don't want to change. The ability to tightly wrap just the affected code makes this approach powerful, but it can also make the code harder to read and maintain, especially if the directives are scattered or forgotten over time.

To suppress the `ASPIREE000` diagnostic with preprocessor directives, consider the following examples:

**File-level suppression:**

```csharp
#pragma warning disable ASPIREE000

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
        #pragma warning disable ASPIREE000
        // Code that triggers the diagnostic
        #pragma warning restore ASPIREE000
    }
}
```

You can use preprocessor directives to suppress the diagnostic for a specific block of code. This is useful when you want to limit the scope of the suppression to a particular section of your code.

For more information, see [C# preprocessor directives](/dotnet/csharp/language-reference/preprocessor-directives).

### Suppress with editor configuration

Suppressing diagnostics using an _.editorconfig_ file is ideal for enforcing or adjusting analyzer behavior at scale. It allows teams to standardize severity levels (or disable rules) across an entire solution or per directory/file pattern. This method keeps suppression cleanly out of the codebase, and it works well for communicating team-wide conventions. However, it can be a bit opaque—developers need to know to look in the config file—and it doesn't offer the precision of an attribute or pragma when dealing with one-off cases.

To suppress the `ASPIREE000` diagnostic in an _.editorconfig_ file, add the following code:

```ini
[*.cs]
dotnet_diagnostic.ASPIREE000.severity = none
```

This configuration applies to all C# files in the project. You can also scope it to specific files or directories by adjusting the section header.

## See also

- [.NET docs: How to suppress code analysis warnings](/dotnet/fundamentals/code-analysis/suppress-warnings)
- [Visual Studio docs: Suppress code analysis violations](/visualstudio/code-quality/in-source-suppression-overview)
