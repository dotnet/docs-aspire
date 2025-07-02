using System.Net;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Logging;

namespace HttpProxy.Hosting;

/// <summary>
/// Lifecycle hook that manages HTTP proxy resources.
/// </summary>
public class HttpProxyLifecycleHook : IDistributedApplicationLifecycleHook
{
    private readonly ILogger<HttpProxyLifecycleHook> _logger;
    private readonly Dictionary<string, HttpListener> _listeners = new();

    public HttpProxyLifecycleHook(ILogger<HttpProxyLifecycleHook> logger)
    {
        _logger = logger;
    }

    public Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task AfterEndpointsAllocatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task AfterResourcesCreatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
    {
        // Find and start HTTP proxy resources
        var proxyResources = appModel.Resources.OfType<HttpProxyResource>();

        foreach (var resource in proxyResources)
        {
            StartProxy(resource);
        }

        return Task.CompletedTask;
    }

    private void StartProxy(HttpProxyResource resource)
    {
        try
        {
            _logger.LogInformation("Starting HTTP proxy {ResourceName} -> {TargetUrl}", 
                resource.Name, resource.TargetUrl);

            // Create and start HTTP listener on a dynamic port
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:0/"); // Use system-assigned port
            listener.Start();

            _listeners[resource.Name] = listener;

            // Start processing requests in the background
            _ = Task.Run(() => ProcessRequests(resource, listener));

            _logger.LogInformation("HTTP proxy {ResourceName} started successfully", resource.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start HTTP proxy {ResourceName}", resource.Name);
        }
    }

    private async Task ProcessRequests(HttpProxyResource resource, HttpListener listener)
    {
        var requestCount = 0;

        while (listener.IsListening)
        {
            try
            {
                var context = await listener.GetContextAsync();
                requestCount++;

                _logger.LogInformation("Proxy {ResourceName} handling request {RequestCount}: {Method} {Path}",
                    resource.Name, requestCount, context.Request.HttpMethod, context.Request.Url?.PathAndQuery);

                // Simple response for demonstration
                var response = context.Response;
                response.StatusCode = 200;
                var responseString = $"Proxy {resource.Name} would forward to {resource.TargetUrl}";
                var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                await response.OutputStream.WriteAsync(buffer);
                response.Close();
            }
            catch (HttpListenerException)
            {
                // Listener was stopped
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request in proxy {ResourceName}", resource.Name);
            }
        }
    }
}