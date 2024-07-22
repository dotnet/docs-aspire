---
title: Add Dockerfiles to your .NET app model
description: Learn how to add Dockerfiles to your .NET app model.
ms.date: 07/19/2024
---

# Add Dockerfiles to your .NET app model

Using .NET Aspire it is possible to specify a _Dockerfile_ to build when the apphost is started using either the <xref:Aspire.Hosting.ResourceBuilderExtensions.AddDockerfile%2A> or <xref:Aspire.Hosting.ResourceBuilderExtensions.WithDockerfile%2A> extension methods.

In the example below the <xref:Aspire.Hosting.ResourceBuilderExtensions.AddDockerfile%2A> extension method is used to specify a container by referencing the context path for the container build.

```csharp
var builder = DistributedApplication.CreateBuilder(args);
var container = builder.AddDockerfile("mycontainer", "relative/context/path");
```

Unless the context path argument is a rooted path the context path is interpretted as being relative to the app host projects directory (where the AppHost `*.csproj` folder is located).

By default the name of the Dockerfile which is used is `Dockerfile` and is expected to be within the context path directory. It is possible to explicitly specify the _Dockerfile_ name either as an absolute path or a relative path to the context path.

This is useful if you wish to modify the specific dockerfile being used when running locally vs. deployment.

```csharp
var builder = DistributedApplication.CreateBuilder(args);
var container = builder.ExecutionContext.IsRunMode
    ? builder.AddDockerfile("mycontainer", "relative/context/path", "Dockerfile.debug")
    : builder.AddDockerfile("mycontainer", "relative/context/path", "Dockerfile.release");
```

## Customizing existing container resources

When using <xref:Aspire.Hosting.ResourceBuilderExtensions.AddDockerfile%2A> the return value is an `IResourceBuilder<ContainerResource>`. .NET Aspire includes many custom resource types that are derived from <xref:Aspire.Hosting.ApplicationModel.ContainerResource>.

Using the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithDockerfile%2A> extension method it is possible to continue using these strongly typed resource types and customize the underlying container that is used.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var pgsql = builder.AddPostgres("pgsql")
                   .WithDockerfile("path/to/context")
                   .WithPgAdmin();
```

## Passing build arguments

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithBuildArg%2A> method can be used to pass arguments into the container image build.

```csharp
var builder = DistributedApplication.CreateBuilder(args);
var container = builder.AddDockerfile("mygoapp", "relative/context/path")
                       .WithBuildArg("GO_VERSION", "1.22");
```

The value parameter on the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithBuildArg%2A> method can be a literal value (boolean, string, int) or it can be a resource builder for a parameter resource. The following code replaces the `GO_VERSION` with a parameter value that can be specified at deployment time.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var goVersion = builder.AddParameter("goversion");

var container = builder.AddDockerfile("mygoapp", "relative/context/path")
                       .WithBuildArg("GO_VERSION", goVersion);
```

Build arguments correspond to the `ARG` command in _Dockerfiles_. Expanding the example above, this is a mult-stage Dockerfile which specifies specific container image version to use as a parameter.

```Dockerfile
# Stage 1: Build the Go program
ARG GO_VERSION=1.22
FROM golang:${GO_VERSION} AS builder
WORKDIR /build
COPY . .
RUN go build mygoapp.go

# Stage 2: Run the Go program
FROM mcr.microsoft.com/cbl-mariner/base/core:2.0
WORKDIR /app
COPY --from=builder /build/mygoapp .
CMD ["./mygoapp"]
```

> NOTE: Build arguments hardcode values into the produced container image which means that the container image needs to rebuilt whenever a change is required. This is often not desirable and the use of environment variables is recommended to values that change frequently.

## Passing build secrets

In addition to build arguments it is possible to specify build secrets using <xref:Aspire.Hosting.ResourceBuilderExtensions.WithBuildSecret%2A> which are made selectively available to individual commands in the _Dockerfile_ using the `--mount=type=secret` syntax on `RUN` commands.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var accessToken = builder.AddParameter("accesstoken", secret: true);

var container = builder.AddDockerfile("myapp", "relative/context/path")
                       .WithBuildSecret("ACCESS_TOKEN", accessToken);
```

Example of `RUN` command in Dockerfile which exposes the specified secret to the specific command:

```
# The helloworld command can read the secret from /run/secrets/ACCESS_TOKEN
RUN --mount=type=secret,id=ACCESS_TOKEN helloworld
```

> NOTE: Caution should be used whenever secrets are handed in build environments. Common scenarios include using a token to restore dependencies from private repositories/feeds prior to a build occuring. The injected secrets should not be copied into the final or intermediate images.
