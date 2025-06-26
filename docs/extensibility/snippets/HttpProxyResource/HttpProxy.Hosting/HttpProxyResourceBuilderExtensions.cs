using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Aspire.Hosting;

/// <summary>
/// Extension methods for adding HTTP proxy resources to the application model.
/// </summary>
public static class HttpProxyResourceBuilderExtensions
{
    /// <summary>
    /// Adds an HTTP proxy resource to the application model.
    /// </summary>
    /// <param name="builder">The distributed application builder.</param>
    /// <param name="name">The name of the resource.</param>
    /// <param name="targetUrl">The target URL to proxy requests to.</param>
    /// <param name="port">The port to listen on (optional).</param>
    /// <returns>A resource builder for the HTTP proxy resource.</returns>
    public static IResourceBuilder<HttpProxy.Hosting.HttpProxyResource> AddHttpProxy(
        this IDistributedApplicationBuilder builder,
        string name,
        string targetUrl,
        int? port = null)
    {
        var resource = new HttpProxy.Hosting.HttpProxyResource(name, targetUrl);
        
        // Register the lifecycle hook for this resource type
        builder.Services.TryAddSingleton<HttpProxy.Hosting.HttpProxyLifecycleHook>();
        builder.Services.AddLifecycleHook<HttpProxy.Hosting.HttpProxyLifecycleHook>();

        return builder.AddResource(resource)
                      .WithHttpEndpoint(port: port, name: HttpProxy.Hosting.HttpProxyResource.HttpEndpointName);
    }
}