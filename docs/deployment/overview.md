---
title: Aspire publishing and deployment
description: Learn how Aspire solutions are published and deployed.
ms.topic: overview
ms.date: 09/26/2025
---

# Aspire publishing and deployment overview

Aspire separates the act of _producing deployment assets_ from _executing a deployment_. The Aspire CLI (`aspire`) provides two high‑level entrypoints:

- `aspire publish`: Generates intermediate, parameterized assets for one or more hosting integrations.
- `aspire deploy`: Executes a deployment (when an integration implements deploy semantics) by resolving parameters and applying changes to a target environment.

These commands provide direct access to publishing and deployment capabilities. The actual behavior (what gets generated, how deployment happens) comes from **hosting integrations** you reference (for example: Docker, Kubernetes, Azure). The system is **extensible**—you can build your own publishing or deployment integrations that plug into the same model.

The `aspire publish` command produces deployment artifacts that contain unresolved parameters (placeholders). The `aspire deploy` command uses these artifacts, resolves the parameters when supported by the target integration, and then executes the deployment. Some integrations don't support the `deploy` command.

## Aspire CLI commands (conceptual behavior)

| Command | What it does | Outputs | Parameter state | Requires integration capability |
|---------|--------------|---------|-----------------|---------------------------------|
| `aspire publish` | Transforms the application model into integration-specific assets (Compose files, manifests, specifications, etc.). | Intermediate artifacts (not directly production-final). | Unresolved (placeholders, e.g. `${VAR}` or similar). | Publish support |
| `aspire deploy` | Runs a deployment using one or more integrations (build, parameter resolution, apply). | Real resources / applied changes. | Resolved. | Deploy support |

If an integration does not implement deploy functionality, `aspire deploy` will not deploy that target (it may warn or no-op for it).

When you run `aspire publish` without any integrations that support publishing, you'll see:

```Output
Analyzing the distributed application model for publishing and deployment capabilities. 00:00:00
           No resources in the distributed application model support publishing.
```

Similarly, when you run `aspire deploy` without any integrations that support deployment, you'll see this error:

```Output
FAILED: Analyzing the distributed application model for publishing and deployment capabilities.
           No resources in the distributed application model support deployment.
```

These messages indicate that you need to add hosting integrations to your AppHost project. Hosting integrations are NuGet packages (like `Aspire.Hosting.Docker`, `Aspire.Hosting.Kubernetes`, or `Aspire.Hosting.Azure`) that provide the publishing and deployment capabilities for specific target platforms.

## Parameter placeholders

Published assets intentionally contain placeholders instead of concrete values. For Docker Compose–based publish output, parameterization appears as standard environment variable references. For example, a publish artifact might include:

```yaml
services:
  pg:
    image: "docker.io/library/postgres:17.6"
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

Aspire uses a flexible publisher model that distributes publishing behavior across your application graph. Resources support publishing and deployment through annotations:

- `aspire publish`
- `aspire deploy`

This design enables hybrid and heterogeneous deployments, where different services within the same app can be deployed to different targets (cloud, edge, local).

### Compute environments

A **compute environment** is a core deployment concept in Aspire that represents a target platform where your application resources will be deployed. Compute environments define how resources should be transformed and what deployment artifacts should be generated.

**Compute resources** are the runnable parts of your application, such as .NET projects, containers, and executables that need to be deployed to a compute environment.

When you add a compute environment like Docker Compose or Kubernetes, Aspire applies the correct publishing behavior to all compatible compute resources in your app model—no extra configuration needed.

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

This example shows how you could explicitly map services to different compute environments. For example, a frontend in Kubernetes and a backend in Docker Compose.

## Hosting integration support matrix

| Integration Package | Target | Publish | Deploy | Notes |
|---------------------|--------|---------|--------|-------|
| `Aspire.Hosting.Docker` | Docker / Docker Compose | ✅ Yes | ❌ No | Use generated Compose with your own scripts or tooling. |
| `Aspire.Hosting.Kubernetes` | Kubernetes | ✅ Yes | ❌ No | Apply with `kubectl`, GitOps, or other controllers. |
| `Aspire.Hosting.Azure.AppContainers` | Azure Container Apps | ✅ Yes | ✅ Yes (Preview) | Deploy capability is in Preview and may change. |
| `Aspire.Hosting.Azure.AppService` | Azure App Service | ✅ Yes | ✅ Yes (Preview) | Deploy capability is in Preview and may change. |

> [!TIP]
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

### 3. Using `aspire deploy`

If an integration supports deployment, you can run:

```bash
aspire deploy
```

This resolves parameters and applies deployment changes for integrations that support deployment.

## Extensibility

The `aspire publish` and `aspire deploy` commands support extensible workflows through annotations that you can add to resources. This functionality is in preview and may change in future releases.

### Custom publishing and deployment callbacks

Resources support custom publishing and deployment behavior through annotations:

- **`PublishingCallbackAnnotation`**: Executes custom logic during `aspire publish` operations
- **`DeployingCallbackAnnotation`**: Executes custom logic during `aspire deploy` operations

The following example demonstrates using `DeployingCallbackAnnotation` to register custom deployment behavior:

```csharp
#pragma warning disable ASPIREPUBLISHERS001
#pragma warning disable ASPIREINTERACTION001

