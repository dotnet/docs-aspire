using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("Storage");

if (builder.Environment.IsDevelopment())
{
    storage.RunAsEmulator();
}

var blobs = storage.AddBlobs("BlobConnection");
var queues = storage.AddQueues("QueueConnection");

builder.AddProject<Projects.AspireStorage>("aspirestorage")
    .WithReference(blobs)
    .WithReference(queues);

builder.AddProject<Projects.AspireStorage_Worker>("aspirestorage-worker")
    .WithReference(queues);

var app = builder.Build();
app.Run();
