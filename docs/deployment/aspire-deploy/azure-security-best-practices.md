---
title: Azure security best practices for .NET Aspire deployments
description: Learn about the default security posture of .NET Aspire deployments to Azure Container Apps and additional steps to enhance security.
ms.date: 01/15/2025
uid: deployment/azure/azure-security-best-practices
---

# Azure security best practices for .NET Aspire deployments

When you deploy .NET Aspire applications to Azure using the Aspire CLI or Azure Developer CLI (`azd`), the platform provides several security features by default. This article explains the default security posture and provides guidance on additional steps you can take to enhance the security of your applications.

> [!div class="checklist"]
>
> - Understand the default security configuration for .NET Aspire deployments
> - Learn about built-in security features in Azure Container Apps
> - Implement additional security hardening measures
> - Configure secure communication between services
> - Apply security best practices for production deployments

## Default security posture

When you deploy a .NET Aspire application to Azure Container Apps using `azd`, several security measures are automatically configured:

### Container-level security

**Secure base images**: .NET Aspire generates container images using secure Microsoft-maintained base images that are regularly updated with security patches.

**Non-root execution**: Containers run with non-root user permissions by default, reducing the attack surface.

**Minimal image surface**: The generated containers include only the necessary runtime components, following the principle of least privilege.

### Network security

**Private networking**: Azure Container Apps environments use virtual networks with private IP addresses for internal communication between services.

**Secure service-to-service communication**: Inter-service communication uses Azure Container Apps' built-in service discovery with encrypted traffic.

**HTTPS endpoints**: Public-facing endpoints are automatically configured with HTTPS using Azure-managed certificates.

### Identity and access management

**Managed identities**: Azure Container Apps are configured with system-assigned managed identities by default, eliminating the need to store credentials in your application code.

**Azure RBAC integration**: Resources are deployed with appropriate role-based access control (RBAC) permissions.

### Configuration and secrets management

**Secure configuration**: Application configuration is stored in Azure App Configuration when available, separate from your application code.

**Environment variables**: Sensitive configuration values are injected as secure environment variables rather than being embedded in container images.

## Additional security hardening

While .NET Aspire provides good security defaults, you can implement additional measures for enhanced protection:

### Enable Azure Key Vault integration

Store sensitive configuration data and secrets in Azure Key Vault:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add Azure Key Vault resource
var keyVault = builder.AddAzureKeyVault("key-vault");

// Reference Key Vault in your services
builder.AddProject<Projects.ApiService>()
       .WithReference(keyVault);
```

For more information, see [.NET Aspire Azure Key Vault integration](../../security/azure-security-key-vault-integration.md).

### Configure user-assigned managed identities

For more granular control over permissions, use user-assigned managed identities:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Create or reference a user-assigned managed identity
var managedIdentity = builder.AddAzureUserAssignedIdentity("app-identity");

// Apply the identity to your container apps
builder.AddProject<Projects.ApiService>()
       .WithReference(managedIdentity);
```

For detailed guidance, see [User-assigned managed identity](../../azure/user-assigned-managed-identity.md).

### Enable comprehensive monitoring

**Application Insights**: Monitor your applications for security anomalies and performance issues:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add Application Insights
var appInsights = builder.AddAzureApplicationInsights("monitoring");

builder.AddProject<Projects.ApiService>()
       .WithReference(appInsights);
```

For more information, see [Use .NET Aspire with Application Insights](application-insights.md).

**Azure Monitor**: Configure alerts for suspicious activities and resource consumption anomalies.

## Additional security resources

For comprehensive guidance on Azure security, see [Azure security best practices and patterns](/azure/security/fundamentals/best-practices-and-patterns).

## Next steps

- [Deploy a .NET Aspire project to Azure Container Apps](aca-deployment.md)
- [Customize .NET Aspire Azure deployments](customize-deployments.md)
- [.NET Aspire Azure Key Vault integration](../../security/azure-security-key-vault-integration.md)
- [User-assigned managed identity](../../azure/user-assigned-managed-identity.md)
