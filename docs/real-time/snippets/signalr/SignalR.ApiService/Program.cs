using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Connections;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

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

app.UseExceptionHandler();

var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

if (isServerlessMode)
{
    app.MapPost("/chathub/negotiate", async (ServiceManager sm, ILogger<Program> logger) =>
    {
        var hubContext = await sm.CreateHubContextAsync("chathub", CancellationToken.None);

        NegotiationResponse negotiateResponse = await hubContext.NegotiateAsync();

        return Results.Json(negotiateResponse, new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });
    });

    // try in the command line `CURL -X POST https://localhost:53282/broadcast` to broadcast messages to the clients
    app.MapPost("/broadcast", async (ServiceManager sm) =>
    {
        var hubContext = await sm.CreateHubContextAsync("chathub", CancellationToken.None);

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
