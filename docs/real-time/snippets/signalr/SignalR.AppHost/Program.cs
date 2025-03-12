using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

var isServerless = true;

var signalR = builder.AddAzureSignalR("signalr", isServerless
                         ? AzureSignalRServiceMode.Serverless
                         : AzureSignalRServiceMode.Default)
                     .RunAsEmulator();

void IsServerlessEnvironmentVariable(EnvironmentCallbackContext context)
{
    context.EnvironmentVariables["IS_SERVERLESS"] = isServerless;
}

var apiService = builder.AddProject<Projects.SignalR_ApiService>("apiservice")
                        .WithReference(signalR)
                        .WaitFor(signalR)
                        .WithEnvironment(IsServerlessEnvironmentVariable);

var web = builder.AddProject<Projects.SignalR_Web>("webfrontend")
                 .WithReference(apiService)
                 .WaitFor(apiService);

if (isServerless)
{
    web.WithReference(signalR)
       .WaitFor(signalR)
       .WithEnvironment(IsServerlessEnvironmentVariable);
}

builder.Build().Run();
