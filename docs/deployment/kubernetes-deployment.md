---
title: Deploy .NET Aspire projects to Kubernetes
description: Learn how to deploy .NET Aspire projects to Kubernetes clusters using Aspir8 or manual YAML generation.
ms.date: 01/16/2025
ms.topic: how-to
---

# Deploy a .NET Aspire project to Kubernetes

.NET Aspire projects are designed to run in containerized environments. Kubernetes is a popular container orchestration platform that can run .NET Aspire projects in any cloud provider or on-premises environment. This article walks you through deploying .NET Aspire solutions to Kubernetes clusters using different approaches. You'll learn how to complete the following tasks:

> [!div class="checklist"]
>
> - Deploy using the Aspir8 tool for automated manifest generation
> - Manually create and customize Kubernetes YAML manifests
> - Use Visual Studio Code with Kubernetes extensions for development workflow
> - Configure .NET Aspire for Kubernetes deployment

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

## Prerequisites

Before deploying to Kubernetes, ensure you have:

- A running Kubernetes cluster. You can use:
  - [Azure Kubernetes Service (AKS)](/azure/aks/learn/quick-kubernetes-deploy-portal) for cloud deployment
  - [Docker Desktop](https://docs.docker.com/desktop/kubernetes/) for local development
  - [minikube](https://minikube.sigs.k8s.io/docs/start/) for local testing
  - Any other Kubernetes distribution
- [kubectl](https://kubernetes.io/docs/tasks/tools/) configured to access your cluster
- [Docker](https://docs.docker.com/get-docker/) for building container images

> [!TIP]
> For production deployments on Azure, see [Quickstart: Deploy an Azure Kubernetes Service (AKS) cluster using the Azure portal](/azure/aks/learn/quick-kubernetes-deploy-portal).

## Deployment approaches

There are three main approaches to deploy .NET Aspire projects to Kubernetes:

1. **Aspir8 tool** - Automated manifest generation from .NET Aspire app model (recommended)
2. **Built-in .NET Aspire 9.3+ Kubernetes publishing** - Native Kubernetes manifest generation
3. **Manual YAML creation** - Hand-authored Kubernetes manifests for full control

### Option 1: Deploy using Aspir8

[Aspir8](https://prom3theu5.github.io/aspirational-manifests/) is an open-source project that automatically generates Kubernetes deployment YAML based on your .NET Aspire app host manifest. This is the most straightforward approach for getting started.

#### Install Aspir8

Install the Aspir8 global tool:

```dotnetcli
dotnet tool install -g aspirate
```

#### Generate and deploy manifests

1. Navigate to your .NET Aspire project's app host directory:

   ```bash
   cd YourAspireApp.AppHost
   ```

1. Initialize Aspir8 in your project:

   ```dotnetcli
   aspirate init
   ```

   This creates an `aspirate.json` configuration file where you can customize deployment settings.

1. Generate Kubernetes manifests:

   ```dotnetcli
   aspirate generate
   ```

   This command:
   - Builds your .NET Aspire projects
   - Creates container images  
   - Generates Kubernetes YAML manifests in the `manifests` folder

1. Review the generated manifests in the `manifests` folder. You'll find:
   - `Deployment` objects for your application services
   - `Service` objects for networking
   - `ConfigMap` objects for configuration
   - Container registry configurations

1. Apply the manifests to your Kubernetes cluster:

   ```dotnetcli
   aspirate apply
   ```

#### Manage deployments

Aspir8 provides additional commands for managing your deployments:

- **Update deployment**: Run `aspirate generate` and `aspirate apply` again after making code changes
- **Remove deployment**: Use `aspirate destroy` to clean up all resources

### Option 2: Use .NET Aspire 9.3+ built-in Kubernetes publishing

.NET Aspire 9.3 introduced native Kubernetes manifest generation capabilities that allow you to customize the output programmatically.

#### Configure Kubernetes publishing

In your app host project, you can configure Kubernetes-specific settings:

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

1. Build your project and generate manifests:

   ```dotnetcli
   dotnet run --publisher kubernetes --output-path ./k8s-manifests
   ```

1. Apply the generated manifests:

   ```bash
   kubectl apply -f ./k8s-manifests
   ```

### Option 3: Create manual Kubernetes manifests

For scenarios requiring full control over Kubernetes resources, you can hand-author YAML manifests. This approach provides maximum flexibility but requires more effort.

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

Visual Studio Code provides excellent support for developing and managing Kubernetes applications. This workflow complements .NET Aspire development perfectly.

### Install Kubernetes extension

1. Install the [Kubernetes extension](https://marketplace.visualstudio.com/items?itemName=ms-kubernetes-tools.vscode-kubernetes-tools) for Visual Studio Code.

1. Open your .NET Aspire project in VS Code.

### Use Kubernetes extension features

The Kubernetes extension provides several features that work seamlessly with .NET Aspire:

- **Cluster explorer**: View and manage your Kubernetes resources
- **YAML intellisense**: Auto-completion and validation for Kubernetes manifests  
- **Pod logs**: Stream logs directly in VS Code
- **Port forwarding**: Access cluster services locally
- **Apply manifests**: Deploy YAML files directly from the editor

<!-- TODO: Add screenshot of VS Code with Kubernetes extension showing .NET Aspire deployment -->

### Debugging workflow

1. Use Aspir8 or built-in publishing to generate manifests
1. Apply manifests to your development cluster
1. Use VS Code to:
   - Monitor pod status
   - View application logs
   - Port-forward services for testing
   - Update and redeploy manifests

This workflow allows you to use the same tools you're familiar with for Kubernetes development while benefiting from .NET Aspire's orchestration capabilities.

## Configuration considerations

### Service discovery

.NET Aspire's service discovery works differently in Kubernetes compared to local development. Configure service discovery for Kubernetes by:

1. Using Kubernetes DNS names in connection strings
1. Leveraging Kubernetes `Service` objects for stable endpoints
1. Configuring [.NET Aspire service discovery](../service-discovery/overview.md) for container environments

### Secrets and configuration

For production deployments, store sensitive configuration in Kubernetes `Secret` objects:

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

.NET Aspire health checks work automatically in Kubernetes when properly configured:

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

### Common issues

- **Image pull errors**: Ensure your container registry is accessible from the cluster
- **Service connectivity**: Verify Kubernetes `Service` objects and DNS resolution
- **Resource limits**: Configure appropriate CPU and memory limits
- **Configuration mismatch**: Check that connection strings match Kubernetes service names

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
- [Explore Aspir8 documentation](https://prom3theu5.github.io/aspirational-manifests/)

## See also

- [.NET Aspire manifest format](manifest-format.md)
- [Deploy to Azure Container Apps](azure/aca-deployment.md)
- [.NET microservices deployment patterns](/training/modules/dotnet-deploy-microservices-kubernetes/)
