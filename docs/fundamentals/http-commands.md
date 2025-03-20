---
title: Custom HTTP commands in .NET Aspire
description: Learn how to create custom HTTP commands in .NET Aspire.
ms.date: 03/20/2025
ms.topic: how-to
---

# Custom HTTP commands in .NET Aspire

In .NET Aspire, you can add custom HTTP commands to resources using the `WithHttpCommand` API. This API extends existing functionality, where you can provide [custom resource commands](custom-resource-commands.md). This feature enables a command on a resource that sends an HTTP request to the specified endpoint and path. This is useful for scenarios such as triggering database migrations, clearing caches, or performing custom actions on resources through HTTP requests.

## Add HTTP commands to a resource

The available APIs provide extensive capabilities with numerous parameters to customize the HTTP command. To add an HTTP command to a resource, use the `WithHttpCommand` extension method on the resource builder. There are two overloads available:

<!-- TODO: Replace with xref when available... -->

```csharp
public static IResourceBuilder<TResource> WithHttpCommand<TResource>(
    this IResourceBuilder<TResource> builder,
    string path,
    string displayName,
    [EndpointName] string? endpointName = null,
    HttpMethod? method = null,
    string? httpClientName = null,
    Func<HttpCommandRequestContext, Task>? configureRequest = null,
    Func<HttpCommandResultContext, Task<ExecuteCommandResult>>? getCommandResult = null,
    string? commandName = null,
    Func<UpdateCommandStateContext, ResourceCommandState>? updateState = null,
    string? displayDescription = null,
    string? confirmationMessage = null,
    string? iconName = null,
    IconVariant? iconVariant = null,
    bool isHighlighted = false)
    where TResource : IResourceWithEndpoints;
```

The parameters are defined as follows:

- `path`: The relative path for the HTTP request.
- `displayName`: The name displayed in the [.NET Aspire dashboard](dashboard/overview.md).
- `endpointName`: (Optional) The name of the endpoint to use for the HTTP request.
- `method`: (Optional) The HTTP method to use (defaults to <xref:System.Net.Http.HttpMethod.Post?displayProperty=nameWithType>).
- `httpClientName`: (Optional) The name of the HTTP client to use when creating it via <xref:System.Net.Http.IHttpClientFactory.CreateClient(System.String)?displayProperty=nameWithType>.
- `configureRequest`: (Optional) A callback to be invoked to configure the HTTP request before it is sent.
- `getCommandResult`: (Optional) A callback to be invoked after the HTTP response is received to determine the result of the command invocation.
- `commandName`: (Optional) The unique identifier for the command.
- `updateState`: (Optional) A callback that is used to update the command state. The callback is executed when the command's resource snapshot is updated. By default, the command is enabled when the resource is <xref:Aspire.Hosting.ApplicationModel.KnownResourceStates.Running?displayProperty=nameWithType>.
- `displayDescription`: (Optional) A description displayed in the dashboard. Could be used as a tooltip and might be localized.
- `confirmationMessage`: (Optional) When a confirmation message is specified, the UI prompts the user to confirm the execution of the command with an Ok/Cancel dialog.
- `iconName`: (Optional) The name of the icon to display in the dashboard. The icon is optional, but when you do provide it, it should be a valid [Fluent UI Blazor icon name](https://www.fluentui-blazor.net/Icon#explorer).
- `iconVariant`: (Optional) The variant of the icon to display in the dashboard, valid options are `Regular` (default) or `Filled`.
- `isHighlighted`: (Optional) Indicates whether the command is highlighted in the dashboard.

```csharp
public static IResourceBuilder<TResource> WithHttpCommand<TResource>(
    this IResourceBuilder<TResource> builder,
    string path,
    string displayName,
    Func<EndpointReference>? endpointSelector,
    HttpMethod? method = null,
    string? httpClientName = null,
    Func<HttpCommandRequestContext, Task>? configureRequest = null,
    Func<HttpCommandResultContext, Task<ExecuteCommandResult>>? getCommandResult = null,
    string? commandName = null,
    Func<UpdateCommandStateContext, ResourceCommandState>? updateState = null,
    string? displayDescription = null,
    string? confirmationMessage = null,
    string? iconName = null,
    IconVariant? iconVariant = null,
    bool isHighlighted = false)
    where TResource : IResourceWithEndpoints;
```

The parameters are defined as follows:

- `path`: The relative path for the HTTP request.
- `displayName`: The name displayed in the [.NET Aspire dashboard](dashboard/overview.md).
- `endpointSelector`: (Optional) A callback that selects the HTTP endpoint to send the request to when the command is invoked.
- `method`: (Optional) The HTTP method to use (defaults to <xref:System.Net.Http.HttpMethod.Post?displayProperty=nameWithType>).
- `httpClientName`: (Optional) The name of the HTTP client to use when creating it via <xref:System.Net.Http.IHttpClientFactory.CreateClient(System.String)?displayProperty=nameWithType>.
- `configureRequest`: (Optional) A callback to be invoked to configure the HTTP request before it is sent.
- `getCommandResult`: (Optional) A callback to be invoked after the HTTP response is received to determine the result of the command invocation.
- `commandName`: (Optional) The unique identifier for the command.
- `updateState`: (Optional) A callback that is used to update the command state. The callback is executed when the command's resource snapshot is updated. By default, the command is enabled when the resource is <xref:Aspire.Hosting.ApplicationModel.KnownResourceStates.Running?displayProperty=nameWithType>.
- `displayDescription`: (Optional) A description displayed in the dashboard. Could be used as a tooltip and might be localized.
- `confirmationMessage`: (Optional) When a confirmation message is specified, the UI prompts the user to confirm the execution of the command with an Ok/Cancel dialog.
- `iconName`: (Optional) The name of the icon to display in the dashboard. The icon is optional, but when you do provide it, it should be a valid [Fluent UI Blazor icon name](https://www.fluentui-blazor.net/Icon#explorer).
- `iconVariant`: (Optional) The variant of the icon to display in the dashboard, valid options are `Regular` (default) or `Filled`.
- `isHighlighted`: (Optional) Indicates whether the command is highlighted in the dashboard.

## See also

- [Custom resource commands in .NET Aspire](custom-resource-commands.md)
