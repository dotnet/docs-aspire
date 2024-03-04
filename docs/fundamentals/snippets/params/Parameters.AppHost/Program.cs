var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddSqlServer("sql")
                .PublishAsConnectionString()
                .AddDatabase("db");

var insertionRows = builder.AddParameter("insertionRows");

builder.AddProject<Projects.Parameters_ApiService>("api")
       .WithEnvironment("InsertionRows", insertionRows)
       .WithReference(db);

builder.Build().Run();
