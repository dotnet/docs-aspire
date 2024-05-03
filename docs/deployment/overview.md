---
title: .NET Aspire deployments
description: Learn about essential deployment concepts for .NET Aspire.
ms.topic: overview
ms.date: 04/03/2024
---

# .NET Aspire deployments

.NET Aspire applications are built with cloud-agnostic principles, allowing deployment flexibility across various platforms supporting .NET and containers. Users can adapt the provided guidelines for deployment on other cloud environments or local hosting. The manual deployment process, while feasible, involves exhaustive steps prone to errors. Users prefer leveraging CI/CD pipelines and cloud-specific tooling for a more streamlined deployment experience tailored to their chosen infrastructure.

## Deployment manifest

To enable deployment tools from Microsoft and other cloud providers to understand the structure of .NET Aspire applications, specialized targets of the [AppHost project](../fundamentals/app-host-overview.md) can be executed to generate a manifest file describing the projects/services used by the app and the properties necessary for deployment, such as environment variables.

For more information on the schema of the manifest and how to run app host project targets, see [.NET Aspire manifest format for deployment tool builders](manifest-format.md).

## Deploy to Azure

.NET Aspire enables deployment to Azure Container Apps. The number of environments .NET Aspire can deploy to will grow over time.

### Azure Container Apps

.NET Aspire apps are designed to run in containerized environments. Azure Container Apps is a fully managed environment that enables you to run microservices and containerized applications on a serverless platform. The [Azure Container Apps](azure/aca-deployment.md) topic describes how to deploy Aspire apps to ACA manually, using bicep or using the Azure Developer CLI (azd).

### Use Application Insights for .NET Aspire telemetry

.NET Aspire apps are designed to emit telemetry using OpenTelemetry which uses a provider model. .NET Aspire Apps can direct their telemetry to Azure Monitor / Application Insights using the Azure Monitor telemetry distro. For more information, see [Use Application Insights for .NET Aspire telemetry](azure/application-insights.md) for step-by-step instructions.

## Deploy to Kubernetes

Kubernetes is a popular container orchestration platform that can run .NET Aspire applications. To deploy .NET Aspire apps to Kubernetes clusters, you need to map the .NET Aspire JSON manifest to a Kubernetes YAML manifest file. There are two ways to do this: by using the Aspir8 project and by manually creating Kubernetes manifests.

### The Aspir8 project

**Aspir8**, an open-source project, handles the generation of deployment YAML based on the .NET Aspire app host manifest. The project outputs a .NET global tool that can be used to perform a series of tasks, resulting in the generation of Kubernetes manifests:

- `aspirate init`: Initializes the **Aspir8** project in the current directory.
- `aspirate generate`: Generates Kubernetes manifests based on the .NET Aspire app host manifest.
- `aspirate apply`: Applies the generated Kubernetes manifests to the Kubernetes cluster.
- `aspirate destroy`: Deletes the resources created by the `apply` command.

With these commands, you can build your apps, containerize them, and deploy them to Kubernetes clusters. For more information, see [Aspir8](https://prom3theu5.github.io/aspirational-manifests/getting-started.html).

### Manually create Kubernetes manifests

Alternatively, the Kubernetes manifests can be created manually. This involves more effort and is more time consuming. For more information, see [Deploy a .NET microservice to Kubernetes](/training/modules/dotnet-deploy-microservices-kubernetes/).
