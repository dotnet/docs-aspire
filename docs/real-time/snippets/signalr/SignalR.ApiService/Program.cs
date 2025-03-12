var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

var isServerlessMode = builder.Configuration.GetValue<bool>("IS_SERVERLESS");

if (isServerlessMode)
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
else
{
    builder.Services.AddSignalR()
                    .AddNamedAzureSignalR("signalr");
}

var app = builder.Build();

app.UseExceptionHandler();

if (isServerlessMode)
{
    app.MapPost("/negotiate", async (ServiceManager serviceManager, string? userId) =>
    {
        var healthy = await serviceManager.IsServiceHealthy(CancellationToken.None);
        if (healthy is false)
        {
            return Results.Problem("SignalR service is not healthy.");
        }

        var hubContext = await serviceManager.CreateHubContextAsync(HubEndpoints.ChatHubWithoutRouteSlash, CancellationToken.None);
        var negotiateResponse = await hubContext.NegotiateAsync(new NegotiationOptions
        {
            UserId = userId ?? "user1"
        });

        return Results.Ok(negotiateResponse);
    });

    var serviceManager = app.Services.GetRequiredService<ServiceManager>();
    var context = await serviceManager.CreateHubContextAsync(HubEndpoints.ChatHubWithoutRouteSlash, CancellationToken.None);

    await context.Clients.All.SendAsync(
        HubEventNames.MessageReceived,
        new UserMessage("server", "Started..."));
}
else
{
    app.MapHub<ChatHub>(HubEndpoints.ChatHub);
}

app.MapDefaultEndpoints();

app.Run();
