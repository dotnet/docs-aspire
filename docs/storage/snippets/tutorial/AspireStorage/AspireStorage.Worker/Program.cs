using AspireStorage.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.AddAzureQueueService("QueueConnection");

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
