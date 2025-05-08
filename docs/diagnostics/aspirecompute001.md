---
title: Compiler Error ASPIRECOMPUTE001
description: Learn more about compiler Error ASPIRECOMPUTE001. Compute related types and members are for evaluation purposes only and are subject to change or removal in future updates. Suppress this diagnostic to proceed.
ms.date: 05/08/2025
f1_keywords:
  - "ASPIRECOMPUTE001"
helpviewer_keywords:
  - "ASPIRECOMPUTE001"
---

# Compiler Error ASPIRECOMPUTE001

**Version introduced:** 9.3

> Compute related types and members are for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

## Example

The following code generates `ASPIRECOMPUTE001`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("env");
// or
builder.AddAzureContainerAppEnvironment("env")
// or
builder.AddKubernetesEnvironment("env")
```

## To correct this Error

Suppress the Error with either of the following methods:

- Set the severity of the rule in the _.editorConfig_ file.

  ```ini
  [*.{cs,vb}]
  dotnet_diagnostic.ASPIRECOMPUTE001.severity = none
  ```

  For more information about editor config files, see [Configuration files for code analysis rules](/dotnet/fundamentals/code-analysis/configuration-files).

- Add the following `PropertyGroup` to your project file:

  ```xml
  <PropertyGroup>
      <NoWarn>$(NoWarn);ASPIRECOMPUTE001</NoWarn>
  </PropertyGroup>
  ```

- Suppress in code with the `#pragma warning disable ASPIRECOMPUTE001` directive.
