var builder = DistributedApplication.CreateBuilder(args);

AddAzureInfrastructure(builder);

builder.AddAzureAppConfiguration("config");

builder.AddAzureApplicationInsights("app-insights");

builder.AddAzureCosmosDB("cosmos");

var eventHubs = builder.AddAzureEventHubs("event-hubs");
eventHubs.AddHub("messages");

builder.AddAzureKeyVault("key-vault");

builder.AddAzureLogAnalyticsWorkspace("log-analytics-workspace");

var openai = builder.AddAzureOpenAI("openai");
openai.AddDeployment(
    new AzureOpenAIDeployment(
        name: "preview",
        modelName: "gpt-4.5-preview",
        modelVersion: "2025-02-27"));

builder.AddAzurePostgresFlexibleServer("postgres-flexible");

builder.AddAzureRedis("redis");

builder.AddAzureSearch("search");

builder.AddAzureServiceBus("service-bus");

builder.AddAzureSignalR("signalr");

builder.AddAzureSqlServer("sql");

builder.AddAzureStorage("storage");

var worker = builder.AddProject<Projects.WorkerService>("wrkr")
    .WithExternalHttpEndpoints();

var webPubSub = builder.AddAzureWebPubSub("web-pubsub");
var messagesHub = webPubSub.AddHub("messages");
messagesHub.AddEventHandler($"{worker.GetEndpoint("https")}/eventhandler/", systemEvents: ["connected"]);

builder.Build().Run();
