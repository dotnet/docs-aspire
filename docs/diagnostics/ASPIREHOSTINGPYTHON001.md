---
title: Compiler Error ASPIREHOSTINGPYTHON001
description: Learn more about compiler Error ASPIREHOSTINGPYTHON001. `AddPythonApp` is for evaluation purposes only and is subject to change or removal in future updates.
ms.date: 04/21/2025
f1_keywords:
  - "ASPIREHOSTINGPYTHON001"
helpviewer_keywords:
  - "ASPIREHOSTINGPYTHON001"
---

# Compiler Error ASPIREHOSTINGPYTHON001

**Version introduced:** 9.0

> `AddPythonApp` is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

.NET Aspire provides a way to add Python executables or applications to the .NET Aspire app host with the `AddPythonApp` extension. Since the shape of this API is expected to change in the future, it's experimental.

## Example

The following code generates `ASPIREHOSTINGPYTHON001`:

```csharp
var pythonApp = builder.AddPythonApp("hello-python", "../hello-python", "main.py")
       .WithHttpEndpoint(env: "PORT")
       .WithExternalHttpEndpoints()
       .WithOtlpExporter();
```

## To correct this Error

Suppress the Error with either of the following methods:

- Set the severity of the rule in the _.editorConfig_ file.

  ```ini
  [*.{cs,vb}]
  dotnet_diagnostic.ASPIREHOSTINGPYTHON001.severity = none
  ```

  For more information about editor config files, see [Configuration files for code analysis rules](/dotnet/fundamentals/code-analysis/configuration-files).

- Add the following `PropertyGroup` to your project file:

  ```xml
  <PropertyGroup>
      <NoWarn>$(NoWarn);ASPIREHOSTINGPYTHON001</NoWarn>
  </PropertyGroup>
  ```

- Suppress in code with the `#pragma warning disable ASPIREHOSTINGPYTHON001` directive.
