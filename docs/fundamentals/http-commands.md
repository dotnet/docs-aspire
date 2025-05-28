---
title: Custom HTTP commands in .NET Aspire
description: Learn how to create custom HTTP commands in .NET Aspire.
ms.date: 03/25/2025
ms.topic: how-to
---

# Custom HTTP commands in .NET Aspire

In .NET Aspire, you can add custom HTTP commands to resources using the `WithHttpCommand` API. This API extends existing functionality, where you provide [custom commands on resources](custom-resource-commands.md). This feature enables a command on a resource that sends an HTTP request to a specified endpoint and path. This is useful for scenarios such as triggering database migrations, clearing caches, or performing custom actions on resources through HTTP requests.

To implement custom HTTP commands, you define a command on a resource and a corresponding HTTP endpoint that handles the request. This article provides an overview of how to create and configure custom HTTP commands in .NET Aspire.

## HTTP command APIs

The available APIs provide extensive capabilities with numerous parameters to customize the HTTP command. To add an HTTP command to a resource, use the `WithHttpCommand` extension method on the resource builder. There are two overloads available:

<!-- TODO: Replace with xref when available... -->

The `WithHttpCommand` API provides two overloads to add custom HTTP commands to resources in .NET Aspire. These APIs are designed to offer flexibility and cater to different use cases when defining HTTP commands.

1. **Overload with `endpointName`:**

    This version is ideal when you have a predefined endpoint name that the HTTP command should target. It simplifies the configuration by directly associating the command with a specific endpoint. This is useful in scenarios where the endpoint is static and well-known during development.

1. **Overload with `endpointSelector`:**

    This version provides more dynamic behavior by allowing you to specify a callback (`endpointSelector`) to determine the endpoint at runtime. This is useful when the endpoint might vary based on the resource's state or other contextual factors. It offers greater flexibility for advanced scenarios where the endpoint can't be hardcoded.

Both overloads allow you to customize the HTTP command extensively, providing an `HttpCommandOptions` subclass of the `CommandOptions` type, including specifying the HTTP method, configure the request, handling the response, and define UI-related properties like display name, description, and icons. The choice between the two depends on whether the endpoint is static or dynamic in your use case.

These APIs are designed to integrate seamlessly with the .NET Aspire ecosystem, enabling developers to extend resource functionality with minimal effort while maintaining control over the behavior and presentation of the commands.

## Considerations when registering HTTP commands

Since HTTP commands are exposed via HTTP endpoints, consider potential security risks. Limit these endpoints to development or staging environments when possible. Always validate incoming requests to ensure they originate from trusted sources. For more information, see [ASP.NET Core security](/aspnet/core/security).

Use the `HttpCommandOptions.PrepareRequest` callback to enhance security by adding authentication headers or other measures. A common approach is to use a shared secret, [external parameter](external-parameters.md), or token known only to the app host and resource. This shared value can be used to validate requests and prevent unauthorized access.

## Add a custom HTTP command

