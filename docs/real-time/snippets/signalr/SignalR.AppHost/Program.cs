var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureProvisioning();

var signalr = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureSignalR("signalr")
    : builder.AddConnectionString("signalr");

var apiService = builder.AddProject<Projects.SignalR_ApiService>("apiservice")
                        .WithReference(signalr);
  
builder.AddProject<Projects.SignalR_Web>("webfrontend")
       .WithReference(apiService);

builder.Build().Run();
