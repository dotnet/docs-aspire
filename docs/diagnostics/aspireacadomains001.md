---
title: Compiler Error ASPIREACADOMAINS001
description: Learn more about compiler Error ASPIREACADOMAINS001. `ConfigureCustomDomain` is for evaluation purposes only and is subject to change or removal in future updates.
ms.date: 04/21/2025
f1_keywords:
  - "ASPIREACADOMAINS001"
helpviewer_keywords:
  - "ASPIREACADOMAINS001"
---

# Compiler Error ASPIREACADOMAINS001

**Version introduced:** 9.0

> `ConfigureCustomDomain` is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

Aspire 9.0 introduces the ability to customize container app resources using any of the following extension methods:

- `Aspire.Hosting.AzureContainerAppProjectExtensions.PublishAsAzureContainerApp`
- `Aspire.Hosting.AzureContainerAppContainerExtensions.PublishAsAzureContainerApp`
- `Aspire.Hosting.AzureContainerAppExecutableExtensions.PublishAsAzureContainerApp`

When you use one of these methods, the Azure Developer CLI (`azd`) can no longer preserve custom domains. Instead use the `Aspire.Hosting.ContainerAppExtensions.ConfigureCustomDomain` method to configure a custom domain within the Aspire AppHost.

However, `app.ConfigureCustomDomain` is an experimental API and you must suppress it to use it.

## Example

The following code generates `ASPIREACADOMAINS001`:

```csharp
var customDomain = builder.AddParameter("customDomain");
var certificateName = builder.AddParameter("certificateName");

builder.AddProject<Projects.AzureContainerApps_ApiService>("api")
       .WithExternalHttpEndpoints()
       .PublishAsAzureContainerApp((infra, app) =>
       {
           app.ConfigureCustomDomain(customDomain, certificateName);
       });
```

## To correct this error

Suppress the error with either of the following methods:

- Set the severity of the rule in the _.editorconfig_ file.

  ```ini
  [*.{cs,vb}]
  dotnet_diagnostic.ASPIREACADOMAINS001.severity = none
  ```

  For more information about editor config files, see [Configuration files for code analysis rules](/dotnet/fundamentals/code-analysis/configuration-files).

- Add the following `PropertyGroup` to your project file:

  ```xml
  <PropertyGroup>
      <NoWarn>$(NoWarn);ASPIREACADOMAINS001</NoWarn>
  </PropertyGroup>
  ```

- Suppress in code with the `#pragma warning disable ASPIREACADOMAINS001` directive.
