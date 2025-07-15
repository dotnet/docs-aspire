using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Example 1: Basic persistent container
var postgres = builder.AddPostgres("postgres")
                      .WithLifetime(ContainerLifetime.Persistent);

var db = postgres.AddDatabase("inventorydb");

// Example 2: Persistent container with custom name
var redis = builder.AddRedis("cache")
                   .WithLifetime(ContainerLifetime.Persistent)
                   .WithContainerName("my-shared-redis");

// Example 3: Multiple persistent services
var rabbitmq = builder.AddRabbitMQ("messaging")
                      .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.InventoryService>("inventory")
       .WithReference(db)
       .WithReference(redis)
       .WithReference(rabbitmq);

builder.Build().Run();