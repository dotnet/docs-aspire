using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

var signalR = builder.AddAzureSignalR("signalr", AzureSignalRServiceMode.Serverless)
                     .RunAsEmulator();

var apiService = builder.AddProject<Projects.SignalR_ApiService>("apiservice")
                        .WithReference(signalR)
                        .WaitFor(signalR);
  
builder.AddProject<Projects.SignalR_Web>("webfrontend")
       .WithReference(apiService)
       .WaitFor(apiService);

builder.Build().Run();
