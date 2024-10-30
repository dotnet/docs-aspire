var builder = FunctionsApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddAzureQueueClient("queues");
builder.AddAzureBlobClient("blobs");

builder.ConfigureFunctionsWebApplication();

await builder.Build().RunAsync();
