---
title: "Breaking change - Launch profiles now ignore unsupported commandName values"
description: "Learn about the breaking change in Aspire 9.5 where only Project and Executable launch profiles are considered by default and unsupported profiles (for example IISExpress) are skipped."
ms.date: 09/23/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/4739
---

# Launch profiles now ignore unsupported commandName values

In Aspire 9.5, launch profile selection filters `launchSettings.json` profiles by an allow list of supported `commandName` values (`Project` and `Executable`). Unsupported profiles such as `IISExpress`, `Docker`, or others are ignored unless explicitly selected. Previously, the first profile in the file could be chosen even if its `commandName` wasn't supported, leading to unexpected startup behavior.

## Version introduced

Aspire 9.5

## Previous behavior

The first profile defined in `launchSettings.json` was selected regardless of its `commandName`. If an `IISExpress` profile appeared first (common in older templates), Aspire would attempt to use it—even though it's not a supported hosting mode—causing incorrect assumptions or failures.

```json
{
  "profiles": {
    "IISExpress": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "environmentVariables": { "ASPNETCORE_ENVIRONMENT": "Development" }
    },
    "MyService": {
      "commandName": "Project",
      "applicationUrl": "http://localhost:5189"
    }
  }
}
```

In this example, `IISExpress` (unsupported) would have been chosen by virtue of ordering.

## New behavior

Profile selection now considers only supported `commandName` values by default:

- `Project`
- `Executable`

Unsupported profiles are skipped automatically unless you explicitly direct Aspire to use one (for example via custom tooling or explicit selection logic outside the default behavior). Case-insensitive matching is applied to the supported values.

```json
{
  "profiles": {
    "IISExpress": { "commandName": "IISExpress" },
    "MyService": { "commandName": "Project", "applicationUrl": "http://localhost:5189" }
  }
}
```

Result: `MyService` is selected; `IISExpress` is ignored.

## Type of breaking change

This is a [behavioral](../categories.md#behavioral-change) change.

## Reason for change

Filtering prevents accidental selection of unsupported or legacy launch profiles, improving predictability and reducing confusion when the environment differs between local runs and integrated tooling.

## Recommended action

1. Move any required supported profile (`Project` or `Executable`) into `launchSettings.json` if it doesn't already exist.
1. Remove obsolete `IISExpress` (or other unsupported) profiles if no longer needed.
1. If you previously relied on ordering (unsupported profile first), update your configuration to ensure a valid supported profile is available.
1. For custom automation that enumerates profiles, mirror the new filtering logic (allow only `Project` and `Executable` unless you intentionally opt into others).

If you must use a nonstandard profile for specialized tooling, ensure your custom selection code explicitly targets it rather than depending on default selection.

## Affected APIs

None.
