using Aspire.Hosting.ApplicationModel;

namespace HttpProxy.Hosting;

/// <summary>
/// Represents an HTTP proxy resource that forwards requests to a target URL.
/// </summary>
/// <param name="name">The name of the resource.</param>
/// <param name="targetUrl">The target URL to proxy requests to.</param>
public class HttpProxyResource(string name, string targetUrl) : Resource(name), IResourceWithEndpoints
{
    /// <summary>
    /// Gets the target URL that requests will be proxied to.
    /// </summary>
    public string TargetUrl { get; } = targetUrl;

    /// <summary>
    /// Gets the name of the HTTP endpoint.
    /// </summary>
    public const string HttpEndpointName = "http";
}