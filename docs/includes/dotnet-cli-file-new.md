If you haven't already installed the [.NET Aspire templates](../fundamentals/setup-tooling.md#install-the-net-aspire-templates), run the following `dotnet new install` command:

```dotnetcli
dotnet new install Aspire.ProjectTemplates
```

The preceding .NET CLI command ensures that you have the .NET Aspire templates available. To create the .NET Aspire Starter App from the template, run the following `dotnet new` command:

```dotnetcli
dotnet new aspire-starter --use-redis-cache --output AspireSample
```

For more information, see [dotnet new](/dotnet/core/tools/dotnet-new). The .NET CLI creates a new solution that is structured to use .NET Aspire.
