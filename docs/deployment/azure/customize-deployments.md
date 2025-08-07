---
title: Customize .NET Aspire Azure deployments
description: Learn how to use Azure Developer CLI (azd) infrastructure generation to customize and configure .NET Aspire deployments for production scenarios with Bicep templates, security best practices, and scalable resource configurations.
ms.date: 06/25/2025
ms.custom: devx-track-extended-azdevcli
---

# Customize .NET Aspire Azure deployments

The Azure Developer CLI (`azd`) provides a powerful feature called infrastructure generation that allows you to generate and customize the underlying infrastructure code for your .NET Aspire applications. This capability is essential for production scenarios where you need fine-grained control over Azure resources, security configurations, and deployment patterns.

This article covers how to use `azd infra gen` to:

> [!div class="checklist"]
>
> - Generate Bicep infrastructure files from your .NET Aspire app model.
> - Customize generated infrastructure for production requirements.
> - Apply security best practices to generated resources.
> - Manage infrastructure as code with proper version control.

[!INCLUDE [aspire-prereqs](../../includes/aspire-prereqs.md)]

You will also need to have the Azure Developer CLI [installed locally](/azure/developer/azure-developer-cli/install-azd).

## How infrastructure generation works

Infrastructure generation in `azd` transforms your .NET Aspire app model into concrete Azure infrastructure definitions using Bicep templates. This process bridges the gap between the development-time orchestration in .NET Aspire and the production infrastructure required in Azure.

When you run `azd infra gen`, the CLI:

1. Analyzes your .NET Aspire AppHost project.
1. Identifies all resources and their dependencies.
1. Generates corresponding Azure resource definitions in Bicep.
1. Creates supporting configuration files for deployment.

## Use infrastructure generation

Call the generate infrastructure command on your .NET Aspire solution:

```azdeveloper
azd infra gen
```

This command creates an `infra` folder in your AppHost project directory with the following structure:

```Directory
└───📂 infra
     ├─── abbreviations.json   # Azure resource naming conventions  
     ├─── main.bicep           # Main infrastructure entry point
     ├─── main.parameters.json # Parameter values for deployment
     └─── resources.bicep      # Resource definitions    
```

## Production considerations

The generated infrastructure provides a solid foundation for your deployment, but production environments require additional configuration for security, scalability, and maintainability. This section covers the key areas you should customize when preparing for production deployment.

### Security configurations

When preparing for production deployments, review and enhance the generated infrastructure with appropriate security controls:

**Network isolation:**

```bicep
// Example: Configure Container Apps Environment with network restrictions
resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' = {
  name: environmentName
  location: location
  properties: {
    vnetConfiguration: {
      infrastructureSubnetId: subnetId
      internal: true
    }
    workloadProfiles: [
      {
        name: 'Consumption'
        workloadProfileType: 'Consumption'
      }
    ]
  }
}
```

**Identity and access management:**

```bicep
// Example: Configure managed identity with least privilege access
resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: identityName
  location: location
}

// Assign specific roles rather than broad permissions
resource acrPullRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: containerRegistry
  name: guid(containerRegistry.id, managedIdentity.id, 'AcrPull')
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d') // AcrPull
    principalId: managedIdentity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}
```

### Resource sizing and scaling

Review generated resource configurations for production requirements:

```bicep
// Example: Configure appropriate resource limits
resource containerApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: appName
  location: location
  properties: {
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
        allowInsecure: false // Ensure HTTPS only
      }
    }
    template: {
      containers: [
        {
          name: containerName
          image: image
          resources: {
            cpu: json('1.0')      // Adjust based on load requirements
            memory: '2.0Gi'       // Adjust based on memory needs
          }
        }
      ]
      scale: {
        minReplicas: 2          // Ensure availability
        maxReplicas: 10         // Control costs
        rules: [
          {
            name: 'http-requests'
            http: {
              metadata: {
                concurrentRequests: '100'
              }
            }
          }
        ]
      }
    }
  }
}
```

### Environment-specific configurations

Use parameters to manage environment-specific settings:

```bicep
@description('Environment name (dev, staging, prod)')
param environmentType string = 'dev'

@description('Application tier configuration')
var tierConfigurations = {
  dev: {
    skuName: 'Consumption'
    replicas: 1
  }
  staging: {
    skuName: 'Dedicated'
    replicas: 2
  }
  prod: {
    skuName: 'Dedicated'
    replicas: 3
  }
}

var currentTier = tierConfigurations[environmentType]
```

## Iterative customization workflow

After generating initial infrastructure, establish a workflow for ongoing customization:

1. **Make infrastructure changes** to the generated Bicep files.
2. **Test deployments** in development environments.
3. **Version control** your infrastructure changes.
4. **Document customizations** for team collaboration.

> [!IMPORTANT]
> Running `azd infra gen` again will regenerate files and may overwrite your customizations. Always version control your changes and be prepared to re-apply customizations after regeneration.

## Advanced customization patterns

### Custom resource definitions

Extend generated infrastructure with additional Azure resources:

```bicep
// Add Application Insights for monitoring
resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${resourceBaseName}-ai'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Redfield'
    Request_Source: 'IbizaWebAppExtensionCreate'
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}

// Configure container apps to use Application Insights
resource containerApp 'Microsoft.App/containerApps@2023-05-01' = {
  // ... other properties
  properties: {
    template: {
      containers: [
        {
          // ... other container properties
          env: [
            {
              name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
              value: applicationInsights.properties.ConnectionString
            }
          ]
        }
      ]
    }
  }
}
```

### Infrastructure validation

Add validation rules to ensure proper resource configuration:

```bicep
@description('Validates that environment type is supported')
@allowed(['dev', 'staging', 'prod'])
param environmentType string

@description('Validates location is in allowed regions')
@allowed(['eastus', 'westus2', 'northeurope'])
param location string
```

## Best practices

- **Version control**: Always commit generated infrastructure files to source control.
- **Environment separation**: Use separate resource groups and naming conventions for different environments.
- **Security scanning**: Implement automated security scanning of Bicep templates.
- **Cost monitoring**: Set up budget alerts and resource tags for cost tracking.
- **Documentation**: Maintain documentation of customizations and their rationale.

## Next steps

- [Deploy a .NET Aspire project to Azure Container Apps using azd](aca-deployment.md)
- [Azure Container Apps with azd (In-depth)](aca-deployment-azd-in-depth.md)
- [Deploy using azd and CI/CD](aca-deployment-github-actions.md)
