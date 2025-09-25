using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

var isServerless = true;

var signalR = builder.AddAzureSignalR("signalr", isServerless
                         ? AzureSignalRServiceMode.Serverless
                         : AzureSignalRServiceMode.Default)
                     .RunAsEmulator();

var apiService = builder.AddProject<Projects.SignalR_ApiService>("apiservice")
                        .WithReference(signalR)
                        .WaitFor(signalR)
                        .WithEnvironment("IS_SERVERLESS", isServerless.ToString());

var web = builder.AddProject<Projects.SignalR_Web>("webfrontend")
                 .WithReference(apiService)
                 .WaitFor(apiService);

builder.Build().Run();
