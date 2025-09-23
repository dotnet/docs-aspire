---
title: .NET Aspire dev tunnels integration
description: Learn how to use the .NET Aspire dev tunnels integration to securely expose local endpoints publicly during development.
ms.date: 01/17/2025
---

# .NET Aspire dev tunnels integration

[Dev tunnels](https://learn.microsoft.com/azure/developer/dev-tunnels/overview) allow developers to securely share local web services across the internet. The .NET Aspire dev tunnels integration makes it easy to model dev tunnels in your AppHost projects so that they're automatically managed during development.

> [!IMPORTANT]
> Dev tunnels are for ad-hoc testing and development, not for production workloads.

Dev tunnels are useful for:

- Sharing a running local service (e.g., a Web API) with teammates, mobile devices, or webhooks.
- Testing incoming callbacks from external SaaS systems (GitHub / Stripe / etc.) without deploying.
- Quickly publishing a temporary, TLS‑terminated endpoint during development.

> [!NOTE]
> By default tunnels require authentication and are available only to the user who created them. You can selectively enable anonymous (public) access per tunnel or per individual port.

## Prerequisites

Before you create a dev tunnel, you first need to download and install the devtunnel CLI (Command Line Interface) tool that corresponds to your operating system. See the [devtunnel CLI installation documentation](https://learn.microsoft.com/azure/developer/dev-tunnels/get-started#install) for more details.

## Get started

To get started with the .NET Aspire dev tunnels integration, install the [📦 Aspire.Hosting.DevTunnels](https://www.nuget.org/packages/Aspire.Hosting.DevTunnels) NuGet package in the AppHost project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.DevTunnels
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.DevTunnels"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the AppHost project, add a dev tunnel and configure it to expose specific resources:

:::code source="snippets/dev-tunnels/DevTunnels.AppHost/Program.cs" range="8-11":::

When you run the AppHost, the dev tunnel will be created and configured to expose the web application endpoints publicly. The tunnel URLs will be shown in the .NET Aspire dashboard.

## Configuration

### Basic tunnel configuration

The `AddDevTunnel` method creates a dev tunnel resource with the specified name:

```csharp
var tunnel = builder.AddDevTunnel("api-tunnel");
```

You can optionally specify a custom tunnel ID and additional options:

:::code source="snippets/dev-tunnels/DevTunnels.AppHost/Program.cs" range="18-29":::

### Exposing resources through tunnels

#### Expose all endpoints on a resource

To expose all endpoints of a resource through the tunnel:

:::code source="snippets/dev-tunnels/DevTunnels.AppHost/Program.cs" range="8-11":::

#### Expose specific endpoints

To expose only specific endpoints:

```csharp
var api = builder.AddProject<Projects.ApiService>("api");

var tunnel = builder.AddDevTunnel("api-tunnel")
                    .WithReference(api.GetEndpoint("public"));
```

#### Multiple endpoints with different access levels

You can control anonymous access at the port (endpoint) level:

:::code source="snippets/dev-tunnels/DevTunnels.AppHost/Program.cs" range="33-36":::

### Anonymous access

#### Entire tunnel anonymous access

To enable anonymous access for the entire tunnel:

:::code source="snippets/dev-tunnels/DevTunnels.AppHost/Program.cs" range="13-16":::

#### Per-endpoint anonymous access

You can control anonymous access at the individual endpoint level:

```csharp
var tunnel = builder.AddDevTunnel("api-tunnel")
                    .WithReference(api.GetEndpoint("webhook"), allowAnonymous: true)
                    .WithReference(api.GetEndpoint("admin"), allowAnonymous: false);
```

### Multiple tunnels

You can create multiple tunnels for different purposes:

:::code source="snippets/dev-tunnels/DevTunnels.AppHost/Program.cs" range="13-16,38-40":::

## Service discovery integration

When another resource references a dev tunnel, environment variables are injected using the [.NET Aspire service discovery](../service-discovery/overview.md) configuration format:

:::code source="snippets/dev-tunnels/DevTunnels.AppHost/Program.cs" range="42-44":::

This injects environment variables like:

```env
services__web__https__0=https://myweb-1234.westeurope.devtunnels.ms/
```

This lets downstream resources use the tunneled address exactly like any other .NET Aspire service discovery entry.

> [!NOTE]
> Referencing a tunnel delays the consumer resource's start until the tunnel has started and its endpoint is fully allocated.

> [!IMPORTANT]
> Dev tunnels are a development time concern only and are not included when publishing or deploying an AppHost, including any service discovery information.

## Dev tunnel options

The <xref:Aspire.Hosting.DevTunnels.DevTunnelOptions> class provides several configuration options:

| Property | Description |
|----------|-------------|
| `Description` | A description for the tunnel that appears in the dev tunnels service. |
| `Labels` | A list of labels to apply to the tunnel for organization and filtering. |
| `AllowAnonymous` | Whether to allow anonymous access to the entire tunnel. |

## Dev tunnel port options

The <xref:Aspire.Hosting.DevTunnels.DevTunnelPortOptions> class provides configuration for individual tunnel ports:

| Property | Description |
|----------|-------------|
| `Protocol` | The protocol to use (`http`, `https`, or `auto`). If not specified, uses the endpoint's scheme. |
| `Description` | A description for this specific port. |
| `Labels` | Labels to apply to this port. |
| `AllowAnonymous` | Whether to allow anonymous access to this specific port. |

## Security considerations

- Prefer authenticated tunnels during normal development.
- Only enable anonymous access for endpoints that are safe to expose publicly.
- Treat public tunnel URLs as temporary & untrusted (rate limit / validate input server-side).

## Tunnel lifecycle

Dev tunnels automatically:

- Install the devtunnel CLI if not already available
- Ensure the user is logged in to the dev tunnels service
- Create and manage tunnel lifecycle
- Clean up unmodeled ports from previous runs
- Provide detailed logging and diagnostics

Tunnels will expire after not being hosted for 30 days by default, so they won't be forcibly deleted when the resource or AppHost is stopped.

## Troubleshooting

### Authentication required

If you see authentication errors, ensure you're logged in to the dev tunnels service:

```bash
devtunnel user login
```

### Port conflicts

If you encounter port binding issues, check that no other processes are using the same ports, or configure different ports for your endpoints.

### Tunnel not accessible

Verify that:

- The tunnel is running and healthy in the .NET Aspire dashboard
- You're using the correct tunnel URL
- Anonymous access is configured correctly if accessing without authentication

## See also

- [Dev tunnels service documentation](https://learn.microsoft.com/azure/developer/dev-tunnels/overview)
- [Dev tunnels FAQ](https://learn.microsoft.com/azure/developer/dev-tunnels/faq)
- [.NET Aspire service discovery](../service-discovery/overview.md)
- [.NET Aspire networking overview](../fundamentals/networking-overview.md)
