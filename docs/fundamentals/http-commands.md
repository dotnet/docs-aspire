---
title: Custom HTTP commands in .NET Aspire
description: Learn how to create custom HTTP commands in .NET Aspire.
ms.date: 03/24/2025
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

Both overloads allow you to customize the HTTP command extensively, including specifying the HTTP method, configure the request, handling the response, and define UI-related properties like display name, description, and icons. The choice between the two depends on whether the endpoint is static or dynamic in your use case.

These APIs are designed to integrate seamlessly with the .NET Aspire ecosystem, enabling developers to extend resource functionality with minimal effort while maintaining control over the behavior and presentation of the commands.

## Considerations when registering HTTP commands

Since HTTP commands are exposed as HTTP endpoints on a resource, you should consider potential security implications. It might be best to only expose the HTTP endpoints in development or staging environments. Likewise, you should validate all requests to the HTTP command endpoint to ensure they're coming from a trusted source.

You can use the `configureRequest` callback to add authentication headers or other security measures to the request. One common approach is to use a shared secret, [external parameter](external-parameters.md), or token that is known only to the resource in the app host. The app host code can provide that shared value to the resource explicitly so it can be used to validate the request. This helps to prevent unauthorized access to the HTTP command.

## Add a custom HTTP command

In your app host _Program.cs_ file, you can add a custom HTTP command to a resource using the `WithHttpCommand` API. Here's an example of how to do this:

:::code source="snippets/http-commands/AspireApp/AspireApp.AppHost/Program.cs":::

The preceding code:

- Creates a new distributed application builder.
- Adds a [Redis cache](../caching/stackexchange-redis-integration.md) named `cache` to the application.
- Adds a parameter named `ApiCacheInvalidationKey` to the application. This parameter is marked as a secret, meaning its value will be treated securely.
- Adds a project named `AspireApp_Api` to the application.
- Adds a reference to the Redis cache and [waits for it to be ready before proceeding](app-host-overview.md#waiting-for-resources).
- Configures an HTTP command for the project with the following properties:
  - **Path**: Specifies the URL path for the HTTP command (`/cache/invalidate`).
  - **Display Name**: Sets the name of the command as it appears in the UI (`Invalidate cache`).
  - **Display Description**: Provides a description of the command that will be shown in the UI.
  - **Configure Request**: A callback function that configures the HTTP request before it's sent. In this case, it adds a custom header (`X-CacheInvalidation-Key`) with the value of the `ApiCacheInvalidationKey` parameter.
  - **Icon Name**: Specifies the icon to be used for the command in the UI (`DocumentLightningFilled`).
  - **Is Highlighted**: Indicates whether the command should be highlighted in the UI.
- The `configureRequest` callback is used to add a custom header to the HTTP request. In this case, it adds an `X-CacheInvalidation-Key` header with the value of the `ApiCacheInvalidationKey` parameter.
- The `Task.CompletedTask` is returned to indicate that the request configuration is complete.
- Finally, the application is built and run.

The HTTP endpoint is responsible for invalidating the cache. When the command is executed, it sends an HTTP request to the specified path (`/cache/invalidate`) with the configured parameters. Since there's an added security measure, the request will include the `X-CacheInvalidation-Key` header with the value of the `ApiCacheInvalidationKey` parameter. This ensures that only authorized requests can trigger the cache invalidation process.

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

## See also

- [Custom resource commands in .NET Aspire](custom-resource-commands.md)
