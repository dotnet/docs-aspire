var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Networking_ApiService>("apiService")
    .WithEndpoint("external", static endpoint =>
    {
        endpoint.Port = 17003;
        endpoint.IsExternal = true;
        endpoint.AsHttp2();
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
