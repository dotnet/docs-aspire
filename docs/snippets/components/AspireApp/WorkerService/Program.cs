using WorkerService;

var builder = Host.CreateApplicationBuilder(args);

builder.AddNpgsqlDataSource("customers");

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
