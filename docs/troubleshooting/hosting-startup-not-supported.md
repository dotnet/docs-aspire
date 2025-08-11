---
title: HostingStartup is not supported with .NET Aspire integrations
description: Learn how to migrate from HostingStartup to the IHostApplicationBuilder pattern for use with .NET Aspire integrations.
ms.date: 08/04/2025
ai-usage: ai-assisted
---

# HostingStartup is not supported with .NET Aspire integrations

.NET Aspire integrations require the use of <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder>, but `HostingStartup` only provides access to <xref:Microsoft.AspNetCore.Hosting.IWebHostBuilder>. This fundamental incompatibility means that you can't configure .NET Aspire integrations from within a `HostingStartup` implementation.

## Symptoms

When attempting to use .NET Aspire integrations within a HostingStartup implementation, you might encounter:

- **Compilation errors**: Aspire integration extension methods like `AddNpgsqlDbContext` or `AddRedis` are not available on `IWebHostBuilder`.
- **Runtime configuration issues**: Even if you access the underlying services, the proper configuration and service registration won't occur.
- **Missing telemetry and resilience**: Aspire's built-in observability, health checks, and resilience patterns won't be applied.

## Why HostingStartup doesn't work with .NET Aspire

.NET Aspire integrations extend <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to provide:

- Standardized configuration patterns.
- Built-in health checks.
- Telemetry and observability.
- Resilience patterns.
- Service discovery integration.

The `HostingStartup` feature was designed for the older ASP.NET Core hosting model and only provides access to <xref:Microsoft.AspNetCore.Hosting.IWebHostBuilder>, which doesn't include these modern hosting capabilities.

## Migrating from HostingStartup

The `HostingStartup` feature represents an older ASP.NET Core hosting model that predates the modern <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> pattern that .NET Aspire requires. Migration is necessary to leverage .NET Aspire's integrations and modern hosting capabilities.

### Understanding the API changes

The fundamental difference lies in the hosting abstractions:

**Before (HostingStartup pattern):**

:::code language="csharp" source="snippets/hosting-startup-not-supported/hosting-startup-before.cs":::

**After (Modern hosting pattern):**

:::code language="csharp" source="snippets/hosting-startup-not-supported/host-application-builder-after.cs":::

### Key conceptual changes

When migrating from `HostingStartup` to the modern hosting model, you're moving between these approaches:

| Legacy pattern | Modern pattern | Benefit |
|---|---|---|
| `IWebHostBuilder` | `IHostApplicationBuilder` | Access to modern hosting features and .NET Aspire integrations |
| Separate startup classes | Program.cs configuration | Service configuration moves directly into the application's entry point for better clarity and debugging |
| Manual service registration | Integration packages | .NET Aspire integrations handle service registration, configuration, health checks, and telemetry automatically |

### Migration resources

For detailed migration guidance, see:

- [Migrate from ASP.NET Core 5.0 to 6.0](/aspnet/core/migration/50-to-60?view=aspnetcore-9.0) - Covers the transition to the minimal hosting model
- [David Fowl's ASP.NET Core 6.0 migration guide](https://gist.github.com/davidfowl/0e0372c3c1d895c3ce195ba983b1e03d) - Provides practical migration patterns and examples

## Additional considerations

- **Service discovery**: .NET Aspire integrations automatically configure service discovery. If you were using HostingStartup for service-to-service communication, consider using Aspire's [service discovery features](../service-discovery/overview.md).

- **Configuration management**: Instead of hard-coding connection strings in HostingStartup, use .NET Aspire's configuration patterns with connection string names that map to resources in your app host.

- **Testing**: .NET Aspire provides [testing capabilities](../testing/overview.md) that work with the new hosting model.

For more information about .NET Aspire integrations and the hosting model, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md).
