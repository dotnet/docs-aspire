---
title: "Breaking change - DefaultAzureCredential defaults to ManagedIdentityCredential on ACA and App Service"
description: "Learn about the breaking change in Aspire 13.0 where DefaultAzureCredential behavior is changed to only use ManagedIdentityCredential when deploying to Azure Container Apps and Azure App Service."
ms.date: 10/17/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/5154
---

# DefaultAzureCredential defaults to ManagedIdentityCredential on ACA and App Service

In Aspire 13.0, when deploying to Azure Container Apps and Azure App Service, the default behavior of `DefaultAzureCredential` has been changed to only use `ManagedIdentityCredential`. This is accomplished by setting the `AZURE_TOKEN_CREDENTIALS` environment variable to ensure deterministic credential resolution.

## Version introduced

Aspire 13.0

## Previous behavior

Previously, `DefaultAzureCredential` used the full chain of identity providers by default. This meant that `EnvironmentCredential` and `WorkloadIdentityCredential` would be attempted before `ManagedIdentityCredential` when authenticating to Azure resources.

```csharp
// No explicit environment variable was set
// DefaultAzureCredential would try credentials in this order:
// 1. EnvironmentCredential
// 2. WorkloadIdentityCredential
// 3. ManagedIdentityCredential
// ... and others
```

## New behavior

Now `DefaultAzureCredential` only uses `ManagedIdentityCredential` when deploying to Azure Container Apps and Azure App Service. This is achieved by setting the `AZURE_TOKEN_CREDENTIALS` environment variable automatically.

```csharp
// The AZURE_TOKEN_CREDENTIALS environment variable is automatically set
// DefaultAzureCredential now only uses ManagedIdentityCredential
```

This change forces `DefaultAzureCredential` to behave in a deterministic manner and optimizes the underlying `ManagedIdentityCredential` for resilience.

## Type of breaking change

This is a [behavioral change](../categories.md#behavioral-change).

## Reason for change

This change enforces Azure SDK best practices for production environments. Using deterministic credentials improves both security and reliability by:

- Ensuring predictable authentication behavior
- Reducing potential authentication failures from trying multiple credential types
- Optimizing credential acquisition performance
- Following the principle of least privilege by using managed identities

For more information, see:

- [Use deterministic credentials in production environments](https://learn.microsoft.com/dotnet/azure/sdk/authentication/best-practices?tabs=aspdotnet#use-deterministic-credentials-in-production-environments)
- [GitHub PR dotnet/aspire#11832](https://github.com/dotnet/aspire/pull/11832)
- [Azure SDK resilience improvements](https://github.com/Azure/azure-sdk-for-net/pull/52545)

## Recommended action

If you were relying on `EnvironmentCredential` or `WorkloadIdentityCredential` in your application, you can choose one of the following options to revert to the old behavior or adapt your code:

### Option 1: Use specific credential types explicitly

Don't use `DefaultAzureCredential` in your application. Instead, explicitly use `EnvironmentCredential` or `WorkloadIdentityCredential` in production code.

### Option 2: Remove the environment variable in your deployment

Implement a `PublishAsAzureContainerApp` callback and remove the environment variable from the bicep:

```csharp
builder.AddProject<Projects.Frontend>("frontend")
    .PublishAsAzureContainerApp((infra, app) =>
    {
        // Remove the AZURE_TOKEN_CREDENTIALS env var to restore previous behavior
        var containerAppContainer = app.Template.Containers[0].Value!;
        var azureTokenCredentialEnv = containerAppContainer.Env
            .Single(v => v.Value!.Name.Value == "AZURE_TOKEN_CREDENTIALS");
        containerAppContainer.Env.Remove(azureTokenCredentialEnv);
    });
```

For Azure App Service, use a similar approach with `PublishAsAzureAppService`:

```csharp
builder.AddProject<Projects.Frontend>("frontend")
    .PublishAsAzureAppService((infra, app) =>
    {
        // Remove the AZURE_TOKEN_CREDENTIALS env var to restore previous behavior
        var settings = app.Properties.SiteConfig.Value!.AppSettings!;
        var azureTokenCredentialSetting = settings
            .Single(s => s.Value!.Name.Value == "AZURE_TOKEN_CREDENTIALS");
        settings.Remove(azureTokenCredentialSetting);
    });
```

## Affected APIs

- <xref:Aspire.Hosting.Azure.AzureContainerAppExtensions.AddAzureContainerAppEnvironment*?displayProperty=fullName>
- <xref:Aspire.Hosting.Azure.AzureAppServiceEnvironmentExtensions.AddAzureAppServiceEnvironment*?displayProperty=fullName>
