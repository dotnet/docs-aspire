---
title: Customize Azure deployment infrastructure
description: Learn how to customize Azure infrastructure resources during deployment using ConfigureInfrastructure and other customization patterns.
ms.topic: how-to
ms.date: 12/23/2024
---

# Customize Azure deployment infrastructure

.NET Aspire provides powerful capabilities to customize Azure infrastructure during deployment. While .NET Aspire offers sensible defaults for Azure resources, you often need to customize these resources to meet organizational requirements such as tagging policies, specific SKUs, networking configurations, or compliance standards.

This article demonstrates common patterns for customizing Azure infrastructure in .NET Aspire applications using the `ConfigureInfrastructure` API and other customization techniques.

## Prerequisites

- [.NET Aspire SDK](../../fundamentals/setup-tooling.md)
- An Azure subscription
- Basic understanding of [.NET Aspire app host](../../fundamentals/app-host-overview.md)

## Overview of infrastructure customization

All .NET Aspire Azure hosting integrations expose Azure resources that are subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type. This enables a fluent API for customizing the infrastructure using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> extension method.

The `ConfigureInfrastructure` method allows you to:

- Modify resource properties like SKUs, tiers, and configurations
- Add tags for governance and cost management
- Configure networking and security settings
- Set up role assignments and permissions
- Customize generated Bicep templates

## Basic infrastructure customization

### Configure resource properties

Use `ConfigureInfrastructure` to modify basic properties of Azure resources:

:::code language="csharp" source="../../snippets/azure/AppHost/Program.ConfigureStorageInfra.cs" id="configure":::

The preceding code customizes an Azure Storage account by:

- Setting the access tier to `Cool`
- Upgrading the SKU to `PremiumZrs`
- Adding a custom tag

### Add tags for governance

Tags are essential for governance, cost management, and compliance. Here's how to add tags to various Azure resources:

:::code language="csharp" source="../../snippets/azure/AppHost/Program.ConfigureSqlServerInfra.cs" id="configure":::

### Configure SKUs and performance tiers

Customize performance characteristics by modifying SKUs:

:::code language="csharp" source="../../snippets/azure/AppHost/Program.ConfigureServiceBusInfra.cs" id="configure":::

## Advanced customization patterns

### Using external parameters

Flow configuration values from external parameters into your infrastructure:

```csharp
var environmentParam = builder.AddParameter("environment");
var costCenterParam = builder.AddParameter("cost-center");

var storage = builder.AddAzureStorage("storage")
    .ConfigureInfrastructure(infra =>
    {
        var storageAccount = infra.GetProvisionableResources()
                                 .OfType<StorageAccount>()
                                 .Single();

        // Use parameters in resource configuration
        storageAccount.Tags.Add("Environment", environmentParam.AsProvisioningParameter(infra));
        storageAccount.Tags.Add("CostCenter", costCenterParam.AsProvisioningParameter(infra));
        
        // Conditional configuration based on environment
        var env = environmentParam.AsProvisioningParameter(infra);
        storageAccount.AccessTier = env == "Production" 
            ? StorageAccountAccessTier.Hot 
            : StorageAccountAccessTier.Cool;
    });
```

### Configure networking and security

Set up networking configurations and security settings:

```csharp
builder.AddAzureKeyVault("keyvault")
    .ConfigureInfrastructure(infra =>
    {
        var keyVault = infra.GetProvisionableResources()
                           .OfType<KeyVaultService>()
                           .Single();

        // Enable purge protection
        keyVault.EnablePurgeProtection = true;
        
        // Configure network access
        keyVault.NetworkRuleSet = new KeyVaultNetworkRuleSet
        {
            DefaultAction = KeyVaultNetworkRuleAction.Deny,
            IpRules = 
            {
                new KeyVaultIPRule { Value = "203.0.113.0/24" }
            }
        };

        keyVault.Tags.Add("SecurityLevel", "High");
    });
```

### Multiple resource configuration

When working with resources that create multiple Azure components:

:::code language="csharp" source="../../snippets/azure/AppHost/Program.ConfigureEventHubsInfra.cs" id="configure":::

## Common customization scenarios

### Enterprise tagging strategy

Implement a consistent tagging strategy across all resources:

```csharp
public static class TaggingExtensions
{
    public static IResourceBuilder<T> WithStandardTags<T>(
        this IResourceBuilder<T> builder,
        string owner,
        string environment,
        string costCenter) 
        where T : AzureProvisioningResource
    {
        return builder.ConfigureInfrastructure(infra =>
        {
            var resources = infra.GetProvisionableResources();
            
            foreach (var resource in resources)
            {
                if (resource.Tags != null)
                {
                    resource.Tags.Add("Owner", owner);
                    resource.Tags.Add("Environment", environment);
                    resource.Tags.Add("CostCenter", costCenter);
                    resource.Tags.Add("ManagedBy", "AspireApp");
                    resource.Tags.Add("CreatedDate", DateTime.UtcNow.ToString("yyyy-MM-dd"));
                }
            }
        });
    }
}

// Usage
builder.AddAzureStorage("storage")
    .WithStandardTags("TeamAlpha", "Production", "Engineering");
```

### Cost optimization

Configure resources for cost optimization:

```csharp
builder.AddAzureAppConfiguration("config")
    .ConfigureInfrastructure(infra =>
    {
        var appConfig = infra.GetProvisionableResources()
                            .OfType<AppConfigurationStore>()
                            .Single();

        // Use Free tier for development
        appConfig.SkuName = "Free";
        
        // Disable features that incur costs
        appConfig.EnablePurgeProtection = false;
        
        appConfig.Tags.Add("CostOptimized", "true");
    });
```

### Compliance and governance

Configure resources to meet compliance requirements:

```csharp
builder.AddAzureSqlServer("sql-server")
    .ConfigureInfrastructure(infra =>
    {
        var sqlServer = infra.GetProvisionableResources()
                            .OfType<SqlServer>()
                            .Single();

        // Enable Azure AD authentication
        sqlServer.ActiveDirectoryAdministrator = new SqlServerExternalAdministrator
        {
            AdministratorType = "ActiveDirectory",
            Login = "admin@contoso.com",
            PrincipalType = "User"
        };

        // Configure audit settings
        sqlServer.IsAuditEnabled = true;
        
        // Add compliance tags
        sqlServer.Tags.Add("ComplianceLevel", "SOC2");
        sqlServer.Tags.Add("DataClassification", "Confidential");
    });
```

## Working with custom Bicep templates

For advanced scenarios, you can also work with custom Bicep templates. For more information, see [Use custom Bicep templates](../../azure/integrations-overview.md#use-custom-bicep-templates).

## Best practices

1. **Consistent tagging**: Implement a standardized tagging strategy across all resources.

1. **Environment-specific configuration**: Use parameters to vary configuration based on deployment environment.

1. **Security by default**: Configure security settings like network restrictions and authentication in your infrastructure customizations.

1. **Cost awareness**: Consider cost implications when customizing SKUs and features.

1. **Documentation**: Document custom infrastructure patterns for your team.

## See also

- [Azure integrations overview](../../azure/integrations-overview.md)
- [Local Azure provisioning](../../azure/local-provisioning.md)
- [External parameters](../../fundamentals/external-parameters.md)
- [Deploy to Azure Container Apps](aca-deployment.md)
