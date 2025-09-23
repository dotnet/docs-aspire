using Aspire.Hosting;
using Aspire.Hosting.DevTunnels;

var builder = DistributedApplication.CreateBuilder(args);

// Create a web application
var web = builder.AddProject<Projects.WebApp>("web");

// Basic dev tunnel - exposes all endpoints with authentication required
var tunnel = builder.AddDevTunnel("mytunnel")
                    .WithReference(web);

// Dev tunnel with anonymous access for the entire tunnel
var publicTunnel = builder.AddDevTunnel("public-api")
                          .WithReference(web)
                          .WithAnonymousAccess();

// Dev tunnel with custom options
var options = new DevTunnelOptions
{
    Description = "Shared QA validation tunnel",
    Labels = { "qa", "validation" },
    AllowAnonymous = false
};

var customTunnel = builder.AddDevTunnel(
                       name: "qa",
                       tunnelId: "qa-shared",
                       options: options)
                   .WithReference(web);

// Dev tunnel exposing specific endpoints with different access levels
var api = builder.AddProject<Projects.ApiService>("api");

var mixedAccessTunnel = builder.AddDevTunnel("mixed-access")
                               .WithReference(api.GetEndpoint("public"), allowAnonymous: true)
                               .WithReference(api.GetEndpoint("admin")); // Requires authentication

// Multiple tunnels for different audiences
var privateTunnel = builder.AddDevTunnel("private")
                           .WithReference(web); // Requires authentication

// Service discovery - client using tunneled address
builder.AddProject<Projects.ClientApp>("client")
       .WithReference(web, publicTunnel); // Use the tunneled address for 'web'

builder.Build().Run();