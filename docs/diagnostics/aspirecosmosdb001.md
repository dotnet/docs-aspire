---
title: Compiler Error ASPIRECOSMOSDB001
description: Learn more about compiler Error ASPIRECOSMOSDB001. `RunAsPreviewEmulator` is for evaluation purposes only and is subject to change or removal in future updates.
ms.date: 04/21/2025
f1_keywords:
  - "ASPIRECOSMOSDB001"
helpviewer_keywords:
  - "ASPIRECOSMOSDB001"
---

# Compiler Error ASPIRECOSMOSDB001

**Version introduced:** 9.0

> `RunAsPreviewEmulator` is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

-or-

> `WithDataExplorer` is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

Both `RunAsPreviewEmulator` and `WithDataExplorer` are considered experimental APIs.

.NET Aspire provides a way to use the Cosmos DB Linux-based (preview) emulator and data explorer. These APIs are considered experimental and are expected to change in the future.

## Example

The following sample generates `ASPIRECOSMOSDB001`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("cosmos")
                    .RunAsPreviewEmulator(e => e.WithDataExplorer());
```

## To correct this error

Suppress the Error with either of the following methods:

- Set the severity of the rule in the _.editorConfig_ file.

  ```ini
  [*.{cs,vb}]
  dotnet_diagnostic.ASPIRECOSMOSDB001.severity = none
  ```

  For more information about editor config files, see [Configuration files for code analysis rules](/dotnet/fundamentals/code-analysis/configuration-files).

- Add the following `PropertyGroup` to your project file:

  ```xml
  <PropertyGroup>
      <NoWarn>$(NoWarn);ASPIRECOSMOSDB001</NoWarn>
  </PropertyGroup>
  ```

- Suppress in code with the `#pragma warning disable ASPIRECOSMOSDB001` directive.
