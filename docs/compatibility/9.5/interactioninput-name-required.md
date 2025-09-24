---
title: "Breaking change - InteractionInput.Name required and Label optional"
description: "Learn about the breaking change in .NET Aspire 9.5 where InteractionInput now requires Name, makes Label optional, and introduces EffectiveLabel."
ms.date: 09/23/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/4738
---

# `InteractionInput.Name` required and `Label` optional

In .NET Aspire 9.5, the `InteractionInput` API changes its contract: `Name` is now required and `Label` becomes optional. Automatic generation of `Name` from `Label` is removed, and a new `EffectiveLabel` member returns `Label` when specified, otherwise `Name`. This simplifies reasoning about inputs and avoids hidden inference.

## Version introduced

.NET Aspire 9.5

## Previous behavior

- `Name` optional; when omitted it was implicitly generated from the required `Label`.
- `Label` required.
- No `EffectiveLabel` convenience member existed.

Example (worked previously without an explicit `Name`):

```csharp
var inputs = new InteractionInputCollection
{
    new InteractionInput(label: "Subscription ID", value: subscriptionId)
};
```

The runtime inferred a `Name` (for example a normalized token from `"Subscription ID"`).

## New behavior

- `Name` is required and must be supplied explicitly.
- `Label` is optional (use it for a friendly display string distinct from `Name`).
- `EffectiveLabel` surfaces `Label` if present, else falls back to `Name`.
- No implicit generation of `Name` occurs.

Updated example:

```csharp
var inputs = new InteractionInputCollection
{
    new InteractionInput(name: "SubscriptionId", label: "Subscription ID", value: subscriptionId),
    new InteractionInput(name: "Region", value: region) // Label omitted; EffectiveLabel == "Region"
};

foreach (var input in inputs)
{
    Console.WriteLine($"Display: {input.EffectiveLabel} (Name: {input.Name})");
}
```

## Type of breaking change

This is a [source compatibility](../categories.md#source-compatibility) and [binary compatibility](../categories.md#binary-compatibility) change.

## Reason for change

The implicit derivation of `Name` from `Label` introduced ambiguity, made renames risky, and forced all callers to provide a `Label` even when a friendly display value was unnecessary. Making `Name` explicit improves clarity, stability of identifiers, and reduces accidental coupling between display text and programmatic keys. Adding `EffectiveLabel` preserves convenient display semantics without requiring callers to duplicate values.

## Recommended action

1. Add an explicit `name:` argument to every `InteractionInput` instantiation that previously omitted it.
1. Remove any unneeded `label:` arguments when the value would simply duplicate `name:`.
1. If your code depended on the inferred naming convention, replicate the previous transformation manually (for example, remove spaces or punctuation) when choosing the new explicit `Name`.
1. Use `EffectiveLabel` in UI / logging code that previously assumed `Label` was always present.

Migration example (before -> after):

```diff
- new InteractionInput(label: "Resource Group", value: rg)
+ new InteractionInput(name: "ResourceGroup", label: "Resource Group", value: rg)

- new InteractionInput(label: "Region", value: region)
+ new InteractionInput(name: "Region", value: region) // Label not needed
```

## Affected APIs

- <xref:Aspire.Hosting.InteractionInput?displayProperty=fullName>
- `InteractionInput.Name`
- <xref:Aspire.Hosting.InteractionInput.Label?displayProperty=fullName>
