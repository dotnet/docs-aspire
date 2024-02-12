---
title: Deploy a .NET Aspire app to Azure Container Apps using `azd` (in-depth guide)
description: Learn how to use `azd` to deploy .NET Aspire apps to Azure Container Apps.
ms.date: 11/27/2023
---

# Deploy a .NET Aspire app to Azure Container Apps using the Azure Developer CLI (in-depth guide)

The Azure Developer CLI (`azd`) has been extended to support deploying .NET Aspire applications. Use this guide to walk through the process of creating and deploying a .NET Aspire application to Azure Container Apps using the Azure Developer CLI. In this tutorial, you'll learn the following concepts:

> [!div class="checklist"]
>
> - Explore how `azd` integration works with .NET Aspire apps
> - Provision and deploy resources on Azure for a .NET Aspire app using `azd`
> - Generate Bicep infrastructure and other template files using `azd`

[!INCLUDE [aspire-prereqs](../../includes/aspire-prereqs.md)]

You will also need to have the Azure Developer CLI [installed locally](/azure/developer/azure-developer-cli/install-azd). Common install options include the following:

# [Windows](#tab/windows)

```powershell
winget install microsoft.azd
```

# [macOS](#tab/macos)

```bash
brew tap azure/azd && brew install azd
```

# [Linux](#tab/linux)

```bash
curl -fsSL https://aka.ms/install-azd.sh | bash
```

---

## How Azure Developer CLI integration works

The `azd init` workflow provides customized supported for .NET Aspire projects. The following diagram illustrates how this flow works conceptually and how `azd` and .NET Aspire are integrated:

:::image type="content" source="media/azd-internals.png" alt-text="Illustration of internal processing of `azd` when deploying .NET Aspire application.":::

1. When `azd` targets a .NET Aspire application it starts the AppHost with a special command (`dotnet run --project AppHost.csproj -- --publisher manifest`), which produces the Aspire [manifest file](../manifest-format.md).
1. The manifest file is interrogated by the `azd provision` sub-command logic to generate Bicep files in-memory only (by default).
1. After generating the Bicep files, a deployment is triggered using Azure's ARM APIs targeting the subscription and resource group provided earlier.
1. Once the underlying Azure resources are configured, the `azd deploy` sub-command logic is executed which uses the same Aspire manifest file.
1. As part of deployment `azd` makes a call to `dotnet publish` using .NET's built in container publishing support to generate container images.
1. Once `azd` has built the container images it pushes them to the ACR registry that was created during the provisioning phase.
1. Finally, once the container image is in ACR, `azd` updates the resource using ARM to start using the new version of the container image.

