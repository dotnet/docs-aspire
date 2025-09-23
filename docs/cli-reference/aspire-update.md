---
title: aspire update command
description: Update Aspire integration packages to the latest versions for a chosen channel.
ms.topic: how-to
ms.date: 2025-09-23
---

# aspire update command (Preview)

**This article applies to:** ✔️ Aspire CLI 9.5.0 and later versions.

## Name

`aspire update` - Update Aspire integration packages to the latest versions for a chosen channel.

[!INCLUDE [mode-preview](includes/mode-preview.md)]

## Synopsis

```Command
aspire update [options]
```

## Description

The `aspire update` command updates Aspire integration packages (and related version declarations) in your existing AppHost-based solution to the latest versions published on a selected channel (for example, stable, preview, daily).

The command performs the following steps:

- Locates the AppHost project automatically (interactive failure if none found).
- Prompts you to select an Aspire channel (retrieved from configured feeds).
- Determines which Aspire packages (core + integration) are outdated for that channel.
- Updates versions in either:
  - Individual `<PackageReference>` items with inline `Version`
  - Central Package Management (`Directory.Packages.props`)
- Handles complex (diamond) dependency graphs to avoid conflicting updates.
- Skips updates when already at the latest channel-compatible version.

## Basic usage

```bash
# From the solution/AppHost directory (or a subfolder)
aspire update
```

You're prompted to choose a channel (for example, `stable`, `preview`, `daily`). After selection, package versions are adjusted accordingly.

## Typical flow

1. Run `aspire update`.
1. Select a channel when prompted.
1. The tool analyzes dependencies and applies updates.
1. Build or run (`aspire run`) to verify.

## Channel awareness

Channels map to different feeds/version streams:

- `stable`: Released, supported builds.
- `daily` (or `dev`): Latest *unsupported* builds (fast-moving).

Choose a channel that matches the stability needs of the branch you're working on.

## Usage notes and considerations

| Topic | Notes |
|-------|-------|
| Source control | Commit before running; makes rollback trivial (`git reset --hard`). |
| Single-file AppHost | A lone `.cs` "AppHost" (no project file) is not supported—convert to a proper `.csproj` AppHost. |
| Non-Aspire packages | The command focuses on Aspire-related packages; other dependencies remain unchanged unless implicitly required by dependency resolution. |
| Central Package Management | If `Directory.Packages.props` is present, changes are centralized there—review and commit that file. |
| Channel switching | Moving from `daily` to `stable` might *not* downgrade prerelease versions unless a strictly lower stable is recognized; verify manually if needed. |
| Build failures after update | Usually caused by breaking changes in preview/daily packages—revert via git or re-run selecting a more stable channel. |
| Multiple AppHosts | Run the command separately for each AppHost if your repo models multiple distributed applications. |
| Automation | No non-interactive `--channel` flag yet (consider scripting via expect-like tooling if needed). |
| Pre-release volatility | Daily/previews might introduce API shifts; prefer stable channel for production branches. |
| NuGet feeds | Ensure `nuget.config` includes the feeds for the channel you select (the `aspire new` command scaffolds this for you originally). |
| Partial updates | If nothing changes, you are already on the latest for that channel. |

## Example (Conceptual)

```bash
$ aspire update
Detected AppHost: MyApp.AppHost.csproj
Select channel: (Use arrows)
  stable
  preview
> daily
Updating Aspire.Hosting 9.4.2 -> 9.5.0-daily.20240923.1
...
Done.
```

> [!NOTE]
> Exact formatting and colors might differ.

## When to use

| Scenario | Use `aspire update`? | Rationale |
|----------|---------------------|-----------|
| Adopt latest stable fixes | Yes | Pulls newest stable package versions. |
| Trial new preview features | Yes (choose `preview`) | Evaluates upcoming changes with moderate churn. |
| Experiment with cutting edge | Yes (choose `daily`) | Fast iteration; expect breaking changes. |
| Update non-Aspire libs | Not directly | Use standard NuGet/package tooling separately. |

## FAQ

**Does it modify code?**  
Only package version metadata (and possibly associated channel configuration artifacts).

**Can I script it?**  
Interactive today; non-interactive flags would be a future enhancement.

**Will it update target frameworks (TFMs)?**  
No—framework upgrades are a manual decision.

**Can I downgrade?**  
Yes via channel selection, you might downgrade to a stable from a daily build.

## Options

The following options are available:

- [!INCLUDE [option-project](includes/option-project.md)]

- [!INCLUDE [option-help](includes/option-help.md)]

## See also

- [aspire new command](aspire-new.md)
- [aspire run command](aspire-run.md)
- [Upgrade to .NET Aspire 9.5.0](../get-started/upgrade-to-aspire-9.md)