In your app host _Program.cs_ file, you add a custom HTTP command using the `WithHttpCommand` API on an <xref:Aspire.Hosting.ApplicationModel.IResourceBuilder`1> where `T` is an <xref:Aspire.Hosting.ApplicationModel.IResourceWithEndpoints>. Here's an example of how to do this:

:::code source="snippets/http-commands/AspireApp/AspireApp.AppHost/Program.cs":::

The preceding code:

- Creates a new distributed application builder.
- Adds a [Redis cache](../caching/stackexchange-redis-integration.md) named `cache` to the application.
- Adds a parameter named `ApiCacheInvalidationKey` to the application. This parameter is marked as a secret, meaning its value is treated securely.
- Adds a project named `AspireApp_Api` to the application.
- Adds a reference to the Redis cache and [waits for it to be ready before proceeding](orchestrate-resources.md#waiting-for-resources).
- Configures an HTTP command for the project with the following:
  - `path`: Specifies the URL path for the HTTP command (`/cache/invalidate`).
  - `displayName`: Sets the name of the command as it appears in the UI (`Invalidate cache`).
  - `commandOptions`: An optional instance of `HttpCommandOptions` that configures the command's behavior and appearance in the UI:
    - `Description`: Provides a description of the command that's shown in the UI.
    - `PrepareRequest`: A callback function that configures the HTTP request before sending it. In this case, it adds a custom (`X-CacheInvalidation-Key`) header with the value of the `ApiCacheInvalidationKey` parameter.
    - `IconName`: Specifies the icon to be used for the command in the UI (`DocumentLightningFilled`).
    - `IsHighlighted`: Indicates whether the command should be highlighted in the UI.
- Finally, the application is built and run.

The HTTP endpoint is responsible for invalidating the cache. When the command is executed, it sends an HTTP request to the specified path (`/cache/invalidate`) with the configured parameters. Since there's an added security measure, the request includes the `X-CacheInvalidation-Key` header with the value of the `ApiCacheInvalidationKey` parameter. This ensures that only authorized requests can trigger the cache invalidation process.

### Example HTTP endpoint

The preceding app host code snippet defined a custom HTTP command that sends a request to the `/cache/invalidate` endpoint. The ASP.NET Core minimal API project defines an HTTP endpoint that handles the cache invalidation request. Consider the following code snippet from the project's _Program.cs_ file:

:::code source="snippets/http-commands/AspireApp/AspireApp.Api/Program.cs" id="post":::

The preceding code:

- Assumes that the `app` variable is an instance of <xref:Microsoft.AspNetCore.Builder.IApplicationBuilder> and is set up to handle HTTP requests.
- Maps an HTTP POST endpoint at the path `/cache/invalidate`.
- The endpoint expects a header named `X-CacheInvalidation-Key` to be present in the request.
- It retrieves the value of the `ApiCacheInvalidationKey` parameter from the configuration.
- If the header value doesn't match the expected key, it returns an unauthorized response.
- If the header is valid, it calls the `ClearAllAsync` method on the `ICacheService` to clear all cached items.
- Finally, it returns an HTTP OK response.

### Example dashboard experiences

The sample app host and corresponding ASP.NET Core minimal API projects demonstrate both sides of the HTTP command implementation. When you run the app host, the dashboard's **Resources** page displays the custom HTTP command as a button. When you specify that the command should be highlighted (`isHighlighted: true`), the button appears on the **Actions** column of the **Resources** page. This allows users to easily trigger the command from the dashboard, as shown in the following screenshot:

:::image type="content" source="media/custom-http-command-highlighted.png" lightbox="media/custom-http-command-highlighted.png" alt-text=".NET Aspire dashboard: Resources page showing a highlighted custom HTTP command.":::

If you're to omit the `isHighlighted` parameter, or set it to `false`, the command appears nested under the horizontal ellipsis menu (three dots) in the **Actions** column of the **Resources** page. This allows users to access the command without cluttering the UI with too many buttons. The following screenshot shows the same command appearing in the ellipsis menu:

:::image type="content" source="media/custom-http-command.png" lightbox="media/custom-http-command.png" alt-text=".NET Aspire dashboard: Resources page showing a custom HTTP command in the ellipsis menu.":::

When the user selects the button, the command is executed, and the HTTP request is sent to the specified endpoint. The dashboard provides feedback on the command's execution status, allowing users to monitor the results. When it's starting, a toast notification appears:

:::image type="content" source="media/custom-http-command-starting.png" lightbox="media/custom-http-command-starting.png" alt-text=".NET Aspire dashboard: Toast notification showing the custom HTTP command is starting.":::

When the command completes, the dashboard updates the status and provides feedback on whether it was successful or failed. The following screenshot shows a successful execution of the command:

:::image type="content" source="media/custom-http-command-succeeded.png" lightbox="media/custom-http-command-succeeded.png" alt-text=".NET Aspire dashboard: Toast notification showing the custom HTTP command succeeded.":::

## See also

- [.NET Aspire orchestration overview](app-host-overview.md)
- [Custom resource commands in .NET Aspire](custom-resource-commands.md)
- [.NET Aspire GitHub repository: Playground sample](https://github.com/dotnet/aspire/tree/4fdfdbf57d35265913a3bbac38b92d98ed255a5d/playground/TestShop)
