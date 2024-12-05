var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureAppConfiguration("config");
builder.AddAzureApplicationInsights("insights");
builder.AddAzureCosmosDB("cosmos");
builder.AddAzureEventHubs("event-hubs");
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
builder.AddAzureStorage("blob-storage").AddBlobs("blobs");
builder.AddAzureStorage("queue-storage").AddQueues("queues");
builder.AddAzureStorage("table-storage").AddTables("tables");
builder.AddAzureWebPubSub("web-pub-sub");

builder.Build().Run();
