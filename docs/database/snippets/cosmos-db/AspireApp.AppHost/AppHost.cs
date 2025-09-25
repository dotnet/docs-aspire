var builder = DistributedApplication.CreateBuilder(args);

// If you experience the following error with the emulator:
//   "The evaluation period has expired."
// Note, the emulator is only for evaluation. Simply delete the image and run this again. ♻️
var cosmos = builder.AddAzureCosmosDB("cosmos")
    .AddDatabase("languages")
    .RunAsEmulator();

var apiService = builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
    .WithReference(cosmos);

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
