---
title: .NET Aspire Azure SignalR Service integration
description: Learn how to integrate Azure SignalR Service with .NET Aspire.
ms.date: 07/22/2025
---

# .NET Aspire Azure SignalR Service integration

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[Azure SignalR Service](/azure/azure-signalr/signalr-overview) is a fully managed real-time messaging service that simplifies adding real-time web functionality to your applications. The .NET Aspire Azure SignalR Service integration enables you to easily provision, configure, and connect your .NET applications to Azure SignalR Service instances.

This article describes how to integrate Azure SignalR Service into your .NET Aspire applications, covering both hosting and client integration.

## Hosting integration

The .NET Aspire Azure SignalR Service hosting integration models Azure SignalR resources as the following types:

- <xref:Aspire.Hosting.ApplicationModel.AzureSignalRResource>: Represents an Azure SignalR Service resource, including connection information to the underlying Azure resource.
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

var api = builder.AddProject<Projects.ApiService>("api")
                 .WithReference(signalR)
                 .WaitFor(signalR);

builder.AddProject<Projects.WebApp>("webapp")
       .WithReference(api)
       .WaitFor(api);

// Continue configuring and run the app...
```

In the preceding example:

- An Azure SignalR Service resource named `signalr` is added.
- The `signalr` resource is referenced by the `api` project.
- The `api` project is referenced by the `webapp` project.

This architecture allows the `webapp` project to communicate with the `api` project, which in turn communicates with the Azure SignalR Service resource.

> [!IMPORTANT]
> Calling `AddAzureSignalR` implicitly enables Azure provisioning support. Ensure your app host is configured with the appropriate Azure subscription and location. For more information, see [Local provisioning: Configuration](../azure/local-provisioning.md#configuration).

### Provisioning-generated Bicep

When you add an Azure SignalR Service resource, .NET Aspire generates provisioning infrastructure using [Bicep](/azure/azure-resource-manager/bicep/overview). The generated Bicep includes defaults for location, SKU, and role assignments:

:::code language="bicep" source="../snippets/azure/AppHost/signalr/signalr.bicep":::

The preceding Bicep is a module that provisions an Azure SignalR Service resource. Additionally, role assignments are created for the Azure resource in a separate module:

:::code language="bicep" source="../snippets/azure/AppHost/signalr-roles/signalr-roles.bicep":::

The generated Bicep is a starting point and is influenced by changes to the provisioning infrastructure in C#. Customizations to the Bicep file directly will be overwritten, so make changes through the C# provisioning APIs to ensure they are reflected in the generated files.

### Customize provisioning infrastructure

All .NET Aspire Azure resources are subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type. This enables customization of the generated Bicep by providing a fluent API to configure the Azure resources using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure})> API:

:::code language="csharp" source="../snippets/azure/AppHost/Program.ConfigureSignalRInfra.cs" id="configure":::

The preceding code:

- Chains a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> API:
  - The `infra` parameter is an instance of the <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type.
  - The provisionable resources are retrieved by calling the <xref:Azure.Provisioning.Infrastructure.GetProvisionableResources> method.
  - The single <xref:Azure.Provisioning.SignalR.SignalRService> resource is retrieved.
  - The <xref:Azure.Provisioning.SignalR.SignalRService.Sku?displayProperty=nameWithType> property is assigned a name of `Premium_P1` and a capacity of `10`.
  - The <xref:Azure.Provisioning.SignalR.SignalRService.PublicNetworkAccess?displayProperty=nameWithType> property is set to `Enabled`.
  - A tag is added to the SignalR service resource with a key of `ExampleKey` and a value of `Example value`.

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

[!INCLUDE [azure-configuration](../azure/includes/azure-configuration.md)]

For more information on treating Azure SignalR resources as existing resources, see [Use existing Azure resources](../azure/integrations-overview.md#use-existing-azure-resources).

> [!NOTE]
> Alternatively, instead of representing an Azure SignalR Service resource, you can add a connection string to the app host. This approach is weakly-typed, and doesn't work with role assignments or infrastructure customizations. For more information, see [Add existing Azure resources with connection strings](../azure/integrations-overview.md#add-existing-azure-resources-with-connection-strings).

### Add an Azure SignalR Service emulator resource

The Azure SignalR Service emulator is a local development and testing tool that emulates the behavior of Azure SignalR Service. This emulator only supports [_Serverless_ mode](/azure/azure-signalr/concept-service-mode#serverless-mode), which requires a specific configuration when using the emulator.

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

In the preceding example, the `RunAsEmulator` method configures the Azure SignalR Service resource to run as an emulator. The emulator is based on the `mcr.microsoft.com/signalr/signalr-emulator:latest` container image. The emulator is started when the app host is run, and is stopped when the app host is stopped.

#### Azure SignalR Service modes

While the Azure SignalR Service emulator only supports the _Serverless_ mode, the Azure SignalR Service resource can be configured to use either of the following modes:

- `AzureSignalRServiceMode.Default`
- `AzureSignalRServiceMode.Serverless`

<!-- TODO add xref

- <xref:Aspire.Hosting.ApplicationModel.AzureSignalRServiceMode.Default?displayProperty=nameWithType>
- <xref:Aspire.Hosting.ApplicationModel.AzureSignalRServiceMode.Serverless?displayProperty=nameWithType>
-->

The _Default_ mode is the "default" configuration for Azure SignalR Service. Each mode has its own set of features and limitations. For more information, see [Azure SignalR Service modes](/azure/azure-signalr/concept-service-mode).

> [!IMPORTANT]
> The Azure SignalR Service emulator only works in _Serverless_ mode and the `AddNamedAzureSignalR` method doesn't support _Serverless_ mode.

## Hub host integration

There isn't an official .NET Aspire Azure SignalR [_client integration_](../fundamentals/integrations-overview.md#client-integrations). However, there is limited support for similar experiences. In these scenarios, the Azure SignalR Service acts as a proxy between the server (where the <xref:Microsoft.AspNetCore.SignalR.Hub> or <xref:Microsoft.AspNetCore.SignalR.Hub`1> are hosted) and the client (where the SignalR client is hosted). The Azure SignalR Service routes traffic between the server and client, allowing for real-time communication.

