---
title: .NET Aspire deployments
description: Learn about essential deployment concepts for .NET Aspire.
ms.topic: overview
ms.date: 09/26/2025
---

# .NET Aspire deployment overview

.NET Aspire separates the act of *producing deployment assets* from *executing a deployment*. The Aspire CLI (`aspire`) provides two high‑level entrypoints:

- `aspire publish` – Generates intermediate, parameterized assets for one or more hosting integrations.
- `aspire deploy` – Executes a deployment (when an integration implements deploy semantics) by resolving parameters and applying changes to a target environment.

These commands are generic orchestration surfaces. Actual behavior (what gets generated, how deployment happens) comes from **hosting integrations** you reference (for example: Docker, Kubernetes, Azure). The system is **extensible**—you can build your own publishing or deployment integrations that plug into the same model.

> Summary: `aspire publish` outputs artifacts with unresolved parameters (placeholders). `aspire deploy` resolves those parameters (when supported) and carries out the deployment. Not every integration supports deploy.

## Aspire CLI commands (conceptual behavior)

| Command | What it does | Outputs | Parameter state | Requires integration capability |
|---------|--------------|---------|-----------------|---------------------------------|
| `aspire publish` | Transforms the application model into integration-specific assets (Compose files, manifests, specifications, etc.). | Intermediate artifacts (not directly production-final). | Unresolved (placeholders, e.g. `${VAR}` or similar). | Publish support |
| `aspire deploy` | Runs a deployment using one or more integrations (build, parameter resolution, apply). | Real resources / applied changes. | Resolved. | Deploy support |

If an integration does not implement deploy functionality, `aspire deploy` will not deploy that target (it may warn or no-op for it).

When you run `aspire deploy` without any integrations that support deployment, you'll see this error:

```
FAILED: Analyzing the distributed application model for publishing and deployment capabilities.
           No resources in the distributed application model support deployment.
```

Similarly, when you run `aspire publish` without any integrations that support publishing, you'll see:

```
Analyzing the distributed application model for publishing and deployment capabilities. 00:00:00
           No resources in the distributed application model support publishing.
```

These messages indicate that you need to add deployment integrations to your AppHost project. Deployment integrations are NuGet packages (like `Aspire.Hosting.Docker`, `Aspire.Hosting.Kubernetes`, or `Aspire.Hosting.Azure`) that provide the publishing and deployment capabilities for specific target platforms.

## Parameter placeholders

Published assets intentionally contain placeholders instead of concrete values. For Docker Compose–based publish output, parameterization appears as standard environment variable references. For example, a publish artifact might include:

```yaml
services:
  pg:
    image: "docker.io/library/postgres:17.2"
    environment:
      POSTGRES_HOST_AUTH_METHOD: "scram-sha-256"
      POSTGRES_INITDB_ARGS: "--auth-host=scram-sha-256 --auth-local=scram-sha-256"
      POSTGRES_USER: "postgres"
      POSTGRES_PASSWORD: "${PG_PASSWORD}"
    ports:
      - "8000:5432"
    networks:
      - "aspire"
  dbsetup:
    image: "${DBSETUP_IMAGE}"
    environment:
      OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES: "true"
      OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES: "true"
      OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY: "in_memory"
      ASPNETCORE_FORWARDEDHEADERS_ENABLED: "true"
      HTTP_PORTS: "8001"
      ConnectionStrings__db: "Host=pg;Port=5432;Username=postgres;Password=${PG_PASSWORD};Database=db"
    ports:
      - "8002:8001"
      - "8004:8003"
    depends_on:
      pg:
        condition: "service_started"
    networks:
      - "aspire"
```

Key points:

- `${PG_PASSWORD}` and `${DBSETUP_IMAGE}` (and similar) are *placeholders* in the published asset.
- They are not resolved during `aspire publish`.
- A deployment engine (which could be `aspire deploy`, `docker compose` plus a script exporting variables, CI/CD variable injection, etc.) supplies their values later.
- This keeps secrets and environment-specific values decoupled from generated structure.

Different integrations may use different placeholder conventions (environment variables, tokens, or parameter metadata), but the principle remains: publish preserves *shape*, deploy injects *values*.

## Publisher model and compute environments

.NET Aspire uses a flexible publisher model that distributes publishing behavior across your application graph rather than relying on a single top-level publisher. Instead of selecting a target environment by calling methods like `AddDockerComposePublisher()`, Aspire includes a built-in publisher that looks for a `PublishingCallbackAnnotation` on each resource. This annotation describes how that resource should be published—for example, as a Docker Compose service, Kubernetes manifest, or Azure Bicep module.

This architectural shift enables hybrid and heterogeneous deployments, where different services within the same app can be deployed to different targets (cloud, edge, local).

### Most apps only need one environment

In typical apps, you only need to add a single compute environment, such as:

```csharp
builder.AddAzureContainerAppEnvironment("env");
```

Aspire applies the correct publishing behavior to all compute resources in your app model—no extra configuration needed.

### Multiple environments require disambiguation

If you add multiple compute environments, Aspire needs to know which resource goes where. Compute environments apply their transformations to all applicable compute resources (projects, containers, executables). If more than one environment matches a given resource, Aspire throws an ambiguous environment exception at publish time.

You can resolve this by using `WithComputeEnvironment(...)`:

