var builder = DistributedApplication.CreateBuilder(args);

WithEndpoint(builder);
// ContainerPort(builder);
// EnvVarPort(builder);
// HostPortWithRandomServicePort(builder);
// OmitHostPort(builder);
// WithReplicas(builder);
// WithLaunchProfile(builder);

builder.Build().Run();
