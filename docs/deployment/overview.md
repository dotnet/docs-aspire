---
title: .NET Aspire deployments
description: Learn about essential deployment concepts for .NET Aspire.
ms.topic: overview
ms.date: 02/26/2024
---

# .NET Aspire deployments

.NET Aspire applications are built with cloud-agnostic principles, allowing deployment flexibility across various platforms supporting .NET and containers. Users can adapt the provided guidelines for deployment on other cloud environments or local hosting. The manual deployment process, while feasible, involves exhaustive steps prone to errors. We anticipate users will prefer leveraging CI/CD pipelines and cloud-specific tooling for a more streamlined deployment experience tailored to their chosen infrastructure.

## Deployment manifest

To enable deployment tools from Microsoft and other cloud providers to understand the structure of .NET Aspire applications, specialized targets of the AppHost project can be executed to generate a manifest file describing the projects/services used by the app and the properties necessary for deployment, such as environment variables.

For more information on the schema of the manifest and how to run AppHost project targets, see [.NET Aspire manifest format for deployment tool builders](manifest-format.md).

## Deploy to Azure

.NET Aspire enables deployment to Azure Container Apps. We expect the number of environments Aspire can deploy to to grow over time.

### Azure Container Apps

.NET Aspire apps are designed to run in containerized environments. Azure Container Apps is a fully managed environment that enables you to run microservices and containerized applications on a serverless platform. The [Azure Container Apps](azure/aca-deployment.md) topic describes how to deploy Aspire apps to ACA manually, using bicep or using the Azure Developer CLI (azd).

### Use Application Insights for .NET Aspire telemetry

.NET Aspire apps are designed to emit Telemetry using OpenTelemetry which uses a provider model. .NET Aspire Apps can direct their telemetry to Azure Monitor / Application Insights using the Azure Monitor telemetry distro. For more information, see [Application Insights](/azure/azure-monitor/app/app-insights-overview) for step-by-step instructions.
