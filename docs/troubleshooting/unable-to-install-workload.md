---
title: Troubleshoot installing the .NET Aspire workload
description: Explore strategies for troubleshooting issues when installing the .NET Aspire workload.
ms.date: 05/14/2024
---

# Troubleshoot installing the .NET Aspire workload

This article provides guidance on how to troubleshoot issues that you might encounter when installing the .NET Aspire workload.

## Symptoms

When you install the .NET Aspire workload, you might encounter an installation error. The error message might indicate that the installation failed, or that the workload couldn't be installed. The error message might also indicate that a package source is unavailable, or that a package source is not found.

## Possible solution

Ensure that any recursive _Nuget.config_ files are correctly configured to specify the correct package sources and NuGet feeds. For example, if you have a _Nuget.config_ file in your user profile directory, ensure that it doesn't specify a package source that is no longer available.

<!-- TODO: Ask Jose to help fill in more details, and perhaps even more symptoms. -->