---
title: Deploy .NET Aspire apps to a Kubernetes cluster
description: Learn how to use the Aspirate tool to deploy .NET Aspire apps to a Kubernetes cluster.
ms.date: 12/06/2023
---

# Deploy .NET Aspire apps to a Kubernetes cluster

.NET Aspire helps you to create cloud-native apps that are divided into microservices. Each microservice is designed to run in a containerized environment. By deploying .NET Aspire apps to a Kubernetes cluster, you can benefit from Kubernetes' advanced container management features. This article will explain how to deploy a completed .NET Aspire solution to a Kubernetes cluster by using the Aspirate tool. You'll learn:

> [!div class="checklist"]
>
> - The prerequisites that must be in placed for the Aspirate tool.
> - How .NET Aspire and Kubernetes manifest files differ.
> - How to install and initialize the Aspirate tool.
> - How to create and manifests and containers and deploy them to a Kubernetes cluster.

[!INCLUDE [aspire-prereqs](../../includes/aspire-prereqs.md)]

## Aspirate prerequisites

The Aspirate tool needs the following prerequisites to be in place before you can use it to deploy a .NET Aspire app:

- .NET 8 installed on the computer where you will run Aspirate. You use the `dotnet` CLI to install Aspirate and the .NET framework is required for the app to be built.
- A container registry. You can either specify the location of the registry in the Aspire manifest file or configure it when you run Aspirate for the first time. After Aspirate builds the app, it uploads the resulting container images to this registry.
- The Kuberbetes CLI tool, `kubectl`, installed on the complete where you will run Aspirate.
- A Kubernetes cluster. The Kubernetes CLI tool must be configured to connect to this cluster in the **kubeconfig** file.

## Comparing manifest formats

Manifest files are used in many application development and hosting platforms to describe the components of an app. Aspire can create manifest files that help tool developers to create  deployment code. Kubernetes also uses manifest files to describe how the system should create and manage resources in the cluster. However, these two types of manifest file have completely different formats and are not interchangeable. 

An Aspire manifest file is in JSON format and describes each project in the solution file, bindings to other projects, configuration values, and other properties. This example includes a web front end, a back end API service, and a Redis cache component:

```json
{
  "resources": {
    "cache": {
      "type": "redis.v0"
    },
    "apiservice": {
      "type": "project.v0",
      "path": "..\\AspireApp.ApiService\\AspireApp.ApiService.csproj",
      "env": {
        "OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES": "true",
        "OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES": "true"
      },
      "bindings": {
        "http": {
          "scheme": "http",
          "protocol": "tcp",
          "transport": "http"
        },
        "https": {
          "scheme": "https",
          "protocol": "tcp",
          "transport": "http"
        }
      }
    },
    "webfrontend": {
      "type": "project.v0",
      "path": "..\\AspireApp.Web\\AspireApp.Web.csproj",
      "env": {
        "OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES": "true",
        "OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES": "true",
        "ConnectionStrings__cache": "{cache.connectionString}",
        "services__apiservice__0": "{apiservice.bindings.http.url}",
        "services__apiservice__1": "{apiservice.bindings.https.url}"
      },
      "bindings": {
        "http": {
          "scheme": "http",
          "protocol": "tcp",
          "transport": "http"
        },
        "https": {
          "scheme": "https",
          "protocol": "tcp",
          "transport": "http"
        }
      }
    }
  }
}
```

> [!NOTE]
> For more information about Aspire manifest files, see [.NET Aspire manifest format for deployment tool builders](manifest-format.md)

Kubernetes manifest files are in YAML format and describe the desired state for a cluster. Kubernetes automatically manages pods, containers, and services to meet the desired state:

```yaml
---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: webfrontend
spec:
  minReadySeconds: 60
  replicas: 1
  selector:
    matchLabels:
      app: webfrontend
  strategy:
    rollingUpdate:
      maxUnavailable: 0
      maxSurge: 1
  template:
    metadata:
      labels:
        app: webfrontend
    spec:
      containers:
      - name: webfrontend
        image: localhost:5000/webfrontend:latest
        imagePullPolicy: Always
        ports:
        - containerPort: 8080
        envFrom:
        - configMapRef:
            name: webfrontend-env
      terminationGracePeriodSeconds: 180
```

You can use the Aspirate tool to create the Kubernetes manifest files and containers and deploy them to a cluster. 

## Install Aspirate

You can use the `dotnet` command to install Aspirate from NuGet:

```bash
dotnet tool install -g aspirate --prerelease
```

> [!NOTE]
> At the time of writing, the Aspirate tool is in preview and the `--prerelease` option is required.

## Initializing Aspirate

Aspirate takes three settings from the project file for the Aspire AppHost project:

- **ContainerRegistry.** This is the location of a Docker image registry. Aspirate will store the images it creates in this registry and deploy to the Kuberbetes cluster from there.
- **ContainerTag.** This tag will be added to the Docker images Aspirate creates.
- **TemplatePath.** This value modifies the default path to Visual Studio template that wil be transformed into manifests.

You can manually set these values in the project file, or issue this command:

```bash
aspirate init
```

Aspirate leads you through the process of setting these values. They are persisted in an **aspirate.json** file.

## Creating manifests

You can use the `generate` command to create Kubernetes manifests for the projects in your solution. Before you issue the command, change to the top level folder for your **AppHost** project:

```bash
cd AspireApp.AppHost
aspirate generate
```

The `generate` command:

- Creates Kubernetes manifest file for each component in the solution and stores them in the **aspirate-output** subfolder.
- Prompts for you to select components to build.
- Builds the projects you specify.
- Pushes the built containers to the **ContainerRegistry** you specified in the `init` command.

> [!NOTE]
> The `aspirate build` command is similar to the `generate` command. It creates manifest files, builds projects, and pushes the containers to the registry by default. However, if you use the `--aspire-manifest` option, you can use an existing .NET Aspire manifest file.

## Install containers in a Kubernetes cluster

Once the containers are built and stored in your registry, you can use Aspirate to run them on your Kubernetes cluster:

```bash
aspirate apply
```

The command runs the containers on the cluster specified in your **kubeconfig** file.
