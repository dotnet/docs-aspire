---
title: Custom resource commands
description: Learn how to create custom resource commands in .NET Aspire.
ms.date: 08/07/2025
ms.topic: how-to
ms.custom: sfi-ropc-nochange
---

# Custom resource commands in .NET Aspire

Each resource in the .NET Aspire [app model](app-host-overview.md#define-the-app-model) is represented as an <xref:Aspire.Hosting.ApplicationModel.IResource> and when added to the [distributed application builder](xref:Aspire.Hosting.IDistributedApplicationBuilder), it's the generic-type parameter of the <xref:Aspire.Hosting.ApplicationModel.IResourceBuilder`1> interface. You use the _resource builder_ API to chain calls, configuring the underlying resource, and in some situations, you might want to add custom commands to the resource. Some common scenario for creating a custom command might be running database migrations or seeding/resetting a database. In this article, you learn how to add a custom command to a Redis resource that clears the cache.

> [!IMPORTANT]
> These [.NET Aspire dashboard](dashboard/overview.md) commands are only available when running the dashboard locally. They're not available when running the dashboard in Azure Container Apps.

## Add custom commands to a resource

Start by creating a new .NET Aspire Starter App from the [available templates](aspire-sdk-templates.md). To create the solution from this template, follow the [Quickstart: Build your first .NET Aspire solution](../get-started/build-your-first-aspire-app.md). After creating this solution, add a new class named _RedisResourceBuilderExtensions.cs_ to the [app host project](app-host-overview.md#apphost-project). Replace the contents of the file with the following code:

:::code source="snippets/custom-commands/AspireApp/AspireApp.AppHost/RedisResourceBuilderExtensions.cs":::

The preceding code:

- Shares the <xref:Aspire.Hosting> namespace so that it's visible to the AppHost project.
- Is a `static class` so that it can contain extension methods.
- It defines a single extension method named `WithClearCommand`, extending the `IResourceBuilder<RedisResource>` interface.
- The `WithClearCommand` method registers a command named `clear-cache` that clears the cache of the Redis resource.
- The `WithClearCommand` method returns the `IResourceBuilder<RedisResource>` instance to allow chaining.

The `WithCommand` API adds the appropriate annotations to the resource, which are consumed in the [.NET Aspire dashboard](dashboard/overview.md). The dashboard uses these annotations to render the command in the UI. Before getting too far into those details, let's ensure that you first understand the parameters of the `WithCommand` method:

- `name`: The name of the command to invoke.
- `displayName`: The name of the command to display in the dashboard.
- `executeCommand`: The `Func<ExecuteCommandContext, Task<ExecuteCommandResult>>` to run when the command is invoked, which is where the command logic is implemented.
- `updateState`: The `Func<UpdateCommandStateContext, ResourceCommandState>` callback is invoked to determine the "enabled" state of the command, which is used to enable or disable the command in the dashboard.
- `iconName`: The name of the icon to display in the dashboard. The icon is optional, but when you do provide it, it should be a valid [Fluent UI Blazor icon name](https://www.fluentui-blazor.net/Icon#explorer).
- `iconVariant`: The variant of the icon to display in the dashboard, valid options are `Regular` (default) or `Filled`.

## Execute command logic

The `executeCommand` delegate is where the command logic is implemented. This parameter is defined as a `Func<ExecuteCommandContext, Task<ExecuteCommandResult>>`. The `ExecuteCommandContext` provides the following properties:

- `ExecuteCommandContext.ServiceProvider`: The `IServiceProvider` instance that's used to resolve services.
- `ExecuteCommandContext.ResourceName`: The name of the resource instance that the command is being executed on.
- `ExecuteCommandContext.CancellationToken`: The <xref:System.Threading.CancellationToken> that's used to cancel the command execution.

In the preceding example, the `executeCommand` delegate is implemented as an `async` method that clears the cache of the Redis resource. It delegates out to a private class-scoped function named `OnRunClearCacheCommandAsync` to perform the actual cache clearing. Consider the following code:

```csharp
private static async Task<ExecuteCommandResult> OnRunClearCacheCommandAsync(
    IResourceBuilder<RedisResource> builder,
    ExecuteCommandContext context)
{
    var connectionString = await builder.Resource.GetConnectionStringAsync() ??
        throw new InvalidOperationException(
            $"Unable to get the '{context.ResourceName}' connection string.");

    await using var connection = ConnectionMultiplexer.Connect(connectionString);

    var database = connection.GetDatabase();

    await database.ExecuteAsync("FLUSHALL");

    return CommandResults.Success();
}
```

The preceding code:

- Retrieves the connection string from the Redis resource.
- Connects to the Redis instance.
- Gets the database instance.
- Executes the `FLUSHALL` command to clear the cache.
- Returns a `CommandResults.Success()` instance to indicate that the command was successful.

## Update command state logic

The `updateState` delegate is where the command state is determined. This parameter is defined as a `Func<UpdateCommandStateContext, ResourceCommandState>`. The `UpdateCommandStateContext` provides the following properties:

- `UpdateCommandStateContext.ServiceProvider`: The `IServiceProvider` instance that's used to resolve services.
- `UpdateCommandStateContext.ResourceSnapshot`: The snapshot of the resource instance that the command is being executed on.

The immutable snapshot is an instance of `CustomResourceSnapshot`, which exposes all sorts of valuable details about the resource instance. Consider the following code:

```csharp
private static ResourceCommandState OnUpdateResourceState(
    UpdateCommandStateContext context)
{
    var logger = context.ServiceProvider.GetRequiredService<ILogger<Program>>();

    if (logger.IsEnabled(LogLevel.Information))
    {
        logger.LogInformation(
            "Updating resource state: {ResourceSnapshot}",
            context.ResourceSnapshot);
    }

    return context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy
        ? ResourceCommandState.Enabled
        : ResourceCommandState.Disabled;
}
```

The preceding code:

- Retrieves the logger instance from the service provider.
- Logs the resource snapshot details.
- Returns `ResourceCommandState.Enabled` if the resource is healthy; otherwise, it returns `ResourceCommandState.Disabled`.

## Test the custom command

To test the custom command, update your AppHost project's _Program.cs_ file to include the following code:

:::code source="snippets/custom-commands/AspireApp/AspireApp.AppHost/Program.cs" highlight="4":::

The preceding code calls the `WithClearCommand` extension method to add the custom command to the Redis resource. Run the app and navigate to the .NET Aspire dashboard. You should see the custom command listed under the Redis resource. On the **Resources** page of the dashboard, select the ellipsis button under the **Actions** column:

:::image source="media/custom-clear-cache-command.png" lightbox="media/custom-clear-cache-command.png" alt-text=".NET Aspire dashboard: Redis cache resource with custom command displayed.":::

The preceding image shows the **Clear cache** command that was added to the Redis resource. The icon displays as a rabbit crosses out to indicate that the speed of the dependant resource is being cleared.

Select the **Clear cache** command to clear the cache of the Redis resource. The command should execute successfully, and the cache should be cleared:

:::image source="media/custom-clear-cache-command-succeeded.png" lightbox="media/custom-clear-cache-command-succeeded.png" alt-text=".NET Aspire dashboard: Redis cache resource with custom command executed.":::

## See also

- [Custom HTTP commands in .NET Aspire](http-commands.md)
- [.NET Aspire dashboard: Resource submenu actions](dashboard/explore.md#resource-submenu-actions)
- [.NET Aspire orchestration overview](app-host-overview.md)
