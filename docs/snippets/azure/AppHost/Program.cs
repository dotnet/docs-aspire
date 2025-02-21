var builder = DistributedApplication.CreateBuilder(args);

AddAzureInfrastructure(builder);

builder.AddAzureAppConfiguration("config");
builder.AddAzureApplicationInsights("app-insights");
builder.AddAzureCosmosDB("cosmos");
builder.AddAzureEventHubs("event-hubs").AddHub("messages");
builder.AddAzureKeyVault("key-vault");
builder.AddAzureLogAnalyticsWorkspace("log-analytics-workspace");
builder.AddAzureOpenAI("openai");
builder.AddAzurePostgresFlexibleServer("postgres-flexible");
builder.AddAzureRedis("redis");
builder.AddAzureSearch("search");
builder.AddAzureServiceBus("service-bus");
builder.AddAzureSignalR("signalr");
builder.AddAzureSqlServer("sql");
builder.AddAzureStorage("storage");
builder.AddAzureWebPubSub("web-pub-sub");

builder.Build().Run();
