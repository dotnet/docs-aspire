---
title: .NET Aspire architecture overview  
description: Learn about the overall architecture of .NET Aspire, including its integrations, orchestration, and networking capabilities.  
ms.date: 04/23/2025
---

# .NET Aspire architecture overview  

.NET Aspire brings together a powerful suite of tools and libraries, designed to deliver a seamless and intuitive experience for developers. Its modular and extensible architecture empowers you to define your application model with precision, orchestrating intricate systems composed of services, containers, and executables. Whether your components span different programming languages, platforms, stacks, or operating systems, .NET Aspire ensures they work harmoniously, simplifying the complexity of modern application development.

### The app model architecture

Resources are the building blocks of your app model. They're used to represent abstract concepts like services, containers, executables, and external integrations. Specific resources enable developers to define dependencies on concrete implementations of these concepts. For example, a `Redis` resource can be used to represent a Redis cache, while a `Postgres` resource can represent a PostgreSQL database.

While the app model is often synonymous with a collection of resources, it's a high level representation of your entire application topology. This is important, as its architected for lowering. In this way, .NET Aspire can be thought of as a compiler for application topology.

**Lowering the model**

In a traditional compiler, the process of "lowering" involves translating a high-level programming language into progressively simpler representations:

- **Intermediate Representation (IR):** The first step abstracts away language-specific features, creating a platform-neutral representation.
- **Machine Code:** The IR is then transformed into machine-specific instructions tailored to a specific CPU architecture.

Similarly, .NET Aspire applies this concept to applications, treating the app model as the high-level language:

- **Intermediate constructs:** The app model is first lowered into intermediate constructs, such as CDK-style object graphs. These constructs may be platform-agnostic or partially tailored to specific targets.
- **Target runtime representation:** Finally, a publisher generates the deployment-ready artifacts—YAML, HCL, JSON, or other formats—required by the target platform.

This layered approach unlocks several key benefits:

- **Validation and enrichment:** Models can be validated and enriched during the transformation process, ensuring correctness and completeness.
- **Multi-target support:** Aspire supports multiple deployment targets, enabling flexibility across diverse environments.
- **Customizable workflow:** Developers can hook into each phase of the process to customize behavior, tailoring the output to specific needs.
- **Clean and portable models:** The high-level app model remains expressive, portable, and free from platform-specific concerns.

Perhaps most importantly, the translation process itself is highly extensible. You can define custom transformations, enrichments, and output formats, allowing .NET Aspire to seamlessly adapt to your unique infrastructure and deployment requirements. This extensibility ensures that .NET Aspire remains a powerful and versatile tool, capable of evolving alongside your application's needs.

### Modality and extensibility

.NET Aspire operates in two primary modes, each tailored to streamline your specific needs—detailed in the following section. Both modes leverage a robust set of familiar APIs and a rich ecosystem of [integrations](../fundamentals/integrations-overview.md). These integrations work together like puzzle pieces, enabling you to define resources, express dependencies, and configure behavior effortlessly—whether you're running locally or deploying to production.

#### Run mode

The default mode is run mode, which is ideal for local development and testing. In this mode, the .NET Aspire app host orchestrates your application model, including processes, containers, and cloud emulators, to facilitate fast and iterative development. Resources behave like real runtime entities with lifecycles that mirror production. With a simple <kbd>F5</kbd>, the app host launches everything in your [app model](xref:Aspire.Hosting.ApplicationModel.DistributedApplicationModel)—storage, databases, caches, messaging, jobs, APIs, frontends—all fully configured and ready to debug locally.

