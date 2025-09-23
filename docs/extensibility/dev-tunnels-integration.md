---
title: .NET Aspire dev tunnels integration
description: Learn how to use the .NET Aspire dev tunnels integration to securely expose local endpoints publicly during development.
ms.date: 09/23/2025
---

# .NET Aspire dev tunnels integration

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[Dev tunnels](Aspire.OpenAI/azure/developer/dev-tunnels/overview) allow developers to securely share local web services across the internet. The .NET Aspire dev tunnels integration makes it easy to model dev tunnels in your AppHost projects so that they're automatically managed during development.

> [!IMPORTANT]
> Dev tunnels are for ad-hoc testing and development, not for production workloads.

Dev tunnels are useful for:

- Sharing a running local service (for example, a Web API) with teammates, mobile devices, or webhooks.
- Testing incoming callbacks from external SaaS systems (GitHub / Stripe / etc.) without deploying.
- Quickly publishing a temporary, TLS‑terminated endpoint during development.

> [!NOTE]
> By default tunnels require authentication and are available only to the user who created them. You can selectively enable anonymous (public) access per tunnel or per individual port.

## Prerequisites

Before you create a dev tunnel, you first need to download and install the devtunnel CLI (Command Line Interface) tool that corresponds to your operating system. See the [devtunnel CLI installation documentation](Aspire.OpenAI/azure/developer/dev-tunnels/get-started#install) for more details.

## Hosting integration

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

### Add a dev tunnel resource

In the AppHost project, add a dev tunnel and configure it to expose specific resources:

:::code source="snippets/dev-tunnels/DevTunnels.AppHost/AppHost.cs":::

When you run the AppHost, the dev tunnel is created to expose the web application endpoints publicly. The tunnel URLs are shown in the .NET Aspire dashboard. By default, the tunnel requires authentication and is available only to the user who created it.

### Allow anonymous access

To allow anonymous (public) access to the entire tunnel, chain a call to the `WithAnonymousAccess` method:

:::code source="snippets/dev-tunnels/DevTunnels.AppHost/AppHost.Anonymous.cs" id="anonymous":::

The preceding code:

- Creates a new `IDistributedApplicationBuilder` instance.
- Adds a project reference to the `web` project.
- Adds a dev tunnel named `public-api` that exposes the `web` project.
- Configures the tunnel to allow anonymous access.

### Configure dev tunnel options

To configure other options for the dev tunnel, provide the `DevTunnelOptions` to the `AddDevTunnel` method:

:::code source="snippets/dev-tunnels/DevTunnels.AppHost/AppHost.Options.cs" id="options":::

The preceding code:

- Creates a new `IDistributedApplicationBuilder` instance.
- Adds a project reference to the `web` project.
- Creates a `DevTunnelOptions` instance to configure the tunnel.
- Adds a dev tunnel named `qa` with a specific `tunnelId` that exposes the `web` project.
- Configures the tunnel with a description, labels, and disables anonymous access.

### Configure for mixed access

To allow anonymous access to specific endpoints, use the appropriate `WithReference` overload as shown in the following code:

:::code source="snippets/dev-tunnels/DevTunnels.AppHost/AppHost.MixedAccess.cs" id="mixedaccess":::

The preceding code:

- Creates a new `IDistributedApplicationBuilder` instance.
- Adds a project reference to the `api` project.
- Adds a dev tunnel named `mixed-access` that exposes:
  - The `public` endpoint of the `api` project with anonymous access.
  - The `admin` endpoint of the `api` project that requires authentication.

### Service discovery integration

When another resource references a dev tunnel, environment variables are injected using the [.NET Aspire service discovery](../service-discovery/overview.md) configuration format. Use the `WithReference` overloads that accept the `IResourceBuilder<DevTunnelResource>` parameter to reference a dev tunnel. This injects environment variables like:

```env
services__web__https__0=https://myweb-1234.westeurope.devtunnels.ms/
```

This lets downstream resources use the tunneled address exactly like any other .NET Aspire service discovery entry.

> [!NOTE]
> Referencing a tunnel delays the consumer resource's start until the tunnel has started and its endpoint is fully allocated.

> [!IMPORTANT]
> Dev tunnels are a development time concern only and aren't included when publishing or deploying an AppHost, including any service discovery information.

## Configuration

### Dev tunnel options

The `DevTunnelOptions` class provides several configuration options:

| Property | Description |
|--|--|
| `Description` | A description for the tunnel that appears in the dev tunnels service. |
| `Labels` | A list of labels to apply to the tunnel for organization and filtering. |
| `AllowAnonymous` | Whether to allow anonymous access to the entire tunnel. |

### Dev tunnel port options

The `DevTunnelPortOptions` class provides configuration for individual tunnel ports:

| Property | Description |
|--|--|
| `Protocol` | The protocol to use (`http`, `https`, or `auto`). If not specified, uses the endpoint's scheme. |
| `Description` | A description for this specific port. |
| `Labels` | Labels to apply to this port. |
| `AllowAnonymous` | Whether to allow anonymous access to this specific port. |

### Security considerations

- Prefer authenticated tunnels during normal development.
- Only enable anonymous access for endpoints that are safe to expose publicly.
- Treat public tunnel URLs as temporary & untrusted (rate limit / validate input server-side).

### Tunnel lifecycle

Dev tunnels automatically:

- Install the devtunnel CLI if not already available.
- Ensure the user is logged in to the dev tunnels service.
- Create and manage tunnel lifecycle.
- Clean up unmodeled ports from previous runs.
- Provide detailed logging and diagnostics.

Tunnels will expire after not being hosted for 30 days by default, so they won't be forcibly deleted when the resource or AppHost is stopped.

### Troubleshooting

#### Authentication required

If you see authentication errors, ensure you're logged in to the dev tunnels service:

```bash
devtunnel user login
```

#### Port conflicts

If you encounter port binding issues, check that no other processes are using the same ports, or configure different ports for your endpoints.

#### Tunnel not accessible

Verify that:

- The tunnel is running and healthy in the .NET Aspire dashboard.
- You're using the correct tunnel URL.
- Anonymous access is configured correctly if accessing without authentication.

## See also

- [Dev tunnels service documentation](/azure/developer/dev-tunnels/overview)
- [Dev tunnels FAQ](/azure/developer/dev-tunnels/faq)
- [.NET Aspire service discovery](../service-discovery/overview.md)
- [.NET Aspire networking overview](../fundamentals/networking-overview.md)