> [!IMPORTANT]
> It's important to disambiguate between .NET Aspire client integrations and the .NET SignalR client. SignalR exposes hubs—which act as a server-side concept—and SignalR clients connect to those hubs. The .NET projects that host SignalR hubs are where you integrate with .NET Aspire. The SignalR client is a separate library that connects to those hubs, in a different project.

There are two packages available for, each with addressing specific scenarios such as managing the client connection to Azure SignalR Service, and hooking up to the Azure SignalR Service resource. To get started, install the [📦 Microsoft.Azure.SignalR](https://www.nuget.org/packages/Microsoft.Azure.SignalR) NuGet package in the project hosting your SignalR hub.

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

### Configure named Azure SignalR Service in Default mode

In _Default_ mode, your consuming project needs to rely on a named Azure SignalR Service resource. Consider the following diagram that illustrates the architecture of Azure SignalR Service in _Default_ mode:

:::image type="content" source="media/default-mode-thumb.png" alt-text="Azure SignalR Service: Default mode diagram." lightbox="media/default-mode.png":::

For more information on _Default_ mode, see [Azure SignalR Service: Default mode](/azure/azure-signalr/concept-service-mode#default-mode).

In your SignalR hub host project, configure Azure SignalR Service by chaining calls to `.AddSignalR().AddNamedAzureSignalR("name")`:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR()
                .AddNamedAzureSignalR("signalr");

var app = builder.Build();

app.MapHub<ChatHub>("/chat");

app.Run();
```

The `AddNamedAzureSignalR` method configures the project to use the Azure SignalR Service resource named `signalr`. The connection string is read from the configuration key `ConnectionStrings:signalr`, and additional settings are loaded from the `Azure:SignalR:signalr` configuration section.

> [!NOTE]
> If you're using the Azure SignalR emulator, you cannot use the `AddNamedAzureSignalR` method.

### Configure Azure SignalR Service in Serverless mode

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

Azure SignalR _Serverless_ mode doesn't require a hub server to be running. The Azure SignalR Service is responsible for maintaining client connections. Additionally, in this mode, you cannot use traditional SignalR Hubs, such as <xref:Microsoft.AspNetCore.SignalR.Hub>, <xref:Microsoft.AspNetCore.SignalR.Hub`1>, or <xref:Microsoft.AspNetCore.SignalR.IHubContext`1>. Instead, [configure an upstream endpoint which is usually an Azure Function SignalR trigger](/azure/azure-signalr/concept-upstream). Consider the following diagram that illustrates the architecture of Azure SignalR Service in _Serverless_ mode:

:::image type="content" source="media/serverless-mode-thumb.png" alt-text="Azure SignalR Service: Serverless mode diagram." lightbox="media/serverless-mode.png":::

For more information on _Serverless_ mode, see [Azure SignalR Service: Serverless mode](/azure/azure-signalr/concept-service-mode#serverless-mode).

In a project that's intended to communicate with the Azure SignalR Service, register the appropriate services by calling <xref:Microsoft.Extensions.DependencyInjection.SignalRDependencyInjectionExtensions.AddSignalR*> and then registering the `ServiceManager` using the `signalr` connection string and add a `/negotiate` endpoint:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(sp =>
{
   return new ServiceManagerBuilder()
       .WithOptions(options =>
       {
           options.ConnectionString = builder.Configuration.GetConnectionString("signalr");
       })
       .BuildServiceManager();
});

var app = builder.Build();

app.MapPost("/negotiate", async (string? userId, ServiceManager sm, CancellationToken token) =>
{
    // The creation of the ServiceHubContext is expensive, so it's recommended to 
    // only create it once per named context / per app run if possible.
    var context = await sm.CreateHubContextAsync("messages", token);
    
    var negotiateResponse = await context.NegotiateAsync(new NegotiationOptions
    {
        UserId = userId
    }, token);
    
    // The JSON serializer options need to be set to ignore null values, otherwise the
    // response will contain null values for the properties that are not set.
    // The .NET SignalR client will not be able to parse the response if the null values are present.
    // For more information, see https://github.com/dotnet/aspnetcore/issues/60935.
    return Results.Json(negotiateResponse, new JsonSerializerOptions(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    });
});

app.Run();
```

The preceding code configures the Azure SignalR Service using the `ServiceManagerBuilder` class, but doesn't call `AddSignalR` or `MapHub`. These two extensions aren't required with _Serverless_ mode. The connection string is read from the configuration key `ConnectionStrings:signalr`. When using the emulator, only the HTTP endpoint is available. Within the app, you can use the `ServiceManager` instance to create a `ServiceHubContext`. The `ServiceHubContext` is used to broadcast messages and manage connections to clients.

The `/negotiate` endpoint is required to establish a connection between the connecting client and the Azure SignalR Service. The `ServiceHubContext` is created using the `ServiceManager.CreateHubContextAsync` method, which takes the hub name as a parameter. The `NegotiateAsync` method is called to negotiate the connection with the Azure SignalR Service, which returns an access token and the URL for the client to connect to.

For more information, see [Use Azure SignalR Management SDK](/azure/azure-signalr/signalr-howto-use-management-sdk).

## See also

- [Azure SignalR Service overview](/azure/azure-signalr/signalr-overview)
- [Scale ASP.NET Core SignalR applications with Azure SignalR Service](/azure/azure-signalr/signalr-concept-scale-aspnet-core)
- [.NET Aspire Azure integrations overview](../azure/integrations-overview.md)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
