var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspireSql_ApiService>("apiservice");

// Provisions a containerized SQL Server database when published
var sqlServer2 = builder.AddSqlServer("sqlserver")
                      .AddDatabase("sqldb");

builder.AddProject<Projects.AspireSql_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
