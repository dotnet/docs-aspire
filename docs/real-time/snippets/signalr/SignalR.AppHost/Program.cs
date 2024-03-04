var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureProvisioning();

var signalr = builder.AddAzureSignalR("signalr");

var apiService = builder.AddProject<Projects.SignalR_ApiService>("apiservice")
                        .WithReference(signalr);
  
builder.AddProject<Projects.SignalR_Web>("webfrontend")
       .WithReference(apiService);

builder.Build().Run();
