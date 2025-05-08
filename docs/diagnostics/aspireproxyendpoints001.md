---
title: Compiler Error ASPIREPROXYENDPOINTS001
description: Learn more about compiler Error ASPIREPROXYENDPOINTS001. Members are for evaluation purposes only and are subject to change or removal in future updates.
ms.date: 05/08/2025
f1_keywords:
  - "ASPIREPROXYENDPOINTS001"
helpviewer_keywords:
  - "ASPIREPROXYENDPOINTS001"
---

# Compiler Error ASPIREPROXYENDPOINTS001

**Version introduced:** 9.1

> `WithEndpointProxySupport` is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

-or-

> `ProxySupportAnnotation` is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

Both `WithEndpointProxySupport` and `ProxySupportAnnotation` are considered experimental APIs.

Container resources use proxied endpoints by default. Adjusting this setting is experimental. For more information, see <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithEndpointProxySupport*>.

## Example

The following code generates `ASPIREPROXYENDPOINTS001`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis($"example-redis", 1234)
                   .WithEndpointProxySupport(false);
```

## To correct this Error

Suppress the Error with either of the following methods:

- Set the severity of the rule in the _.editorConfig_ file.

  ```ini
  [*.{cs,vb}]
  dotnet_diagnostic.ASPIREPROXYENDPOINTS001.severity = none
  ```

  For more information about editor config files, see [Configuration files for code analysis rules](/dotnet/fundamentals/code-analysis/configuration-files).

- Add the following `PropertyGroup` to your project file:

  ```xml
  <PropertyGroup>
      <NoWarn>$(NoWarn);ASPIREPROXYENDPOINTS001</NoWarn>
  </PropertyGroup>
  ```

- Suppress in code with the `#pragma warning disable ASPIREPROXYENDPOINTS001` directive.
