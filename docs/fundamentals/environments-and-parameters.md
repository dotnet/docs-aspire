---
title: Environments and parameters
description: Learn about environments and parameters in software development.
ms.date: 02/26/2024
---

# Environments and parameters

In this article, you learn how how to add parameters and connection strings to resources that are shared across your app host model. The [app host model](app-host-overview.md) exposes the ability to express dependencies through resources. Resources contain environment variables, such as connection strings, or other parameters that are used to configure the application. Resources can be referenced by other resources, using the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> API.
