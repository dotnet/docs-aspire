var builder = DistributedApplication.CreateBuilder(args);

var web = builder.AddProject<Projects.Web>("web");

builder.AddDevTunnel("tunnel")
       .WithReference(web);

builder.Build().Run();