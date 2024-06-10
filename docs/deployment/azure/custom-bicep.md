---
title: Use custom Bicep templates
description: Learn how to customize the Bicep templates provided by .NET Aspire to better suit your needs.
ms.date: 06/10/2024
---

# Use custom Bicep templates

When you're targetting Azure as your desired cloud provider, you can use Bicep to define your infrastructure as code. [Bicep is a Domain Specific Language (DSL)](/azure/azure-resource-manager/bicep/overview) for deploying Azure resources declaratively. It aims to drastically simplify the authoring experience with a cleaner syntax and better support for modularity and code re-use.

.NET Aspire provides a set of pre-built Bicep templates that you can use to deploy your application to Azure. However, you may want to customize these templates to better suit your needs. This article explains the concepts and corresponding APIs that you can use to customize the Bicep templates.

> [!IMPORTANT]
> This article is not intended to teach Bicep, but rather to provide guidance on how to create customize Bicep templates for use with .NET Aspire.

As part of the Azure deployment story for .NET Aspire, the Azure Developer CLI (`azd`) provides an understanding of your .NET Aspire application and the ability to deploy it to Azure. The `azd` CLI uses the Bicep templates to deploy the application to Azure.

## Reference Bicep templates
