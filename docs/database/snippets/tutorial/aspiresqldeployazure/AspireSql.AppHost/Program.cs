var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspireSql_ApiService>("apiservice");

// Provisions an Azure SQL Database when published
var sqlServer = builder.AddAzureSqlServer("sqlserver")
                       .AddDatabase("sqldb");

builder.AddProject<Projects.AspireSql_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
