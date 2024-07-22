var builder = DistributedApplication.CreateBuilder(args);

builder.AddPythonProject("hello-python", "../hello-python", "main.py")
       .WithEndpoint(targetPort: 8111, scheme: "http", env: "PORT")
       //.WithEnvironment("OTEL_PYTHON_OTLP_TRACES_SSL", "false")
       ;

builder.Build().Run();
