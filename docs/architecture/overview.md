---
title: .NET Aspire architecture overview  
description: Learn about the overall architecture of .NET Aspire, including its integrations, orchestration, and networking capabilities.  
ms.date: 04/23/2025
---

# .NET Aspire architecture overview  

.NET Aspire brings together a powerful suite of tools and libraries, designed to deliver a seamless and intuitive experience for developers. Its modular and extensible architecture empowers you to define your application model with precision, orchestrating intricate systems composed of services, containers, and executables. Whether your components span different programming languages, platforms, stacks, or operating systems, .NET Aspire ensures they work harmoniously, simplifying the complexity of modern application development.

## App model architecture

Resources are the building blocks of your app model. They're used to represent abstract concepts like services, containers, executables, and external integrations. Specific resources enable developers to define dependencies on concrete implementations of these concepts. For example, a `Redis` resource can be used to represent a Redis cache, while a `Postgres` resource can represent a PostgreSQL database.

While the app model is often synonymous with a collection of resources, it's a high level representation of your entire application topology. This is important, as its architected for lowering. In this way, .NET Aspire can be thought of as a compiler for application topology.

**Lowering the model**

In a traditional compiler, the process of "lowering" involves translating a high-level programming language into progressively simpler representations:

- **Intermediate Representation (IR):** The first step abstracts away language-specific features, creating a platform-neutral representation.
- **Machine Code:** The IR is then transformed into machine-specific instructions tailored to a specific CPU architecture.

Similarly, .NET Aspire applies this concept to applications, treating the app model as the high-level language:

- **Intermediate constructs:** The app model is first lowered into intermediate constructs, such as cloud development kit (CDK)-style object graphs. These constructs might be platform-agnostic or partially tailored to specific targets.
- **Target runtime representation:** Finally, a publisher generates the deployment-ready artifacts—YAML, HCL, JSON, or other formats—required by the target platform.

This layered approach unlocks several key benefits:

- **Validation and enrichment:** Models can be validated and enriched during the transformation process, ensuring correctness and completeness.
- **Multi-target support:** .NET Aspire supports multiple deployment targets, enabling flexibility across diverse environments.
- **Customizable workflow:** Developers can hook into each phase of the process to customize behavior, tailoring the output to specific needs.
- **Clean and portable models:** The high-level app model remains expressive, portable, and free from platform-specific concerns.

Most importantly, the translation process itself is highly extensible. You can define custom transformations, enrichments, and output formats, allowing .NET Aspire to seamlessly adapt to your unique infrastructure and deployment requirements. This extensibility ensures that .NET Aspire remains a powerful and versatile tool, capable of evolving alongside your application's needs.

### Modality and extensibility

.NET Aspire operates in two primary modes, each tailored to streamline your specific needs—detailed in the following section. Both modes use a robust set of familiar APIs and a rich ecosystem of [integrations](../fundamentals/integrations-overview.md). These integrations work together like puzzle pieces, enabling you to define resources, express dependencies, and configure behavior effortlessly—whether you're running locally or deploying to production.

Why is modality important when it comes to the app host's execution context? This is because it allows you to define your app model once and with the appropriate APIs, specify how resources operate in each mode. Consider the following collection of resources:

- Database: PostgreSQL
- Cache: Redis
- AI service: Ollama or OpenAI
- Backend: ASP.NET Core minimal API
- Frontend: React app

Depending on the mode, the app host might treat these resources differently. For example, in run mode, the app host might use a local PostgreSQL database and Redis cache—using containers, while in publish mode, it might generate deployment artifacts for Azure PostgreSQL and Redis Cache.

#### Run mode

The default mode is run mode, which is ideal for local development and testing. In this mode, the .NET Aspire app host orchestrates your application model, including processes, containers, and cloud emulators, to facilitate fast and iterative development. Resources behave like real runtime entities with lifecycles that mirror production. With a simple <kbd>F5</kbd>, the app host launches everything in your [app model](xref:Aspire.Hosting.ApplicationModel.DistributedApplicationModel)—storage, databases, caches, messaging, jobs, APIs, frontends—all fully configured and ready to debug locally. Let's considering the app model from the previous section—where app host would orchestrate the following resources locally:

<!--

Mermaid live:

