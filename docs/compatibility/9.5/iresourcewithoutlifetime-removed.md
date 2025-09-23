---
title: "Breaking change - IResourceWithoutLifetime removed from parameter and connection string resources"
description: "Learn about the breaking change in .NET Aspire 9.5 where ParameterResource and ConnectionStringResource now participate in lifetimes and waiting semantics instead of implementing IResourceWithoutLifetime."
ms.date: 09/23/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/4734
---

# IResourceWithoutLifetime removed from parameter and connection string resources

In .NET Aspire 9.5, `ParameterResource` and `ConnectionStringResource` no longer behave as lifetime-less resources. The marker interface (or equivalent classification) `IResourceWithoutLifetime` is removed from these resource types. They now support waiting on readiness, expose a `Running` state, and share a simplified, consolidated initialization path.

## Version introduced

.NET Aspire 9.5

## Previous behavior

- `ParameterResource` instances were treated as static configuration and not *awaitable*.
- `ConnectionStringResource` used bespoke logic (special-case `WaitFor`) to produce its value.
- Neither resource reported a standard `Running` lifecycle state (parameters were effectively always available, connection strings had ad-hoc readiness semantics).

Example (parameter assumed immediately usable):

```csharp
var parameter = appBuilder.AddParameter("SubscriptionId", subscriptionId);
// Code assumed value available; no awaiting of readiness possible / required.
```

## New behavior

- `ParameterResource` participates in the standard readiness / waiting pipeline.
- `ConnectionStringResource` initialization and wait logic is unified and simplified.
- Both resources now surface a `Running` state consistent with other resources.
- Code might explicitly wait (or the hosting model might schedule waits) before consumption.

Revised usage (conceptual):

```csharp
var parameter = appBuilder.AddParameter("SubscriptionId", subscriptionId);
await parameter.ReadyAsync(); // Now possible (pattern representative; actual API name may differ)
```

## Type of breaking change

This is a [behavioral](../categories.md#behavioral-change) and [binary compatibility](../categories.md#binary-compatibility) change.

## Reason for change

Parameters and connection strings can exhibit runtime mutability or sequencing requirements (for example, depending on provisioning or environment preparation). Treating them as lifetime-less prevented consistent orchestration and forced special-case logic. Unifying their lifecycle improves reliability, removes duplicated wait code, and clarifies status reporting.

## Recommended action

1. Audit code that assumes immediate availability of parameter or connection string values; add explicit waits if the new lifecycle requires readiness before use.
1. Remove any workarounds or custom `WaitFor` logic targeting `ConnectionStringResource`.
1. If you reflected over `IResourceWithoutLifetime` to branch behavior, update that logic to use explicit capability checks (for example, a readiness interface) or rely on the common lifecycle abstractions.
1. Adjust tests: introduce a readiness wait hook instead of assuming synchronous availability.

Migration guidance (illustrative):

```diff
- var conn = appBuilder.AddConnectionString("Database", valueFactory);
- UseConnection(conn.Value); // Implicitly assumed available
+ var conn = appBuilder.AddConnectionString("Database", valueFactory);
+ await conn.ReadyAsync(); // Ensure generated / resolved
+ UseConnection(conn.Value);
```

## Affected APIs

- <xref:Aspire.Hosting.ApplicationModel.ParameterResource>
- <xref:Aspire.Hosting.ApplicationModel.ConnectionStringResource?displayProperty=fullName>
- <xref:Aspire.Hosting.ApplicationModel.IResourceWithoutLifetime>
