---
title: .NET Aspire deployments
description: Learn about essential deployment concepts for .NET Aspire.
ms.topic: overview
ms.date: 11/11/2023
---

# .NET Aspire deployments

Once you're happy with your .NET Aspire application, it's time to deploy it - either to a cloud environment or to local hosting. .NET Aspire isn't tied to any particular cloud and the steps in, for example, [Deploy a .NET Aspire app to Azure Container Apps using the Azure CLI](azure/aca-deployment.md), are adaptable to other environments.

## Deployment manifest

To enable deployment tools from Microsoft and other cloud providers to understand the structure of .NET Aspire applications, specialized targets of the AppHost project can be executed to generate a manifest file describing the projects/services used by the app and the properties necessary for deployment, such as environment variables.

For more details on the schema of the manifest file and how to run AppHost project targets, see the [Deployment manifest](manifest-format.md) topic.

## Deploy to Azure

With .NET Aspire, Preview 1 enables deployment to Azure Container Apps. We expect the number of environments Aspire can deploy to to grow over time.

### Azure Container Apps

.NET Aspire apps are designed to run in containerized environments. Azure Container Apps is a fully managed environment that enables you to run microservices and containerized applications on a serverless platform. The [Azure Container Apps](azure/aca-deployment.md) topic describes how to deploy Aspire apps to ACA manually, using bicep or using the Azure Developer CLI (azd).

### Use Application Insights for .NET Aspire telemetry

.NET Aspire apps are designed to emit Telemetry using OpenTelemetry which uses a provider model. .NET Aspire Apps can direct their telemetry to Azure Monitor / Application Insights using the Azure Monitor telemetry distro. For more information, see [Application Insights](/azure/azure-monitor/app/app-insights-overview) for step-by-step instructions.
