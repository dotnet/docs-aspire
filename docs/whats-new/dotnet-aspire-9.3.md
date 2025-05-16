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

## 💔 Breaking changes

With every release, we strive to make .NET Aspire better. However, some changes may break existing functionality. The following breaking changes are introduced in .NET Aspire 9.3:

- [Breaking changes in .NET Aspire 9.3](../compatibility/9.3/index.md)
