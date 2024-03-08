var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Networking_ApiService>("apiService")
    .WithEndpoint("admin", static endpoint =>
    {
        endpoint.Port = 17003;
        endpoint.UriScheme = "http";
        endpoint.Transport = "http";
    });

builder.AddProject<Projects.Networking_Frontend>("frontend")
       .WithReference(apiService);

// WithEndpoint(builder);
// ContainerPort(builder);
// EnvVarPort(builder);
// HostPortWithRandomServicePort(builder);
// OmitHostPort(builder);
// WithReplicas(builder);
// WithLaunchProfile(builder);

builder.Build().Run();
