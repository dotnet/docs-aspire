var builder = DistributedApplication.CreateBuilder(args);

var apiservice =
    builder.AddProject<Projects.Networking_ApiService>(
        name: "apiservice",
        launchProfileName: null);

builder.AddProject<Projects.Networking_Frontend>("frontend")
       .WithReference(apiservice);

// WithEndpoint(builder);
// ContainerPort(builder);
// EnvVarPort(builder);
// HostPortWithRandomServicePort(builder);
// OmitHostPort(builder);
// WithReplicas(builder);
// WithLaunchProfile(builder);

builder.Build().Run();
