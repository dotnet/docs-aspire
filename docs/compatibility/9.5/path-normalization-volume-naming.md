---
title: "Breaking change - Comprehensive path normalization for volume naming"
description: "Learn about the breaking change in .NET Aspire 9.5 where path normalization and (on Windows) lowercasing produce consistent volume names."
ms.date: 09/23/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/4740
---

# Comprehensive path normalization for volume naming

In .NET Aspire 9.5, all file system paths used for deriving container volume names are first normalized with `Path.GetFullPath()`. On Windows, the normalized path is also lowercased. This ensures consistent deterministic hashing and volume naming regardless of input casing differences or whether a path was relative or absolute. The change affects scenarios where you previously relied on the incidental differences in volume names when launching with different working directories, path spellings, or letter casing.

## Version introduced

.NET Aspire 9.5

## Previous behavior

Paths that were equivalent at the file system level but differed in representation often produced different volume hashes. Examples included:

- A relative form versus its absolute equivalent (for example, `..\\src\\data` versus `C:\\repo\\src\\data`) produced different hashes.
- Letter casing differences on Windows (for example, `C:\\Data` versus `c:\\data`) produced different hashes.
- Redundant navigation segments (for example, `C:\\data\\..\\data`) produced a different hash than the simplified path.

As a result, logically identical paths could map to distinct volumes.

- A container might receive differently named volumes between `dotnet run` and Visual Studio <kbd>F5</kbd>.
- Persisted data could appear to reset because a new volume name was generated.
- Custom code that attempted to predict volume names by hashing raw input strings could behave inconsistently.

Example (pseudo-code illustrating the conceptual difference):

```csharp
// Suppose this path was supplied differently across runs
var path = args[0]; // "..\\src\\data" (run A) vs. "C:\\repo\\src\\data" (run B)

// Previous behavior: differing raw strings produced different volume names.
var volumeName = Hash(path); // Non-deterministic across equivalent paths
```

## New behavior

All paths are normalized first:

- `Path.GetFullPath(path)` is applied.
- On Windows, the full path is converted to lower-case (ordinal invariant) before hashing.
- Equivalent paths now yield identical volume names across runs and tooling contexts.

Example:

```csharp
var input = args[0];
var normalized = Path.GetFullPath(input);
#if WINDOWS
normalized = normalized.ToLowerInvariant();
#endif
var volumeName = Hash(normalized); // Stable and deterministic
```

## Type of breaking change

This is a [behavioral](../../categories.md#behavioral-change) change.

## Reason for change

The prior approach produced nondeterministic volume naming across environments and launch methods, leading to surprise data duplication or loss and complicating reproducibility. Normalization eliminates casing- and representation-based divergence, making volume mapping predictable and resilient.

## Recommended action

1. Remove any workarounds that intentionally varied path casing or formatting to isolate data sets; use explicit distinct directories instead.
1. If you relied on differing volume names to trigger data resets, explicitly clear or recreate volumes instead of depending on path representation differences.
1. If you precomputed expected volume names in custom tooling by hashing raw input paths, update that logic to normalize exactly as Aspire now does (full path + lowercase on Windows) before hashing.
1. Audit any persisted development data folders: if multiple variant volume directories were created previously, consolidate or remove stale ones.

To align custom code with the new logic:

```csharp
string NormalizeForVolume(string path)
{
    var full = Path.GetFullPath(path);

    if (OperatingSystem.IsWindows())
    {
        full = full.ToLowerInvariant();
    }

    return full; // Pass this into the hashing / naming routine
}
```

## Affected APIs

None.
