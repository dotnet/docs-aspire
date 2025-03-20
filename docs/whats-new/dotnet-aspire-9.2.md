---
title: What's new in .NET Aspire 9.2
description: Learn what's new in the official general availability release of .NET Aspire 9.2.
ms.date: 03/20/2025
---

# What's new in .NET Aspire 9.2

📢 .NET Aspire 9.2 is the next minor version release of .NET Aspire; it supports _both_:

- .NET 8.0 Long Term Support (LTS) _or_
- .NET 9.0 Standard Term Support (STS).

> [!NOTE]
> You're able to use .NET Aspire 9.2 with either .NET 8 or .NET 9!

As always, we focused on highly requested features and pain points from the community. Our theme for 9.1 was "polish, polish, polish"—so you see quality of life fixes throughout the whole platform. Some highlights from this release are resource relationships in the dashboard, support for working in GitHub Codespaces, and publishing resources as a Dockerfile.

If you have feedback, questions, or want to contribute to .NET Aspire, collaborate with us on [:::image type="icon" source="../media/github-mark.svg" border="false"::: GitHub](https://github.com/dotnet/aspire) or join us on [:::image type="icon" source="../media/discord-icon.svg" border="false"::: Discord](https://discord.com/invite/h87kDAHQgJ) to chat with team members.

Whether you're new to .NET Aspire or have been with us since the preview, it's important to note that .NET Aspire releases out-of-band from .NET releases. While major versions of .NET Aspire align with .NET major versions, minor versions are released more frequently. For more details on .NET and .NET Aspire version support, see:

- [.NET support policy](https://dotnet.microsoft.com/platform/support/policy): Definitions for LTS and STS.
- [.NET Aspire support policy](https://dotnet.microsoft.com/platform/support/policy/aspire): Important unique product life cycle details.

## ⬆️ Upgrade to .NET Aspire 9.2

Moving between minor releases of .NET Aspire is simple:

1. In your app host project file (that is, _MyApp.AppHost.csproj_), update the [📦 Aspire.AppHost.Sdk](https://www.nuget.org/packages/Aspire.AppHost.Sdk) NuGet package to version `9.2.0`:

    ```xml
    <Project Sdk="Microsoft.NET.Sdk">

        <Sdk Name="Aspire.AppHost.Sdk" Version="9.2.0" />
        
        <!-- Omitted for brevity -->
    
    </Project>
    ```

    For more information, see [.NET Aspire SDK](xref:dotnet/aspire/sdk).

1. Check for any NuGet package updates, either using the NuGet Package Manager in Visual Studio or the **Update NuGet Package** command in VS Code.
1. Update to the latest [.NET Aspire templates](../fundamentals/aspire-sdk-templates.md) by running the following .NET command line:

    ```dotnetcli
    dotnet new update
    ```

    > [!NOTE]
    > The `dotnet new update` command updates all of your templates to the latest version.

If your app host project file doesn't have the `Aspire.AppHost.Sdk` reference, you might still be using .NET Aspire 8. To upgrade to 9.0, you can follow [the documentation from last release](../get-started/upgrade-to-aspire-9.md).

## TODO

More stuff...
