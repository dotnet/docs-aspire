---
title: Create custom .NET Aspire component
description: Learn how to create a custom resource for an existing containerized application.
ms.date: 07/12/2024
ms.topic: how-to
---

# Create custom .NET Aspire component

This article is a continution of the [Create custom resource types for .NET Aspire](custom-resources.md) article. In this article, you create a .NET Aspire component that relies on [MailKit](https://github.com/jstedfast/MailKit) to send emails and is used as a component in the Newsletter app you built.

## Prerequisites

If you're following along, you should have already completed the steps in the [Create custom resource types for .NET Aspire](custom-resources.md) article.

## Create library for component

[.NET Aspire components](../fundamentals/components-overview.md) are delivered as NuGet packages, but in this example, it's beyond the scope of this article to publish a NuGet package. Instead, you create a class library project that contains the component and reference it as a project.

1. Create a new class library project named `MailDev.Client` in the same solution as the `MailDevResource.NewsletterService` project.

    ```dotnetcli
    dotnet new classlib -o MailDev.Client
    ```

// TODO: Wip
