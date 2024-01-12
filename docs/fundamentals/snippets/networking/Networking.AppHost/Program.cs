var builder = DistributedApplication.CreateBuilder(args);

// ContainerPort(builder);
// EnvVarPort(builder);
// HostPortWithRandomServicePort(builder);
// OmitHostPort(builder);
// WithReplicas(builder);
// WithLaunchProfile(builder);

builder.Build().Run();
