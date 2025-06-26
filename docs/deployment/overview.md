---
title: .NET Aspire deployments
description: Learn about essential deployment concepts for .NET Aspire.
ms.topic: overview
ms.date: 05/21/2024
---

# .NET Aspire deployments

.NET Aspire projects are built with cloud-agnostic principles, allowing deployment flexibility across various platforms supporting .NET and containers. Users can adapt the provided guidelines for deployment on other cloud environments or local hosting. The manual deployment process, while feasible, involves exhaustive steps prone to errors. Users prefer leveraging CI/CD pipelines and cloud-specific tooling for a more streamlined deployment experience tailored to their chosen infrastructure.

## Deployment manifest

To enable deployment tools from Microsoft and other cloud providers to understand the structure of .NET Aspire projects, specialized targets of the [AppHost project](../fundamentals/app-host-overview.md) can be executed to generate a manifest file describing the projects/services used by the app and the properties necessary for deployment, such as environment variables.

For more information on the schema of the manifest and how to run app host project targets, see [.NET Aspire manifest format for deployment tool builders](manifest-format.md).

## Deploy to Azure

.NET Aspire enables deployment to Azure Container Apps. The number of environments .NET Aspire can deploy to will grow over time.

### Azure Container Apps

.NET Aspire projects are designed to run in containerized environments. Azure Container Apps is a fully managed environment that enables you to run microservices and containerized applications on a serverless platform. The [Azure Container Apps](azure/aca-deployment.md) topic describes how to deploy Aspire apps to ACA manually, using bicep, or using the Azure Developer CLI (azd).

### Use Application Insights for .NET Aspire telemetry

.NET Aspire projects are designed to emit telemetry using OpenTelemetry which uses a provider model. .NET Aspire projects can direct their telemetry to Azure Monitor / Application Insights using the Azure Monitor telemetry distro. For more information, see [Use Application Insights for .NET Aspire telemetry](azure/application-insights.md) for step-by-step instructions.

## Deploy to Kubernetes

Kubernetes is a popular container orchestration platform that can run .NET Aspire solutions. To deploy .NET Aspire solutions to Kubernetes clusters, you can use .NET Aspire's built-in Kubernetes publishing features or manually create Kubernetes manifests.

For a comprehensive guide on deploying to Kubernetes, including AKS, see [Deploy a .NET Aspire solution to Kubernetes](kubernetes-deployment.md).

### Built-in Kubernetes publishing

.NET Aspire includes native support for generating Kubernetes manifests directly from your app host configuration. This approach allows you to customize Kubernetes resources programmatically while maintaining the benefits of .NET Aspire's orchestration model.

### Manually create Kubernetes manifests

Alternatively, the Kubernetes manifests can be created manually. This involves more effort and is more time consuming. For more information, see [Deploy a .NET microservice to Kubernetes](/training/modules/dotnet-deploy-microservices-kubernetes/).
