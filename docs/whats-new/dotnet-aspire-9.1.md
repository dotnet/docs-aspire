---
title: What's new in .NET Aspire 9.1
description: Learn what's new in the official general availability release of .NET Aspire 9.1.
ms.date: 02/20/2025
---

# What's new in .NET Aspire 9.1

📢 .NET Aspire 9.1 is the next minor version release of .NET Aspire; it supports _both_:

- .NET 8.0 Long Term Support (LTS) _or_
- .NET 9.0 Standard Term Support (STS).

> [!NOTE]
> You're able to use .NET Aspire 9.1 with either .NET 8 or .NET 9!

As always, we focused on highly requested features and pain points from the community. Our theme for 9.1 was "polish, polish, polish"—so you see quality of life fixes throughout the whole platform. Some highlights from this release are resource relationships in the dashboard, support for working in GitHub Codespaces, and publishing resources as a Dockerfile.

If you have feedback, questions, or want to contribute to .NET Aspire, collaborate with us on [:::image type="icon" source="../media/github-mark.svg" border="false"::: GitHub](https://github.com/dotnet/aspire) or join us on [:::image type="icon" source="../media/discord-icon.svg" border="false"::: Discord](https://discord.com/invite/h87kDAHQgJ) to chat with team members.

For more information on the official .NET version and .NET Aspire version support, see:

- [.NET support policy](https://dotnet.microsoft.com/platform/support/policy): Definitions for LTS and STS.
- [.NET Aspire support policy](https://dotnet.microsoft.com/platform/support/policy/aspire): Important unique product life cycle details.

## Upgrade to .NET Aspire 9.1

Moving between minor releases of .NET Aspire is simple:

1. In your app host project file (that is, _MyApp.AppHost.csproj_), update the `Aspire.AppHost.Sdk` version to `9.1.0`:

    ```xml
    <Project Sdk="Microsoft.NET.Sdk">

        <Sdk Name="Aspire.AppHost.Sdk" Version="9.1.0" />
        
        <!-- Omitted for brevity -->
    
    </Project>
    ```

1. Check for any NuGet package updates, either using the NuGet Package Manager in Visual Studio or the **Update NuGet Package** command in VS Code.
1. Update to the latest .NET Aspire templates by running the following .NET command line:

    ```dotnetcli
    dotnet new update
    ```

    > [!NOTE]
    > The `dotnet new update` command updates all of your templates to the latest version.

If your app host project file doesn't have the `Aspire.AppHost.Sdk` reference, you might still be using .NET Aspire 8. To upgrade to 9.1, you can follow [the documentation from last release](../get-started/upgrade-to-aspire-9).

## Dashboard UX and customization

With every release of .NET Aspire, the [dashboard](../fundamentals/dashboard/overview.md) gets more powerful and customizable.

### Resource relationships

The dashboard now supports "parent" and "child" resource relationships. For instance, when you create a Postgres instance with multiple databases, these databases are nested under the same instance on the **Resource** page.

:::image type="content" source="media/dashboard-parentchild.png" lightbox="media/dashboard-parentchild.png" alt-text="A screenshot of the .NET Aspire dashboard showing the Postgres resource with a database nested underneath it.":::

### Localization overrides

The dashboard defaults to the language set in your browser. This release introduces the ability to override this setting and change the dashboard language independently from the browser language. Consider the following screen capture, that demonstrates the addition of the language dropdown in the dashboard:

:::image type="content" source="media/dashboard-language.png" lightbox="media/dashboard-language.png" alt-text="A screenshot of the .NET Aspire dashboard showing the new flyout menu to change language.":::

### Clear logs and telemetry from the dashboard

New buttons were added to the **Console logs**, **Structured logs**, **Traces** and **Metrics** pages to clear data. There is also a "Remove all" button in the settings popup to remove everything with one action.

Now you use this feature to reset the dashboard to a blank slate, test your app, view only the relevant logs and telemetry, and repeat.

:::image type="content" source="media/dashboard-remove-telemetry.png" lightbox="media/dashboard-remove-telemetry.png" alt-text="A screenshot of the .NET Aspire dashboard showing the remove button on the structured logs page.":::

This feature is a community contribution from [@Daluur](https://github.com/Daluur).

### Filtering

You can now filter what you see in the **Resource** page by **Resource type**, **State**, and **Health state**. Consider the following screen capture, which demonstrates the addition of the filter options in the dashboard:

:::image type="content" source="media/dashboard-filter.png" lightbox="media/dashboard-filter.png" alt-text="A screenshot of the .NET Aspire dashboard showing the new filter options.":::

### More resource details

When you select a resource in the dashboard, more data points are now available in the details pane, including **References**, **Back references**, and **Volumes** with their mount types.

:::image type="content" source="media/dashboard-resourcedetails.png" lightbox="media/dashboard-resourcedetails.png" alt-text="A screenshot of the .NET Aspire dashboard with references and back references showing.":::

### CORS support for custom local domains

You can now set the `DOTNET_DASHBOARD_CORS_ALLOWED_ORIGINS` environment variable to allow the dashboard to receive telemetry from other browser apps, such as if you have resources running on custom localhost domains.

For more information, see [.NET Aspire app host: Dashboard configuration](../app-host/configuration.md#dashboard).

### Flexibility with console logs

The console log page has two new options. You're now able to download your logs so you can view them in your own diagnostics tools. Plus, you can turn timestamps on or off to reduce visual clutter when needed.

:::image type="content" source="media/consolelogs-download.png" lightbox="media/consolelogs-download.png" alt-text="A screenshot of the console logs page with the download button, turn off timestamps button, and logs that don't show timestamps.":::

### Various UX improvements

Several new features in .NET Aspire 9.1 enhance and streamline popular tasks:

- Resource commands, such as **Start** and **Stop** buttons, are now available on the **Console logs** page.
- Single selection to open in the _text visualizer_.
- URLs within logs are now automatically clickable, with commas removed from endpoints.

Additionally, the scrolled position now resets when switching between different resources.

For more details on the latest dashboard enhancements, check out [James Newton-King](https://bsky.app/profile/james.newtonking.com) on Bluesky, where he's been sharing new features daily.

## Local dev

### Start resources on demand

You can now tell resources not to start with the rest of your app by using `WithExplicitStart()` on the resource in your app host. Then, you can start it whenever you're ready from inside the dashboard.

For more information, see [Configure explicit resource start](../fundamentals/app-host-overview.md#configure-explicit-resource-start).

### Better Docker integration

The `PublishAsDockerfile()` feature was introduced for all projects and executable resources. This enhancement allows for complete customization of the Docker container and Dockerfile used during the publish process.

### Cleaning up Docker networks

In 9.1, we addressed a persistent issue where Docker networks created by .NET Aspire would remain active even after the application was stopped. This bug, tracked in [issue #6504](https://github.com/dotnet/aspire/issues/6504), is resolved. Now, Docker networks are properly cleaned up, ensuring a more efficient and tidy development environment.

## Integrations

.NET Aspire thrives on [its integrations](../fundamentals/integrations-overview.md) with other platforms. This release has numerous updates to existing integrations, and details about migrations of ownership.

### Azure updates

This release also focused on improving various [Azure integrations](../azure/integrations-overview.md):

#### New emulators

We are very excited to bring new emulators for making local development much easier. The following integrations got new emulators in this release:

- [Azure Service Bus](../messaging/azure-service-bus-integration.md#add-azure-service-bus-emulator-resource)
- [Azure Cosmos DB Linux-based (preview)](../database/includes/cosmos-app-host.md#use-linux-based-emulator-preview)
- [Azure SignalR](/azure/azure-signalr/signalr-howto-emulator)

These new emulators work side-by-side with the existing emulators for:

- [Azure Storage](../storage/azure-storage-integrations.md)
- [Azure EventHubs](../messaging/azure-event-hubs-integration.md#add-azure-event-hubs-emulator-resource)
- [Azure Cosmos DB](../database/includes/cosmos-app-host.md#add-azure-cosmos-db-emulator-resource)

#### Cosmos DB

Along with support for the new emulator, Cosmos DB added the following features.

1. Support for Entra ID authentication by default

Previously, the Cosmos DB integration used access keys and a Key Vault secret to connect to the service. .NET Aspire 9.1 added support for using more secure authentication using managed identities by default. If you need to keep using access key authentication, you can get back to the previous behavior by calling `.WithAccessKeyAuthentication()`.

2. Support for modeling Database and Containers in the AppHost

You can define CosmosDB database and containers in the AppHost and these resources will be available when you run the application in both the emulator and in Azure. This allows you to define these resources up front and no longer need to create them from the application, which may not have permissions to create them.

#### Service Bus and Event Hubs

Similar to Cosmos DB above, the Service Bus and Event Hubs integrations allow you to model Azure Service Bus queues, topics, and subscriptions, and Azure Event Hubs hub instances and consumer groups in AppHost code. These resources can now be created outside of the applications that use them, simplifying app logic.

#### Working with existing resources

We've heard a lot of feedback to make it easier to connect to existing Azure resources in .NET Aspire. With 9.1, you can now easily connect to an existing Azure resource either directly with strings, or via app model parameters which can be changed at deployment time. For example to connect to an Azure Service Bus account, we can use the following code:

```csharp
var serviceBusName = builder.AddParameter("serviceBusName");
var serviceBusResourceGroup = builder.AddParameter("serviceBusResourceGroup");

var serviceBus = builder.AddAzureServiceBus("messaging")
                        .AsExisting(existingServiceBusName, existingServiceBusResourceGroup);
```

This code will read the name and resource group from the parameters, and connect to the existing resource when the application is run or deployed. For more information see [use existing Azure resources](../azure/integrations-overview.md#use-existing-azure-resources).

#### Azure Container Apps

Experimental support for configuring custom domains in Azure Container Apps (ACA) was added. For example:

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

### Even more integration updates

- OpenAI now supports the [📦 Microsoft.Extensions.AI](https://www.nuget.org/packages/Microsoft.Extensions.AI) NuGet package.
- RabbitMQ updated to version 7, and MongoDB to version 3. These updates introduced breaking changes, leading to the release of new packages with version-specific suffixes. The original packages continue to use the previous versions:
  - [📦 Aspire.RabbitMQ.Client.v7](https://www.nuget.org/packages/Aspire.RabbitMQ.Client.v7) NuGet package. For more information, see the [.NET Aspire RabbitMQ client integration](../messaging/rabbitmq-integration.md#client-integration) documentation.
  - [📦 Aspire.MongoDB.Driver.v3](https://www.nuget.org/packages/Aspire.MongoDB.Driver.v3) NuGet package. For more information, see the [.NET Aspire MongoDB client integration](../database/mongodb-integration.md#client-integration) documentation.
- Dapr migrated to the [CommunityToolkit](https://github.com/CommunityToolkit/Aspire/tree/main/src/CommunityToolkit.Aspire.Hosting.Dapr) to facilitate faster innovation.
- Numerous other integrations received updates, fixes, and new features. For detailed information, refer to our [GitHub release notes](https://github.com/dotnet/aspire/releases).

The [📦 Aspire.Hosting.AWS](https://www.nuget.org/packages/Aspire.Hosting.AWS) NuGet package and source code migrated under [AWS ownership](https://github.com/aws/integrations-on-dotnet-aspire-for-aws). This migration happened as part of .NET Aspire 9.0, we're just restating that change here.

## Deployment

Significant improvements to the Azure Container Apps (ACA) deployment process can be found in 9.1, enhancing both the `azd` CLI and app host options. One of the most requested features—support for deploying npm applications to ACA—is now implemented. This new capability allows npm applications to be deployed to ACA just like other resources, streamlining the deployment process and providing greater flexibility for developers.

## Upgrade today

Follow the directions outlined in the [Upgrade to .NET Aspire 9.1](#upgrade-to-net-aspire-91) section to make the switch to 9.1 and take advantage of all these new features today! As always, we're listening for your feedback on [GitHub](https://github.com/dotnet/aspire/issues)-and looking out for what you want to see in 9.2 ☺️.
