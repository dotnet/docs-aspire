using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Add an HTTP proxy that forwards requests to an external API
var proxy = builder.AddHttpProxy("api-proxy", "https://jsonplaceholder.typicode.com", port: 5100);

// Add a web project that can use the proxy
var webapp = builder.AddProject<Projects.WebApp>("webapp")
                    .WithReference(proxy);

builder.Build().Run();