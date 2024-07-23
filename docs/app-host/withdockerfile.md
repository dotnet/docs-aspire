---
title: Add Dockerfiles to your .NET app model
description: Learn how to add Dockerfiles to your .NET app model.
ms.date: 07/23/2024
---

# Add Dockerfiles to your .NET app model

With .NET Aspire it's possible to specify a _Dockerfile_ to build when the [app host](../fundamentals/app-host-overview.md) is started using either the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.AddDockerfile%2A> or <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithDockerfile%2A> extension methods.

## Add a Dockerfile to the app model

In the following example the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.AddDockerfile%2A> extension method is used to specify a container by referencing the context path for the container build.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var container = builder.AddDockerfile(
    "mycontainer", "relative/context/path");
```

Unless the context path argument is a rooted path the context path is interpreted as being relative to the app host projects directory (where the AppHost `*.csproj` folder is located).

By default the name of the _Dockerfile_ which is used is `Dockerfile` and is expected to be within the context path directory. It's possible to explicitly specify the _Dockerfile_ name either as an absolute path or a relative path to the context path.

This is useful if you wish to modify the specific _Dockerfile_ being used when running locally or when the app host is deploying.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var container = builder.ExecutionContext.IsRunMode
    ? builder.AddDockerfile(
          "mycontainer", "relative/context/path", "Dockerfile.debug")
    : builder.AddDockerfile(
          "mycontainer", "relative/context/path", "Dockerfile.release");
```

## Customize existing container resources

When using <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.AddDockerfile%2A> the return value is an `IResourceBuilder<ContainerResource>`. .NET Aspire includes many custom resource types that are derived from <xref:Aspire.Hosting.ApplicationModel.ContainerResource>.

Using the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithDockerfile%2A> extension method it's possible to continue using these strongly typed resource types and customize the underlying container that is used.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var pgsql = builder.AddPostgres("pgsql")
                   .WithDockerfile("path/to/context")
                   .WithPgAdmin();
```

## Pass build arguments

The <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithBuildArg%2A> method can be used to pass arguments into the container image build.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var container = builder.AddDockerfile("mygoapp", "relative/context/path")
                       .WithBuildArg("GO_VERSION", "1.22");
```

The value parameter on the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithBuildArg%2A> method can be a literal value (`boolean`, `string`, `int`) or it can be a resource builder for a [parameter resource](../fundamentals/external-parameters.md). The following code replaces the `GO_VERSION` with a parameter value that can be specified at deployment time.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var goVersion = builder.AddParameter("goversion");

var container = builder.AddDockerfile("mygoapp", "relative/context/path")
                       .WithBuildArg("GO_VERSION", goVersion);
```

Build arguments correspond to the [`ARG` command](https://docs.docker.com/build/guide/build-args/) in _Dockerfiles_. Expanding the preceding example, this is a multi-stage _Dockerfile_ which specifies specific container image version to use as a parameter.

```dockerfile
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

> [!NOTE]
> Instead of hardcoding values into the container image, it's recommended to use environment variables for values that frequently change. This avoids the need to rebuild the container image whenever a change is required.

## Pass build secrets

In addition to build arguments it's possible to specify build secrets using <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithBuildSecret%2A> which are made selectively available to individual commands in the _Dockerfile_ using the `--mount=type=secret` syntax on `RUN` commands.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var accessToken = builder.AddParameter("accesstoken", secret: true);

var container = builder.AddDockerfile("myapp", "relative/context/path")
                       .WithBuildSecret("ACCESS_TOKEN", accessToken);
```

For example, consider the `RUN` command in a _Dockerfile_ which exposes the specified secret to the specific command:

```dockerfile
# The helloworld command can read the secret from /run/secrets/ACCESS_TOKEN
RUN --mount=type=secret,id=ACCESS_TOKEN helloworld
```

> [!CAUTION]
> Caution should be exercised when passing secrets in build environments. This is often done when using a token to retrieve dependencies from private repositories or feeds before a build. It is important to ensure that the injected secrets are not copied into the final or intermediate images.
