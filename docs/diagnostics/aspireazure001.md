---
title: Compiler Error ASPIREAZURE001
description: Learn more about compiler Error ASPIREAZURE001. Publishers are for evaluation purposes only and are subject to change or removal in future updates.
ms.date: 04/21/2025
f1_keywords:
  - "ASPIREAZURE001"
helpviewer_keywords:
  - "ASPIREAZURE001"
---

# Compiler Error ASPIREAZURE001

**Version introduced:** 9.2

> Publishers are for evaluation purposes only and are subject to change or removal in future updates. Suppress this diagnostic to proceed.

The .NET Aspire Azure hosting integration now ships with a publisher. If you're using any of the `Aspire.Hosting.AzurePublisherExtensions.AddAzurePublisher*` APIs, you might see a compiler error/warning indicating that the API is experimental. This behavior is expected, as the API is still in preview and the shape of this API is expected to change in the future.

## Example

The following code generates `ASPIREAZURE001`:

```csharp
builder.AddAzurePublisher();
```

## To correct this Error

Suppress the Error with either of the following methods:

- Set the severity of the rule in the _.editorconfig_ file.

  ```ini
  [*.{cs,vb}]
  dotnet_diagnostic.ASPIREAZURE001.severity = none
  ```

  For more information about editor config files, see [Configuration files for code analysis rules](/dotnet/fundamentals/code-analysis/configuration-files).

- Add the following `PropertyGroup` to your project file:

  ```xml
  <PropertyGroup>
      <NoWarn>$(NoWarn);ASPIREAZURE001</NoWarn>
  </PropertyGroup>
  ```

- Suppress in code with the `#pragma warning disable ASPIREAZURE001` directive.
