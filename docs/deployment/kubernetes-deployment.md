---
title: Deploy .NET Aspire solutions to Kubernetes
description: Learn how to deploy .NET Aspire solutions to Kubernetes clusters using built-in publishing or manual YAML generation.
ms.date: 06/26/2025
ms.topic: how-to
---

# Deploy a .NET Aspire solution to Kubernetes

.NET Aspire solutions are designed to run in containerized environments. Kubernetes is a popular container orchestration platform that can run .NET Aspire resources in any cloud provider or on-premises environment. This article walks you through deploying .NET Aspire solutions to Kubernetes clusters using different approaches. You'll learn how to complete the following tasks:

> [!div class="checklist"]
>
> - Use built-in .NET Aspire Kubernetes publishing for automated manifest generation.
> - Manually create and customize Kubernetes YAML manifests.
> - Use Visual Studio Code with Kubernetes extensions for development workflow.
> - Configure .NET Aspire for Kubernetes deployment.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

Before deploying to Kubernetes, ensure you also have:

- A running Kubernetes cluster. You can use:
  - [Azure Kubernetes Service (AKS)](/azure/aks/learn/quick-kubernetes-deploy-portal) for cloud deployment.
  - [Docker Desktop](https://docs.docker.com/desktop/kubernetes/) for local development.
  - [minikube](https://minikube.sigs.k8s.io/docs/start/) for local testing.
  - Any other Kubernetes distribution.
- [kubectl](https://kubernetes.io/docs/tasks/tools/) configured to access your cluster.
- [Docker](https://docs.docker.com/get-docker/) for building container images.

> [!TIP]
> For production deployments on Azure, see [Quickstart: Deploy an Azure Kubernetes Service (AKS) cluster using the Azure portal](/azure/aks/learn/quick-kubernetes-deploy-portal).

## Deployment approaches

There are two main approaches to deploy .NET Aspire solutions to Kubernetes. Each approach offers different levels of automation and control over the deployment process.

1. **Built-in .NET Aspire Kubernetes publishing** - Native Kubernetes manifest generation.
2. **Manual YAML creation** - Hand-authored Kubernetes manifests for full control.

### Option 1: Use built-in .NET Aspire Kubernetes publishing

.NET Aspire includes native Kubernetes manifest generation capabilities that allow you to customize the output programmatically. This approach leverages the <xref:Microsoft.Extensions.Hosting.DistributedApplicationBuilder> to configure Kubernetes-specific settings and the <xref:Microsoft.Extensions.Hosting.IDistributedApplicationResourceWithEndpoints.PublishAsKubernetesService%2A> extension method to publish resources as Kubernetes services.

#### Configure Kubernetes publishing

In your app host project, you can configure Kubernetes-specific settings. The following code shows how to set up a Kubernetes environment and configure services for deployment:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Configure Kubernetes environment
builder.AddKubernetesEnvironment("prod")
       .WithProperties(env =>
       {
           env.DefaultImagePullPolicy = "Always";
       });

// Add your services with Kubernetes-specific customizations
var apiService = builder.AddProject<Projects.YourApp_ApiService>("apiservice")
    .PublishAsKubernetesService(resource =>
    {
        // Customize deployment settings
        resource.Deployment!.Spec.RevisionHistoryLimit = 5;
        
        // Add labels or annotations
        resource.Service!.Metadata.Labels.Add("app", "your-api");
    });

var webService = builder.AddProject<Projects.YourApp_Web>("webfrontend")
    .WithReference(apiService)
    .WithExternalHttpEndpoints()
    .PublishAsKubernetesService(resource =>
    {
        // Configure service type for external access
        resource.Service!.Spec.Type = "LoadBalancer";
    });

builder.Build().Run();
```

#### Publish to Kubernetes

Generate Kubernetes manifests using the built-in publishing target:

```dotnetcli
dotnet run --publisher kubernetes --output-path ./k8s-manifests
```

This command generates standard Kubernetes YAML files in the specified output directory that you can deploy using `kubectl` or any GitOps workflow.

#### Deploy the generated manifests

Apply the generated manifests to your cluster:

```bash
kubectl apply -f ./k8s-manifests
```

### Option 2: Create manual Kubernetes manifests

For scenarios requiring full control over Kubernetes resources, you can hand-author YAML manifests. This approach provides maximum flexibility but requires more effort. Manual creation is useful when you need specific Kubernetes features or configurations that aren't available through the built-in publishing options.

#### Generate the manifest

First, generate the .NET Aspire manifest to understand your application structure:

```dotnetcli
dotnet run --publisher manifest --output-path manifest.json
```

#### Create Kubernetes YAML files

Based on the manifest, create Kubernetes deployment files. Here's an example for a typical .NET Aspire application:

```yaml
# redis.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: redis
spec:
  replicas: 1
  selector:
    matchLabels:
      app: redis
  template:
    metadata:
      labels:
        app: redis
    spec:
      containers:
      - name: redis
        image: redis:7.2.4
        ports:
        - containerPort: 6379
---
apiVersion: v1
kind: Service
metadata:
  name: redis
spec:
  selector:
    app: redis
  ports:
  - port: 6379
    targetPort: 6379

---
# apiservice.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: apiservice
spec:
  replicas: 2
  selector:
    matchLabels:
      app: apiservice
  template:
    metadata:
      labels:
        app: apiservice
    spec:
      containers:
      - name: apiservice
        image: your-registry/apiservice:latest
        ports:
        - containerPort: 8080
        env:
        - name: ConnectionStrings__cache
          value: "redis:6379"
        - name: OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES
          value: "true"
---
apiVersion: v1
kind: Service
metadata:
  name: apiservice
spec:
  selector:
    app: apiservice
  ports:
  - port: 80
    targetPort: 8080
```

#### Deploy manually created manifests

Apply your YAML files to the cluster:

```bash
kubectl apply -f redis.yaml
kubectl apply -f apiservice.yaml
# Apply other service manifests...
```

## Development workflow with Visual Studio Code

Visual Studio Code provides excellent support for developing and managing Kubernetes applications. This workflow complements .NET Aspire development perfectly by allowing you to manage the entire deployment lifecycle from within the editor.

### Install Kubernetes extension

The Kubernetes extension for Visual Studio Code provides comprehensive support for managing Kubernetes resources. Follow these steps to set up your development environment:

1. Install the [Kubernetes extension](https://marketplace.visualstudio.com/items?itemName=ms-kubernetes-tools.vscode-kubernetes-tools) for Visual Studio Code.

1. Open your .NET Aspire solution in VS Code.

### Use Kubernetes extension features

The Kubernetes extension provides several features that work seamlessly with .NET Aspire:

- **Cluster explorer**: View and manage your Kubernetes resources.
- **YAML intellisense**: Auto-completion and validation for Kubernetes manifests.  
- **Pod logs**: Stream logs directly in VS Code.
- **Port forwarding**: Access cluster services locally.
- **Apply manifests**: Deploy YAML files directly from the editor.

:::image type="content" source="media/vscode-kubernetes-extension.png" lightbox="media/vscode-kubernetes-extension.png" alt-text="Visual Studio Code with Kubernetes extension showing .NET Aspire deployment":::

### Debugging workflow

The following workflow demonstrates how to use Visual Studio Code effectively with .NET Aspire deployments:

1. Use built-in publishing to generate manifests.
1. Apply manifests to your development cluster.
1. Use VS Code to:
   - Monitor pod status.
   - View application logs.
   - Port-forward services for testing.
   - Update and redeploy manifests.

This workflow allows you to use the same tools you're familiar with for Kubernetes development while benefiting from .NET Aspire's orchestration capabilities.

## Configuration considerations

When deploying .NET Aspire solutions to Kubernetes, several configuration aspects require special attention to ensure proper functionality in the containerized environment.

### Service discovery

.NET Aspire's service discovery works differently in Kubernetes compared to local development. Configure service discovery for Kubernetes by:

1. Using Kubernetes DNS names in connection strings.
1. Leveraging Kubernetes `Service` objects for stable endpoints.
1. Configuring [.NET Aspire service discovery](../service-discovery/overview.md) for container environments.

### Secrets and configuration

For production deployments, store sensitive configuration in Kubernetes `Secret` objects rather than in plain text configuration files. This approach ensures that sensitive data is properly encrypted and managed by Kubernetes:

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: app-secrets
type: Opaque
data:
  connection-string: <base64-encoded-value>
```

Reference secrets in your deployment:

```yaml
env:
- name: ConnectionStrings__Database
  valueFrom:
    secretKeyRef:
      name: app-secrets
      key: connection-string
```

### Health checks

.NET Aspire health checks work automatically in Kubernetes when properly configured. The following example shows how to configure liveness and readiness probes:

```yaml
containers:
- name: apiservice
  image: your-registry/apiservice:latest
  livenessProbe:
    httpGet:
      path: /health
      port: 8080
  readinessProbe:
    httpGet:
      path: /health/ready
      port: 8080
```

## Troubleshooting

When deploying .NET Aspire solutions to Kubernetes, you might encounter various issues. This section covers common problems and provides debugging strategies to help resolve them.

### Common issues

The following list describes common deployment issues and their typical causes:

- **Image pull errors**: Ensure your container registry is accessible from the cluster.
- **Service connectivity**: Verify Kubernetes `Service` objects and DNS resolution.
- **Resource limits**: Configure appropriate CPU and memory limits.
- **Configuration mismatch**: Check that connection strings match Kubernetes service names.

### Debugging tools

Use these commands to troubleshoot deployments:

```bash
# Check pod status
kubectl get pods

# View pod logs
kubectl logs <pod-name>

# Describe resources for detailed information
kubectl describe deployment <deployment-name>

# Port forward for local testing
kubectl port-forward service/<service-name> 8080:80
```

## Next steps

- [Learn about .NET Aspire service discovery](../service-discovery/overview.md)
- [Configure health checks](../fundamentals/health-checks.md)
- [Deploy to Azure Kubernetes Service](/azure/aks/learn/quick-kubernetes-deploy-portal)

## See also

- [.NET Aspire manifest format](manifest-format.md)
- [Deploy to Azure Container Apps](azure/aca-deployment.md)
- [.NET microservices deployment patterns](/training/modules/dotnet-deploy-microservices-kubernetes/)
