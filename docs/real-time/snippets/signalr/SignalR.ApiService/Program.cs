var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ServiceHubContextFactory>();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var isServerlessMode = builder.Configuration.GetValue<bool>("IS_SERVERLESS");

if (!isServerlessMode)
{
    builder.Services.AddSignalR()
                    .AddNamedAzureSignalR("signalr");
}
else
{
    builder.Services.AddSingleton(sp =>
    {
        return new ServiceManagerBuilder()
            .WithOptions(options =>
            {
                options.ConnectionString = builder.Configuration.GetConnectionString("signalr");
            })
            .WithLoggerFactory(sp.GetRequiredService<ILoggerFactory>())
            .BuildServiceManager();
    });
}
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(_ => _.Servers = []);
}

app.UseExceptionHandler();

var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

if (isServerlessMode)
{
    app.MapPost("/chathub/negotiate", async (string? userId, ServiceHubContextFactory factory) =>
    {
        var hubContext = await factory.GetOrCreateHubContextAsync("chathub", CancellationToken.None);

        NegotiationResponse negotiateResponse = await hubContext.NegotiateAsync(new()
        {
            UserId = userId ?? "user-1",
        });

        return Results.Json(negotiateResponse, new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
    });

    // try in the command line `CURL -X POST https://localhost:53282/broadcast` to broadcast messages to the clients
    app.MapPost("/chathub/broadcast", async (ServiceHubContextFactory factory) =>
    {
        var hubContext = await factory.GetOrCreateHubContextAsync("chathub", CancellationToken.None);

        await hubContext.Clients.All.SendAsync(
            HubEventNames.MessageReceived,
            new UserMessage("server", "Started..."));
    });
}
else
{
    app.MapHub<ChatHub>(HubEndpoints.ChatHub);
}

app.MapDefaultEndpoints();

app.Run();
