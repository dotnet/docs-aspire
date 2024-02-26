using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// Use launch profiles to change DOTNET_ENVIRONMENT. If you use the "UseConnectionString"
// environment you will need to add a Cosmos connection string to your user
// secrets (example):
// {
//   "ConnectionStrings": {
//     "db": "[insert connection string]"
//   }
// }
//
// The expression below is a bit more complex than the average developer app would
// probably have, but in our repo we'll probably want to experiment with separately
// deployed resources a little bit.
var db = builder.AddSqlServer("sql")
                .PublishAsConnectionString()
                .AddDatabase("db");

var insertionrows = builder.AddParameter("insertionrows");

builder.AddProject<Projects.Parameters_ApiService>("api")
       .WithEnvironment("InsertionRows", insertionrows)
       .WithReference(db);

builder.Build().Run();
