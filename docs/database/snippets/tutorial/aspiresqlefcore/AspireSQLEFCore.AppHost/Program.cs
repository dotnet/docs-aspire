var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .AddDatabase("sqldata");

builder.AddProject<Projects.AspireSQLEFCore>("aspiresql")
    .WithReference(sql);

builder.Build().Run();
