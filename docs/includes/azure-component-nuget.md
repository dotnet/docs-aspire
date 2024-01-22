---
ms.date: 01/22/2024
ms.topic: include
---

The .NET Aspire Azure hosting APIs are available in the `Aspire.Hosting.Azure` NuGet package. These APIs provide Azure-specific resources used within the App Host. To add Azure resources to your <xref:Aspire.Hosting.IDistributedApplicationBuilder>, install the [Aspire.Hosting.Azure](https://www.nuget.org/packages/Aspire.Hosting.Azure) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure"
                  Version="[SelectVersion]" />
```

---