https://mermaid.live/edit#pako:eNptkU2PmzAQhv-KNadWIikBAgRVKyWhVXPYNtpwat3DLEwTa8FGg1l1m81_r4G2q1Xqi-3xMx-v3zOUpiLI4MjYnkSRSy3cKpSt6ZuEnB5nVjUkDJcn6iyjVUZL-C71BH5koy3pyrF3hKUV2Lbv7_nmjW4b0Vlk-3akB3aD5cOErg_7-ecPhdgaJnGrtGqwFuv9bsysjNVkBff6JXWLrr1LHPeRuqNKdaJ03VFp4hd0vRsa7MSB-FGVE_ylrrHB_9H5ZlCJFu-xm9i96eyR6ar2a71iNrt5_lQU-3du7Oe_0l7pHJBp8OvwencdyzdSgwcNcYOqcpacB0aCPVFDEjJ3rJAfJEh9cRz21hyedAmZ5Z48YNMfT5D9wLpzt76t0FKu0Pna_Iu6P7OGbyfHR-M9aFFDdoafkAX-fJH4abIKVqkfx8sg8uAJsjSYp3EYLOIgToIkXMUXD34Z46ou5n4YB1EaRcvlwk_jKBzLfR0fp6mOPEj5M6FTSrw1vbaQhavk8hvQD8fF

-->

:::image type="content" source="media/local-app-topology-thumb.png" alt-text="Local app topology for dev-time orchestration" lightbox="media/local-app-topology.png":::

