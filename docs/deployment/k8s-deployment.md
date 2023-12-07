---
title: Deploy .NET Aspire apps to a Kubernetes cluster
description: Learn how to use the Aspirate tool to deploy .NET Aspire apps to a Kubernetes cluster.
ms.date: 12/06/2023
---

# Deploy .NET Aspire apps to a Kubernetes cluster

.NET Aspire helps you to create cloud-native apps that are divided into microservices. Each microservice is designed to run in a containerized environment. By deploying .NET Aspire apps to a Kubernetes cluster, you can benefit from Kubernetes' advanced container management features. This article will explain how to deploy a completed .NET Aspire solution to a Kubernetes cluster by using the [Aspirate tool](https://www.nuget.org/packages/Aspirate). You'll learn:

> [!div class="checklist"]
>
> - The prerequisites that must be in placed for the Aspirate tool.
> - How .NET Aspire and Kubernetes manifest files differ.
> - How to install and initialize the Aspirate tool.
> - How to create and manifests and containers and deploy them to a Kubernetes cluster.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

## Aspirate prerequisites

In addition to the .NET Aspire prerequisites, the Aspirate tool needs the following to be in place before you can use it to deploy a .NET Aspire app:

- A container registry. You can either specify the location of the registry in the .NET Aspire manifest file or configure it when you run Aspirate for the first time. After Aspirate builds the app, it uploads the resulting container images to this registry.
- The [Kuberbetes CLI tool](https://kubernetes.io/docs/tasks/tools/), `kubectl`, installed on the computer where you will run Aspirate.
- A Kubernetes cluster. The Kubernetes CLI tool must be configured to connect to this cluster in the **kubeconfig** file.

## Compare manifest formats

Manifest files are used in many application development and hosting platforms to describe the components of an app. .NET Aspire can create manifest files that help tool developers to create  deployment code. Kubernetes also uses manifest files to describe how the system should create and manage resources in the cluster. However, these two types of manifest file have completely different formats and are not interchangeable.

A .NET Aspire manifest file is in JSON format and describes each project in the solution file, bindings to other projects, configuration values, and other properties. This example includes a web front end, a back end API service, and a Redis cache component:

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
> For more information about .NET Aspire manifest files, see [.NET Aspire manifest format for deployment tool builders](manifest-format.md)

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

To install Aspirate, use the `dotnet tool` command:

```dotnetcli
dotnet tool install -g aspirate --prerelease
```

> [!NOTE]
> At the time of writing, the Aspirate tool is in preview and the `--prerelease` option is required.

## Initialize Aspirate

Aspirate takes three settings from the project file for the .NET Aspire AppHost project:

- **ContainerRegistry**: The location of a Docker image registry. Aspirate stores the images it creates in this registry and deploys to the Kubernetes cluster from there.
- **ContainerTag**: Aspirate adds this tag to the Docker images it creates.
- **TemplatePath**: Modifies the default path to Visual Studio template that Aspirate transforms into manifests.

You can manually set these values in the project file, or issue this command:

```dotnetcli
aspirate init
```

Aspirate leads you through the process of setting these values and persists them in an _aspirate.json_ file.

## Create manifests

To create Kubernetes manifests for the projects in your solution, use the `generate` command. Before you issue the command, change to the top-level folder for your _*.AppHost_ project:

```dotnetcli
cd AspireApp.AppHost
aspirate generate
```

The `generate` command:

- Creates Kubernetes manifest file for each component in the solution and stores them in the _aspirate-output_ subfolder.
- Prompts for you to select components to build.
- Builds the projects you specify.
- Pushes the built containers to the **ContainerRegistry** you specified in the `init` command.

> [!NOTE]
> The `aspirate build` command is similar to the `generate` command. It creates manifest files, builds projects, and pushes the containers to the registry by default. However, if you use the `--aspire-manifest` option, you can use an existing .NET Aspire manifest file.

## Install containers in a Kubernetes cluster

Once the containers are built and stored in your registry, you can use Aspirate to run them on your Kubernetes cluster:

```dotnetcli
aspirate apply
```

The command runs the containers on the cluster specified in your _kubeconfig_ file.
