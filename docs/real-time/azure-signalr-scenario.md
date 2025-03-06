---
title: .NET Aspire Azure SignalR Service integration
description: Learn how to integrate Azure SignalR Service with .NET Aspire.
ms.date: 03/06/2025
---

# .NET Aspire Azure SignalR Service integration

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[Azure SignalR Service](/azure/azure-signalr/signalr-overview) is a fully managed real-time messaging service that simplifies adding real-time web functionality to your applications. The .NET Aspire Azure SignalR Service integration enables you to easily provision, configure, and connect your .NET applications to Azure SignalR Service instances.

This article describes how to integrate Azure SignalR Service into your .NET Aspire applications, covering both hosting and client integration.

## Hosting integration

The .NET Aspire Azure SignalR Service hosting integration models Azure SignalR resources as the following type:

- <xref:Aspire.Hosting.Azure.AzureSignalRResource>: Represents an Azure SignalR Service resource, including connection information to the underlying Azure resource.
- <xref:Aspire.Hosting.Azure.AzureSignalREmulatorResource>: Represents an emulator for Azure SignalR Service, allowing local development and testing without requiring an Azure subscription.

To access the hosting types and APIs for expressing these resources in the distributed application builder, install the [📦 Aspire.Hosting.Azure.SignalR](https://www.nuget.org/packages/Aspire.Hosting.Azure.SignalR) NuGet package in your [app host](../fundamentals/app-host-overview.md#app-host-project) project:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.SignalR
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.SignalR"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add an Azure SignalR Service resource

To add an Azure SignalR Service resource to your app host project, call the <xref:Aspire.Hosting.AzureSignalRExtensions.AddAzureSignalR*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var signalR = builder.AddAzureSignalR("signalr");

builder.AddProject<Projects.ApiService>("apiService")
       .WithReference(signalR)
       .WaitFor(signalR);

builder.AddProject<Projects.WebApp>("webapp")
       .WithReference(signalR)
       .WaitFor(signalR);

// Continue configuring and run the app...
```

In the preceding example:

- An Azure SignalR Service resource named `signalr` is added.
- The resource is referenced by the consuming projects (`apiService` and `webapp`), providing them with the necessary connection information.

> [!IMPORTANT]
> Calling `AddAzureSignalR` implicitly enables Azure provisioning support. Ensure your app host is configured with the appropriate Azure subscription and location. For more information, see [Local provisioning: Configuration](../azure/local-provisioning.md#configuration).

### Generated provisioning Bicep

When you add an Azure SignalR Service resource, .NET Aspire generates provisioning infrastructure using [Bicep](/azure/azure-resource-manager/bicep/overview). The generated Bicep includes defaults for location, SKU, and role assignments:

```bicep
@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param principalType string

param principalId string

resource signalr 'Microsoft.SignalRService/signalR@2024-03-01' = {
  name: take('signalr-${uniqueString(resourceGroup().id)}', 63)
  location: location
  properties: {
    cors: {
      allowedOrigins: [
        '*'
      ]
    }
    features: [
      {
        flag: 'ServiceMode'
        value: 'Default'
      }
    ]
  }
  kind: 'SignalR'
  sku: {
    name: 'Free_F1'
    capacity: 1
  }
  tags: {
    'aspire-resource-name': 'signalr'
  }
}

resource signalr_SignalRAppServer 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(signalr.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '420fcaa2-552c-430f-98ca-3264be4806c7'))
  properties: {
    principalId: principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '420fcaa2-552c-430f-98ca-3264be4806c7')
    principalType: principalType
  }
  scope: signalr
}

output hostName string = signalr.properties.hostName
```

The generated Bicep provides a starting point and can be customized further.

### Customize provisioning infrastructure

You can customize the generated Bicep using the `ConfigureInfrastructure` API:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var signalR = builder.AddAzureSignalR("signalr")
                     .ConfigureInfrastructure(infra =>
                     {
                         var resources = infra.GetProvisionableResources();
                         var signalRResource = resources.OfType<SignalRService>().Single();

                         signalRResource.Sku.Name = "Premium_P1";
                         signalRResource.Sku.Capacity = 2;
                         signalRResource.PublicNetworkAccess = "Enabled";
                         signalRResource.AddTag("Environment", "Production");
                     });
```

### Connect to an existing Azure SignalR Service

You might have an existing Azure SignalR Service that you want to connect to. You can chain a call to annotate that your <xref:Aspire.Hosting.ApplicationModel.AzureSignalRResource> is an existing resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var existingSignalRName = builder.AddParameter("existingSignalRName");
var existingSignalRResourceGroup = builder.AddParameter("existingSignalRResourceGroup");

var signalr = builder.AddAzureSignalR("signalr")
                     .AsExisting(existingSignalRName, existingSignalRResourceGroup);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(signalr);

// After adding all resources, run the app...
```

For more information on treating Azure SignalR resources as existing resources, see [Use existing Azure resources](../azure/integrations-overview.md#use-existing-azure-resources).

Alternatively, instead of representing an Azure SignalR resource, you can add a connection string to the app host. Which is a weakly-typed approach that's based solely on a `string` value. To add a connection to an existing Azure SignalR Service, call the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString%2A> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var signalr = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureSignalR("signalr")
    : builder.AddConnectionString("signalr");

builder.AddProject<Projects.ApiService>("apiService")
       .WithReference(signalr);
```

