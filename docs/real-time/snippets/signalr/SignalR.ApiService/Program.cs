using Microsoft.Azure.SignalR.Management;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddSignalR();
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

var app = builder.Build();

app.UseExceptionHandler();

app.MapHub<ChatHub>(HubEndpoints.ChatHub);

app.MapPost("/negotiate", async (ServiceManager serviceManager, string? userId) =>
{
    var healthy = await serviceManager.IsServiceHealthy(CancellationToken.None);
    if (healthy is false)
    {
        return Results.Problem("SignalR service is not healthy.");
    }

    var hubContext = await serviceManager.CreateHubContextAsync(HubEndpoints.ChatHub, default);
    var negotiateResponse = await hubContext.NegotiateAsync(new NegotiationOptions
    {
        UserId = userId ?? "user1"
    });

    return Results.Ok(negotiateResponse);
});

app.MapDefaultEndpoints();

app.Run();