For more information on how run mode works, see [Dev-time orchestration](#dev-time-orchestration).

#### Publish mode

The publish mode generates deployment-ready artifacts tailored to your target environment. The .NET Aspire app host compiles your app model into outputs like Kubernetes manifests, Terraform configs, Bicep/ARM templates, Docker Compose files, or CDK constructs—ready for integration into any deployment pipeline. The output format depends on the chosen publisher, giving you flexibility across deployment scenarios. When you consider the app model from the previous section, the app host doesn't orchestrate anything—instead, it emits publish artifacts that can be used to deploy your application to a cloud provider. For example, let's assume you want to deploy to Azure—the app host would emit Bicep templates that define the following resources:

<!--

Mermaid live:

https://mermaid.live/edit#pako:eNptUl1vozAQ_CvWPrUSTQkQQlBViYRWRbrecQWp0tV92IAbrAYbGXNqm-a_n4F-pOr5xfbuzHhn1zsoZMkghI3CpiJ5TAUxK-d6y-4opN16y9uKwj0VY-ZSSaGZKE3yhmGhCTbN2VqdH0UvnWIk06h5QW7ZmkRNczwQe9oSi8eRFWXp5OdFTlbSwGsueI1bEqXJgcjKPIFcMPVVY4VF1Rc17Ifw_k4epCI3rOTtJyFK-vcSkjH1lxeHlF8NE1HyiYyXBhmjxjW2h7j30KCeylZvFMt-_zj-T0PIycn561Wep6fGzOu74S_ue8hY7fdwlHyPxUsqwIKaqRp5aYa06zEUdMVqRiE0xxLVIwUq9gaHnZbZsygg1KpjFijZbSoIH3DbmlvXlKhZzNFMuv6ImoZpqa7HPzB8BQsaFBDu4AlCx55M53YwXziLwPb9meNZ8Axh4EwC33WmvuPPnbm78PcWvEhpVKcT2_UdL_C82WxqB77nDnJ_huRY1Ub1Vt4qNE6ZWslOaAhdb7H_B9Ycy5w

-->

:::image type="content" source="media/publish-app-topology-thumb.png" alt-text="Published app topology" lightbox="media/publish-app-topology.png":::

For more information on how to publish mode works, see [Publish mode](../architecture/publish-mode.md).

## Dev-time orchestration

In run mode, [the app host orchestrates](../fundamentals/app-host-overview.md) all resources defined in your app model. But how does it achieve this?

> [!IMPORTANT]
> The app host isn't a production runtime. It's a development-time orchestration tool that simplifies the process of running and debugging your application locally.

In this section, several key questions are answered to help you understand how the app host orchestrates your app model:

- **What powers the orchestration?**

  Orchestration is delegated to the [Microsoft Developer Control Plane](#developer-control-plane) (DCP), which manages resource lifecycles, startup order, dependencies, and network configurations across your app topology.

- **How is the app model used?**

  The app model defines all resources via implementations of <xref:Aspire.Hosting.ApplicationModel.IResource>, including containers, processes, databases, and external services—forming the blueprint for orchestration.

- **What role does the app host play?**

  The app host provides a high-level declaration of the desired application state. It delegates execution to DCP, which interprets the app model and performs orchestration accordingly.

- **What resources are monitored?**

  All declared resources—including containers, executables, and integrations—are monitored to ensure correct behavior and to support a reliable dev loop.

- **How are containers and executables managed?**

  Containers and processes are initialized with their configs, launched in order, and networked as needed. DCP ensures their readiness and connectivity during orchestration.

- **How are resource dependencies handled?**

  Dependencies are defined in the app model and evaluated by DCP to determine correct startup sequencing, ensuring resources are available before dependents start.

- **How is networking configured?**

  Networking—such as port bindings—is autoconfigured unless explicitly defined. DCP resolves conflicts and ensures availability, enabling seamless communication between services.

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

- `dcp.exe`: API server that exposes a Kubernetes-like API endpoint for the app host to communicate with. Additionally, it exposes log streaming to the app host, which ultimately streams logs to the developer dashboard.
- `dcpctrl.exe`: Controller that monitors the API server for new objects and changes, ensuring that the real-world environment matches the specified model.

> [!NOTE]
> DCP strives for "eventual consistency" rather than "strong consistency." This means that while DCP aims to keep the real-world environment in sync with the specified model, it might not always be perfectly aligned.

When you run the app host, it communicates with DCP using Kubernetes client libraries. The app host gives the app model to DCP, which looks at the resources in the model and turns them into specs it can understand. This process involves translating the app model into a set of Kubernetes custom resource definitions (CRDs) that represent the desired state of the application

DCP performs the following tasks:

- Prepares the resources for execution:
  - Configures service endpoints.
    - Assigns names and ports dynamically, unless explicitly set (DCP ensures that the ports are available and not in use by other processes).
  - Initializes container networks.
  - Pulls container images based on their applied [pull policy](xref:Aspire.Hosting.ApplicationModel.ImagePullPolicy).
  - Creates and starts containers.
  - Runs executables with the required arguments and environment variables.
- Monitors resources:
  - Provides change notifications about objects managed within DCP, including process IDs, running status, and exit codes (the app host subscribes to these changes to manage the [application's lifecycle](../app-host/eventing.md) effectively).
- Starts the developer dashboard.

Continuing from the [diagram in the previous](#app-host-dcp-flow) section, consider the following diagram that helps to visualize the responsibilities of DCP:

<span id="dcp-architecture"></span>

:::image type="content" source="media/dcp-architecture-thumb.png" alt-text="A diagram depicting the architecture of the Developer Control Plane (DCP)." lightbox="media/dcp-architecture.png":::

DCP logs are streamed back to the app host, and then the app host pushes them to the developer dashboard. DCP registers default commands for the developer dashboard as well, such as start, stop, and restart. For more information on commands, see [Custom resource commands in .NET Aspire](../fundamentals/custom-resource-commands.md).

## Developer dashboard

The [.NET Aspire developer dashboard](../fundamentals/dashboard/overview.md) is a powerful tool designed to simplify local development and resource management. It also supports a [standalone mode](../fundamentals/dashboard/standalone.md) and integrates seamlessly when publishing to Azure Container Apps. With its intuitive interface, the dashboard empowers developers to monitor, manage, and interact with application resources effortlessly.

### Monitor and manage resources

The dashboard provides a user-friendly interface for inspecting resource states, viewing logs, and executing commands. Whether you're debugging locally or deploying to the cloud, the dashboard ensures you have full visibility into your application's behavior.

### Built-in and custom commands

The dashboard relies on DCP to expose a set of default commands for resources, such as start, stop, and restart. These commands are readily available in the dashboard UI, streamlining resource management. For more information, see [Stop or Start a resource](../fundamentals/dashboard/explore.md#stop-or-start-a-resource).

In addition to the default commands, you can define custom commands tailored to your application's needs. These commands are registered in the app model and appear directly in the dashboard, offering flexibility, and control. Learn more about custom commands in [Custom resource commands in .NET Aspire](../fundamentals/custom-resource-commands.md).

### Real-time log streaming

Stay informed with the dashboard's [real-time log streaming](../fundamentals/dashboard/explore.md#console-logs-page) feature. Logs from all resources in your app model are streamed from DCP to the app host and displayed in the dashboard. With advanced filtering options—by resource type, severity, and more—you can quickly pinpoint relevant information and troubleshoot effectively.

The developer dashboard is more than just a tool—it's your command center for building, debugging, and managing .NET Aspire applications with confidence and ease.

## See also

- [Orchestration overview](../fundamentals/app-host-overview.md)
- [Explore the .NET Aspire dashboard](../fundamentals/dashboard/explore.md)
