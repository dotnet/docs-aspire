---
title: Aspire Kubernetes hosting integration
description: Learn how to use the Aspire Kubernetes hosting integration to generate Kubernetes deployment manifests.
ms.date: 10/01/2025
uid: deployment/kubernetes-integration
---

# Aspire Kubernetes hosting integration

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

The Aspire Kubernetes hosting integration enables you to generate Kubernetes deployment manifests from your Aspire application model. This integration allows you to define your application's infrastructure and deployment configuration using the familiar Aspire AppHost and then publish it as Kubernetes YAML manifests for deployment to any Kubernetes cluster.

## Hosting integration

To get started with the Aspire Kubernetes hosting integration, install the [📦 Aspire.Hosting.Kubernetes](https://www.nuget.org/packages/Aspire.Hosting.Kubernetes) NuGet package in the [AppHost](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Kubernetes
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Kubernetes"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Add Kubernetes environment

After installing the package, add a Kubernetes environment to your AppHost project using the <xref:Aspire.Hosting.KubernetesEnvironmentExtensions.AddKubernetesEnvironment*> method:

:::code source="snippets/kubernetes-integration/AppHost.cs" id="apphost":::

> [!TIP]
> With the `k8s` variable assigned, you can pass that to the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithComputeEnvironment*> API to disambiguate compute resources for solutions that define more than one. Otherwise, the `k8s` variable isn't required.

## Configure Kubernetes environment properties

You can customize the Kubernetes environment using the <xref:Aspire.Hosting.KubernetesEnvironmentExtensions.WithProperties*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.MyApi>("api");

builder.AddKubernetesEnvironment("k8s")
       .WithProperties(k8s =>
       {
           k8s.HelmChartName = "my-aspire-app";
       });

builder.Build().Run();
```

The `WithProperties` method allows you to configure various aspects of the Kubernetes deployment, including the Helm chart name that will be used for generating the Kubernetes resources.

## Generate Kubernetes manifests

To generate Kubernetes manifests from your Aspire application, use the `aspire publish` command:

```Aspire
aspire publish -o k8s-artifacts
```

For more information, see [aspire publish command reference](../cli-reference/aspire-publish.md).

This command generates a complete set of Kubernetes YAML manifests in the specified output directory (`k8s-artifacts` in this example). The generated artifacts include:

- **Deployments** or **StatefulSets** for your application services.
- **Services** for network connectivity.
- **ConfigMaps** for application configuration.
- **Secrets** for sensitive data.
- **Helm charts** for easier deployment management.

## Supported resources

The Kubernetes hosting integration supports converting various Aspire resources to their Kubernetes equivalents:

- **Project resources** → Deployments or StatefulSets.
- **Container resources** → Deployments or StatefulSets.
- **Connection strings** → ConfigMaps and Secrets.
- **Environment variables** → ConfigMaps and Secrets.
- **Endpoints** → Services and ingress configuration.
- **Volumes** → PersistentVolumes and PersistentVolumeClaims.

## Deployment considerations

When deploying to Kubernetes, consider the following:

### Container images

Ensure your application projects are configured to build container images. The Kubernetes publisher will reference the container images for your projects. If you haven't specified custom container images, the integration will use parameterized Helm values that you can override during deployment.

### Resource names

Resource names in Kubernetes must follow DNS naming conventions. The integration automatically converts Aspire resource names to valid Kubernetes resource names by:

- Converting to lowercase.
- Replacing invalid characters with hyphens.
- Ensuring names don't start or end with hyphens.

### Environment-specific configuration

Use [external parameters](../fundamentals/external-parameters.md) to configure environment-specific values that should be different between development and production environments.

## See also

- [Deploy to Kubernetes](overview.md#deploy-to-kubernetes)
- [Aspire integrations overview](../fundamentals/integrations-overview.md)
- [Aspire GitHub repo](https://github.com/dotnet/aspire)
- [Kubernetes documentation](https://kubernetes.io/docs/)
- [Helm documentation](https://helm.sh/docs/)
