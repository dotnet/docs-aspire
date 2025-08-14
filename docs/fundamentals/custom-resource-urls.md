---
title: Define custom resource URLs
description: Learn how to create custom URLs for .NET Aspire resources.
ms.date: 05/06/2025
ms.topic: how-to
---

# Define custom resource URLs

In .NET Aspire, resources that expose endpoints only configure host and port, which aren't known until run time. If you need to access a specific path on one of these endpoints—especially from the [dashboard](dashboard/overview.md)—you can define custom resource URLs. You can also add custom URLs that aren't tied to any endpoint. All custom URLs are only available in "run" mode, since they're meant for dashboard use. This article demonstrates how to define custom URLs.

## Default endpoint behavior

By default, .NET Aspire project resources rely on existing configurations such as Kestrel or launch profiles to determine the host and port of a resource for a configured endpoint—and the endpoints are always displayed on the dashboard.

Likewise, you can explicitly expose endpoints using the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithEndpoint*> API. This API allows you to specify the host and port for a resource, which is then used to create the default URL for that resource. The default URL is typically in the format `<scheme>://<host>:<port>`. To omit the host port, use one of the following methods:

- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithHttpEndpoint*>
- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithHttpsEndpoint*>

For more information, see [Endpoint extension methods](networking-overview.md#endpoint-extension-methods).

## Supported resource types

Custom resource URLs are supported for the following resource types:

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

Both [Scalar](https://scalar.com/) and [Swagger](https://swagger.io/tools/swagger-ui/) are common API services that enhance the usability of endpoints. These services are accessed via URLs tied to declared endpoints.

To customize the URL for the first associated resource endpoint, use the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithUrlForEndpoint*> method.

If you want to add a separate URL (even for the same endpoint) you need to call the `WithUrl` overload that takes a <xref:Aspire.Hosting.ApplicationModel.ReferenceExpression> or interpolated string, or call `WithUrls` and add the URL to the `Urls` list on the context.

:::code source="snippets/custom-urls/AspireApp.AppHost/Program.WithUrlForEndpoint.cs" id="withurlforendpoint":::

The preceding example assumes that the `api` project resource has an `https` endpoint configured. The `WithUrlForEndpoint` method updates the <xref:Aspire.Hosting.ApplicationModel.ResourceUrlAnnotation> associated with the endpoint. In this case, it assigns the display text to `Scalar (HTTPS)` and assigns the relative `/scalar` path to the URL.

When the resource is started, the URL is available in the dashboard as shown in the following screenshot:

:::image type="content" source="dashboard/media/custom-urls/custom-url-scalar-https.png" alt-text=".NET Aspire dashboard with custom Scalar URL." lightbox="dashboard/media/custom-urls/custom-url-scalar-https.png":::

Alternatively, you could use the overload that accepts a `Func<EndpointReference, ResourceUrlAnnotation>` as a callback. This allows you to specify deep-links on target <xref:Aspire.Hosting.ApplicationModel.EndpointReference> instances.

### Customize multiple resource URLs

To customize multiple URLs for a resource, use the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithUrls*> method. This method allows you to specify multiple URLs for a resource, each with its own display text. The following example demonstrates how to set multiple URLs for a project resource:

:::code source="snippets/custom-urls/AspireApp.AppHost/Program.WithUrls.cs" id="withurls":::

The preceding code iterates through the URLs defined for the `api` project resource and assigns a display text with scheme. The resulting URLs are available in the dashboard as shown in the following screenshot:

:::image type="content" source="dashboard/media/custom-urls/custom-url-ordered.png" alt-text=".NET Aspire dashboard custom ordered and named URLs.":::

> [!TIP]
> The <xref:Aspire.Hosting.ApplicationModel.ResourceUrlsCallbackContext> exposes an extension method that enables you to easily access the underlying resource named endpoints. Call the `GetEndpoint` API on a context instance to achieve this.

## URL customization lifecycle

URL customization callbacks run during the application model lifecycle, specifically during the <xref:Aspire.Hosting.ApplicationModel.BeforeResourceStartedEvent> event processing. URLs associated with endpoints become active and appear on the dashboard once the endpoint itself becomes active. URLs not associated with endpoints become active only when the resource enters the "Running" state. This ensures that all custom URLs are accurately represented and available when the application resources are fully operational.

## See also

- [.NET Aspire dashboard overview](dashboard/overview.md)
- [.NET Aspire AppHost](app-host-overview.md)
