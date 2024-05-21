---
title: Troubleshoot installing the .NET Aspire workload
description: Explore strategies for troubleshooting issues when installing the .NET Aspire workload.
ms.date: 05/15/2024
---

# Troubleshoot installing the .NET Aspire workload

This article provides guidance on how to troubleshoot issues that you might encounter when installing the .NET Aspire workload from the .NET CLI.

## Symptoms

When you install the .NET Aspire workload, you might encounter an installation error. The error message might indicate that the installation failed, or that the workload couldn't be installed. The error message might also indicate that a package source is unavailable, or that a package source isn't found often similar to:

```Output
Workload update failed: One or more errors ocurred: (Version X.Y.00Z of package A.B.C is not found in NuGet feeds.
```

One common issue is that your SDK is aware of some workload manifest or workload pack versions that are not present in any of the feeds configured when you are trying to run the dotnet workload commands. This can happen if the SDK, during its daily check for updates, finds a new version of a workload manifest in a feed that isn't used when running `dotnet workload` commands. This discrepancy can cause errors during installation.

A less common issue, even when using the correct feeds, is that a workload manifest may have a dependency on a workload pack that is not published on the same feed. This can also lead to errors during installation as the required pack cannot be found.

## Possible solution

Ensure that any recursive _Nuget.config_ files are configured to specify the correct package sources and NuGet feeds. For example, if you have a _Nuget.config_ file in your user profile directory, ensure that it doesn't specify a package source that is no longer available.

If you encounter errors related to the SDK being aware of workload manifest or workload pack versions not present in your configured feeds, you may need to adjust your feeds or find the feed where the new version of the manifest or required pack is located.

In the case where a workload manifest has a dependency on a workload pack not published on the same feed, you will need to find and add the feed where that pack is located to your NuGet configuration.

> [!IMPORTANT]
> Some development environments may depend on private feeds that provide newer versions of the workload manifest or workload pack. In these situations, you may want to disable the daily SDK check for updates to avoid encountering errors during installation.
>
> To disable the daily SDK check for updates, set the `DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE` environment variable to `true`.

## See also

- [.NET SDK: Diagnosing issues with .NET SDK Workloads](https://github.com/dotnet/sdk/pull/40912)
- [.NET CLI: dotnet workload install](/dotnet/core/tools/dotnet-workload-install)
- [NuGet: nuget.config reference](/nuget/reference/nuget-config-file)
