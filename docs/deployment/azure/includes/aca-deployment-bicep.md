---
ms.topic: include
---

[!INCLUDE [file-new-aspire](../../../includes/file-new-aspire.md)]

[!INCLUDE [aca-bicep-cli-shared-steps](aca-bicep-cli-shared-steps.md)]

## Create the Azure Bicep provisioning files

Now, you'll create two new files &mdash; `infra\provision.bicep` and `infra\provision.parms.bicepparam` - representing the Infrastructure-as-Code (IaC) layer in the app. The `provision.bicep` file is an [Azure Bicep](/azure/azure-resource-manager/bicep/overview?tabs=bicep) template that provisions all of the Azure resources the app will need to run.

1. In `infra/provision.bicep`, add the following code to the top of the empty file. These lines of code represent the parameters you'll feed the Bicep file using the environment variables you set earlier. The `env` variable is an array that will be passed to both of the container apps hosting your Aspire project code, setting some conventional environment variables useful during development.

    ```bicep
    @minLength(1)
    @maxLength(64)
    @description('Name of the resource group that will contain all the resources')
    param resourceGroupName string = 'aspiretoacarg'

    @minLength(1)
    @description('Primary location for all resources')
    param location string = 'westus'

    @minLength(5)
    @maxLength(50)
    @description('Name of the Azure Container Registry resource into which container images will be published')
    param containerRegistryName string = 'aspiretoacacr'

    @minLength(1)
    @maxLength(64)
    @description('Name of the identity used by the apps to access Azure Container Registry')
    param identityName string = 'aspiretoacaid'

    @description('CPU cores allocated to a single container instance, e.g., 0.5')
    param containerCpuCoreCount string = '0.25'

    @description('Memory allocated to a single container instance, e.g., 1Gi')
    param containerMemory string = '0.5Gi'

    var resourceToken = toLower(uniqueString(subscription().id, resourceGroupName, location))
    var helloWorldContainerImage = 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'

    // common environment variables used by each of the apps
    var env = [
        {
            name: 'ASPNETCORE_ENVIRONMENT'
            value: 'Development'
        }
        {
            name: 'Logging__Console__FormatterName'
            value: 'simple'
        }
        {
            name: 'Logging__Console__FormatterOptions__SingleLine'
            value: 'true'
        }
        {
            name: 'Logging__Console__FormatterOptions__IncludeScopes'
            value: 'true'
        }
        {
            name: 'ASPNETCORE_LOGGING__CONSOLE__DISABLECOLORS'
            value: 'true'
        }
    ]
    ```

1. Add the following Bicep code to `infra\provision.bicep`. When you execute this Bicep template against your Azure subscription, these nodes of the file will produce the ACA Environment and the prerequisite Log Analytics instance in the resource group you created earlier.

    ```bicep
    // log analytics
    resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2021-12-01-preview' = {
        name: 'logs${resourceToken}'
        location: location
        properties: any({
            retentionInDays: 30
            features: {
                searchVersion: 1
            }
            sku: {
                name: 'PerGB2018'
            }
        })
    }

    // the container apps environment
    resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2023-04-01-preview' = {
        name: 'acae${resourceToken}'
        location: location
        properties: {
            appLogsConfiguration: {
                destination: 'log-analytics'
                logAnalyticsConfiguration: {
                    customerId: logAnalytics.properties.customerId
                    sharedKey: logAnalytics.listKeys().primarySharedKey
                }
            }
        }
    }
    ```

1. Add the following code and the end of the `infra\provision.bicep` file to provision the Azure Container Registry (ACR) instance and create a user-assigned Azure Identity. Each of your apps will run as this identity, which will be given access to the ACR instance so the Azure Container Apps hosting your code can securely access your ACR instance and pull containers as they're updated.

    ```bicep
    // the container registry
    resource containerRegistry 'Microsoft.ContainerRegistry/registries@2022-02-01-preview' = {
        name: containerRegistryName
        location: location
        sku: {
            name: 'Basic'
        }
        properties: {
            adminUserEnabled: true
            anonymousPullEnabled: false
            dataEndpointEnabled: false
            encryption: {
                status: 'disabled'
            }
            networkRuleBypassOptions: 'AzureServices'
            publicNetworkAccess: 'Enabled'
            zoneRedundancy: 'Disabled'
        }
    }

    // identity for the container apps
    resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
        name: identityName
        location: location
    }

    var principalId = identity.properties.principalId

    // azure system role for setting up acr pull access
    var acrPullRole = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')

    // allow acr pulls to the identity used for the aca's
    resource aksAcrPull 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
        scope: containerRegistry // Use when specifying a scope that is different than the deployment scope
        name: guid(subscription().id, resourceGroup().id, acrPullRole)
        properties: {
            roleDefinitionId: acrPullRole
            principalType: 'ServicePrincipal'
            principalId: principalId
        }
    }
    ```

