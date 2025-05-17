---
title: What's new in .NET Aspire 9.3
description: Learn what's new in the official general availability release of .NET Aspire 9.3.
ms.date: 05/07/2025
---

# What's new in .NET Aspire 9.3

📢 .NET Aspire 9.3 is the next minor version release of .NET Aspire; it supports:

- .NET 8.0 Long Term Support (LTS)
- .NET 9.0 Standard Term Support (STS)

If you have feedback, questions, or want to contribute to .NET Aspire, collaborate with us on [:::image type="icon" source="../media/github-mark.svg" border="false"::: GitHub](https://github.com/dotnet/aspire) or join us on [:::image type="icon" source="../media/discord-icon.svg" border="false"::: Discord](https://discord.com/invite/h87kDAHQgJ) to chat with team members.

It's important to note that .NET Aspire releases out-of-band from .NET releases. While major versions of .NET Aspire align with .NET major versions, minor versions are released more frequently. For more information on .NET and .NET Aspire version support, see:

- [.NET support policy](https://dotnet.microsoft.com/platform/support/policy): Definitions for LTS and STS.
- [.NET Aspire support policy](https://dotnet.microsoft.com/platform/support/policy/aspire): Important unique product life cycle details.

🖥️ App model enhancements

### ✨ Zero-friction container configuration

Many container integrations now expose **first-class helpers** to set ports, usernames, and passwords without digging through internal properties.
All three settings can be supplied **securely via parameters**, keeping secrets out of source:

```csharp
var pgPwd = builder.AddParameter("pg-pwd", secret: true);

builder.AddPostgres("pg")
       .WithHostPort(6045)          // choose the host-side port
       .WithPassword(pgPwd)         // reference a secret parameter
```

The new `WithHostPort`, `WithPassword`, and `WithUserName` (or equivalent per-service) extension methods are available on **PostgreSQL, SQL Server, Redis, and several other container resources**, giving you consistent, declarative control across the stack.

### 🔗 Streamlined custom URLs

9.3 makes resource links both **smarter and easier to place**:

* **Pick where a link appears** – each link now carries a `UrlDisplayLocation` (`SummaryAndDetails` or `DetailsOnly`), so you can keep diagnostic links out of the main grid yet still see them in the details pane.
* **Relative paths are auto-resolved** – hand the helper `"/health"` and Aspire rewrites it to the full host-qualified URL when the endpoint is allocated.
* **Multiple links per endpoint** – an overload of `WithUrlForEndpoint` lets you attach extra URLs (docs, admin UIs, probes) to the same endpoint without redefining it.
* **Endpoint helper inside callbacks** – `context.GetEndpoint("https")` fetches the fully-resolved endpoint so you can build custom links programmatically.
* **Custom URLs for any resource** – `WithUrl*` also works for custom resources.

```csharp
var frontend = builder.AddProject<Projects.Frontend>("frontend")

    // Hide the plain-HTTP link from the Resources grid
    .WithUrlForEndpoint("http",
        url => url.DisplayLocation = UrlDisplayLocation.DetailsOnly)

    // Add an extra link under the HTTPS endpoint that points to /health
    .WithUrlForEndpoint("https", ep => new()
    {
        Url            = "/health",                  // relative path supported
        DisplayText    = "Health",
        DisplayLocation = UrlDisplayLocation.DetailsOnly
    });
```

These tweaks let you surface the right links, in the right place, with virtually zero boilerplate.

### 🙈 Hide resources without "faking" their state 

Historically the only way to keep a resource out of the Dashboard was to put it in the **`Hidden`** *state*—a hack that also made the resource look "terminal" to APIs such as `WaitForResourceAsync`. In 9.3 every snapshot now carries a **boolean `IsHidden` flag**, completely decoupling *visibility* from *lifecycle state*.

* **Cleaner defaults** – low-level helpers like `AddParameter` and `AddConnectionString` mark themselves hidden so they don’t clutter the UI:

  ```csharp
  var apiKey = builder.AddParameter("api-key", secret: true);   // IsHidden = true ✔
  ```

* **Accurate waits & health flows** – `WaitForResourceAsync` was updated to treat `IsHidden` as a separate predicate, so hidden resources can still be awaited or surfaced programmatically without special-casing states. ([GitHub][1])

This small change removes ambiguity in the model while giving you precise control over what shows up in the Dashboard.

### 🔔 New lifecycle events

.NET Aspire 9.3 introduces two new lifecycle events that make it easier to build custom resources with predictable behavior—without relying on hacks like `Task.Run` or polling:

#### `InitializeResourceEvent`

This event fires **after a resource is added**, but **before endpoints are allocated**. It's especially useful for custom resources that don't have a built-in lifecycle (like containers or executables), giving you a clean place to kick off background logic, set default state, or wire up behavior.

For example, this minimal custom resource publishes a running state when initialized:

```csharp
var myCustom = new MyCustomResource("my-resource");

builder.AddResource(myCustom);
builder.Eventing.Subscribe<InitializeResourceEvent>(myCustom, async (e, ct) =>
{
    await e.Notifications.PublishUpdateAsync(e.Resource,
        s => s with { State = KnownResourceStates.Running });
});
```

This replaces awkward patterns like `Task.Run` inside constructors or `Configure()` methods. You can see a more complex version in the [TalkingClock sample](https://github.com/dotnet/aspire-samples/tree/3dee8cd7c7880fe421ea61ba167301eb1369000a/samples/CustomResources/CustomResources.AppHost) in the official Aspire samples repo.

#### `ResourceEndpointsAllocatedEvent`

This event fires once a resource's endpoints have been assigned (e.g., after port resolution or container allocation). It's scoped per resource, so you can safely get an EndpointReference and build derived URLs or diagnostics.

```csharp
builder.Eventing.Subscribe<ResourceEndpointsAllocatedEvent>((e, ct) =>
{
    if (e.Resource is IResourceWithEndpoints resource)
    {
        var http = resource.GetEndpoint("http");

        Console.WriteLine($"Endpoint http - Allocated {http.IsAllocated}, Port: {http.Port}");
    }

    return Task.CompletedTask;
});
```

These events make resource authoring smoother, safer, and more deterministic—no lifecycle guesswork needed.

Absolutely—here’s the updated version with the ports removed from the `Address` fields, aligning with how Aspire resolves service names internally via the network:

---

Perfect—here’s the revised version with all the accurate caveats clearly called out:

---

### 🌐 YARP Integration (Preview)

.NET Aspire 9.3 introduces **preview support for [YARP](https://aka.ms/yarp)** (Yet Another Reverse Proxy)—a long-requested addition that brings reverse proxying into the Aspire application model.

This integration makes it easy to add a lightweight proxy container to your distributed app, powered by the official [YARP container image](http://yarp.dot.net). It currently supports **configuration-based routing only**, using a JSON file you supply.

#### Add a reverse proxy to your Aspire app:

```csharp
builder.AddYarp("apigateway")
       .WithConfigFile("yarp.json")
       .WithReference(basketService)
       .WithReference(catalogService);
```

The config file is mounted into the container and used as the runtime YARP configuration.

#### Example `yarp.json`:

```json
{
  "ReverseProxy": {
    "Routes": {
      "catalog": {
        "ClusterId": "catalog",
        "Match": {
          "Path": "/catalog/{**catch-all}"
        }
      },
      "basket": {
        "ClusterId": "basket",
        "Match": {
          "Path": "/basket/{**catch-all}"
        }
      }
    },
    "Clusters": {
      "catalog": {
        "Destinations": {
          "catalog/d1": {
            "Address": "http://catalog/"
          }
        }
      },
      "basket": {
        "Destinations": {
          "basket/d1": {
            "Address": "http://basket/"
          }
        }
      }
    }
  }
}
```

The `.WithReference(...)` calls automatically ensure that the proxy container can resolve the referenced services by name (`catalog`, `basket`), using Aspire’s internal network graph.


### ⚠️ Known limitations in this preview

* **Only configuration-based routing is supported**. Code-based or programmatic route generation is not available yet.
* **The configuration file is not deployed** as part of publish operations—you must manage the file manually.
* **Routing from containers to projects will not work on Podman**, due to host-to-container networking limitations.

> 💡 Want to learn more about authoring YARP configs? See the official [YARP documentation](https://aka.ms/yarp).
> 🧪 This integration is in preview—APIs and behavior may evolve. Feedback welcome!

### 🐬 MySQL `AddDatabase` now creates the database

In .NET Aspire 9.3, the MySQL integration now supports **automatic database creation** via the `AddDatabase` API—matching the behavior already available for SQL Server and PostgreSQL.

Previously, calling `.AddDatabase("dbname")` on a MySQL resource only created a logical reference in Aspire's app model—it did **not** create the database on the server. This mismatch caused confusion, especially when users expected Aspire to provision the database like it does for other integrations.

#### ✅ New behavior in 9.3:

```csharp
var mysql = builder.AddMySql("db");

mysql.AddDatabase("appdb");
```

At runtime, Aspire now executes a `CREATE DATABASE` command for `"appdb"` against the running MySQL container or server. If the database already exists, the command is skipped safely.

This brings MySQL in line with the broader Aspire database ecosystem:

| Integration | Automatically creates database?           |
| ----------- | ----------------------------------------- |
| SQL Server  | ✅ Yes                                     |
| PostgreSQL  | ✅ Yes                                     |
| **MySQL**   | ✅ **Yes (new in 9.3)**                    |
| MongoDB     | ❌ No (not needed; created on first write) |
| Oracle      | ❌ No (not supported yet)                  |

No additional configuration is required—the same `AddDatabase` call you already use now provisions the database for you behind the scenes.

## 📊 Dashboard delights

### 🧠 Remembers your filter settings

The .NET Aspire dashboard now **remembers your resource filter settings** between sessions. Previously, if you filtered the Resources view (for example, to hide support services or highlight only frontend apps), those filters were reset on page reload.

As of 9.3, filter state is **persisted in local storage**, so your selections stick across refreshes and restarts. This small improvement makes it easier to focus on the parts of your app that matter most—especially in large graphs with many supporting services like Redis, SQL, or queues.

### 🧵 Uninstrumented resources now appear in Traces

In 9.3, the dashboard can now **visualize outgoing calls to resources that don’t emit their own telemetry**—such as databases, caches, and other infrastructure components that lack built-in tracing.

Previously, these dependencies were invisible in the **Traces** view unless they were emitting OTLP traces. Now, if your app makes an HTTP, SQL, or Redis call to a **modeled Aspire resource** that doesn’t emit spans itself, Aspire still shows it as a **referenced peer** in the trace timeline.

This helps you:

* Understand the full chain of dependencies—even if some components are passive
* Debug latency or failures in calls to uninstrumented services
* Keep the trace UI consistent across infrastructure types

> 💡 This is especially useful for services like SQL Server, PostgreSQL, Redis, or blob storage where outgoing client telemetry exists, but the service itself doesn’t participate in distributed tracing.

🧪 No instrumentation changes are needed—Aspire infers the mapping based on resource references.


### 🖱️ Resource context menus & quick-launch actions

.NET Aspire 9.3 makes the dashboard more interactive and easier to navigate by introducing new **context menus** and enhancing how **resource URLs** are surfaced across views.

#### 🧭 Right-click context menus in the graph

You can now **right-click any resource node** in the **Resource Graph** view to bring up a context menu with quick actions:

* Open structured logs, console logs, traces, or metrics for that resource
* Launch external URLs associated with the resource (like PGAdmin, Swagger, or Grafana)
* Jump directly to the resource's detail pane

This reduces the number of clicks and lets you stay in the graph while investigating specific services.

#### 🔗 Resource URLs in console log actions

Resource URLs defined via `WithUrlForEndpoint(...)` are now **more prominently integrated** into the dashboard UI. They appear:

* In the **console logs view** action bar
* In the new **right-click menus**
* On the **resource detail pane**, as before

This makes common destinations—like admin UIs, health checks, and docs—instantly accessible wherever you’re working.

Together, these improvements turn the Aspire dashboard into a true control plane for navigating your distributed app—**less friction, more focus.**

### ⏸️ Metrics pause warning

The dashboard now shows a **warning banner** when metrics collection is paused. This makes it clear that data may be stale if you've temporarily halted telemetry.

### 📝 Friendly names in console logs

When a resource has only **one replica**, the Aspire dashboard now uses the **friendly resource name** (like `frontend`, `apigateway`, or `redis`) instead of the replica ID (like `frontend-0`) in the **console logs view**.

This small change makes logs easier to read and reduces visual noise—especially in common single-instance setups during development.

> In multi-replica scenarios, Aspire still uses full replica IDs so you can distinguish between instances.

Thanks — here’s a rewritten version that reflects the correct architecture shift, ties it to future capabilities, and uses your tone and sample verbatim where needed:

## 🚀 Deployment & publish

### 🏗️ New publisher model & compute environment support

.NET Aspire 9.3 introduces a **new publisher model** that distributes publishing behavior across your application graph instead of relying on a single top-level publisher.

Rather than selecting a target environment (like Docker or Azure) by calling `AddDockerComposePublisher()` or similar, Aspire now includes a **built-in publisher** that looks for a `PublishingCallbackAnnotation` on each resource. This annotation describes how that resource should be published—for example, as a Docker Compose service, Kubernetes manifest, or Azure Bicep module.

> ✅ This architectural shift lays the groundwork for **hybrid and heterogeneous deployments**, where different services within the same app can be deployed to different targets (cloud, edge, local).

#### Most apps only need one environment

In typical apps, you only need to add a **single compute environment**, like:

```csharp
builder.AddAzureContainerAppEnvironment("prod");
```

In this case, Aspire applies the correct publishing behavior to all compute resources in your app model—no extra configuration needed.

#### Multiple environments require disambiguation

If you add **multiple compute environments**, Aspire needs to know which resource goes where. Compute environments apply their transformations to **all applicable compute resources** (projects, containers, executables). If more than one environment matches a given resource, Aspire throws an **ambiguous environment exception** at publish time.

You can resolve this by using `WithComputeEnvironment(...)`:

```csharp
var k8s = builder.AddKubernetesEnvironment("k8s-env");
var compose = builder.AddDockerComposeEnvironment("docker-env");

builder.AddProject<Projects.Api>("api")
       .WithComputeEnvironment(compose);

builder.AddProject<Projects.Frontend>("frontend")
       .WithComputeEnvironment(k8s);
```

This (contrived) example shows how you could explicitly map services to different compute targets—modeling, for example, a frontend in Kubernetes and a backend in Docker Compose.

> 💡 Imagine a real-world case where your frontend is deployed to a CDN or GitHub Pages, and your backend runs in Azure Container Apps. This new model makes that future possible.

📦 Implemented in [#9096](https://github.com/dotnet/aspire/pull/9096)
🔧 All previous publisher registration APIs (like `AddDockerComposePublisher()`) have been removed in favor of this new model.

#### Supported compute environments

.NET Aspire 9.3 supports the following environment resources:

* `AddDockerComposeEnvironment(...)`
* `AddKubernetesEnvironment(...)`
* `AddAzureContainerAppEnvironment(...)`
* `AddAzureAppServiceEnvironment(...)` — [see new App Service support →](#azure-app-service-preview)

These represent deployment targets that can transform and emit infrastructure-specific artifacts from your app model.

### 🐳 Docker Compose enhancements

.NET Aspire 9.3 introduces powerful new capabilities for customizing Docker Compose output using strongly typed, C#-based configuration. You can now declaratively configure both the **global Compose file** and individual **services** directly from the Aspire app model—making your deployment output easy to reason about, customize, and automate.

#### 🛠️ Customize the Compose file and service definitions

You can now programmatically configure the top-level Compose file and the behavior of each individual service using two new APIs:

* `ConfigureComposeFile(...)` — customize the `docker-compose.yml` metadata
* `PublishAsDockerComposeService(...)` — modify the generated service for any compute resource (like a container or project)

```csharp
builder.AddDockerComposeEnvironment("env")
       .WithProperties(env =>
       {
           env.BuildContainerImages = false; // skip image build step
       })
       .ConfigureComposeFile(file =>
       {
           file.Name = "aspire-ai-chat"; // sets the file name
       });

// Add a container to the app
builder.AddContainer("service", "nginx")
       .WithEnvironment("ORIGINAL_ENV", "value")
       .PublishAsDockerComposeService((resource, service) =>
       {
           service.Labels["custom-label"] = "test-value";
           service.AddEnvironmentalVariable("CUSTOM_ENV", "custom-value");
           service.Restart = "always";
       });
```

These APIs give you a structured, strongly typed way to mutate the generated output—enabling richer CI automation, custom tooling, and environment-specific adjustments without editing YAML manually.

#### 🔗 Map parameters and expressions into Docker Compose

.NET Aspire now supports **binding values from the app model**—like parameters and references—into the Docker Compose definition via environment variable placeholders.

This makes it easy to flow dynamic configuration (e.g., from the CI pipeline or secret store) directly into the final output.

```csharp
builder.AddDockerComposeEnvironment("docker-compose");

var containerNameParam = builder.AddParameter("param-1", "default-name", publishValueAsDefault: true);

builder.AddContainer("service", "nginx")
       .WithEnvironment("ORIGINAL_ENV", "value")
       .PublishAsDockerComposeService((resource, service) =>
       {
           service.ContainerName = containerNameParam.AsEnvironmentPlaceholder(resource);
       });
```

The key API here is `.AsEnvironmentPlaceholder(...)`, which tells Aspire to emit a Compose variable like `${PARAM_1}` and register the mapping so the `.env` file is updated accordingly.

> 🧠 This tightly couples your infrastructure parameters with the Docker Compose model—without hardcoding values—unlocking composability across environments.

---

These enhancements make Docker Compose a **fully programmable publishing target**, ideal for local development, container-based CI workflows, and teams that need structured control without brittle YAML overlays.

Perfect — here’s a polished section modeled after the Docker Compose one, highlighting the real Kubernetes APIs introduced in 9.3 with your example and accurate terminology:

---

### ☸️ Kubernetes manifest customization

.NET Aspire 9.3 adds support for **programmatically customizing generated Kubernetes manifests** as part of the publish process. This gives you fine-grained control over the YAML artifacts Aspire emits—without writing raw manifest overlays or patches.

Like Docker Compose, Aspire now supports both **global environment-level settings** and **per-resource customization**.

---

#### 🛠️ Configure global and per-resource settings

You can use the following APIs to configure Kubernetes output in C#:

* `WithProperties(...)` on the compute environment to set global behaviors
* `PublishAsKubernetesService(...)` on compute resources to modify their specific Kubernetes resources

```csharp
builder.AddKubernetesEnvironment("env")
       .WithProperties(env =>
       {
           env.DefaultImagePullPolicy = "Always"; // e.g., Always, IfNotPresent
       });

builder.AddContainer("service", "nginx")
       .WithEnvironment("ORIGINAL_ENV", "value")
       .PublishAsKubernetesService(resource =>
       {
           // Add custom deployment-level settings
           resource.Deployment!.Spec.RevisionHistoryLimit = 5;
       });
```

This gives you fully typed access to the Kubernetes object model, enabling powerful modifications like:

* Overriding container image pull policies
* Customizing replica counts or deployment strategies
* Injecting labels or annotations into Services, Deployments, or ConfigMaps

> 🧠 Aspire emits standard Kubernetes manifests under the hood—you can still use `kubectl`, `helm`, or GitOps workflows to deploy them, but now you can shape them directly from your app definition.

## 💔 Breaking changes

With every release, we strive to make .NET Aspire better. However, some changes may break existing functionality. The following breaking changes are introduced in .NET Aspire 9.3:

- [Breaking changes in .NET Aspire 9.3](../compatibility/9.3/index.md)