For more information on how run mode works, see [Dev-time orchestration](#dev-time-orchestration).

#### Publish mode

Th publish mode generates deployment-ready artifacts tailored to your target environment. The .NET Aspire app host compiles your app model into outputs like Kubernetes manifests, Terraform configs, Bicep/ARM templates, Docker Compose files, or CDK constructs—ready for integration into any deployment pipeline. Output format depends on the chosen publisher, giving you flexibility across deployment scenarios.

For more information on how publish mode works, see [Publish mode](../architecture/publish-mode.md).

## Dev-time orchestration

In run mode, the app host orchestrates all resources defined in your app model. But how does it achieve this? This section explains the orchestration process step by step:

- **What technologies power the orchestration?**

    The orchestration is powered by the [Microsoft Developer Control Plane](#developer-control-plane) (DCP), which manages resource lifecycles, dependencies, and network configurations.

- **How is the app model's resource collection utilized?**

    The app model defines a collection of resources, each implementing <xref:Aspire.Hosting.ApplicationModel.IResource>. These resources can represent processes, containers, databases, queues, external services, or other components your application needs.

- **What services are involved?**

    The app host delegate out to DCP, which works to evaluate and orchestrate the resources in your app model. The app host provides a high-level view of the desired state, while DCP handles the actual orchestration.

- **Which resources are monitored?**

    All resources defined in the app model are monitored, including containers, executables, and external integrations. This ensures they are running as expected during development.

- **How are container and executable resources managed?**

    Containers are created, started, and connected to initialized networks. Executables are launched with their required configurations or arguments. DCP ensures that all resources are properly set up and running.

- **How are dependencies resolved?**

    Dependencies between resources are expressed in the app model. DCP evaluates these dependencies and ensures they are resolved before starting the dependent resources.

- **How are network configurations applied?**

    Network configurations, such as port mappings, are dynamically assigned unless explicitly specified. DCP ensures that ports are available and not in conflict with other processes.

The orchestration process follows a layered architecture. At its core, the app host represents the developer's desired view of the distributed application's resources. DCP ensures that this desired state is realized by orchestrating the resources and maintaining consistency.

The [app model](../fundamentals/app-host-overview.md#define-the-app-model) serves as a blueprint for DCP to orchestrate your application. Under the hood, the app host is a .NET console application powered by the [📦 Aspire.Hosting.AppHost](https://www.nuget.org/packages/Aspire.Hosting.AppHost) NuGet package. This package includes build targets that register orchestration dependencies, enabling seamless dev-time orchestration.

The .NET Aspire app host contains an implementation of the `k8s.KubernetesClient` (from the [📦 KubernetesClient](https://www.nuget.org/packages/KubernetesClient) NuGet package), which is a .NET client for Kubernetes. This client is used to communicate with the DCP API server, allowing the app host to delegate orchestration tasks to DCP.

When you run the app host, DCP takes over, evaluates the app model, and orchestrates the resources. This process ensures that your application is fully configured and ready for local development, allowing you to focus on building features without worrying about infrastructure complexities. Consider the following diagram that helps to visualize the orchestration process:

<span id="app-host-dcp-flow"></span>

:::image type="content" source="media/app-host-dcp-flow-thumb.png" alt-text="A flow diagram depicting how the app host delegates to DCP." lightbox="media/app-host-dcp-flow.png":::

For more information on the app host and APIs for building the app model, see [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md).

### Developer Control Plane

DCP is at the core of the .NET Aspire app host orchestration functionality. It's responsible for orchestrating all resources defined in your app model, starting the developer dashboard, ensuring that everything is set up correctly for local development and testing. DCP manages the lifecycle of resources, applies network configurations, and resolves dependencies.

DCP is written in Go, aligning with Kubernetes and its ecosystem, which are also Go-based. This choice enables deep, native integration with Kubernetes APIs, efficient concurrency, and access to mature tooling like Kubebuilder. DCP is delivered as two executables:

- _dcp.exe_: API server that exposes a Kubernetes-like API endpoint for the app host to communicate with. Additionally, it exposes log streaming to the app host, which ultimately streams logs to the developer dashboard.
- _dcpctrl.exe_: Controller that monitors the API server for new objects and changes, ensuring that the real-world environment matches the specified model.

When you run the app host, it communicates with DCP using Kubernetes client libraries. The app host passes the app model to DCP, which evaluates the collection of resources defined within the app model and converts those resource types into specs that DCP understands. This process involves translating the app model into a set of Kubernetes custom resource definitions (CRDs) that represent the desired state of the application

DCP performs the following tasks:

- Prepares the resources for execution:
  - Services have their endpoints configured.
    - Names and ports are dynamic, unless explicitly set.
    - DCP ensures that the ports are available and not in use by other processes.
  - Container networks are initialized.
  - Containers are created and started.
  - Executables are run with the required configurations or arguments.
- Resources are monitored:
  - DCP provides change notifications about the objects it manages, including process IDs, running status, and exit codes.
  - The app host subscribes to these changes to manage the [application's lifecycle](../app-host/eventing.md) effectively.
- The developer dashboard is started:
  - DCP starts the developer dashboard, which provides a user interface for monitoring and managing the resources.
  - The dashboard is accessible via a web browser, allowing developers to visualize the state of their application.

Continuing from the [diagram in the previous](#app-host-dcp-flow) section, consider the following diagram that helps to visualize the responsibilities of DCP:

<span id="dcp-architecture"></span>

:::image type="content" source="media/dcp-architecture-thumb.png" alt-text="A diagram depicting the architecture of the Developer Control Plane (DCP)." lightbox="media/dcp-architecture.png":::

DCP logs are streamed back to the app host, and then the app host pushes them to the developer dashboard. DCP registers default commands for the developer dashboard as well, such as start, stop, and restart. For more information on commands, see [Custom resource commands in .NET Aspire](../fundamentals/custom-resource-commands.md).

## Developer dashboard

The [.NET Aspire developer dashboard](../fundamentals/dashboard/overview.md) is ideal for local development, but it also supports a [standalone mode](../fundamentals/dashboard/standalone.md), and is actually also available when publishing to Azure Container Apps. For more information on the developer dashboard, see:

- [Explore the .NET Aspire dashboard](../fundamentals/dashboard/explore.md)
- [Dashboard configuration](../fundamentals/dashboard/configuration.md)
- [Enable browser telemetry](../fundamentals/dashboard/enable-browser-telemetry.md)
