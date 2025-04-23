---
title: Compiler Error ASPIRE006
description: Learn more about compiler Error ASPIRE006. Application model items must have valid names.
ms.date: 04/21/2025
f1_keywords:
  - "ASPIRE006"
helpviewer_keywords:
  - "ASPIRE006"
---

# Compiler Error ASPIRE006

**Version introduced:** 8.2.2

> Application model items must have valid names.

This diagnostic error is reported when a resource's name is invalid, such as a name with consecutive hyphen (`-`) characters.

This error shouldn't be be suppressed, as invalid model names throw an exception at runtime.

## Example

The following code generates `ASPIRE006`:

```csharp
var bbsContainer = builder.AddContainer("bbs--server", "coldwall/mystic")
                          .WithEndpoint(19991, 23);
```

## To correct this Error

Use a valid name. For more information, see [Resource naming conventions](../fundamentals/orchestrate-resources.md#resource-naming-conventions).
