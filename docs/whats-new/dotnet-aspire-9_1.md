---
title: What's new in .NET Aspire 9.1
description: Learn what's new in the official general availability release of .NET Aspire 9.1.
ms.date: 2/18/2024
---

# What's new in .NET Aspire 9.1

📢 .NET Aspire 9.1 is the next minor version release of .NET Aspire; it supports _both_:

- .NET 8.0 Long Term Support (LTS) _or_
- .NET 9.0 Standard Term Support (STS).

> [!NOTE]
> You're able to use .NET Aspire 9.1 with either .NET 8 or .NET 9!

As always, we focused on highly requested features and pain points from the community. Our theme for 9.1 was "polish, polish, polish" - so you'll see quality of life fixes throughout the whole platform! Some highlights from this release are resource relationships in the dashboard, support for working in GitHub Codespaces, and publishing resources as a Dockerfile.

If you have feedback, questions, or want to contribute to .NET Aspire, collaborate with us on [:::image type="icon" source="../media/github-mark.svg" border="false"::: GitHub](https://github.com/dotnet/aspire) or join us on [:::image type="icon" source="../media/discord-icon.svg" border="false"::: Discord](https://discord.com/invite/h87kDAHQgJ) to chat with team members.

For more information on the official .NET version and .NET Aspire version support, see:

- [.NET support policy](https://dotnet.microsoft.com/platform/support/policy): Definitions for LTS and STS.
- [.NET Aspire support policy](https://dotnet.microsoft.com/platform/support/policy/aspire): Important unique product life cycle details.

## Upgrade to 9.1

Moving between minor releases of .NET Aspire is simple:

1. In your app host project file (ie, MyApp.AppHost.csproj), update the Aspire.AppHost.Sdk version to 9.1.0
`<Sdk Name="Aspire.AppHost.Sdk" Version="9.0.0" />`
1. Check for any NuGet package updates, either using the NuGet Package Manager in Visual Studio or the "Update NuGet Package" command in VS Code.
1. Update to the latest .NET Aspire templates by running `dotnet new update` in the command line.

If your project file doesn't have `Aspire.AppHost.Sdk`, you might still be using .NET Aspire 8. To upgrade to 9.1, you can follow [the documentation from last release](../get-started/upgrade-to-aspire-9).

## Dashboard UX and customization

### Resource relationships


### Papercuts


## App model and orchestration

## Deployment

## Integrations