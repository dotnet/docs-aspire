var builder = DistributedApplication.CreateBuilder(args);

builder.AddPythonProject("hello-python", "../hello-python", "main.py")
       .WithEndpoint(targetPort: 8111, scheme: "http", env: "PORT");

builder.Build().Run();
