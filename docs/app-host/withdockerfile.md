---
title: Add Dockerfiles to your .NET app model
description: Learn how to add Dockerfiles to your .NET app model.
ms.date: 07/19/2024
---

# Add Dockerfiles to your .NET app model

<!--
https://github.com/dotnet/docs-aspire/issues/1063

We are adding a new extension method called WithDockerfile which allows .NET Aspire to dynamically build a container image from a Dockerfile and run it.

Article abstract
Needs to cover these things:

Basics of getting it working.
Using build arguments
Using build secrets
Using to augment existing custom container resources (e.g. AddSqlServer(...)).
Troubleshooting (example of images not being rebuilt)
-->