---
title: Deploy .NET Aspire apps to Azure Kubernetes Service
description: Learn how to use the Aspirate tool to deploy .NET Aspire apps to the Azure Kubernetes Service.
ms.date: 12/01/2023
---

# Deploy .NET Aspire apps to Azure Kubernetes Service

.NET Aspire helps you to create cloud-native apps that are divided into microservices. Each microservice is designed to run in a containerized environment. By deploying .NET Aspire apps to a Kubernetes cluster, you can benefit from Kubernetes' advanced container management features. This article will explain how to deploy a completed .NET Aspire solution to a Kubernetes cluster by using the Aspirate tool. You'll learn how to:

> [!div class="checklist"]
>
> - TODO: Complete this list at the end.

> [!NOTE]
> In this article, you'll deploy an app to the Azure Kubernetes Service (AKS), which is a complete implementation of Kubernetes in the cloud and allows you to host a cluster in the cloud. If you want to deploy to an alternative location, such as an on-premises cluster, you can adapt these steps.

> [!TODO]
> Check the above statement - how would you adapt them?

[!INCLUDE [aspire-prereqs](../../includes/aspire-prereqs.md)]

## Comparing manifest formats

Manifest files are used in many application development and hosting platforms to describe the components of any app. Aspire can create manifest files that help tool developers to create  deployment code. Kubernetes also uses manifest files to describe how the system should create and manage resources in the cluster. However, these two types of manifest file have completely different formats and are not interchangeable. 

An Aspire manifest file is in JSON format and describes each project in the solution file, bindings to other projects, configuration values, and other properties:

> TODO Insert example Aspire manifest.

> [!NOTE]
> For more information about Aspire manifest files, see [.NET Aspire manifest format for deployment tool builders](../manifest-format.md)

Kubernetes manifest files are in YAML format and describe the desired state for a cluster. Kubernetes automatically manages pods, containers, and services to meet the desired state:

> [!TODO]
> Example K8s manifest

You can use the Aspirate tool to create the Kubernetes manifest, based on the Aspire manifest, and deploy it to a cluster. 


- THe sample video covers deploying to a local kubernetes cluster. Can we do it to a AKS cluster?
- Role of the container registry
- Explain the Aspirate commands
  - init
  - generate
  - build
  - apply
  - destroy
- Prereqs
- Steps

## Install Aspirate

You can use the `dotnet` command to install Aspirate from NuGet:

```bash
dotnet tool install -g aspirate --prerelease
```

> [!NOTE]
> At the time of writing, the Aspirate tool is in preview and the `--prerelease` option is required.
