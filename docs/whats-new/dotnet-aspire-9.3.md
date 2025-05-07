---
title: What's new in .NET Aspire 9.3
description: Learn what's new in the official general availability release of .NET Aspire 9.3.
ms.date: 05/07/2025
---

# What's new in .NET Aspire 9.3

📢 .NET Aspire 9.3 is the next minor version release of .NET Aspire; it supports:

- .NET 8.0 Long Term Support (LTS)
- .NET 9.0 Standard Term Support (STS)

If you have feedback, questions, or want to contribute to .NET Aspire, collaborate with us on [:::image type="icon" source="../media/github-mark.svg" border="false"::: GitHub](https://github.com/dotnet/aspire) or join us on [:::image type="icon" source="../media/discord-icon.svg" border="false"::: Discord](https://discord.com/invite/h87kDAHQgJ) to chat with team members.

It's important to note that .NET Aspire releases out-of-band from .NET releases. While major versions of .NET Aspire align with .NET major versions, minor versions are released more frequently. For more information on .NET and .NET Aspire version support, see:

- [.NET support policy](https://dotnet.microsoft.com/platform/support/policy): Definitions for LTS and STS.
- [.NET Aspire support policy](https://dotnet.microsoft.com/platform/support/policy/aspire): Important unique product life cycle details.

## ⬆️ Upgrade to .NET Aspire 9.3

> [!IMPORTANT]
> If you are using `azd` to deploy Azure PostgreSQL or Azure SQL Server, you now have to configure Azure Managed Identities. For more information, see [🛡️ Improved Managed Identity defaults](#improved-managed-identity-defaults).

Moving between minor releases of .NET Aspire is simple:

1. In your app host project file (that is, _MyApp.AppHost.csproj_), update the [📦 Aspire.AppHost.Sdk](https://www.nuget.org/packages/Aspire.AppHost.Sdk) NuGet package to version `9.3.0`:

    ```diff
    <Project Sdk="Microsoft.NET.Sdk">

        <Sdk Name="Aspire.AppHost.Sdk" Version="9.3.0" />
        
        <PropertyGroup>
            <OutputType>Exe</OutputType>
            <TargetFramework>net9.0</TargetFramework>
    -       <IsAspireHost>true</IsAspireHost>
            <!-- Omitted for brevity -->
        </PropertyGroup>
        
        <ItemGroup>
            <PackageReference Include="Aspire.Hosting.AppHost" Version="9.3.0" />
        </ItemGroup>
    
        <!-- Omitted for brevity -->
    </Project>
    ```

1. Check for any NuGet package updates, either using the NuGet Package Manager in Visual Studio or the **Update NuGet Package** command in VS Code.
1. Update to the latest [.NET Aspire templates](../fundamentals/aspire-sdk-templates.md) by running the following .NET command line:

    ```dotnetcli
    dotnet new update
    ```

    > [!IMPORTANT]
    > The `dotnet new update` command updates all of your templates to the latest version.

If your app host project file doesn't have the `Aspire.AppHost.Sdk` reference, you might still be using .NET Aspire 8. To upgrade to 9.0, follow [the upgrade guide](../get-started/upgrade-to-aspire-9.md).

## 💔 Breaking changes

With every release, we strive to make .NET Aspire better. However, some changes may break existing functionality. The following breaking changes are introduced in .NET Aspire 9.3:

- [Breaking changes in .NET Aspire 9.3](../compatibility/9.3/index.md)
