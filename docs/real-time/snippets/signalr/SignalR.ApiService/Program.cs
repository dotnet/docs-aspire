using SignalR.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddSignalR(static o => o.EnableDetailedErrors = true)
                .AddNamedAzureSignalR("signalr");

var app = builder.Build();

app.UseExceptionHandler();

app.MapHub<ChatHub>(KnownChatHub.Endpoint);

app.MapDefaultEndpoints();

app.Run();
