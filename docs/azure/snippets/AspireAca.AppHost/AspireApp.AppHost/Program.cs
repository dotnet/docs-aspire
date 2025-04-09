var builder = DistributedApplication.CreateBuilder(args);

var acaEnv = builder.AddAzureContainerAppEnvironment("aca-env");

// Omitted for brevity...

builder.Build().Run();
