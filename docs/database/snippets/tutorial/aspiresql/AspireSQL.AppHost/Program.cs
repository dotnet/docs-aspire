var builder = DistributedApplication.CreateBuilder(args);

var sqlpassword = builder.Configuration["sqlpassword"];

var sql = builder.AddSqlServerContainer("sql", sqlpassword).AddDatabase("sqldata");

var myService = builder.AddProject<Projects.AspireSQL>("aspiresql")
                       .WithReference(sql);

builder.Build().Run();
