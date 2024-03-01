var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddSignalR()
                .AddNamedAzureSignalR("signalr");

var app = builder.Build();

app.UseExceptionHandler();

app.MapHub<ChatHub>(HubEndpoints.ChatHub);

app.MapDefaultEndpoints();

app.Run();
