---
title: Host external executables in .NET Aspire
description: Learn how to use ExecutableResource and AddExecutable to host external executable applications in your .NET Aspire app host.
ms.date: 08/11/2025
ai-usage: ai-assisted
---

# Host external executables in .NET Aspire

In .NET Aspire, you can host external executable applications alongside your projects using the <xref:Aspire.Hosting.ExecutableResourceBuilderExtensions.AddExecutable%2A> method. This capability is useful when you need to integrate executable applications or tools into your distributed application, such as Node.js applications, Python scripts, or specialized CLI tools.

## When to use executable resources

Use executable resources when you need to:

- Host non-.NET applications that don't have containerized equivalents.
- Integrate command-line tools or utilities into your application.
- Run external processes that other resources depend on.
- Develop with tools that provide local development servers.

Common examples include:

- **Frontend development servers**: Tools like [Vercel CLI](https://vercel.com/docs/cli), Vite, or webpack dev server.
- **Language-specific applications**: Node.js apps, Python scripts, or Go applications.
- **Database tools**: Migration utilities or database seeders.
- **Build tools**: Asset processors or code generators.

## Basic usage

The <xref:Aspire.Hosting.ExecutableResourceBuilderExtensions.AddExecutable%2A> method requires a resource name, the executable path, and optionally command-line arguments and a working directory:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Basic executable without arguments
var nodeApp = builder.AddExecutable("frontend", "node", ".", "server.js");

// Executable with command-line arguments
var pythonApp = builder.AddExecutable(
        "api", "python", ".", "-m", "uvicorn", "main:app", "--reload", "--host", "0.0.0.0", "--port", "8000");

builder.Build().Run();
```

This code demonstrates setting up a basic executable resource. The first example runs a Node.js server script, while the second starts a Python application using Uvicorn with specific configuration options passed as arguments directly to the AddExecutable method.

## Resource dependencies and environment configuration

You can provide command-line arguments directly in the AddExecutable call and configure environment variables for resource dependencies. Executable resources can reference other resources and access their connection information.

### Arguments in the AddExecutable call

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Arguments provided directly in AddExecutable
var app = builder.AddExecutable("vercel-dev", "vercel", ".", "dev", "--listen", "3000");
```

### Resource dependencies with environment variables

For arguments that depend on other resources, use environment variables:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("cache");
var postgres = builder.AddPostgres("postgres").AddDatabase("appdb");

var app = builder.AddExecutable("worker", "python", ".", "worker.py")
    .WithReference(redis)      // Provides ConnectionStrings__cache
    .WithReference(postgres);  // Provides ConnectionStrings__appdb
```

When one resource depends on another, `WithReference` passes along environment variables containing the dependent resource's connection details. For example, the `worker` executable's reference to `redis` and `postgres` provides it with the `ConnectionStrings__cache` and `ConnectionStrings__appdb` environment variables, which contain connection strings to these resources.

### Access specific endpoint information

For more control over how connection information is passed to your executable:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("cache");

var app = builder.AddExecutable("app", "node", ".", "app.js")
    .WithReference(redis)
    .WithEnvironment(context =>
    {
        // Provide individual connection details
        context.EnvironmentVariables["REDIS_HOST"] = redis.Resource.PrimaryEndpoint.Property(EndpointProperty.Host);
        context.EnvironmentVariables["REDIS_PORT"] = redis.Resource.PrimaryEndpoint.Property(EndpointProperty.Port);
    });
```

## Practical example: Vercel CLI

Here's a complete example using the [Vercel CLI](https://vercel.com/docs/cli) to host a frontend application with a backend API:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Backend API
var api = builder.AddProject<Projects.Api>("api")
    .WithExternalHttpEndpoints();

// Frontend with Vercel CLI
var frontend = builder.AddExecutable(
        "vercel-dev", "vercel", ".", "dev", "--listen", "3000")
    .WithEnvironment("API_URL", api.GetEndpoint("http"))
    .WithHttpEndpoint(port: 3000, name: "http");

builder.Build().Run();
```

## Configure endpoints

Executable resources can expose HTTP endpoints that other resources can reference:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var frontend = builder.AddExecutable(
        "vite-dev", "npm", ".", "run", "dev", "--", "--port", "5173", "--host", "0.0.0.0")
    .WithHttpEndpoint(port: 5173, name: "http");

// Another service can reference the frontend
var e2eTests = builder.AddExecutable("playwright", "npx", ".", "playwright", "test")
    .WithEnvironment("BASE_URL", frontend.GetEndpoint("http"));
```

## Environment configuration

Configure environment variables for your executable:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var app = builder.AddExecutable(
        "api", "uvicorn", ".", "main:app", "--reload", "--host", "0.0.0.0")
    .WithEnvironment("DEBUG", "true")
    .WithEnvironment("LOG_LEVEL", "info")
    .WithEnvironment(context =>
    {
        // Dynamic environment variables
        context.EnvironmentVariables["START_TIME"] = DateTimeOffset.UtcNow.ToString();
    });
```

## Publishing with PublishAsDockerfile

For production deployment, executable resources need to be containerized. Use the <xref:Aspire.Hosting.ExecutableResourceBuilderExtensions.PublishAsDockerFile*> method to specify how the executable should be packaged:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var app = builder.AddExecutable(
        "frontend", "npm", ".", "start", "--port", "3000")
    .PublishAsDockerfile();
```

When you call `PublishAsDockerfile()`, .NET Aspire generates a Dockerfile during the publish process. You can customize this by providing your own Dockerfile:

### Custom Dockerfile for publishing

Create a `Dockerfile` in your executable's working directory:

```dockerfile
FROM node:22-alpine
WORKDIR /app
COPY package*.json ./
RUN npm ci --only=production
COPY . .
EXPOSE 3000
CMD ["npm", "start"]
```

Then reference it in your app host:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var app = builder.AddExecutable("frontend", "npm", ".", "start")
    .PublishAsDockerfile([new DockerfileBuildArg("NODE_ENV", "production")]);
```

## Best practices

When working with executable resources:

1. **Use explicit paths**: For better reliability, use full paths to executables when possible.
1. **Handle dependencies**: Use `WithReference` to establish proper dependency relationships.
1. **Configure explicit start**: Use `WithExplicitStart()` for executables that shouldn't start automatically.
1. **Prepare for deployment**: Always use `PublishAsDockerfile()` for production scenarios.
1. **Environment isolation**: Use environment variables rather than command-line arguments for sensitive configuration.
1. **Resource naming**: Use descriptive names that clearly identify the executable's purpose.

## See also

- [App host overview](../fundamentals/app-host-overview.md)
- [Add Dockerfiles to the app model](withdockerfile.md)
- [Node.js apps in .NET Aspire](../get-started/build-aspire-apps-with-nodejs.md)
- [Python apps in .NET Aspire](../get-started/build-aspire-apps-with-python.md)
