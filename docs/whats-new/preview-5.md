---
title: .NET Aspire preview 5
description: .NET Aspire preview 5 is now available and includes many improvements and new capabilities.
ms.date: 04/16/2024
---

# .NET Aspire preview 5

.NET Aspire preview 5 introduces breaking changes to hosting NuGet packages. In addition to these breaking changes, there are several sweeping improvements and additions to be aware of, including support for AWS and improvements for Azure. The following article provides an overview of the major changes in .NET Aspire preview 5: `8.0.0-preview.5.24201.12`.

If you're looking to upgrade from a previous version of .NET Aspire, see the [upgrade guide](#upgrade-to-preview-5).

## Packaging changes

As part of our journey to GA we have split up the `Aspire.Hosting` and `Aspire.Hosting.Azure` packages. These changes will allow us greater flexibility for servicing and ensure that appropriate boundaries are maintained between our core abstractions for .NET Aspire and various cloud-native dependencies that an application may require.

The following table maps between Aspire extension methods you might be using today in your AppHost and the package
in which they are now contained:

| Extension method     | Package                      |
|----------------------|------------------------------|
| `AddProject(...)`    | `Aspire.Hosting` (unchanged) |
| `AddContainer(...)`  | `Aspire.Hosting` (unchanged) |
| `AddExecutable(...)` | `Aspire.Hosting` (unchanged) |
| `AddKafka(...)`      | `Aspire.Hosting.Kafka`       |
| `AddMongoDB(...)`    | `Aspire.Hosting.MongoDB`     |
| `AddMySql(...)`      | `Aspire.Hosting.MySql`       |
| `AddNpmApp(...)`     | `Aspire.Hosting.NodeJs`      |
| `AddNodeApp(...)`    | `Aspire.Hosting.NodeJs`      |
| `AddOracle(...)`     | `Aspire.Hosting.Oracle`      |
| `AddPostgres(...)`   | `Aspire.Hosting.PostgreSQL`  |
| `AddRabbitMQ(...)`   | `Aspire.Hosting.RabbitMQ`    |
| `AddRedis(...)`      | `Aspire.Hosting.Redis`       |
| `AddSeq(...)`        | `Aspire.Hosting.Seq`         |
| `AddSqlServer(...)`  | `Aspire.Hosting.SqlServer`   |

For more information, see [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md).

The `Aspire.Hosting.Azure` APIs have been broken up in to the following packages:

| Extension method                     | Package                                    |
|--------------------------------------|--------------------------------------------|
| `AddBicepTemplate(...)`              | `Aspire.Hosting.Azure` (unchanged)         |
| `AddBicepTemplateString(...)`        | `Aspire.Hosting.Azure` (unchanged)         |
| `AddAzureConstruct(...)`             | `Aspire.Hosting.Azure` (unchanged)         |
| `AddAzureAppConfiguration(...)`      | `Aspire.Hosting.Azure.AppConfiguration`    |
| `AddAzureApplicationInsights(...)`   | `Aspire.Hosting.Azure.ApplicationInsights` |
| `AddAzureOpenAI(...)`                | `Aspire.Hosting.Azure.CognitiveServices`   |
| `AddAzureCosmosDB(...)`              | `Aspire.Hosting.Azure.CosmosDB`            |
| `AddAzureEventHubs(...)`             | `Aspire.Hosting.Azure.EventHubs`           |
| `AddAzureKeyVault(...)`              | `Aspire.Hosting.Azure.KeyVault`            |
| `AddAzureLogAnalyticsWorkspace(...)` | `Aspire.Hosting.Azure.OperationalInsights` |
| `AsAzurePostgresFlexibleServer(...)` | `Aspire.Hosting.Azure.PostgreSQL`          |
| `AsAzureRedis(...)`                  | `Aspire.Hosting.Azure.Redis`               |
| `AddAzureSearch(...)`                | `Aspire.Hosting.Azure.Search`              |
| `AddAzureServiceBus(...)`            | `Aspire.Hosting.Azure.ServiceBus`          |
| `AddAzureSignalR(...)`               | `Aspire.Hosting.Azure.SignalR`             |
| `AsAzureSqlDatabase(...)`            | `Aspire.Hosting.Azure.Sql`                 |
| `AddAzureStorage(...)`               | `Aspire.Hosting.Azure.Storage`             |

