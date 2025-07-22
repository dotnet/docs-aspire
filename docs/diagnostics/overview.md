---
title: .NET Aspire diagnostics overview
description: Learn about the diagnostics tools and features available in .NET Aspire.
ms.topic: overview
ms.date: 07/22/2025
---

# .NET Aspire diagnostics list

The following table lists the possible MSBuild and .NET Analyzer warnings and errors you might encounter with .NET Aspire:

| Diagnostic ID | Type | Description |
|--|--|--|
| [`ASPIRE001`](aspire001.md) | Warning | <span id="ASPIRE001"></span> The code language isn't fully supported by Aspire, some code generation targets will not run. |
| [`ASPIRE002`](aspire002.md) | Warning | <span id="ASPIRE002"></span> Project is an Aspire AppHost project but necessary dependencies aren't present. Are you missing an Aspire.Hosting.AppHost PackageReference? |
| [`ASPIRE003`](aspire003.md) | Warning | <span id="ASPIRE003"></span> 'Project' is a .NET Aspire AppHost project that requires Visual Studio version 17.10 or above to work correctly. |
| [`ASPIRE004`](aspire004.md) | Warning | <span id="ASPIRE004"></span> 'Project' is referenced by an Aspire Host project, but it is not an executable. |
| [`ASPIRE006`](aspire006.md) | (Experimental) Error | <span id="ASPIRE006"></span> Application model items must have valid names. |
| [`ASPIRE007`](aspire007.md) | Error | <span id="ASPIRE007"></span> 'Project' requires a reference to "Aspire.AppHost.Sdk" with version "9.0.0" or greater to work correctly. |
| [`ASPIRE008`](aspire008.md) | Error | <span id="ASPIRE008"></span> The Aspire workload that this project depends on is now deprecated. |
| [`ASPIREACADOMAINS001`](aspireacadomains001.md) | (Experimental) Error | <span id="ASPIREACADOMAINS001"></span> `ConfigureCustomDomain` is for evaluation purposes only and is subject to change or removal in future updates. |
| [`ASPIREAZURE001`](aspireazure001.md) | (Experimental) Error | <span id="ASPIREAZURE001"></span> Publishers are for evaluation purposes only and are subject to change or removal in future updates. |
| [`ASPIRECOMPUTE001`](aspirecompute001.md) | (Experimental) Error | <span id="ASPIRECOMPUTE001"></span> Compute related types and members are for evaluation purposes only and is subject to change or removal in future updates. |
| [`ASPIRECOSMOSDB001`](aspirecosmosdb001.md) | (Experimental) Error | <span id="ASPIRECOSMOSDB001"></span> `RunAsPreviewEmulator` is for evaluation purposes only and is subject to change or removal in future updates. |
| [`ASPIREHOSTINGPYTHON001`](aspirehostingpython001.md) | (Experimental) Error | <span id="ASPIREHOSTINGPYTHON001"></span> `AddPythonApp` is for evaluation purposes only and is subject to change or removal in future updates. |
| [`ASPIREPROXYENDPOINTS001`](aspireproxyendpoints001.md) | (Experimental) Error | <span id="ASPIREPROXYENDPOINTS001"></span> ProxyEndpoint members are for evaluation purposes only and are subject to change or removal in future updates. |
| [`ASPIREPUBLISHERS001`](aspirepublishers001.md) | Error | <span id="ASPIREPUBLISHERS001"></span> Publishers are for evaluation purposes only and are subject to change or removal in future updates. |