```csharp
var k8s = builder.AddKubernetesEnvironment("k8s-env");
var compose = builder.AddDockerComposeEnvironment("docker-env");

builder.AddProject<Projects.Frontend>("frontend")
    .WithComputeEnvironment(k8s);

builder.AddProject<Projects.Backend>("backend")
    .WithComputeEnvironment(compose);
```

This example shows how you could explicitly map services to different compute targets—modeling, for example, a frontend in Kubernetes and a backend in Docker Compose.

### Supported compute environments

.NET Aspire has preview support for the following environment resources:

- `AddDockerComposeEnvironment(...)`
- `AddKubernetesEnvironment(...)`
- `AddAzureContainerAppEnvironment(...)`
- `AddAzureAppServiceEnvironment(...)`

These represent deployment targets that can transform and emit infrastructure-specific artifacts from your app model.

## Hosting integration support matrix

| Integration Package | Target | Publish | Deploy | Notes |
|---------------------|--------|---------|--------|-------|
| `Aspire.Hosting.Docker` | Docker / Docker Compose | Yes | No | Use generated Compose with your own scripts or tooling. |
| `Aspire.Hosting.Kubernetes` | Kubernetes | Yes | No | Apply with `kubectl`, GitOps, or other controllers. |
| `Aspire.Hosting.Azure` | Azure (Container Apps / App Service / related Azure resources) | Yes | Yes (Preview) | Deploy capability is in Preview and may change. |

> Deploy support is integration-specific. Absence of deploy support means you use the published artifacts with external tooling.

## Typical workflows

### 1. Generate artifacts (any integration)

```bash
aspire publish -o artifacts/
```

Review the contents of `artifacts/` (for example: Docker Compose files, Kubernetes manifests, Azure specification documents, etc.).

### 2. Run locally (Docker example)

```bash
# Provide or export required environment variables, then:
docker compose -f artifacts/docker-compose.yml up --build
```

Missing variables like `PG_PASSWORD` must be set in the shell, an `.env` file, or injected by your chosen runner.

### 3. CI/CD pipeline pattern

1. Run `aspire publish -o artifacts/` in a build job.
2. Archive or inspect artifacts (linting, security scanning, image signing).
3. Inject parameter values (environment variables, secrets store, variable groups).
4. Execute deployment:
   - Docker: `docker compose up -d`
   - Kubernetes: `kubectl apply -f artifacts/`
   - Azure (if using deploy-capable integration): `aspire deploy` (invoked in a suitable context)
   - Custom: your own script or orchestrator.

### 4. Using `aspire deploy`

If an integration supports deployment, you can run:

```bash
aspire deploy
```

(Only use documented flags. Avoid assuming support for flags not present in current tooling.)

This performs build + parameter resolution + application of changes for that integration's targets.

## Extensibility

The system is designed so you can author new integrations that participate in:

- Publish phase: generating artifacts for a target platform (for example, a service mesh config, an infrastructure-as-code definition, or a proprietary orchestrator spec).
- Deploy phase: resolving parameters and applying those artifacts (if your integration can perform or coordinate deployment actions).

A custom integration can:

- Contribute resource translators (map application model elements to platform units).
- Provide parameter mapping strategies (for example: infer required variables; annotate defaults).
- Emit diagnostic metadata (for tooling or UX).
- Implement deployment execution (apply/update/delete, health probing, diffing).

Because published assets are parameterized, integrators can cleanly plug in secret/value resolution at the final step without modifying the structural generation logic.

## Diagnostics & auditing

Publishing gives you an immutable snapshot of intended structure before secrets appear. You can:

- Diff published outputs between commits.
- Scan for disallowed images or configuration.
- Preserve a record for compliance, then separately record the resolved set applied at deployment time.

## Additional tools

### Azure Developer CLI (`azd`)

[Azure Developer CLI (azd)](https://learn.microsoft.com/azure/developer/azure-developer-cli/) can provision infrastructure, manage environments, and coordinate secret/value injection. You can incorporate Aspire publish artifacts into `azd` workflows or use the Azure integration (preview) directly.

### Aspir8 (Kubernetes YAML generation)

[Aspir8 (Aspirate)](https://prom3theu5.github.io/aspirational) remains a community tool that can transform an Aspire application model into Kubernetes manifests. While the Kubernetes hosting integration covers publish generation, Aspir8 may offer additional transforms or workflow preferences.

## Legacy deployment manifest (footnote)

Earlier workflows emphasized a single "deployment manifest" generated from specialized AppHost targets. The modern approach centers on `aspire publish` + integration extensibility. The legacy manifest format is **not being evolved further**, but you can still generate it for inspection or debugging:

```bash
aspire publish --publisher manifest -o diagnostics/
```

This:

- Produces a manifest snapshot useful for understanding resource graphs or troubleshooting.
- Should not be treated as the primary deployment contract.
- Is provided solely for backward compatibility and debugging visibility.

## Key takeaways

- Publish first, then deploy (separation of structure and values).
- Artifacts are parameterized; resolution happens later.
- Integrations define actual publish/deploy behaviors.
- The system is extensible—build your own integration to target new platforms or internal tooling.
- The legacy manifest can still be generated, but it's static and not evolving.

## See also

- Hosting integration reference packages (`Aspire.Hosting.*`)
- Observability and telemetry guidance
- Secret and parameter handling recommendations
- Azure deployment (preview) notes
- Community tooling (Aspir8)

---
*Design for extensibility: treat publish output as an invariant structural contract and keep environment-specific values out until deployment time.*
