---
title: Testing .NET Aspire apps
description: Learn how to test your .NET Aspire apps using the xUnit testing framework.
ms.date: 04/22/2024
---

# Testing .NET Aspire apps

.NET Aspire include an [xUnit testing project template](setup-tooling.md#net-aspire-project-templates) that you can use to test your .NET Aspire apps. The testing project template is based on the xUnit testing framework and includes a sample test that you can use as a starting point for your tests. In this article, you'll learn how to create a test project, write, and run tests for your .NET Aspire apps.

## Create a test project

The easiest way to create a .NET Aspire test project is to use the testing project template. If you're starting a new .NET Aspire app and want to include test projects, the [Visual Studio tooling supports that option](setup-tooling.md#create-test-project). If you're adding a test project to an existing .NET Aspire app, you can use the `dotnet new` command to create a test project:

```dotnetcli
dotnet new aspire-xunit
```

## Explore the test project

The .NET Aspire test project takes a project reference dependency on the target app host. Consider the template project:

:::code language="xml" source="snippets/testing/AspireApp1/AspireApp1.Tests/AspireApp1.Tests.csproj":::

The preceding project file is fairly standard. There's a `PackageReference` to the [Aspire.Hosting.Testing](https://www.nuget.org/packages/Aspire.Hosting.Testing) NuGet package, which includes the required types to write tests for .NET Aspire apps.

The template test project includes a `WebTests` class with a single test fact. The test fact verifies the following scenario:

- The app host is successfully created and started.
- An HTTP request can be made to the `webfrontend` resource and returns a successful response.

Consider the following test class:

:::code language="csharp" source="snippets/testing/AspireApp1/AspireApp1.Tests/WebTests.cs":::
