var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("password", secret: true);

var sql = builder.AddSqlServer("sql", password);
var db = sql.AddDatabase("database");

builder.AddProject<Projects.AspireApp_ExampleProject>("exampleproject")
       .WithReference(db)
       .WaitFor(db);

// After adding all resources, run the app...

builder.Build().Run();
