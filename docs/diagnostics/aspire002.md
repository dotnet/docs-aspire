---
title: Compiler Warning ASPIRE002
description: Learn more about compiler Warning ASPIRE002. Project is an Aspire AppHost project but necessary dependencies aren't present. Are you missing an Aspire.Hosting.AppHost PackageReference?
ms.date: 05/08/2025
f1_keywords:
  - "ASPIRE002"
helpviewer_keywords:
  - "ASPIRE002"
---

# Compiler Warning ASPIRE002

**Version introduced:** 8.0.0

> 'Project' is an Aspire AppHost project but necessary dependencies aren't present. Are you missing an Aspire.Hosting.AppHost PackageReference?

This diagnostic warning is reported when your project is missing reference to the .NET Aspire App Host.

## To correct this warning

Add reference to the [📦 Aspire.Hosting.AppHost](https://www.nuget.org/packages/Aspire.Hosting.AppHost) NuGet package. For more information about the app host, see [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md).
