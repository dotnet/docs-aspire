---
title: .NET Aspire MailPit hosting integration
description: Learn how to use the .NET Aspire MailPit hosting integration to add MailPit containers to your distributed application.
ms.date: 01/22/2025
---

# .NET Aspire MailPit hosting integration

In this article, you learn how to use the .NET Aspire MailPit hosting integration. The `CommunityToolkit.Aspire.Hosting.MailPit` library is used to register [MailPit](https://mailpit.axllent.org/) containers in your distributed application.

MailPit is a mail testing tool that provides a fake SMTP server for development and testing. It provides a web interface where you can view and manage captured emails, making it ideal for testing email functionality in your applications.

## Hosting integration

The MailPit hosting integration models MailPit as the `MailPitResource` type. To access this resource and the extension methods to add it to your app model, add the [📦 CommunityToolkit.Aspire.Hosting.MailPit](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.MailPit) NuGet package in the [app host](../fundamentals/app-host-overview.md) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.MailPit
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.MailPit"
                  Version="*" />
```

---

In your app host project, register a MailPit resource using the `AddMailPit` extension method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var mailpit = builder.AddMailPit("mailpit");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(mailpit);

builder.Build().Run();
```

The `AddMailPit` method adds a MailPit resource to the application model. The resource is configured to use the default MailPit image and ports.

When you add a MailPit resource, the following services are exposed:

| Service | Scheme | Port |
|---------|--------|------|
| SMTP | `smtp` | 1025 |
| Web UI | `http` | 8025 |

The SMTP service can be used by your applications to send emails for testing purposes, while the Web UI provides an interface to view captured emails.

## Data persistence

By default, MailPit stores emails in memory, which means all emails are lost when the container is stopped. To persist emails across container restarts, you can add a data volume:

```csharp
var mailpit = builder.AddMailPit("mailpit")
                     .WithDataVolume("mailpit-data");
```

The data volume persists the MailPit data directory, ensuring emails are retained across container restarts.

You can also use a bind mount to map a local directory to the MailPit container:

```csharp
var mailpit = builder.AddMailPit("mailpit")
                     .WithDataBindMount(@"C:\MailPitData");
```

## Client integration

There's no Community Toolkit client integration for MailPit. Instead, use any SMTP client library with the connection string provided by the MailPit resource. The MailPit resource provides an SMTP connection string that can be used to send emails to the MailPit server.

The connection string includes the following information:

- **Host**: The hostname or IP address of the MailPit SMTP server
- **Port**: The port number (typically 1025)

Here's an example of how to use the connection string in a .NET application:

```csharp
public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var connectionString = _configuration.GetConnectionString("mailpit");
        var connectionInfo = ConnectionString.Parse(connectionString);

        using var client = new SmtpClient();
        await client.ConnectAsync(connectionInfo.Host, connectionInfo.Port, SecureSocketOptions.None);
        
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Test Sender", "test@example.com"));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
```

This example uses the [MailKit](https://nuget.org/packages/MailKit) library to send emails via the MailPit SMTP server.

## See also

- [MailPit documentation](https://mailpit.axllent.org/)
- [.NET Aspire integrations overview](../fundamentals/integrations-overview.md)
- [.NET Aspire Community Toolkit overview](overview.md)
