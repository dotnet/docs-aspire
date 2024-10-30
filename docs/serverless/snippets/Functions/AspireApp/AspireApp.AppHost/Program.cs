var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator();
var blobs = storage.AddBlobs("blobs");
var queues = storage.AddQueues("queues");

var functions =
    builder.AddAzureFunctionsProject<Projects.AspireApp_Functions>("functions")
           .WithHostStorage(storage)
           .WithReference(queues)
           .WithReference(blobs)
           .WaitFor(storage);

builder.AddProject<Projects.AspireApp_Web>("web")
       .WithReference(queues)
       .WithReference(blobs)
       .WaitFor(functions);

builder.Build().Run();
