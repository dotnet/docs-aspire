using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("Storage");

if (builder.Environment.IsDevelopment())
{
    storage.RunAsEmulator();
}

var blobs = storage.AddBlobs("BlobConnection");
var queues = storage.AddQueues("QueueConnection");

var apiService = builder.AddProject<Projects.AspireStorage_ApiService>("apiservice");

builder.AddProject<Projects.AspireStorage_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(blobs)
    .WithReference(queues); 

builder.AddProject<Projects.AspireStorage_WorkerService>("aspirestorage-workerservice")
    .WithReference(queues);

builder.Build().Run();
