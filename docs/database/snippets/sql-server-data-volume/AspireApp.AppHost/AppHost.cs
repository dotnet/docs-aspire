var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .WithDataVolume();

var db = sql.AddDatabase("database");

builder.AddProject<Projects.AspireApp_ExampleProject>("exampleproject")
       .WithReference(db)
       .WaitFor(db);

// After adding all resources, run the app...

builder.Build().Run();
