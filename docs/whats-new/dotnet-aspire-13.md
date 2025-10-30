---
title: What's new in Aspire 13
description: Learn what's new in Aspire 13.
ms.date: 10/25/2025
ai-usage: ai-generated
---

# What's new in Aspire 13

📢 Aspire 13 is the next major release of Aspire. It supports:

- .NET 10.0 (Release Candidate and future releases).

> [!NOTE]
> Aspire 13 requires .NET 10 and is designed to work with the latest .NET runtime features and capabilities.

This release introduces several important improvements focused on developer experience, polyglot support, and enhanced security. To join the community, visit us on [:::image type="icon" source="../media/discord-icon.svg" border="false"::: Discord](https://discord.com/invite/h87kDAHQgJ) to chat with team members and collaborate with us on [:::image type="icon" source="../media/github-mark.svg" border="false"::: GitHub](https://github.com/dotnet/aspire).

For more information on the official .NET version and Aspire version support, see:

- [.NET support policy](https://dotnet.microsoft.com/platform/support/policy): Definitions for LTS and STS.
- [Aspire support policy](https://dotnet.microsoft.com/platform/support/policy/aspire): Important unique product lifecycle details.

## Upgrade to Aspire 13

To upgrade from earlier versions of Aspire to Aspire 13, update your project files and dependencies to reference the Aspire 13 packages. Ensure you're using .NET 10 SDK or later.

## AppHost and orchestration enhancements

Aspire 13 introduces several improvements to the AppHost and orchestration capabilities.

### Simplified AppHost SDK usage

The `Aspire.AppHost.Sdk` now supports being set as the sole project SDK. It implicitly adds a package reference to the `Aspire.Hosting.AppHost` package with the version matching the SDK version if a reference isn't explicitly defined.

```xml
<Project Sdk="Aspire.AppHost.Sdk/13.0.0">
    
    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net10.0</TargetFramework>
        <!-- Omitted for brevity -->
    </PropertyGroup>

    <!-- Omitted for brevity -->

</Project>
```

The alternative approach of explicitly listing the `<Sdk />` and `<PackageReference />` elements still works, and it's not a requirement that projects be changed to use the new format.

### AddCSharpApp support

Aspire 13 adds support for `CSharpAppResource` and `AddCSharpApp` as built-in resource types for orchestrating C# projects and file-based apps. This provides an alternative to `AddProject` for certain scenarios.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var app = builder.AddCSharpApp("myapp", "path/to/app.cs");

builder.Build().Run();
```

### Resource certificate trust customization

Aspire 13 introduces new features for customizing certificate trust behavior for resources. This enables better control over SSL/TLS certificate validation in development and production scenarios.

For more information on configuring certificate trust, see the documentation on resource certificate trust customization.

## Polyglot environment variables

Aspire 13 introduces enhanced environment variable support for polyglot applications. When you reference a service or endpoint, Aspire now generates more intuitive environment variables that are easier to use from non-.NET languages.

For example:

```csharp
var projectA = builder.AddProject<ProjectA>("projecta");
var projectB = builder.AddProject<ProjectB>("projectb")
    .WithReference(projectA);
```

This generates environment variables like `PROJECTA_HTTP` and `PROJECTA_HTTPS`, with the pattern `{RESOURCENAME}_{ENDPOINTNAME}`. This makes it easier to consume Aspire-orchestrated services from Python, Node.js, and other languages.

To get a specific endpoint with a custom environment variable name:

```csharp
var projectA = builder.AddProject<ProjectA>("projecta");
var projectB = builder.AddProject<ProjectB>("projectb")
    .WithEnvironment("PROJECTA_URL", projectA.GetEndpoint("https"));
```

This approach provides a more accessible way for polyglot applications to discover and connect to services.

## Python support

Aspire 13 adds first-class support for Python applications, making it easier to build polyglot distributed applications with .NET and Python components.

### Aspire Python templates

New Aspire Python templates help developers get started quickly with Python in Aspire applications. These templates provide scaffolding for Python backend services that integrate seamlessly with the Aspire orchestration model.

### Onboarding existing Python applications

Aspire 13 provides comprehensive documentation and samples for developers who want to add Aspire orchestration to their existing Python applications. This includes:

- Integration steps for adding Python projects to an Aspire AppHost
- Best practices for configuring Python environments
- Samples demonstrating Python and .NET service communication
- Troubleshooting guides and common patterns

For more information, see the Python integration documentation.

## Dashboard enhancements

### OpenID Connect claims actions configuration

The Aspire dashboard now supports configuring claims actions for OpenID Connect authentication. This enables you to customize how claims from your identity provider are mapped to dashboard user identities.

You can configure claims actions using the dashboard configuration:

```json
{
  "Dashboard": {
    "Frontend": {
      "OpenIdConnect": {
        "ClaimActions": [
          {
            "ClaimType": "role",
            "JsonKey": "role"
          }
        ]
      }
    }
  }
}
```

Or using environment variables:

```bash
export Dashboard__Frontend__OpenIdConnect__ClaimActions__0__ClaimType="role"
export Dashboard__Frontend__OpenIdConnect__ClaimActions__0__JsonKey="role"
```

For more information, see the dashboard configuration documentation.

## Encoded parameters

Aspire 13 introduces encoded parameters, which provide enhanced security and flexibility when passing sensitive configuration data between resources. Encoded parameters help ensure that connection strings, API keys, and other sensitive values are properly protected during deployment.

For more information on using encoded parameters, see the manifest format documentation.

## Breaking changes

### DefaultAzureCredential defaults to ManagedIdentityCredential on Azure Container Apps and App Service

With Aspire 13, the default behavior of `DefaultAzureCredential` when deploying to Azure Container Apps and Azure App Service has changed to only use `ManagedIdentityCredential`.

**Previous behavior:** `DefaultAzureCredential` would use the full chain of identities by default, including `EnvironmentCredential` and `WorkloadIdentityCredential` before `ManagedIdentityCredential`.

**New behavior:** `DefaultAzureCredential` now only uses `ManagedIdentityCredential`.

This change enforces Azure SDK best practices. For more information, see [Use deterministic credentials in production environments](/dotnet/azure/sdk/authentication/best-practices?tabs=aspdotnet#use-deterministic-credentials-in-production-environments).

**Recommended action:** If you were relying on `EnvironmentCredential` or `WorkloadIdentityCredential` in your application, you can choose one of the following to revert to old behavior:

1. Don't use `DefaultAzureCredential` in your application. Instead, explicitly use `EnvironmentCredential` or `WorkloadIdentityCredential` in production.
2. Implement a `PublishAsAzureContainerApp` callback and remove the environment variable from the bicep:

```csharp
builder.AddProject<Projects.Frontend>("frontend")
    .PublishAsAzureContainerApp((infra, app) =>
    {
        // Remove the AZURE_TOKEN_CREDENTIALS env var
        var containerAppContainer = app.Template.Containers[0].Value!;
        var azureTokenCredentialEnv = containerAppContainer.Env
            .Single(v => v.Value!.Name.Value == "AZURE_TOKEN_CREDENTIALS");
        containerAppContainer.Env.Remove(azureTokenCredentialEnv);
    });
```

For a complete listing of all breaking changes, see [Breaking changes in Aspire 13](../compatibility/13.0/index.md).

## See also

- [Aspire setup and tooling](../fundamentals/setup-tooling.md)
- [Aspire SDK](../fundamentals/dotnet-aspire-sdk.md)
- [Aspire templates](../fundamentals/aspire-sdk-templates.md)
- [Aspire orchestration overview](../fundamentals/app-host-overview.md)
- [Aspire dashboard overview](../fundamentals/dashboard/overview.md)
- [Explore the Aspire dashboard](../fundamentals/dashboard/explore.md)
