---
title: Rust hosting
author: Alirexaa
description: Learn how to use the Aspire Rust hosting integration to host Rust applications.
ms.date: 11/15/2024
---

# Aspire Rust hosting

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

[Rust](https://www.rust-lang.org/) is a general-purpose programming language emphasizing performance, type safety, and concurrency. It enforces memory safety, meaning that all references point to valid memory.
The Aspire Rust hosting integration allows you to host Rust applications in your Aspire AppHost project, and provide it to other resources in your application.

## Hosting integration

The Rust hosting integration models a Rust application as the `Aspire.Hosting.ApplicationModel.RustAppExecutableResource` type. To access this type and APIs that allow you to add it to your AppHost project, install the [📦 CommunityToolkit.Aspire.Hosting.Rust](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.Rust) NuGet package in the AppHost project.

This integration expects that the Rust programming language has already been installed on the host machine and the Rust package manager [`cargo`](https://doc.rust-lang.org/cargo/getting-started/installation.html) is available in the system path.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.Rust
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.Rust"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Add a Rust resource

In the _:::no-loc text="AppHost.cs":::_ file of your AppHost project, call the `Aspire.Hosting.RustAppHostingExtension.AddRustApp` on the `builder` instance to add a Rust application resource as shown in the following example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var rust = builder.AddRustApp("rust-app", workingDirectory: "../rust-service")
                 .WithHttpEndpoint(env: "PORT");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(rust);

// After adding all resources, run the app...
```

The working directory of the application should be the root of Rust application directory.
Also you can customize running behavior by passing args parameter to the `AddRustApp` method.

```csharp
var rust = builder.AddRustApp("rust-app", workingDirectory: "../rust-service", args: ["--locked"])
                 .WithHttpEndpoint(env: "PORT");
```

The Rust application can be added as a reference to other resources in the AppHost project.

## See also

- [Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
- [Sample Rust app](https://github.com/CommunityToolkit/Aspire/tree/main/examples/rust)
- [Install Rust](https://www.rust-lang.org/tools/install)