> [!NOTE]
> `azd` also enables you to output the generated Bicep to an `infra` folder in your project, which you can read more about in the [Generating Bicep from .NET Aspire app model](/dotnet/aspire/deployment/azure/aca-deployment-azd-in-depth?branch=main#generate-bicep-from-net-aspire-app-model) section.

## Provision and deploy a .NET Aspire starter app

The steps in this section demonstrate how to create a .NET Aspire start app and handle provisioning and deploying the app resources to Azure using `azd`.

### Create the .NET Aspire starter app

Create a new .NET Aspire application using the `dotnet new` command. You can also create the project using Visual Studio.

```dotnetcli
dotnet new aspire-starter --use-redis-cache -o AspireAzdWalkthrough
cd AspireAzdWalkthrough
dotnet run --project AspireAzdWalkthrough.AppHost\AspireAzdWalkthrough.AppHost.csproj
```

The previous commands create a new .NET Aspire application based on the `aspire-starter` template which includes a dependency on Redis cache. It runs the .NET Aspire project which verifies that everything is working correctly.

[!INCLUDE [init workflow](includes/init-workflow.md)]

The _azure.yaml_ file has the following contents:

```yml
# yaml-language-server: $schema=https://raw.githubusercontent.com/Azure/azure-dev/main/schemas/v1.0/azure.yaml.json

name: AspireAzdWalkthrough
services:
  app:
    language: dotnet
    project: .\AspireAzdWalkthrough.AppHost\AspireAzdWalkthrough.AppHost.csproj
    host: containerapp
```

With the `project` field pointing to a .NET Aspire AppHost project, `azd` activates its integration with .NET Aspire and derives the required infrastructure needed to host this application from the application model specified in the _Program.cs_ file of the .NET Aspire app.

The _.azure\aspireazddev\config.json_ file has the following contents:

```json
{
  "services": {
    "app": {
      "config": {
        "exposedServices": [
          "webfrontend"
        ]
      }
    }
  }
}
```

This file is how `azd` remembers (on a per environment basis) which services should be exposed with a public endpoint. `azd` can be configured to support multiple environments.

### Initial deployment

1. In order to deploy the .NET Aspire application, authenticate to Azure AD to call the Azure resource management APIs.

    ```azdeveloper
    azd auth login
    ```

    The previous command will launch a browser to authenticate the command-line session.

1. Once authenticated, use the following command to provision and deploy the application.

    ```azdeveloper
    azd up
    ```

    > [!IMPORTANT]
    > To push containers to an Azure Container Registry (ACR), you need to enable an **Admin user** on the registry. Open the Azure Portal, navigate to the ACR resource / Settings / Access keys, and then select the **Admin user** checkbox. For more information, see [Enable admin user](/azure/container-registry/container-registry-authentication#admin-account).

1. When prompted, select the subscription and location the resources should be deployed to. Once these options are selected the .NET Aspire application
will be deployed.

    :::image type="content" source="media/azd-up-final.png" alt-text="Screenshot of `azd` output after `azd up` command is executed.":::

    The final line of output from the `azd` command is a link to the Azure Portal that shows
    all of the Azure resources that were deployed:

    :::image type="content" source="media/azd-azure-portal-deployed-resources.png" alt-text="Screenshot of Azure Portal showing deployed resources.":::

Three containers are deployed within this application:

- `webfrontend`: Contains code from the web project in the starter template.
- `apiservice`: Contains code from the API service project in the starter template.
- `cache`: A Redis container image to supply a cache to the front-end.

Just like in local development, the configuration of connection strings has been handled automatically. In this case, `azd` was responsible for interpreting the application model and translating it to the appropriate deployment steps. As an example, consider the connection string and service discovery variables that are injected into the `webfrontend` container so that it knows how to connect to the Redis cache and `apiservice`.

:::image type="content" source="media/azd-aca-variables.png" lightbox="media/azd-aca-variables.png" alt-text="A screenshot of environment variables in the webfrontend container app.":::

For more information on how .NET Aspire apps handle connection strings and service discovery, see
[.NET Aspire orchestration overview](../../fundamentals/app-host-overview.md).

### Deploy application updates

When the `azd up` command is executed the underlying Azure resources are _provisioned_ and a container image is built and _deployed_ to the container apps hosting the .NET Aspire app. Typically once development is underway and Azure resources are deployed it won't be necessary to provision Azure resources every time code is updated—this is especially true for the developer inner loop.

To speed up deployment of code changes, `azd` supports deploying code updates in the container image. This is done using the `azd` deploy command:

```azdeveloper
azd deploy
```

:::image type="content" source="media/azd-deploy-output.png" lightbox="media/azd-deploy-output.png" alt-text="A screenshot of the `azd` deploy command output.":::

It's not necessary to deploy all services each time. `azd` understands the .NET Aspire app model, it's possible to deploy just one of the services specified using the following command:

```azdeveloper
azd deploy webfrontend
```

For more information, see [Azure Developer CLI reference: azd deploy](/azure/developer/azure-developer-cli/reference#azd-deploy).

### Deploy infrastructure updates

Whenever the dependency structure within a .NET Aspire app changes, `azd` must re-provision the underlying Azure resources. The `azd provision` command is used to apply these changes to the infrastructure.

To see this in action, update the _Program.cs_ file in the AppHost project to the following:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

// Add the locations database.
var locationsdb = builder.AddPostgres("db").AddDatabase("locations");

// Add the locations database reference to the API service.
var apiservice = builder.AddProject<Projects.AspireAzdWalkthrough_ApiService>("apiservice")
    .WithReference(locationsdb);

builder.AddProject<Projects.AspireAzdWalkthrough_Web>("webfrontend")
    .WithReference(cache)
    .WithReference(apiservice);

builder.Build().Run();
```

Save the file and issue the following command:

```azdeveloper
azd provision
```

The `azd provision` command updates the infrastructure by creating a container app to host the Postgres database. The `azd provision` command didn't update the connection strings for the `apiservice` container. In order to have connection strings updated to point to the newly provisioned Postgres database the `azd deploy` command needs to be invoked again. When in doubt, use `azd up` to both provision and deploy.

### Clean up resources

Remember to clean up the Azure resources that you've created during this walkthrough. Because `azd knows the resource group in which it created the resources it can be used to spin down the environment using the following command:

```azdeveloper
azd down
```

The previous command may take some time to execute, but when completed the resource group and all its resources should be deleted.

:::image type="content" source="media/azd-down-success.png" lightbox="media/azd-down-success.png" alt-text="A screenshot showing the azd down command output.":::

## Generate Bicep from .NET Aspire app model

Although development teams are free to use `azd up` (or `azd provision` and `azd deploy`) commands for their deployments both for development and production purposes, some teams may choose to generate Bicep files that they can review and manage as part of version control (this also allows these Bicep files to be referenced as part of a larger more complex Azure deployment).

`azd` includes the ability to output the Bicep it uses for provisioning via following command:

```azdeveloper
azd config set alpha.infraSynth on
azd infra synth
```

After this command is executed in the starter template example used in this guide, the following files are created:

- _infra/main.bicep_: Represents the main entry point for the deployment.
- _infra/main.parameters.json_: Used as the parameters for main Bicep (maps to environment variables defined in _.azure_ folder).
- _infra/resoures.bicep_: Defines the Azure resources required to support the .NET Aspire app model.
- _AspireAzdWalkthrough.Web/manifests/containerApp.tmpl.yaml_: The container app definition for `webfrontend`.
- _AspireAzdWalkthrough.ApiService/manifests/containerApp.tmpl.yaml_: The container app definition for `apiservice`.

The _infra\resources.bicep_ file doesn't contain any definition of the container apps themselves (with the exception of container apps which are dependencies such as Redis and Postgres):

```bicep
@description('The location used for all deployed resources')
param location string = resourceGroup().location

@description('Tags that will be applied to all resources')
param tags object = {}

var resourceToken = uniqueString(resourceGroup().id)

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: 'mi-${resourceToken}'
  location: location
  tags: tags
}

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: replace('acr-${resourceToken}', '-', '')
  location: location
  sku: {
    name: 'Basic'
  }
  tags: tags
}

resource caeMiRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(containerRegistry.id, managedIdentity.id, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d'))
  scope: containerRegistry
  properties: {
    principalId: managedIdentity.properties.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId:  subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
  }
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: 'law-${resourceToken}'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
  tags: tags
}

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' = {
  name: 'cae-${resourceToken}'
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalyticsWorkspace.properties.customerId
        sharedKey: logAnalyticsWorkspace.listKeys().primarySharedKey
      }
    }
  }
  tags: tags
}

resource cache 'Microsoft.App/containerApps@2023-05-02-preview' = {
  name: 'cache'
  location: location
  properties: {
    environmentId: containerAppEnvironment.id
    configuration: {
      service: {
        type: 'redis'
      }
    }
    template: {
      containers: [
        {
          image: 'redis'
          name: 'redis'
        }
      ]
    }
  }
  tags: union(tags, {'aspire-resource-name': 'cache'})
}

resource locations 'Microsoft.App/containerApps@2023-05-02-preview' = {
  name: 'locations'
  location: location
  properties: {
    environmentId: containerAppEnvironment.id
    configuration: {
      service: {
        type: 'postgres'
      }
    }
    template: {
      containers: [
        {
          image: 'postgres'
          name: 'postgres'
        }
      ]
    }
  }
  tags: union(tags, {'aspire-resource-name': 'locations'})
}
output MANAGED_IDENTITY_CLIENT_ID string = managedIdentity.properties.clientId
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = containerRegistry.properties.loginServer
output AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID string = managedIdentity.id
output AZURE_CONTAINER_APPS_ENVIRONMENT_ID string = containerAppEnvironment.id
output AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN string = containerAppEnvironment.properties.defaultDomain
```

For more information on using Bicep to automate deployments to Azure see, [What is Bicep?](/azure/azure-resource-manager/bicep/overview?tabs=bicep)

The definition of the container apps from the .NET service projects is contained within the _containerApp/tmpl.yaml_ files in the `manifests` directory in each project respectively. Here is an example from the `webfrontend` project:

```yml
location: {{ .Env.AZURE_LOCATION }}
identity:
  type: UserAssigned
  userAssignedIdentities:
    ? "{{ .Env.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID }}"
    : {}
properties:
  environmentId: {{ .Env.AZURE_CONTAINER_APPS_ENVIRONMENT_ID }}
  configuration:
    activeRevisionsMode: single
    ingress:
      external: true
      targetPort: 8080
      transport: http
      allowInsecure: false
    registries:
    - server: {{ .Env.AZURE_CONTAINER_REGISTRY_ENDPOINT }}
      identity: {{ .Env.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID }}
  template:
    containers:
    - image: {{ .Env.SERVICE_WEBFRONTEND_IMAGE_NAME }}
      name: webfrontend
      env:
      - name: AZURE_CLIENT_ID
        value: {{ .Env.MANAGED_IDENTITY_CLIENT_ID }}
      - name: ConnectionStrings__cache
        value: {{ connectionString "cache" }}
      - name: OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES
        value: "true"
      - name: OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES
        value: "true"
      - name: services__apiservice__0
        value: http://apiservice.internal.{{ .Env.AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN }}
      - name: services__apiservice__1
        value: https://apiservice.internal.{{ .Env.AZURE_CONTAINER_APPS_ENVIRONMENT_DEFAULT_DOMAIN }}
tags:
  azd-service-name: webfrontend
  aspire-resource-name: webfrontend
```

After executing the `azd infra synth` command, when `azd provision` and `azd deploy` are called they use the Bicep and supporting generated files.

> [!IMPORTANT]
> If `azd infra synth` is called again, it replaces any modified files with freshly generated ones and prompts you for confirmation before doing so.

## Isolated environments for debugging

Because `azd` makes it easy to provision new environments, it's possible for each team member to have an isolated cloud-hosted environment for debugging code in a setting that closely matches production. When doing this each team member should create their own environment using the following command:

```azdeveloper
azd env new
```

This will prompt the user for subscription and resource group information again and subsequent `azd up`, `azd provision`, and `azd deploy` invocations will use this new environment by default. The `--environment` switch can be applied to these commands to switch between environments.

[!INCLUDE [clean-up-resources](../../includes/clean-up-resources.md)]