1. Add the following code to the `infra\provision.bicep` file to provision the Container App hosting the back-end API project.

    ```bicep
    // apiservice - the app's back-end
    resource apiservice 'Microsoft.App/containerApps@2023-04-01-preview' = {
        name: 'apiservice'
        location: location
        identity: {
            type: 'UserAssigned'
            userAssignedIdentities: { '${identity.id}' : {}}
        }
        properties: {
            managedEnvironmentId: containerAppsEnvironment.id
            configuration: {
            activeRevisionsMode: 'Single'
            ingress: {
                external: true
                targetPort: 80
                transport: 'http'
            }
            dapr: { enabled: false }
            registries: [ {
                server: '${containerRegistryName}.azurecr.io'
                identity: identity.id
                } ]
            }
            template: {
                scale: {
                    minReplicas: 1
                    maxReplicas: 1
                }
                serviceBinds: []
                containers: [ {
                    image: helloWorldContainerImage
                    name: 'apiservice'
                    env: env
                    resources: {
                        cpu: json(containerCpuCoreCount)
                        memory: containerMemory
                    }
                } ]
            }
        }
    }
    ```

1. The final code you'll add to _infra/provision.bicep_ will provision containers for the front-end and the Redis Azure Container App add-on your front-end will use to facilitate output caching.

    ```bicep
    // web - the app's front end
    resource web 'Microsoft.App/containerApps@2023-04-01-preview' = {
        name: 'web'
        location: location
        identity: {
            type: 'UserAssigned'
            userAssignedIdentities: { '${identity.id}' : {}}
        }
        properties: {
            managedEnvironmentId: containerAppsEnvironment.id
            configuration: {
            activeRevisionsMode: 'Single'
            ingress: {
                external: true
                targetPort: 80
                transport: 'http'
            }
            dapr: { enabled: false }
            registries: [ {
                server: '${containerRegistryName}.azurecr.io'
                identity: identity.id
                } ]
            }
            template: {
                scale: {
                    minReplicas: 1
                    maxReplicas: 1
                }
                containers: [ {
                    image: helloWorldContainerImage
                    name: 'web'
                    env: env
                    resources: {
                        cpu: json(containerCpuCoreCount)
                        memory: containerMemory
                    }
                    } ]
                serviceBinds: [ {
                    name: 'redis'
                    serviceId: redis.id
                } ]
            }
        }
    }

    // redis - azure container apps service
    resource redis 'Microsoft.App/containerApps@2023-04-01-preview' = {
        name: 'redis'
        location: location
        identity: {
            type: 'None'
            userAssignedIdentities: null
        }
        properties: {
            managedEnvironmentId: containerAppsEnvironment.id
            configuration: {
            activeRevisionsMode: 'Single'
            ingress: {
                external: false
                targetPort: 6379
                transport: 'tcp'
            }
            dapr: { enabled: false }
                service: { type: 'redis' }
            }
            template: {
                scale: {
                    minReplicas: 1
                    maxReplicas: 1
                }
            }
        }
    }
    ```

1. Create a second file in the _infra_ folder named _provision.parms.bicepparam_ and paste this code into it.

    # [PowerShell](#tab/powershell)

    ```powershell
    New-Item provision.parms.bicepparam
    ```

    # [Bash](#tab/bash)

    ```bash
    touch provision.parms.bicepparam
    ```

    ---

1. This file will serve the purpose of reading the environment variables you set earlier. Those environment variables will be picked up and passed into the deployment process, so the template you wrote in `infra\provision.bicep` can read the parameters from your environment.

    ```bicep
    using 'provision.bicep'

    param resourceGroupName = readEnvironmentVariable('RESOURCE_GROUP', 'acatoaspirerg')
    param location = readEnvironmentVariable('LOCATION', 'westus')
    param containerRegistryName = readEnvironmentVariable('CONTAINER_REGISTRY','acatoaspirecr')
    param identityName = readEnvironmentVariable('IDENTITY','acatoaspireid')
    ```

1. Create a new Azure deployment using the Bicep templates. This command will take a few minutes to execute.

    # [PowerShell](#tab/powershell)

    ```powershell
    cd ..
    az deployment group create --resource-group $env:RESOURCE_GROUP --template-file .\infra\provision.bicep --parameters .\infra\provision.parms.bicepparam
    ```

    # [Bash](#tab/bash)

    ```bash
    cd ..
    az deployment group create --resource-group $RESOURCE_GROUP --template-file .\infra\provision.bicep --parameters .\infra\provision.parms.bicepparam
    ```

    ---

## Publish the app container images into a container registry

1. Login to the ACR instance we'll be pushing the app container images to and get the server's FQDN into a parameter you'll use in a moment when you `dotnet build` the .NET Aspire solution:

    # [PowerShell](#tab/powershell)

    ```powershell
    az acr login --name $env:CONTAINER_REGISTRY
    $loginServer = (az acr show --name $env:CONTAINER_REGISTRY --query loginServer --output tsv)
    ```

    # [Bash](#tab/bash)

    ```bash
    az acr login --name $CONTAINER_REGISTRY
    $loginServer = (az acr show --name $CONTAINER_REGISTRY --query loginServer --output tsv)
    ```

    ---

1. .NET's `publish` command supports the `ContainerRegistry` parameter. Setting this results in the output of the `dotnet publish` command being packaged into a container and pushed directly into your ACR instance in the cloud. Publish the solution projects to ACR using the `dotnet publish` command in the solution directory:

    ```dotnetcli
    dotnet publish -r linux-x64 `
        -p:PublishProfile=DefaultContainer `
        -p:ContainerRegistry=$loginServer
    ```

[!INCLUDE [aca-configure-post-deployment](aca-configure-post-deployment.md)]