using Aspire.Hosting.Publishing;
using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

// Custom deployment step defined below
builder.AddDataSeedJob("SeedInitialData", seedDataPath: "data/seeds");

builder.Build().Run();

internal class DataSeedJobResource(string name, string seedDataPath)
    : Resource(name)
{
    public string SeedDataPath { get; } = seedDataPath;
}

internal static class DataSeedJobResourceBuilderExtensions
{
    public static IResourceBuilder<DataSeedJobResource> AddDataSeedJob(
        this IDistributedApplicationBuilder builder,
        string name,
        string seedDataPath = "data/seeds")
    {
        var job = new DataSeedJobResource(name, seedDataPath);
        var resourceBuilder = builder.AddResource(job);

        // Attach a DeployingCallbackAnnotation that will be invoked on `aspire deploy`
        job.Annotations.Add(new DeployingCallbackAnnotation(async ctx =>
        {
            CancellationToken ct = ctx.CancellationToken;

            // Prompt the user for a confirmation using the interaction service
            var interactionService = ctx.Services.GetRequiredService<IInteractionService>();

            var envResult = await interactionService.PromptInputAsync(
                "Environment Configuration",
                "Please enter the target environment name:",
                new InteractionInput
                {
                    Label = "Environment Name",
                    InputType = InputType.Text,
                    Required = true,
                    Placeholder = "dev, staging, prod"
                },
                cancellationToken: ct);

            // Custom deployment logic here
            var reporter = ctx.ActivityReporter;
            await using (var deployStep = await reporter.CreateStepAsync(
                $"Deploying data seed job to {envResult.Value}", ct))
            {
                // Simulate deployment work
                await Task.Delay(2000, ct);
                await deployStep.SucceedAsync("Data seed job deployed successfully", ct);
            }
        }));

        return resourceBuilder;
    }
}
```

This custom deployment logic integrates seamlessly with the `aspire deploy` command, providing interactive prompts and progress reporting.

## Diagnostics & auditing

Publishing gives you an immutable snapshot of intended structure before secrets appear. You can:

- Diff published outputs between commits.
- Scan for disallowed images or configuration.
- Preserve a record for compliance, then separately record the resolved set applied at deployment time.

## Additional tools

### Azure Developer CLI (`azd`)

[Azure Developer CLI (azd)](https://learn.microsoft.com/azure/developer/azure-developer-cli/) has first-class support for deploying Aspire projects. It can provision infrastructure, manage environments, and coordinate secret/value injection. You can incorporate Aspire publish artifacts into `azd` workflows or use the Azure integration (preview) directly.

### Aspir8 (Kubernetes YAML generation)

<a href="https://prom3theu5.github.io/aspirational">Aspir8 (Aspirate)</a> is a community tool that can transform an Aspire application model into Kubernetes manifests. While the Kubernetes hosting integration covers publish generation, Aspir8 may offer additional transforms or workflow preferences.

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

- [Hosting integrations overview](../fundamentals/integrations-overview.md)
- [Azure deployment with Container Apps](azure/aca-deployment.md)
- [Aspire CLI reference](../cli-reference/overview.md)
- [Community tooling (Aspir8)](https://prom3theu5.github.io/aspirational)

---
*Design for extensibility: treat publish output as an invariant structural contract and keep environment-specific values out until deployment time.*