For more information, see [Azure-specific resource types](../deployment/manifest-format.md#azure-specific-resource-types).

## Upgrade to preview 5

If you're using Visual Studio, see the [Use Upgrade Assistant to update to preview 5](#use-upgrade-assistant-to-update-to-preview-5). One of the largest updates is the need to add a reference to the [Aspire.Hosting.AppHost](https://www.nuget.org/packages/Aspire.Hosting.AppHost) NuGet package. In your AppHost project, add the following package reference:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.AppHost --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.AppHost"
                  Version="[SelectVersion]" />
```

---

In addition to the package reference, some APIs were updated in preview 5. For more information, see [Application model changes](#application-model-changes). Some parameter names changed, while others were removed.

All .NET Aspire NuGet package references should be upgraded to `8.0.0-preview.5.24201.12`. If you've created .NET Aspire apps using any of the previous templates, you'll want to update the _Extensions.cs_ file of the service defaults project to reflect the new APIs, as well as update the project file to reference the new NuGet packages. See [service defaults](../fundamentals/service-defaults.md) for the latest source updates.

Additional considerations for upgrading to preview 5 include:

- [Application model changes](#application-model-changes): explore API changes and update your code where needed.
- [Service Discovery API changes](#service-discovery-api-changes): Update your code to account for changes in the service discovery API.
- [Allow unsecure transport for HTTP endpoints](#allow-unsecure-transport-for-http-endpoints): Ensure that your _launchSettings.json_ file includes an `https` profile.
- [Dashboard security updates](#security-updates): Determine if you're impacted by security updates.
- [Component breaking changes](#component-breaking-changes): Update source code to account for breaking component changes.

## Application model changes

At the core of .NET Aspire's capabilities is the application model. The application model is a set of abstractions that allow you to define the components of your application and how they interact with each other. There were several changes to the application model in preview 5.

### Allow unsecure transport for HTTP endpoints

In preview 5, the **app host will crash** if an `applicationUrl` is configured with an unsecure transport. This can be avoided by setting the `ASPIRE_ALLOW_UNSECURED_TRANSPORT` environment variable. For more information, see [Allow unsecure transport in .NET Aspire](../troubleshooting/allow-unsecure-transport.md).

### Enable forwarded headers by default for .NET projects

If a .NET project is added to your distributed application model, and that project has endpoints defined the `ASPNETCORE_FORWARDEDHEADERS_ENABLED` environment variable is automatically set, which has the effect of enabling handling for forwarded headers. This is because the primary deployment target for Aspire applications is containerized environments where a reverse proxy is deployed in front of the workload.

This can be disabled using the `DisableForwardedHeaders()` extension method.

### Custom resources support in the dashboard

We have made improvements to the application model to allow custom resources to update their status in the dashboard and log console output. This is extremely useful for cloud hosted resources that need to be deployed when an application starts.

Two new services exist within the DI container which can be injected into lifecycle hooks called `ResourceNotificationService` and `ResourceLoggerService`. For more information, see the [CustomResources "playground" sample](https://github.com/dotnet/aspire/blob/1b627b7d5d399d4f9118366e7611e11e56de4554/playground/CustomResources/CustomResources.AppHost/TestResource.cs#L30) in the repo for how to use these APIs.

### Improved volume mount APIs

We have improved the ease of configuring persistence between container restarts for many of the container-based .NET Aspire resources. It's now possible to enable persistence on many containers through the use of an extension method. For example, consider volume mounts:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Uses a volume mount to store data between restarts.
var pg1 = builder.AddPostgres("pgsql1")
                 .WithDataVolume();
```

Likewise, it's possible to use a bind mount to store data between restarts. This is useful when you want to store data on the host machine rather than in the container:

```csharp
var builder = DistributedApplication.CreateBuilder(args)

// Uses a bind mount to store data.
var pg1 = builder.AddPostgres("pgsql1")
                 .WithDataBindVolume(path);
```

We've also worked with the Azure Developer CLI team to add support for creating volume mounts in Azure Storage for container apps when Aspire apps are deployed to Azure.

### RabbitMQ management UI

We've added the ability to enable the RabbitMQ management UI:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var messaging = builder.AddRabbitMQ("messaging")
                       .WithManagementPlugin();
```

When the app host starts up the dashboard will show a HTTP endpoint. If you click on the endpoint you will be prompted for a username and password. The username and password is visible in the environment variables assigned to the container.

### Automatic password generation

In previous previews of Aspire, each resource created a random password when the resource was added to the app model, taking an optional password argument if required. In preview 5 we have modified the API to take a `IResourceBuilder<ParameterResource>` argument for usernames and passwords. If these parameters are omitted a parameter will be automatically injected into the application model with a default random value.

During deployment with tools such as the Azure Developer CLI (`azd`) the password will be generated automatically and stored in a KeyVault for later use by container workloads.

### API for expressing Docker build args

Before preview 5, it wasn't possible to express [Docker build args](https://docs.docker.com/reference/cli/docker/image/build/#build-arg) when an executable resource was published as a `dockerfile.v0`. The app model updated the `PublishAsDockerfile` extension method to accept an `IEnumerable<DockerBuildArg>` parameter. This allows you to pass in build arguments to the Docker build process. This is useful when you want to pass in secrets or other configuration values at build time.

```csharp
var builder = DistributedApplication.CreateBuilder();

var frontend = builder
    .AddNpmApp("frontend", "NodeFrontend", "watch")
    .PublishAsDockerFile(
        buildArgs:
        [
            new DockerBuildArg("NODE_ENV", "staging"),
            new DockerBuildArg("WEATHER_API") // Null means docker pulls the env var value
        ]
    );
```

For more information on the output in the manifest, see [Docker build arguments](#docker-build-arguments).

### Deferred string interpolation for WithEnvironment(...) and reference expressions

We have added a new overload to the `WithEnvironment(...)` extension method which supports interpolated strings:

```csharp
public static IResourceBuilder<T> WithEnvironment<T>(
    this IResourceBuilder<T> builder,
    string name,
    in ReferenceExpression.ExpressionInterpolatedStringHandler value)
```

This allows you to write code like this:

```csharp
var containerA = builder.AddContainer("container1", "image")
                        .WithHttpEndpoint(name: "primary", targetPort: 10005);

var endpoint = containerA.GetEndpoint("primary");

// The {endpoint} placeholder is evaluated AFTER containerA has started
// and the dynamically allocated port can be determined.
var containerB =
    builder.AddContainer("container2", "imageB")
        .WithEnvironment("URL", $"{endpoint}/foo")
        .WithEnvironment("PORT", $"{endpoint.Property(EndpointProperty.Port)}")
        .WithEnvironment("TARGET_PORT", $"{endpoint.Property(EndpointProperty.TargetPort)}")
        .WithEnvironment("HOST", $"{test.Resource};name=1");
```

Underlying this improvement is a series of changes to the way that references between resources are handled. For example resources that expose connection strings now have async methods instead of synchronous methods which allow the start-up of a micro-service to block whilst a cloud resource is being initialized.

A good illustration of the changes here can be seen on the `IResourceWithConnectionString` interface. The code below shows the preview 4 and preview 5 versions one after another.

```csharp
// Preview 4
public interface IResourceWithConnectionString : IResource
{
    string? GetConnectionString();

    string? ConnectionStringExpression => { get; }

    string ConnectionStringReferenceExpression { get; }

    string? ConnectionStringEnvironmentVariable { get; }
}

// Preview 5
public interface IResourceWithConnectionString :
    IResource,
    IManifestExpressionProvider,
    IValueProvider,
    IValueWithReferences
{
    ValueTask<string?> GetConnectionStringAsync(
        CancellationToken cancellationToken = default);

    string IManifestExpressionProvider.ValueExpression { get; }

    ValueTask<string?> IValueProvider.GetValueAsync(
        CancellationToken cancellationToken);

    ReferenceExpression ConnectionStringExpression { get; }

    string? ConnectionStringEnvironmentVariable { get; }

    IEnumerable<object> IValueWithReferences.References { get; }
}
```

The `GetConnectionString` method has been made asynchronous. But there are many other properties that have been added. This allows for better tracking of dependencies within the application model. These are defined by interfaces like `IValueProvider` and `IManifestExpressionProvider`.

The basic usage pattern for adding a reference from one resource to another hasn't changed. The changes above primarily impact the internal implementation details and only impact you if you are building custom resource types or if you are using the string interpolation features mentioned above.

## Dashboard

In preview 5, our primary focus has been on non-functional requirements, particularly around security and performance improvements.

### Structured logs details

Structured logs provide structured, contextual data along with the log message. Previously, the dashboard UI displayed all information related to a structured log entry in a single list.

In preview 5, the dashboard UI has been updated to group structured logs information into sections:

- **Log Entry** - The log level, message, and structured data. This is the most frequently used information.
- **Context** - The context in which the log entry was created. For example, the category and trace IDs.
- **Resource** - The app that sent the log entry to the dashboard.

:::image type="content" source="media/preview-5/dashboard-structure-logs-detail.png" lightbox="media/preview-5/dashboard-structure-logs-detail.png" alt-text="Structure logs page with details open":::

### Trace messaging icon

Spans are created by sending and receiving messages using libraries such as Azure Service Bus or RabbitMQ. In preview 5, messaging spans have custom icons.

Sending a message displays an envelope:

:::image type="content" source="media/preview-5/send-message.png" lightbox="media/preview-5/send-message.png" alt-text="Span with send message icon":::

So of course receiving a message is a mailbox:

:::image type="content" source="media/preview-5/receive-message.png" lightbox="media/preview-5/receive-message.png" alt-text="Span with receive message icon":::

### Display times using browser local timezone

The dashboard now uses the local browser's timezone to display times in the UI. Most users will not notice a change, as the dashboard app typically runs on their local machine. However, if the dashboard is hosted on an external server, times will now be displayed in your local timezone.

### Security updates

Communication across the following endpoints has been secured:

- Dashboard frontend
- OTLP endpoint
- Resource service

There is no change in the user experience when the dashboard automatically launches with your Aspire app; it is configured to be secure by default. For instance, telemetry received by the dashboard is now protected by an API key that is automatically configured by the Aspire app host.

However, if you're launching the dashboard in standalone mode, it will now throw an error. Authentication must either be configured or opted out using the `DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS` setting.

```bash
docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard \
    -e DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS='true' \
    mcr.microsoft.com/dotnet/nightly/aspire-dashboard:8.0.0-preview.5
```

This is temporary situation for preview 5. We're working on providing an easy-to-use authentication mechanism for the dashboard frontend. The standalone dashboard will default to it in the future.

#### OTLP endpoint security

The OTLP endpoint can be secured with [client certificate](/aspnet/core/security/authentication/certauth) or API key authentication.

- `Dashboard:Otlp:AuthMode` specifies the authentication mode on the OTLP endpoint. Possible values are `Certificate`, `ApiKey`, `Unsecured`. This configuration is required.
- `Dashboard:Otlp:ApiKey` specifies the API key for the OTLP endpoint when API key authentication is enabled. This configuration is required for API key authentication.

#### Dashboard authentication

The dashboard's web application frontend supports OpenID Connect (OIDC) for authentication. These can be applied via configurable settings, once `Dashboard:Frontend:AuthMode` is set to `OpenIdConnect`:

- `Authentication:Schemes:OpenIdConnect:Authority` &mdash; URL to the identity provider (IdP)
- `Authentication:Schemes:OpenIdConnect:ClientId` &mdash; Identity of the relying party (RP)
- `Authentication:Schemes:OpenIdConnect:ClientSecret`&mdash; A secret that only the real RP would know
- Other properties of [`OpenIdConnectOptions`](/dotnet/api/microsoft.aspnetcore.builder.openidconnectoptions) specified in configuration container `Authentication:Schemes:OpenIdConnect:*`
  
#### Resource server endpoint security

The resource server client supports client certificates. This can be applied via configurable settings, once ResourceServiceClient:AuthMode to Certificate

- `ResourceServiceClient:ClientCertificate:Source` (required) one of:
  - `File` to load the cert from a file path, configured with:
    - `ResourceServiceClient:ClientCertificate:FilePath` (required, string)
    - `ResourceServiceClient:ClientCertificate:Password` (optional, string)
  - `KeyStore` to load the cert from a key store, configured with:
    - `ResourceServiceClient:ClientCertificate:Subject` (required, string)
    - `ResourceServiceClient:ClientCertificate:KeyStore:Name` (optional, [`StoreName`](/dotnet/api/system.security.cryptography.x509certificates.storename), defaults to `My`)
    - `ResourceServiceClient:ClientCertificate:KeyStore:Location` (optional, [`StoreLocation`](/dotnet/api/system.security.cryptography.x509certificates.storelocation), defaults to `CurrentUser`)
- `ResourceServiceClient:Ssl` (optional, [`SslClientAuthenticationOptions`](/dotnet/api/system.net.security.sslclientauthenticationoptions))

#### Cross-site scripting (XSS) fixes

We've reviewed and hardened the dashboard against XSS attacks by carefully managing how external data from the resource service and telemetry is presented.

For more information, see [Prevent Cross-Site Scripting (XSS) in ASP.NET Core](/aspnet/core/security/cross-site-scripting).

### Performance improvements

Preview 5 includes numerous performance enhancements. We've leveraged virtualization for UI components displaying large amounts of data, allowing for more efficient rendering. Notable improvements include:

- **Resources page** - Optimized resource loading with enabled virtualization for smoother experiences with numerous resources.
- **Console log page** - Previously, loading extensive console logs could impact performance. Virtualization now allows for smooth handling of tens of thousands of console logs.
- **Trace detail virtualization** - The trace detail page shows a timeline of spans inside a trace. There is no limit to the number of spans a trace can have. Enabling virtualization again allows the UI to scale up to thousands of items without a problem.
- **Rate limit UI updates** - The dashboard automatically updates as it receives new data, such as resource state changes or new telemetry. The UI now limits to a maximum of 10 UI updates per-second. This change keeps the dashboard responsive while at the same time not overwhelming your local machine with unnecessarily frequent updates.

Console logs page with tens of thousands of console lines:

:::image type="content" source="media/preview-5/console-logs-performance.gif" lightbox="media/preview-5/console-logs-performance.gif" alt-text="Using console logs page with a lot of data":::

## Templates

- HTTPs by default.
- Test project support. For more information, see [.NET Aspire project templates](../fundamentals/setup-tooling.md#net-aspire-project-templates).

## Service Discovery API changes

In preview 5, we've made changes to the service discovery API. The `UseServiceDiscovery` method was marked as obsolete and replaced with the `AddServiceDiscovery` method. The `AddServiceDiscovery` method is used to add service discovery to the application model. The `UseServiceDiscovery` method is still available but will be removed in preview 6. For more information, see [.NET Aspire service discovery](../service-discovery/overview.md).

In addition to these breaking API changes, service discovery now supports auto scheme selection. It's common to use HTTP while developing and testing a service locally and HTTPS when the service is deployed. Service discovery supports this by allowing for a priority list of URI schemes to be specified in the input string given to Service discovery. Service discovery attempts to resolve the services for the schemes in order and stops after an endpoint is found. URI schemes are separated by a + character, for example: `"https+http://basket"`. Service discovery first tries to find HTTPS endpoints for the "basket" service and then falls back to HTTP endpoints. If any HTTPS endpoint is found, Service Discovery doesn't include HTTP endpoints. For more information, see [Scheme selection when resolving HTTP(S) endpoints](/dotnet/core/extensions/service-discovery?tabs=dotnet-cli#scheme-selection-when-resolving-https-endpoints).

## Developer Tools

For Preview 5 we've improved support with our Visual Studio family of products focusing on supporting new capabilities and improving deployment workflows.

### Visual Studio Code C# DevKit tooling

With the April release of C# Dev Kit, you can now launch all projects in a .NET Aspire from Visual Studio Code. To launch your .NET Aspire application, simply Ctrl-F5 (Run without debugging). This will launch the app host project and all the associated projects in your .NET Aspire application (front-end and APIs). Similarly, you can debug your .NET Aspire application, simply F5 (Start debugging) and all the projects will attach to the debugger, allowing you to have breakpoints set across projects and each one will be hit when appropriate.

### Visual Studio tooling updates

In Visual Studio 17.10 we've continued to improve the end-to-end experience for developers using right-click publish to Azure Container Apps. You no longer need to login separately to `azd`, Visual Studio will log you in seamlessly. Now, when you use the "Remove Environment" feature in the Visual Studio publishing dialog, you can opt for deleting the Azure Developer CLI (azd) environment from your live Azure subscription. This makes it easies for you to iterate, creating and deleting environments with simplicity.

:::image type="content" source="media/preview-5/remove-environment.gif" lightbox="media/preview-5/remove-environment.gif" alt-text="Removing a local and live Azure environment":::

This release of Visual Studio also adds support for the Azure Provisioning features in the release. When you're using Azure resources in a .NET Aspire app, like OpenAI, and need to have use the remote resources during your local development, Visual Studio gives you a way of creating or selecting an existing resource group in which those resources can be provisioned. This animation shows the process of adding OpenAI support to an Aspire app, then running the app only to learn the resources need to be created with the warnings in the Aspire dashboard. At this point, you can flip back into Visual Studio and use Connected Services to set up Azure provisioning so your dev-time resources can be created on the fly.

:::image type="content" source="media/preview-5/aspire-preview-5-relnotes.gif" lightbox="media/preview-5/aspire-preview-5-relnotes.gif" alt-text="Using Connected Services to configure Azure provisioning":::

#### Use Upgrade Assistant to update to preview 5

We've also released a new version of the [Upgrade Assistant](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.upgradeassistant), a Visual Studio extension that helps you upgrade projects in various scenarios. We've added support for upgrading .NET Aspire Preview 4 projects to Preview 5. Install the extension and run 'Upgrade' from the context menu of an Aspire-related project like the `AppHost` or client projects to update packages and API changes.

:::image type="content" source="media/preview-5/vs-upgrade-assistant.png" lightbox="media/preview-5/vs-upgrade-assistant.png" alt-text="Visual Studio Upgrade Assistant with .NET Aspire option":::

You will need to run this on each project in your solution.

## Resources and components

Each release brings new resources and components thanks to community feedback and contributions.

### New resources and components

The following list outlines new components and their corresponding component articles:

- [Azure Event Hubs](https://azure.microsoft.com/products/event-hubs): [.NET Aspire Azure Event Hubs component](../messaging/azure-event-hubs-component.md).
- [NATS](https://nats.io/): [.NET Aspire support for NATS](../messaging/nats-component.md).
- [Seq](https://datalust.co/seq): [.NET Aspire support for Seq](../logging/seq-component.md).

### Component breaking changes

In previous versions, it was confusing what project the following code snippet is from:

```csharp
builder.AddRedis("redis");
```

because both the `AppHost` and application projects have extension methods named `builder.AddRedis(string)`. To help reduce this confusion, the runtime component libraries renamed their extension methods to append the word `Client` on the extension methods.

```csharp
builder.AddRedisClient("redis");
```

This makes it clear that we are adding a "client" object to the `WebApplicationBuilder` or `HostApplicationBuilder`.

## Azure improvements

The primary focus of the Azure improvements in preview 5 is to make it easier to use Azure resources in your .NET Aspire application. This includes improvements to the Azure provisioning libraries, support for local development with Azure resources, and other Azure-related improvements.

### Azure provisioning libraries

In preview 5 the Azure-specific extensions for .NET Aspire have adopted the Azure Provisioning libraries being developed by the Azure SDK team. The Azure Provisioning libraries allow as to use a C# object model to declare Azure resources and at deployment time translate that object model into Bicep which is then used to automate deployment.

If you are already using Azure-based resources with your .NET Aspire application the APIs you use today continue to work. For example the following code creates an Azure Cosmos database and wires up the connection string to your application.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder.AddAzureCosmosDB("mycosmos")
                    .AddDatabase("inventory");

builder.AddProject<Projects.InventoryApi>("inventoryapi")
       .WithReference(cosmos);
```

In preview 4, if you didn't like the defaults that we specified for Azure Cosmos you would need to provide your own Bicep and use the `AddBicepTemplate` extension. In preview 5 we provide a callback mechanism which allows you to tweak properties.

For example, if you wanted to modify the consistency level of your Cosmos DB account you could do the following:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cosmos = builder
    .AddAzureCosmosDB(
        "mycosmos",
        (resource, construct, account, databases) =>
        {
            account.AssignProperty(
                p => p.ConsistencyPolicy.DefaultConsistencyLevel,
                "`Session`");
        }
    )
    .AddDatabase("inventory");

builder.AddProject<Projects.InventoryApi>("inventoryapi")
       .WithReference(cosmos);
```

Azure Provisioning libraries are still experimental and evolving rapidly. Developers are free to use them in their applications but expect the API surface to evolve before reaching stability. To remind developers of this, using the overloads that expose Azure Provisioning types will require the use of a code analysis suppression:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

#pragma warning disable ASPIRE001
var cosmos = builder
    .AddAzureCosmosDB(
        "mycosmos",
        (resource, construct, account, databases) =>
        {
            account.AssignProperty(
                p => p.ConsistencyPolicy.DefaultConsistencyLevel,
                "`Session`"
            );
        }
    )
    .AddDatabase("inventory");
#pragma warning restore ASPIRE001

builder.AddProject<Projects.InventoryApi>("inventoryapi")
       .WithReference(cosmos);
```

This can be applied globally in your project or in a more localized way as shown above. For more information, see [.NET Aspire diagnostics overview](../diagnostics/overview.md).

### Azure provisioning for local development

Previous .NET Aspire previews have had limited support for using cloud based resources for local development. You could use an emulator (such as Azurite for Azure Storage) or you could provision a real resource in the cloud and place a connection string in your AppHost's user secrets.

In preview 5 if you want to use an Azure resource where an emulator does not exist you can add
the following settings to user `secrets.json` file:

```json
{
  "Azure": {
    "SubscriptionId": "<subscription id>",
    "Location": "<default location>",
    "ResourceGroup": "<resource group name>"
  }
}
```

When you launch the AppHost, the dashboard shows that it's creating the Azure resources for you and provides helpful links to deployments in the Azure portal. In case of failure, it provides logs that hint as to what might be causing the deployment issue.

:::image type="content" source="media/preview-5/azure-resource-provisioning-on-dashboard.png" lightbox="media/preview-5/azure-resource-provisioning-on-dashboard.png" alt-text=".NET Aspire dashboard: Azure provisioning.":::

To use Azure provisioning you only need to make use of one of an Azure resource. For example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var blobs = builder.AzureStorage("storage")
                   .AddBlobs("blobs");

builder.AddProject<Projects.GalleryApp>("galleryapp")
       .WithReference(blobs);
```

Some resources in .NET Aspire such as Postgres are not cloud specific and can run locally in a container but when deployed use a managed service (such as Azure Postgres Flexible Server).

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("pgsql")
                .PublishAsAzurePostgresFlexibleServer()
                .AddDatabase("inventorydb");

builder.AddProject<Projects.InventoryApi>("inventoryapi")
       .WithReference(db);
```

It's also possible to use an Azure hosted resource for local development by using the `AsAzurePostgresFlexibleServer()` extension method instead. When this method is used the container isn't started locally and a cloud-based instance is created, just like the Azure-only resource types. The `PublishAsX` and `AsX` methods also support callbacks to customize the underlying Azure resources as shown above in the Cosmos DB example.

For more information, see [Local Azure provisioning](../deployment/azure/local-provisioning.md).

### No more exposed endpoint selection in AZD

In previous previews, when an Aspire application was deployed to Azure, part of the initialization
process for Azure Developer CLI (`azd`) was to select the services that would be exposed externally
to the Internet. This included container-based resources that may or may not be hardened for Internet
access.

In preview 5 we have worked with the Azure Developer CLI team to not display this prompt. Now, by default
endpoints on all resources only accessible only within the Azure Container Apps environment. To make http endpoints externally accessible on a resource you need to use the `WithExternalHttpEndpoints(...)` extension
to enable this explicitly in the application model.

```csharp
builder.AddContainer("grafana", "grafana/grafana")
       .WithHttpEndpoint(name: "http", targetPort: 3000)
       .WithExternalHttpEndpoints();
```

This will expose all endpoints defined on the resource. Alternatively it is possible to modify a single
endpoint when defining it:

```csharp
var catalogDb = builder.AddPostgres("postgres")
                       .WithPgAdmin()
                       .WithEndpoint("tcp", endpoint =>
                       {
                           // This callback can be used for mutating other 
                           // values on existing endpoints as well.
                           endpoint.IsExternal = true;
                       })
                       .AddDatabase("catalogdb");
```

### Azure OpenAI

The `AddAzureOpenAI(...)` extension method will now result in an Azure Open AI resource being provisioned in Azure. This support was missing from preview 4 but was added in preview 5 as part of the overall improvements to Azure resource usage for local development mentioned above.

```csharp
var openai = builder
    .AddAzureOpenAI("openai")
    .AddDeployment(new("mydeployment", "gpt-35-turbo", "0613"));
```

> [!NOTE]
> Currently the Azure Open AI resource provider in Azure does not allow two model deployments at the same time. If you want to deploy multiple models within your application you will need to use separate Azure Open AI resources.

### Azure Event Hubs

Also in preview 5 we added support for Azure Event Hubs. You can add Azure Event Hubs to your application model using the `AddAzureEventHubs(...)` extension method. This will result in the creation of an Event Hubs namespace. Use the `AddHub(...)` method.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// required for the event processor client which will use the connectionName to get the connectionString.
var blob = builder.AddAzureStorage("ehstorage")
                  .AddBlobs("checkpoints");

var eventHub = builder.AddAzureEventHubs("eventhubns")
                      .AddEventHub("hub");

builder.AddProject<Projects.EventHubsConsumer>("consumer")
       .WithReference(eventHub)
       .WithReference(blob);

builder.AddProject<Projects.EventHubsApi>("api")
       .WithExternalHttpEndpoints()
       .WithReference(eventHub);

builder.Build().Run();
```

As can be seen above the Event Hubs integration includes support for the Event Hubs processor architecture not just the consumer model (although either can be used). Refer to the README for the `Aspire.Azure.Messaging.EventHubs` package for examples of how to wire-up to the provisioned Event Hubs resource in your service projects.

For more information, see [.NET Aspire Azure Event Hubs component](../messaging/azure-event-hubs-component.md).

## AWS support

Starting with .NET Aspire preview 5, you can use the [Aspire.Hosting.AWS](https://www.nuget.org/packages/Aspire.Hosting.AWS) NuGet package to express AWS resources in your application model. This package provides a set of extension methods that allow you to add AWS resources to your application model.

### Configuring the AWS SDK for .NET

The AWS profile and region the SDK should use can be configured using the `AddAWSSDKConfig` method. The following example creates a config using the dev profile from the `~/.aws/credentials` file and points the SDK to the
`us-west-2` region.

```csharp
var awsConfig = builder.AddAWSSDKConfig()
                        .WithProfile("dev")
                        .WithRegion(RegionEndpoint.USWest2);
```

The configuration can be attached to projects using the `WithReference` method. This will set the `AWS_PROFILE` and `AWS_REGION` environment variables on the project to the profile and region configured by the `AddAWSSDKConfig` method. SDK service clients created in the project without explicitly setting the credentials and region will pick up these environment variables and use them to configure the service client.

```csharp
builder.AddProject<Projects.Frontend>("Frontend")
        .WithReference(awsConfig)
```

### Provision app resources with AWS CloudFormation

AWS application resources like Amazon DynamoDB tables or Amazon Simple Queue Service (SQS) queues can be provisioning during AppHost startup using a CloudFormation template.

In the AppHost project create either a JSON or YAML CloudFormation template. Here is an example template called `app-resources.template` that creates a queue and topic.

```json
{
    "AWSTemplateFormatVersion" : "2010-09-09",
    "Parameters" : {
        "DefaultVisibilityTimeout" : {
            "Type" : "Number",
            "Description" : "The default visiblity timeout for messages in SQS queue."
        }
    },
    "Resources" : {
        "ChatMessagesQueue" : {
            "Type" : "AWS::SQS::Queue",
            "Properties" : {
                "VisibilityTimeout" : { "Ref" : "DefaultVisibilityTimeout" }
            }
        },
        "ChatTopic" : {
            "Type" : "AWS::SNS::Topic",
            "Properties" : {
                "Subscription" : [
                    {"Protocol" : "sqs", "Endpoint" : {"Fn::GetAtt" : [ "ChatMessagesQueue", "Arn"]}}
                ]
            }
        }
    },
    "Outputs" : {
        "ChatMessagesQueueUrl" : {
            "Value" : { "Ref" : "ChatMessagesQueue" }
        },
        "ChatTopicArn" : {
            "Value" : { "Ref" : "ChatTopic" }
        }
    }
}
```

In the AppHost the `AddAWSCloudFormationTemplate` method is used to register the CloudFormation resource. The first parameter, which is the Aspire resource name, is used as the CloudFormation stack name. If the template defines parameters the value can be provided using  the `WithParameter` method. To configure what AWS account and region to deploy the CloudFormation stack, the `WithReference` method is used to associate a SDK configuration.

```csharp
var awsResources = builder.AddAWSCloudFormationTemplate("AspireSampleDevResources", "app-resources.template")
                          .WithParameter("DefaultVisibilityTimeout", "30")
                          .WithReference(awsConfig);
```

The outputs of a CloudFormation stack can be associated to a project using the `WithReference` method.

```csharp
builder.AddProject<Projects.Frontend>("Frontend")
       .WithReference(awsResources);
```

The output parameters from the CloudFormation stack can be found in the `IConfiguration` under the `AWS:Resources` config section. The config section can be changed by setting the `configSection` parameter of the `WithReference` method associating the CloudFormation stack to the project.

```csharp
var chatTopicArn = builder.Configuration["AWS:Resources:ChatTopicArn"];
```

Alternatively a single CloudFormation stack output parameter can be assigned to an environment variable using the `GetOutput` method.

```csharp
builder.AddProject<Projects.Frontend>("Frontend")
       .WithEnvironment("ChatTopicArnEnv", awsResources.GetOutput("ChatTopicArn"))
```

### Import existing AWS resources

To import AWS resources that were created by a CloudFormation stack outside of the AppHost the `AddAWSCloudFormationStack` method can be used. It will associated the outputs of the CloudFormation stack the same as the provisioning method `AddAWSCloudFormationTemplate`.

```csharp
var awsResources = builder.AddAWSCloudFormationStack("ExistingStackName")
                          .WithReference(awsConfig);

builder.AddProject<Projects.Frontend>("Frontend")
       .WithReference(awsResources);
```

## Manifest changes

The manifest format has been updated to support the new features in preview 5. The following sections outline the changes to the manifest format.

### Volume in the manifest

One of the features we have been wanted to add for some time is support
for volumes in the manifest. Volumes are essential for some scale out scenarios
around containers and for resiliency.

Here is an example of a container in the manifest which defines multiple volumes:

```json
{
    "type": "container.v0",
    "image": "image/name:latest",
    "volumes":
    [
      {
          "name": "myvolume",
          "target": "/mount/here",
          "readOnly": false
      },
      {
          "name": "myreadonlyvolume",
          "target": "/mount/there",
          "readOnly": true
      },
      {
          "target": "/mount/everywhere",
          "readOnly": false
      }
    ]
}
```

> [!NOTE]
> The manifest can express both volumes and bind mounts. Bind mounts require a physical mapping from the host machine and may not work in all deployment scenarios.

It's up to the deployment tool that you are using to deploy the Aspire application to interpret these volume mounts and the technology that supports them. For example when using the Azure Developer CLI (`azd`) an Azure Storage account is created which exposes an Azure Files endpoint—and this is bound to the Azure Container App.

A tool that targets Kubernetes might use Kubernetes' own concept of volumes, or one of the many storage providers that Kubernetes supports.

### Endpoints

We have expended the level of support for defining multiple endpoints in the manifest. To support this we added the `"port":` field to items in the `"bindings":` property on `container.v0` and `project.v0` resources. This port defines the exposed port that the target deployment environment will use when exposing the service. If not explicitly provided this port is assigned (in sequence) at manifest generation time.

> [!IMPORTANT]
> The `containerPort` property has been renamed to `targetPort` to make it a little bit more compute agnostic.

We've added a new API to express endpoints as external endpoints in the manifest, with the `WithExternalHttpEndpoints` extension method. This is useful when you want to expose a service to the Internet. Consider the following JSON snippet before calling this API and then after as an example:

**Before:**

```json
"https": {
  "scheme": "https",
  "protocol": "tcp",
  "transport": "http"
},
```

**After:**

```json
"https": {
  "scheme": "https",
  "protocol": "tcp",
  "transport": "http",
  "external": true
},
```

We've worked with the Azure Developer CLI team to make sure `azd` supports these new endpoint features when deploying workloads to Azure Container Apps.

### Docker build arguments

We have added support for Docker build arguments in the manifest. This is useful when you want to pass in secrets or other configuration values at build time. Here is an example of Docker build arguments are represented in the manifest:

```json
{
  "type": "dockerfile.v0",
  "path": "NodeFrontend/Dockerfile",
  "context": "NodeFrontend",
  "buildArgs": {
    "NODE_ENV": "production",
    "WEATHER_API": null
  },
  "env": {
    "NODE_ENV": "production"
  }
}
```

## Known issues

As known issues for `preview-5` are discovered, they will be listed here.

When running a .NET Aspire app, we sometimes see that the run session fails to start and displays the error "context deadline exceeded." With this occurs, the error output resembles the following:

```Output
run session could not be started: {"Executable": {"name":"<app/service-name>"}, "Reconciliation": <Number>, "error": "Put \"<http://localhost:4317/v1/run_session>: context deadline exceeded"}"
```

For more information, see [GitHub dotnet/aspire: Issue #3435](https://github.com/dotnet/aspire/issues/3435).
