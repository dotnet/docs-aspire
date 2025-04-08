---
title: Custom resource URLs
description: Learn how to create custom URLs for .NET Aspire resources.
ms.date: 04/08/2025
ms.topic: how-to
---

# Custom resource URLs

.NET Aspire resources that expose endpoints only configure host and port values. However, there might be situations where you want to access a specific route of an exposed endpoint. The host and port are unknown until run time. In these cases, you can use custom resource URLs to define specific routes on a configured endpoint, which is convenient for accessing resources from the [dashboard](dashboard/overview.md).

## Default endpoint behavior

By default, as described in the [Networking inner loop](networking-overview.md#networking-in-the-inner-loop) article, .NET Aspire relies on existing configurations such as Kestrel or launch profiles to determine the host and port of a resource for a configured endpoint.

Likewise, you can explicitly expose endpoints using the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithEndpoint*> API. This API allows you to specify the host and port for a resource, which is then used to create the default URL for that resource. The default URL is typically in the format `http://<host>:<port>` or `https://<host>:<port>`, depending on the protocol used. To omit the host port, use one of the following methods:

- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithHttpEndpoint*>
- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithHttpsEndpoint*>

For more information, see [Endpoint extension methods](networking-overview.md#endpoint-extension-methods).

## Supported resource types

Currently, custom resource URLs are supported for the following resource types:

- <xref:Aspire.Hosting.ApplicationModel.ContainerResource>
- <xref:Aspire.Hosting.ApplicationModel.ExecutableResource>
- <xref:Aspire.Hosting.ApplicationModel.ProjectResource>

## Customize resource URLs

Use the appropriate `WithUrl` overload, `WithUrls`, or `WithUrlForEndpoint` APIs on any supported resource builder to define custom URLs for a resource. The following example demonstrates how to set a custom URL for a project resource:

:::code source="snippets/custom-urls/AspireApp.AppHost/Program.WithUrl.cs" id="withurl":::

> [!TIP]
> There's an overload that accepts a `string` allowing you to pass any URL. This is useful for defining custom URLs that aren't directly related to the resource's endpoint.

The preceding code assigns a project reference to the `api` variable, which is then used to create a custom URL for the `Admin Portal` route. The `WithUrl` method takes a <xref:Aspire.Hosting.ApplicationModel.ReferenceExpression> and a display name as parameters. The resulting URL is available in the dashboard as shown in the following screenshot:

:::image type="content" source="dashboard/media/custom-urls/custom-url-admin-portal.png" alt-text=".NET Aspire dashboard custom Admin Portal URL." lightbox="dashboard/media/custom-urls/custom-url-admin-portal.png":::

### Customize endpoint URL

<!-- TODO: Add xref to WithUrlForEndpoint when available -->

To expose specific endpoints like Scalar or Swagger in the dashboard, use the `WithUrlForEndpoint` method. The following example demonstrates how to customize the URL for a resource endpoint:

:::code source="snippets/custom-urls/AspireApp.AppHost/Program.WithUrlForEndpoint.cs" id="withurlforendpoint":::

<!-- TODO: Add xref to ResourceUrlAnnotation when available -->

The preceding example assumes that the `api` project resource has an `https` endpoint configured. The `WithUrlForEndpoint` method updates the `ResourceUrlAnnotation` for the endpoint, in this case assigning the display text to `Scalar (HTTPS)` and appends the `/scalar` route to the URL.

When the resource is started, the URL is available in the dashboard as shown in the following screenshot:

:::image type="content" source="dashboard/media/custom-urls/custom-url-scalar-https.png" alt-text=".NET Aspire dashboard with custom Scalar URL." lightbox="dashboard/media/custom-urls/custom-url-scalar-https.png":::

### Customize multiple resource URLs

<!-- TODO: Add xref to WithUrls when available -->

To customize multiple URLs for a resource, use the `WithUrls` method. This method allows you to specify multiple URLs for a resource, each with its own display text. The following example demonstrates how to set multiple URLs for a project resource:

:::code source="snippets/custom-urls/AspireApp.AppHost/Program.WithUrls.cs" id="withurls":::

The preceding code iterates through the URLs defined for the `api` project resource and assigns a display text and order to each URL. The resulting URLs are available in the dashboard as shown in the following screenshot:

:::image type="content" source="dashboard/media/custom-urls/custom-url-ordered.png" alt-text=".NET Aspire dashboard custom ordered and named URLs.":::

## URL customization lifecycle

URL customization callbacks run during the application model lifecycle, specifically during the <xref:Aspire.Hosting.ApplicationModel.BeforeResourceStartedEvent> event processing. URLs associated with endpoints become active and appear on the dashboard once the endpoint itself becomes active. URLs not associated with endpoints become active only when the resource enters the "Running" state. This ensures that all custom URLs are accurately represented and available when the application resources are fully operational.

## See also

- [.NET Aspire dashboard overview](./overview.md)
- [.NET Aspire app host](../app-host.md)
