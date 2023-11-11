---
title: .NET Aspire deployments
description: Learn about essential deployment concepts for .NET Aspire.
ms.topic: overview
ms.date: 11/11/2023
---

# .NET Aspire deployments

.NET Aspire applications are cloud agnostic and should be deployable to any cloud environment that supports .NET and containers. For example, the steps in [Deploy a .NET Aspire app to Azure Container Apps using the Azure CLI](azure/aca-deployment.md) can be adapted to other cloud environments, or local hosting. However, deploying manually requires a number of steps which are exhaustive and error prone. We expect most customers to want to deploy Aspire apps using some form of CI/CD, using cloud specific tooling.

## Deployment manifest

Microsoft and other cloud providers will be supplying tooling for deploying .NET Aspire apps to their respective clouds. To enable those tools to understand .NET Aspire projects, the AppHost project can be run with a specific target to produce a manifest file that describes the projects/services used by the application and the properties necessary for deployment such as the environment variables.

The [Deployment manifest](manifest-format.md) topic describes the schema of the manifest file, and how the AppHost project can be run to generate the manifest. The manifest generation functionality is designed to be used by cloud deployment tools to understand Aspire projects, such as azd for deploying to Azure.

## Deploy to Azure

With .NET Aspire, Preview 1 enables deployment to Azure Container Apps. We expect the services Aspire can deploy to grow over time.

### Azure Container Apps

.NET Aspire apps are designed to run in containerized environments. Azure Container Apps is a fully managed environment that enables you to run microservices and containerized applications on a serverless platform. The [Azure Container Apps](azure/aca-deployment.md) topic describes how to deploy Aspire apps to ACA manually, using bicep or using Azure Developer CLI (azd).

### Use Application Insights for .NET Aspire telemetry

.NET Aspire apps are designed to emit Telemetry using OpenTelemetry which uses a provider model. .NET Aspire Apps can direct their telemetry to Azure Monitor / Application Insights using the Azure Monitor telemetry distro. For more information, see [Application Insights](/azure/azure-monitor/app/app-insights-overview) for step-by-step instructions.