[!INCLUDE [connection-strings-alert](../includes/connection-strings-alert.md)]

The connection string is configured in the app host's configuration, typically under [User Secrets](/aspnet/core/security/app-secrets), under the `ConnectionStrings` section:

```json
{
  "ConnectionStrings": {
    "signalr": "Endpoint=https://your-signalr-instance.service.signalr.net;AccessKey=your-access-key;Version=1.0;"
  }
}
```

For more information, see [Add existing Azure resources with connection strings](../azure/integrations-overview.md#add-existing-azure-resources-with-connection-strings).

### Add an Azure SignalR Service emulator resource

The Azure SignalR Service emulator is a local development and testing tool that emulates the behavior of Azure SignalR Service. This emulator only supports [Serverless_ mode](/azure/azure-signalr/concept-service-mode#serverless-mode), which requires a specific configuration when using the emulator.

To use the emulator, chain a call to the <xref:Aspire.Hosting.AzureSignalRExtensions.RunAsEmulator(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.ApplicationModel.AzureSignalRResource},System.Action{Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzureSignalREmulatorResource}})> method:

```csharp
using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

var signalR = builder.AddAzureSignalR("signalr", AzureSignalRServiceMode.Serverless)
                     .RunAsEmulator();

builder.AddProject<Projects.ApiService>("apiService")
       .WithReference(signalR)
       .WaitFor(signalR);

// After adding all resources, run the app...
```

In the preceding example, the `RunAsEmulator` method configures the Azure SignalR Service resource to run as an emulator. The emulator is started when the app host is run, and it will be stopped when the app host is stopped.

## Client integration

There isn't an official .NET Aspire Azure SignalR client integration. However, there is limited support for similar experiences. There are two specific packages available for [.NET from the Azure team](https://github.com/Azure/azure-signalr) that enable scenarios such as managing the client connection to Azure SignalR Service, and hooking up to the Azure SignalR Service resource. To get started, install the [📦 Microsoft.Azure.SignalR](https://www.nuget.org/packages/Microsoft.Azure.SignalR) NuGet package in the project hosting your SignalR hub.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Microsoft.Azure.SignalR
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Microsoft.Azure.SignalR"
                  Version="*" />
```

---

If you're app host is using the Azure SignalR emulator, you'll also need to install the [📦 Microsoft.Azure.SignalR.Management](https://www.nuget.org/packages/Microsoft.Azure.SignalR.Management) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Microsoft.Azure.SignalR.Management
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Microsoft.Azure.SignalR.Management"
                  Version="*" />
```

---

### Configure the SignalR client

In your SignalR hub host project, configure Azure SignalR Service using the `AddNamedAzureSignalR` extension method chained to `AddSignalR`:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR()
                .AddNamedAzureSignalR("signalr");

var app = builder.Build();

app.MapHub<ChatHub>("/chat");

app.Run();
```

The `AddNamedAzureSignalR` method configures the client to use the Azure SignalR Service resource named `signalr`. The connection string is read from the configuration key `ConnectionStrings:signalr`, and additional settings are loaded from the `Azure:SignalR:signalr` configuration section.

### Configuration options

You can configure additional SignalR client options through configuration providers or code:

```json
{
  "Azure": {
    "SignalR": {
      "signalr": {
        "ServiceMode": "Default",
        "ServerStickyMode": "Required",
        "ConnectionCount": 5
      }
    }
  }
```

Alternatively, configure options in code:

```csharp
builder.Services.AddSignalR()
                .AddNamedAzureSignalR("signalr", options =>
                {
                    options.ServiceMode = ServiceMode.Default;
                    options.ServerStickyMode = ServerStickyMode.Required;
                    options.ConnectionCount = 5;
                });
```

### Hosting integration health checks

The Azure SignalR Service hosting integration automatically adds health checks to verify connectivity and service availability. It relies on the [📦 AspNetCore.HealthChecks.AzureSignalR](https://www.nuget.org/packages/AspNetCore.HealthChecks.AzureSignalR) NuGet package.

## Observability and telemetry

The Azure SignalR Service integration provides built-in support for observability and telemetry through logging, tracing, and metrics.

### Logging

The integration uses the following log categories:

- `Microsoft.Azure.SignalR`
- `Azure.Core`
- `Azure.Identity`

### Tracing

The integration emits tracing activities using OpenTelemetry:

- `Azure.SignalR.*`
- `Microsoft.Azure.SignalR.*`

### Metrics

Metrics are provided for monitoring connection counts, message throughput, and service health.

## See also

- [Azure SignalR Service overview](/azure/azure-signalr/signalr-overview)
- [Scale ASP.NET Core SignalR applications with Azure SignalR Service](/azure/azure-signalr/signalr-concept-scale-aspnet-core)
- [.NET Aspire Azure integrations overview](../azure/integrations-overview.md)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
