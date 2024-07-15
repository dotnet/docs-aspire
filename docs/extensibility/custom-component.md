---
title: Create custom .NET Aspire component
description: Learn how to create a custom .NET Aspire component for an existing containerized application.
ms.date: 07/15/2024
ms.topic: how-to
---

# Create custom .NET Aspire component

This article is a continuation of the [Create custom resource types for .NET Aspire](custom-resources.md) article. It guides you through creating a .NET Aspire component that uses [MailKit](https://github.com/jstedfast/MailKit) to send emails. This component is then integrated into the Newsletter app you previously built. The previous example, omitted the creation of a component and instead relied on the existing .NET `SmtpClient`. It's recommended to use MailKit's `SmtpClient` over the official .NET `SmtpClient` for sending emails, as it's more modern and supports more features/protocols.

## Prerequisites

If you're following along, you should have already completed the steps in the [Create custom resource types for .NET Aspire](custom-resources.md) article.

## Create library for component

[.NET Aspire components](../fundamentals/components-overview.md) are delivered as NuGet packages, but in this example, it's beyond the scope of this article to publish a NuGet package. Instead, you create a class library project that contains the component and reference it as a project. .NET Aspire component packages are intended to wrap a client library, such as MailKit, and provide production-ready telemetry, health checks, configurability, and testability. Let's start by creating a new class library project.

1. Create a new class library project named `MailKit.Client` in the same directory as the _MailDevResource.sln_ from the previous article.

    ```dotnetcli
    dotnet new classlib -o MailKit.Client
    ```

1. Add the project to the solution.

    ```dotnetcli
    dotnet sln /MailDevResource.sln add MailKit.Client/MailKit.Client.csproj
    ```

The next step is to add all the NuGet packages that the component relies on. Rather than having you add each package one-by-one from the .NET CLI, it's likely easier to simply copy and paste the following XML into the _MailKit.Client.csproj_ file.

```xml
  <ItemGroup>
    <PackageReference Include="MailKit" Version="4.7.0" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="8.0.2" />
    <PackageReference Include="Microsoft.Extensions.Resilience" Version="8.7.0" />
    <PackageReference Include="Microsoft.Extensions.Hosting.Abstractions" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Diagnostics.HealthChecks" Version="8.0.7" />
    <PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.9.0" />
  </ItemGroup>
```

## Define component settings

Whenever you're creating a .NET Aspire component, it's best to understand the client library that you're mapping to. In the case of MailKit, you need to understand the configuration settings that are required to connect to an SMTP server. But it's also important to understand if the library has support for health checks, tracing and metrics. MailKit supports tracing and metrics, through its `Telemetry.SmtpClient` class. And for health checks, those are easy enough to implement. Add the following code to the `MailKit.Client` project in a file named _MailKitClientSettings.cs_:

:::code source="snippets/MailDevResource/MailKit.Client/MailKitClientSettings.cs":::

The preceding code defines the `MailKitClientSettings` class with:

- `ConnectionString` property that represents the connection string to the SMTP server.
- `Credentials` property that represents the credentials to authenticate with the SMTP server.
- `DisableHealthChecks` property that determines whether health checks are enabled.
- `DisableTracing` property that determines whether tracing is enabled.
- `DisableMetrics` property that determines whether metrics are enabled.

## Expose component wrapper functionality

The goal of .NET Aspire components is to expose the underlying client library to consumers through dependency injection. In the case of MailKit and for this example, the `SmtpClient` class is what you want to expose. You're not wrapping any functionality, but rather mapping configuration settings to an `SmtpClient` class. It's common to expose both standard and keyed-service registrations for components. Standard registrations are used when there's only one instance of a service, and keyed-service registrations are used when there are multiple instances of a service. Sometimes this can be achieved by using a factory pattern. Add the following code to the `MailKit.Client` project in a file named _MailKitClientFactory.cs_:

:::code source="snippets/MailDevResource/MailKit.Client/MailKitClientFactory.cs":::

The `MailKitClientFactory` class is a factory that creates an `ISmtpClient` instance based on the configuration settings. It's responsible for returning an `ISmtpClient` implementation that's connected to a configured SMTP server and optionally authenticated. Next, you need to expose the functionality for the consumers to register this factory with the dependency injection container. Add the following code to the `MailKit.Client` project in a file named MailKitClientServiceCollectionExtensions.cs_:

:::code source="snippets/MailDevResource/MailKit.Client/MailKitClientServiceCollectionExtensions.cs":::

The preceding code adds two extension methods on the `IHostApplicationBuilder` type, one for the standard registration of MailKit and another for keyed-registration of MailKit. Both extensions ultimately rely on the private `AddMailKitClient` method to register the `MailKitClientFactory` with the dependency injection container as a scoped-life service. The reason for registering the `MailKitClientFactory` as a scoped service is due to the fact that the connection (and authentication) operations are considered expensive and should be reused within the same scope where possible. In other words, for a single request, the same `ISmtpClient` instance should be used.

The pattern for binding component settings to the builder's configuration, is to first instantiate the settings class and then bind settings to the specific section of the configuration.

After they're bound to the existing configuration, the optional `configureSettings` delegate is invoked with the `settings` instance. This allows the consumer to further configure the settings, ensuring that manual code settings are honored over configuration settings. Next, considering whether or not the `serviceKey` value was provided, the `MailKitClientFactory` is registered with the dependency injection container as either a standard or keyed service.

> [!IMPORTANT]
> It's intentional that the `implementationFactory` overload is called when registering services. The `CreateMailKitClientFactory` method throws when the configuration is invalid. This ensures that creation of the `MailKitClientFactory` is deferred until it's needed and it prevents the app from erroring out before logging is available.

The registration of health checks, and telemetry are described in a bit more detail in the following sections.

### Add health checks

[Health checks](../fundamentals/health-checks.md) are a way to monitor the health of a component. In the case of MailKit, you can check if the connection to the SMTP server is healthy. Add the following code to the `MailKit.Client` project in a file named _MailKitHealthCheck.cs_:

:::code source="snippets/MailDevResource/MailKit.Client/MailKitHealthCheck.cs":::

The preceding health check implementation:

- Implements the `IHealthCheck` interface.
- Accepts the `MailKitClientFactory` as a primary constructor parameter.
- Satisfies the `CheckHealthAsync` method by:
  - Attempting to get an `ISmtpClient` instance from the `factory`. If successful, it returns `HealthCheckResult.Healthy`.
  - If an exception is thrown, it returns `HealthCheckResult.Unhealthy`.

As previously shared in the registration of the `MailKitClientFactory`, the `MailKitHealthCheck` is conditionally registered with the `IHeathChecksBuilder`:

```csharp
if (settings.DisableHealthChecks is false)
{
    builder.Services.AddHealthChecks()
        .AddCheck<MailKitHealthCheck>(
            name: serviceKey is null ? "MailKit" : $"MailKit_{connectionName}",
            failureStatus: default,
            tags: [ "live" ]);
}
```

The consumer could choose to omit health checks by setting the `DisableHealthChecks` property to `true` in the configuration. This is a common pattern for components that have optional features.

### Wire up telemetry

As a best practice, the [MailKit client library exposes telemetry](https://github.com/jstedfast/MailKit/blob/master/Telemetry.md). .NET Aspire can take advantage of this telemetry and display it in the [.NET Aspire dashboard](../fundamentals/dashboard/overview.md). Depending on whether or not it's enabled, telemetry is wire up as shown in the following code snippet:

```csharp
if (settings.DisableTracing is false)
{
    builder.Services.AddOpenTelemetry()
        .WithTracing(
            traceBuilder => traceBuilder.AddSource(
                Telemetry.SmtpClient.ActivitySourceName));
}

if (settings.DisableMetrics is false)
{
    builder.Services.AddOpenTelemetry()
        .WithMetrics(
            metricsBuilder => metricsBuilder.AddMeter(
                Telemetry.SmtpClient.MeterName));
}
```

## Update the Newsletter service

With the component library created, you can now update the Newsletter service to use the MailKit client. The first step is to add a reference to the `MailKit.Client` project. Add the _MailKit.Client.csproj_ project reference to the `MailDevResource.NewsletterService` project:

```dotnetcli
dotnet add ./MailDevResource.NewsletterService/MailDevResource.NewsletterService.csproj reference MailKit.Client/MailKit.Client.csproj
```

The final step is to replace the existing _Program.cs_ file in the `MailDevResource.NewsletterService` project with the following code:

:::code source="snippets/MailDevResource/MailDevResource.NewsletterService/Program.cs":::

The most noteable changes in the preceding code are:

- The replacement of the registration for the official .NET `SmtpClient` with the call to the `AddMailKitClient` extension method.
- Replacing both `/subscribe` and `/unsubscribe` map post calls to instead inject the `MailKitClientFactory` and use the `ISmtpClient` instance to send the email.
