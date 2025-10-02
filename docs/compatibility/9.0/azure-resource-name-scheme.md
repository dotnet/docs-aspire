---
title: "Better Azure resource name scheme"
description: This document outlines the breaking changes introduced in Aspire 9.0 GA regarding the Azure resource naming scheme.
ms.date: 10/28/2024
ai-usage: ai-assisted
---

# Better Azure resource name scheme

In Aspire 9.0 GA, the Azure resource naming scheme has been updated to a more robust and flexible system. This change addresses issues with the previous naming scheme, which caused problems such as name truncation and invalid deployments.

## Version introduced

Aspire 9.0 GA

## Previous behavior

The previous version used an early/alpha version of `Azure.Provisioning`, which employed a naming scheme that attempted to be the least common denominator of all resources. This often resulted in truncated or invalid names.

```csharp
protected string GetGloballyUniqueName(string resourceName)
    => $"toLower(take('{resourceName}${{uniqueString(resourceGroup().id)}}', 24))";
```

## New behavior

The new version of `Azure.Provisioning` uses a more sophisticated naming scheme that considers the specific requirements of each resource type, such as maximum length and valid characters.

```csharp
public override BicepValue<string>? ResolveName(
    ProvisioningContext context,
    Resource resource,
    ResourceNameRequirements requirements)
{
    string prefix = SanitizeText(
        resource.ResourceName, requirements.ValidCharacters);

    string separator =
        requirements.ValidCharacters.HasFlag(ResourceNameCharacters.Hyphen) ? "-" :
        requirements.ValidCharacters.HasFlag(ResourceNameCharacters.Underscore) ? "_" :
        requirements.ValidCharacters.HasFlag(ResourceNameCharacters.Period) ? "." :
        "";

    BicepValue<string> suffix = GetUniqueSuffix(context, resource);

    return BicepFunction.Take(
        BicepFunction.Interpolate(
            $"{prefix}{separator}{suffix}"), requirements.MaxLength);
}
```

## Type of breaking change

This change is a [behavioral change](../categories.md#behavioral-change).

## Recommended action

Users who want to maintain the old naming scheme can customize the Azure CDK `ProvisioningContext` object. This can be done by configuring the `AzureProvisioningOptions` class and inserting the `AzureResourceNamePropertyResolverAspireV8` resolver.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.Services.Configure<AzureProvisioningOptions>(options =>
{
    options.ProvisioningBuildOptions.InfrastructureResolvers.Insert(0, new AspireV8ResourceNamePropertyResolver());
});
```

## Affected APIs

None.
