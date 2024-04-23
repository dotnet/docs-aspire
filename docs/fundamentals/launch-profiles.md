---
title: .NET Aspire and launch profiles
description: Learn how .NET Aspire integrates with .NET launch profiles.
ms.date: 04/23/2024
---

# .NET Aspire and launch profiles

.NET Aspire makes use of _launch profiles_ defined in both the app host and service projects to simplify the process of configuring multiple aspects of the debugging and publishing experience for .NET Aspire-based distributed applications.

## Launch profile basics

When creating a new .NET application from a template developers will often see a `Properties` directory which contains a file named _launchSettings.json_. The launch settings file contains a list of _launch profiles_. Each launch profile is a collection of related options which defines how you would like `dotnet` to start your application.

The code below is an example of launch profiles in a _launchSettings.json_ file for an **ASP.NET Core** application.

```json
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5130",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "https://localhost:7106;http://localhost:5130",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

The _launchSettings.json_ file above defines two _launch profiles_, `http` and `https`. Each has its own set of environment variables, launch URLs and other options. When launching a .NET Core application developers can choose which launch profile to use.

```dotnetcli
dotnet run --launch-profile https
```

If no launch profile is specified, then the first launch profile is selected by default. It is possible to launch a .NET Core application without a launch profile using the `--no-launch-profile` option. Some fields from the _launchSettings.json_ file are translated to environment variables. For example, the `applicationUrl` field is converted to the `ASPNETCORE_URLS` environment variable which controls which address and port ASP.NET Core binds to.

In Visual Studio it's possible to select the launch profile when launching the application making it easy to switch between configuration scenarios when manually debugging issues:

:::image type="content" source="./media/launch-profiles/vs-launch-profile-toolbar.png" lightbox="./media/launch-profiles/vs-launch-profile-toolbar.png" alt-text="Screenshot of the standard toolbar in Visual Studio with the launch profile selector highlighted.":::

When a .NET application is launched with a launch profile a special environment variable called `DOTNET_LAUNCH_PROFILE` is populated with the name of the launch profile that was used when launching the process.

## Launch profiles for .NET Aspire app host

In .NET Aspire, the AppHost is just a .NET application. As a result it has a `launchSettings.json` file just like any other application. Here is an example of the `launchSettings.json` file generated when creating a new .NET Aspire application from the starter template (`dotnet new aspire-starter`).

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:17134;http://localhost:15170",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "DOTNET_ENVIRONMENT": "Development",
        "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL": "https://localhost:21030",
        "DOTNET_RESOURCE_SERVICE_ENDPOINT_URL": "https://localhost:22057"
      }
    },
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "http://localhost:15170",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "DOTNET_ENVIRONMENT": "Development",
        "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL": "http://localhost:19240",
        "DOTNET_RESOURCE_SERVICE_ENDPOINT_URL": "http://localhost:20154"
      }
    }
  }
}
```

The .NET Aspire templates have a very similar set of _launch profiles_ to a regular ASP.NET Core application. When the .NET Aspire application launches it hosts a web-server which is used by the .NET Aspire Dashboard to fetch information about resources which are being orchestrated by .NET Aspire. There are some additional environment variables which are defined which are covered in [.NET Aspire dashboard configuration](./dashboard/configuration.md).

## Relationship between app host launch profiles and service projects

In .NET Aspire the app host is responsible for coordinating the launch of multiple service projects. When you run the app host either via the command line or from Visual Studio (or other development environment) a launch profile is selected for the app host. In turn, the app host will attempt to find a matching launch profile in the service projects it is launching and use those options to control the environment and default networking configuration for the service project.

When the app host launches a service project it doesn't simply launch the service project using the `--launch-profile` option. Therefore, there will be no `DOTNET_LAUNCH_PROFILE` environment variable set for service projects. This is because .NET Aspire modifies the `ASPNETCORE_URLS` environment variable (derived from the `applicationUrl` field in the launch profile) to use a different port. By default, .NET Aspire inserts a reverse proxy in front of the ASP.NET Core application to allow for multiple instances of the application using the <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.WithReplicas%2A> method.

Other settings such as options from the `environmentVariables` field are passed through to the application without modification.

## Control launch profile selection

Ideally, it's possible to align the launch profile names between the app host and the service projects to make it easy to switch between configuration options on all projects coordinated by the app host at once. However, it may be desirable to control launch profile that a specific project uses. The <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A> extension method provides a mechanism to do this.

```csharp
var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Projects.InventoryService>(
    "inventoryservice",
    launchProfileName: "mylaunchprofile");
```

The preceding code shows that the `inventoryservice` resource (a .NET project) is launched using the options from the `mylaunchprofile` launch profile. The launch profile precedence logic is as follows:

1. Use the launch profile specified by `launchProfileName` argument if specified.
2. Use the launch profile with the same name as the AppHost (determined by reading the `DOTNET_LAUNCH_PROFILE` environment variable).
3. Use the default (first) launch profile in _launchSettings.json_.
4. Don't use a launch profile.

To force a service project to launch without a launch profile the `launchProfileName` argument on the <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A> method can be set to null.

## Launch profiles and endpoints

When adding an ASP.NET Core project to the app host, .NET Aspire will parse the _launchSettings.json_ file selecting the appropriate launch profile and automatically generate endpoints in the application model based on the URL(s) present in the `applicationUrl` field. To modify the endpoints that are automatically injected the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithEndpoint%2A> extension method.

```csharp
var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Projects.InventoryService>("inventoryservice")
       .WithEndpoint("https", endpoint => endpoint.IsProxied = false);
```

The preceding code shows how to disable the reverse proxy that .NET Aspire deploys in front for the .NET Core application and instead allows the .NET Core application to respond directly on requests over HTTP(S). For more information on networking options within .NET Aspire see [.NET Aspire inner loop networking overview](./networking-overview.md).
