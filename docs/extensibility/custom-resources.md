---
title: Create custom resource types for .NET Aspire
description: Learn how to create a custom resource for an existing containerized application.
ms.date: 04/29/2024
ms.topic: tutorial
ms.custom: devx-track-extended-azdevcli
---

# Create custom resource types for .NET Aspire

.NET Aspire improves the development experience by providing reusable building blocks that can be used to quickly arrange application dependencies and expose them to your own code. One of the key building blocks of an Aspire-based application is the _resource_. Consider the code below:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("cache");

var db = builder.AddPostgres("pgserver")
                .AddDatabase("inventorydb");

builder.AddProject<Projects.InventoryService>("inventoryservice")
       .WithReference(redis)
       .WithReference(db);
```

In the preceding code there are four resources represented:

1. `cache`: A Redis container.
1. `pgserver`: A Postgres container.
1. `inventorydb`: A database hosted on `pgserver`.
1. `inventoryservice`: An ASP.NET Core application.

Most .NET Aspire-related code that the average developer will write, centers around adding resources to the [app model](../fundamentals/app-host-overview.md) and creating references between them.

## Key elements of a .NET Aspire custom resource

Building a custom resource in .NET Aspire requires the following:

1. A custom resource type that implements <xref:Aspire.Hosting.ApplicationModel.IResource>
2. An extension method for <xref:Aspire.Hosting.IDistributedApplicationBuilder> named `Add{CustomResource}` where `{CustomResource}` is the name of the custom resource.

When custom resource requires optional configuration, developers may wish to implement `With*` suffixed extension methods to make these configuration options discoverable using the _builder pattern_.

## A practical example: MailDev

To help understand how to develop custom resources, this article shows an example of how to build a custom resource for [MailDev](https://maildev.github.io/maildev/). MailDev is an open-source tool which provides a local mail server specifically designed to allow developers to test e-mail sending behaviors within their app. For more information, see [the MailDev GitHub repository](https://github.com/maildev/maildev).

In this example you create a new .NET Aspire app as a test environment for the MailDev resource that you create. While you can create custom resources in existing .NET Aspire apps it's a good idea to consider whether the custom resource might be used across multiple .NET Aspire-based solutions and should be developed as a reusable component.

## Setup the starter project

Create a new .NET Aspire app that will be used to test out the new resource that we are developing.

```dotnetcli
dotnet new aspire -o MailDevResource
cd MailDevResource
dir
```

Once the project is created you should a listing containing the following:

- `MailDevResource.AppHost`: The [app host](../fundamentals/app-host-overview.md) used to test out the custom resource.
- `MailDevResource.ServiceDefaults`: The [service defaults](../fundamentals/service-defaults.md) project for use in service-related projects.
- `MailDevResource.sln`: The solution file referencing both projects.

Verify that the project can build and run successfully by executing the following command:

```dotnetcli
dotnet run --project MailDevResource.AppHost/MailDevResource.AppHost.csproj
```

The console output should look similar to the following:

```dotnetcli
Building...
info: Aspire.Hosting.DistributedApplication[0]
      Aspire version: 8.0.0-preview.7.24251.11+6596fdc41a8d419876a6bf4abc17b7c66b9ef63a
info: Aspire.Hosting.DistributedApplication[0]
      Distributed application starting.
info: Aspire.Hosting.DistributedApplication[0]
      Application host directory is: D:\source\repos\docs-aspire\docs\extensibility\snippets\MailDevResource\MailDevResource.AppHost
info: Aspire.Hosting.DistributedApplication[0]
      Now listening on: https://localhost:17251
info: Aspire.Hosting.DistributedApplication[0]
      Login to the dashboard at https://localhost:17251/login?t=928db244c720c5022a7a9bf5cf3a3526
info: Aspire.Hosting.DistributedApplication[0]
      Distributed application started. Press Ctrl+C to shut down.
```

Select the [dashboard link in the browser](../fundamentals/dashboard/explore.md#dashboard-authentication) to see the .NET Aspire dashboard:

:::image type="content" source="media/maildevresource-empty-dashboard.png" lightbox="media/maildevresource-empty-dashboard.png" alt-text="A screenshot of the empty .NET Aspire dashboard for test project.":::

Press <kbd>Ctrl</kbd>+<kbd>C</kbd> to shutdown the app (you can close the browser tab).

## Create library for resource extension

.NET Aspire resources are just classes and methods contained within a class library that references the .NET Aspire Hosting library (`Aspire.Hosting`). By placing the resource in a separate project you can more easily share it between .NET Aspire-based apps and potentially package and share it on NuGet.

1. Create the class library project named _MailDev.Hosting_.

    ```dotnetcli
    dotnet new classlib -o MailDev.Hosting
    ```

1. Add `Aspire.Hosting` to the class library as a package reference.

    ```dotnetcli
    dotnet add ./MailDev.Hosting\MailDev.Hosting.csproj package Aspire.Hosting --version 8.0.0
    ```

1. Add class library reference to the _MailDevResource.AppHost_ project.

    ```dotnetcli
    dotnet add ./MailDevResource.AppHost/MailDevResource.AppHost.csproj reference ./MailDev.Hosting/MailDev.Hosting.csproj
    ```

1. Add class library project to the solution file.

    ```dotnetcli
    dotnet sln ./MailDevResource.sln add ./MailDev.Hosting/MailDev.Hosting.csproj
    ```

Once the following steps are performed you can launch the project:

```dotnetcli
dotnet run --project ./MailDevResource.AppHost/MailDevResource.AppHost.csproj
```

This will result in a warning being displayed to the console:

```Output
.\.nuget\packages\aspire.hosting.apphost\8.0.0-preview.7.24251.11\build\Aspire.Hosting.AppHost.targets(174,5): warning ASPIRE004: '..\MailDev.Hosting\MailDev.Hosting.csproj' is referenced by an A
spire Host project, but it is not an executable. Did you mean to set IsAspireProjectResource="false"? [D:\source\repos\docs-aspire\docs\extensibility\snippets\MailDevResource\MailDevResource.AppHost\MailDevRe
source.AppHost.csproj]
```

This is because .NET Aspire treats project references in the app host as if they are service projects. To tell .NET Aspire that the project reference should be treated as a non-service project modify the _MailDevResource.AppHost\MailDevResource.AppHost.csproj_ files reference to the `MailDev.Hosting` project to be the following:

```xml
<ItemGroup>
  <!-- The IsAspireProjectResource attribute tells .NET Aspire to treat this 
       reference as a standard project reference and not attempt to generate
       a metadata file -->
  <ProjectReference Include="..\MailDev.Hosting\MailDev.Hosting.csproj"
                    IsAspireProjectResource="false" />
</ItemGroup>
```

Now when you launch the app host, there's no warning displayed to the console.

## Define the resource types

The _MailDev.Hosting_ class library contains the resource type and extension methods for adding the resource to the app host. You should first think about the experience that you want to give developers when using your custom resource. In the case of this custom resource, you would want developers to be able to write code like the following:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var maildev = builder.AddMailDev("maildev");

builder.AddProject<Projects.NewsletterService>("newsletterservice")
       .WithReference(maildev);
```

To achieve this, you need a custom resource named `MailDevResource` which implements <xref:Aspire.Hosting.ApplicationModel.IResourceWithConnectionString> so that consumers can use it with <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> extension to inject the connection details for the MailDev server as a connection string.

MailDev is available as a container resource, so you'll also want to derive from <xref:Aspire.Hosting.ApplicationModel.ContainerResource> so that we can make use of various pre-existing container-focused extensions in .NET Aspire.

Replace the contents of the _Class1.cs_ file in the `MailDev.Hosting` project, and rename the file to _MailDevResource.cs_ with the following code:

:::code language="csharp" source="snippets/MailDevResource/MailDev.Hosting/MailDevResource.cs":::

In the preceding custom resource, the <xref:Aspire.Hosting.ApplicationModel.EndpointReference> and <xref:Aspire.Hosting.ApplicationModel.ReferenceExpression> are examples of several types which implement a collection of interfaces, such as <xref:Aspire.Hosting.ApplicationModel.IManifestExpressionProvider>, <xref:Aspire.Hosting.ApplicationModel.IValueProvider>, and <xref:Aspire.Hosting.ApplicationModel.IValueWithReferences>. For more information about these types and their role in .NET Aspire, see [technical details](#technical-details).

## Define the resource extensions

To make it easy for developers to use the custom resource an extension method named `AddMailDev` needs to be added to the _MailDev.Hosting_ project. The `AddMailDev` extension method is responsible for configuring the resource so it can start successfully as a container.

Add the following code to a new file named _MailDevResourceBuilderExtensions.cs_ in the _MailDev.Hosting_ project:

:::code language="csharp" source="snippets/MailDevResource/MailDev.Hosting/MailDevResourceBuilderExtensions.cs":::

## Validate custom component inside the app host

Now that the basic structure for the custom resource is complete it is time to test it in a real AppHost project. Open the _Program.cs_ file in the _MailDevResource.AppHost_ project and update it with the following code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var maildev = builder.AddMailDev("maildev");

builder.Build().Run();
```

After updating the _Program.cs_ file, launch the app host project and open the dashboard:

```dotnetcli
dotnet run --project ./MailDevResource.AppHost/MailDevResource.AppHost.csproj
```

After a few moments the dashboard will show that the `maildev` resource is running and a hyperlink will be available that navigates to the MailDev web app, which shows the content of each e-mail that your app sends.

_The .NET Aspire dashboard should look similar to the following:_

:::image type="content" source="media/maildev-in-aspire-dashboard.png" lightbox="media/maildev-in-aspire-dashboard.png" alt-text="MailDev resource visible in .NET Aspire dashboard.":::

_The MailDev web app should look similar to the following:_

:::image type="content" source="media/maildev-web-ui.png" lightbox="media/maildev-web-ui.png" alt-text="MailDev web-based user interface running as a container managed by .NET Aspire.":::

## Add a .NET service project to the app host for testing

Once .NET Aspire can successfully launch the MailDev component, it's time to consume the connection information for MailDev within a .NET project. In .NET Aspire it's common for there to be a _hosting package_ and one or more _component packages_. For example consider:

- **Hosting package**:
  - `Aspire.Hosting.Redis`
- **Component packages**:
  - `Aspire.StackExchange.Redis`
  - `Aspire.StackExchange.Redis.DistributedCaching`
  - `Aspire.StackExchange.Redis.OutputCaching`

In the case of the MailDev resource, the .NET platform already has an SMTP client in the form of <xref:System.Net.Mail.SmtpClient>. In this example you use this existing API for the sake of simplicity, although other resource types may benefit from custom component libraries to assist developers.

In order to test the end-to-end scenario, you need a .NET project which we can inject the connection information into for the MailDev resource. Add a web api project:

1. Create a new .NET project named _MailDevResource.NewsletterService_.

    ```dotnetcli
    dotnet new webapi --use-minimal-apis --no-openapi -o MailDevResource.NewsletterService
    ```

1. Add a reference to the _MailDev.Hosting_ project.

    ```dotnetcli
    dotnet add ./MailDevResource.NewsletterService/MailDevResource.NewsletterService.csproj reference ./MailDev.Hosting/MailDev.Hosting.csproj
    ```

1. Add a referece to the _MailDevResource.AppHost_ project.

    ```dotnetcli
    dotnet add ./MailDevResource.AppHost/MailDevResource.AppHost.csproj reference ./MailDevResource.NewsletterService\MailDevResource.NewsletterService.csproj
    ```

1. Add the new project to the solution file.

    ```dotnetcli
    dotnet sln ./MailDevResource.sln add ./MailDevResource.NewsletterService/MailDevResource.NewsletterService.csproj
    ```

After the project has been added and references have been updated, open the _Program.cs_ of the _MailDevResource.AppHost.csproj_ project, and update the source file to look like the following:

:::code source="snippets/MailDevResource/MailDevResource.AppHost/Program.cs":::

After updating the `Program` source file, launch the app host again and verify that the .NET project that was added started and that the environment variable `ConnectionStrings__maildev` was added to the process. From the **Resources** page, find the `newsletterservice` row, and select the **View** link on the **Details** column:

:::image type="content" source="media/maildev-envvar.png" lightbox="media/maildev-envvar.png" alt-text="Environment variables for Newsletter Service in .NET Aspire Dashboard.":::

The preceding screenshot shows the environment variables for the `newsletterservice` project. The `ConnectionStrings__maildev` environment variable is the connection string that was injected into the project by the `maildev` resource.

## Use connection string to send messages

To use the SMTP connection details that were injected into the newsletter service project, you inject an instance of <xref:System.Net.Mail.SmtpClient> into the dependency injection container as a singleton. Add the following code to the _Program.cs_ file in the _MailDevResource.NewsletterService_ project to setup the singleton service. In the `Program` class, immediately following the `// Add services to the container` comment, add the following code:

:::code source="snippets/MailDevResource/MailDevResource.NewsletterService/Program.cs" id="smtp":::

To test the client, add two simple `subscribe` and `unsubscribe` GET methods to the newsletter service. Add the following code after the `MapGet` call in the _Program.cs_ file of the _MailDevResource.NewsletterService_ project to setup the ASP.NET Core routes:

:::code source="snippets/MailDevResource/MailDevResource.NewsletterService/Program.cs" id="subs":::

> [!TIP]
> Remember to reference the `System.Net.Mail` and `Microsoft.AspNetCore.Mvc` namespaces in _Program.cs_ if your code editor doesn't automatically add them.

Once the _Program.cs_ file is updated, launch the app host and use your browser, or `curl` to hit the following URLs (alternatively if you're using Visual Studio you can use `.http` files):

```http
POST /subscribe?email=test@test.com HTTP/1.1
Host: localhost:7251
Content-Type: application/json
```

To use this API, you can use `curl` to send the request. The following `curl` command sends an HTTP `POST` request to the `subscribe` endpoint, and it expects an `email` query string value to subscribe to the newsletter. The `Content-Type` header is set to `application/json` to indicate that the request body is in JSON format.:

## [Unix](#tab/unix)

```bash
curl -H "Content-Type: application/json" --request POST https://localhost:7251/subscribe?email=test@test.com
```

## [Windows](#tab/windows)

```powershell
curl -H "Content-Type: application/json" --request POST https://localhost:7251/subscribe?email=test@test.com
```

---

The next API is the `unsubscribe` endpoint. This endpoint is used to unsubscribe from the newsletter.

```http
POST /unsubscribe?email=test@test.com HTTP/1.1
Host: localhost:7251
Content-Type: application/json
```

To unsubscribe from the newsletter, you can use the following `curl` command, passing an `email` parameter to the `unsubscribe` endpoint as a query string:

## [Unix](#tab/unix)

```bash
curl -H "Content-Type: application/json" --request POST https://localhost:7251/unsubscribe?email=test@test.com
```

## [Windows](#tab/windows)

```powershell
curl -H "Content-Type: application/json" --request POST https://localhost:7251/unsubscribe?email=test@test.com
```

---

> [!TIP]
> Make sure that you replace the `https://localhost:7251` with the correct localhost port (the URL of the app host that you are running).

If those API calls return a successful response (HTTP 200, Ok) then you should be able to click on the `maildev` resource the dashboard and the MailDev UI will show the emails that have been sent to the SMTP endpoint.

:::image type="content" source="media/maildev-emails.png" lightbox="media/maildev-emails.png" alt-text="E-mails visible in the MailDev UI":::

## Technical details

In the following sections, various technical details are discussed which are important to understand when developing custom resources for .NET Aspire.

### The `ReferenceExpression` and `EndpointReference` type

In the preceding code, the `MailDevResource` had two properties:

- `SmtpEndpoint`: <xref:Aspire.Hosting.ApplicationModel.EndpointReference> type.
- `ConnectionStringExpression`: <xref:Aspire.Hosting.ApplicationModel.ReferenceExpression> type.

These types are among several which are used throughout .NET Aspire to represent configuration data, which isn't finalized until the .NET Aspire application is either run or published to the cloud via a tool such as [Azure Developer CLI (`azd`)](/azure/developer/azure-developer-cli/overview).

The fundamental problem that these types help to solve, is deferring resolution of concrete configuration information until _all_ the information is available.

For example, the `MailDevResource` exposes a property called `ConnectionStringExpression` as required by the <xref:Aspire.Hosting.ApplicationModel.IResourceWithConnectionString> interface. The type of the property is <xref:Aspire.Hosting.ApplicationModel.ReferenceExpression> and is created by passing in an interpolated string to the <xref:Aspire.Hosting.ApplicationModel.ReferenceExpression.Create%2A> method.

```csharp
public ReferenceExpression ConnectionStringExpression =>
    ReferenceExpression.Create(
        $"smtp://{SmtpEndpoint.Property(EndpointProperty.Host)}:{SmtpEndpoint.Property(EndpointProperty.Port)}"
    );
```

The signature for the <xref:Aspire.Hosting.ApplicationModel.ReferenceExpression.Create%2A> method is as follows:

```csharp
public static ReferenceExpression Create(
    in ExpressionInterpolatedStringHandler handler)
```

This isn't a regular <xref:System.String> argument. The method makes use of the [interpolated string handler pattern](/dotnet/csharp/whats-new/tutorials/interpolated-string-handler), to capture the interpolated string template and the values referenced within it to allow for custom processing. In the case of .NET Aspire, these details are captured in a <xref:Aspire.Hosting.ApplicationModel.ReferenceExpression> which can be evaluated as each value referenced in the interpolated string becomes available.

Here's how the flow of execution works:

1. A resource which implements <xref:Aspire.Hosting.ApplicationModel.IResourceWithConnectionString> is added to the model (for example, `AddMailDev(...)`).
1. The `IResourceBuilder<MailDevResource>` is passed to the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> which has a special overload for handling <xref:Aspire.Hosting.ApplicationModel.IResourceWithConnectionString> implementors.
1. The `WithReference` wraps the resource in a <xref:Aspire.Hosting.ApplicationModel.ConnectionStringReference> instance and the object is captured in a <xref:Aspire.Hosting.ApplicationModel.EnvironmentCallbackAnnotation> which is evaluated after the .NET Aspire application is built and starts running.
1. As the the process that references the connection string starts .NET Aspire starts evaluating the expression. It first gets the <xref:Aspire.Hosting.ApplicationModel.ConnectionStringReference> and calls <xref:Aspire.Hosting.ApplicationModel.ConnectionStringReference.Aspire%23Hosting%23ApplicationModel%23IValueProvider%23GetValueAsync%2A>.
1. The `GetValueAsync` method gets the value of the <xref:Aspire.Hosting.ApplicationModel.IResourceWithConnectionString.ConnectionStringExpression> property to get the <xref:Aspire.Hosting.ApplicationModel.ReferenceExpression> instance.
1. The <xref:Aspire.Hosting.ApplicationModel.ConnectionStringReference.Aspire%23Hosting%23ApplicationModel%23IValueProvider%23GetValueAsync%2A> method then calls <xref:Aspire.Hosting.ApplicationModel.ReferenceExpression.GetValueAsync%2A> to process the previously captured interpolated string.
1. Because the interpolated string contains references to other reference types such as <xref:Aspire.Hosting.ApplicationModel.EndpointReference> they are also evaluated and real value substituted (which at this time are now available).

### Manifest publishing

The <xref:Aspire.Hosting.ApplicationModel.IManifestExpressionProvider> interface is designed to solve the problem of sharing connection information between resources at deployment. The solution for this particular problem is described in the [.NET Aspire inner-loop networking overview](../fundamentals/networking-overview.md). Similarly to local development, many of the values are necessary to configure the app, yet they cannot be determined until the app is being deployed via a tool, such as `azd` (Azure Developer CLI).

To solve this problem [.NET Aspire produces a manifest file](../deployment/manifest-format.md) which `azd` and other deployment tools interpret. Rather than specifying concrete values for connection information between resources an expression syntax is used which deployment tools evaluate. Generally the manifest file isn't visible to developers but it's possible to generate one for manual inspection. The command below can be used on the app host to produce a manifest.

```dotnetcli
dotnet run --project MailDevResource.AppHost/MailDevResource.AppHost.csproj -- --publisher manifest --output-path aspire-manifest.json
```

This command produces a manifest file like the following:

```json
{
  "resources": {
    "maildev": {
      "type": "container.v0",
      "connectionString": "smtp://{maildev.bindings.smtp.host}:{maildev.bindings.smtp.port}",
      "image": "docker.io/maildev/maildev:2.0.2",
      "bindings": {
        "http": {
          "scheme": "http",
          "protocol": "tcp",
          "transport": "http",
          "targetPort": 1080
        },
        "smtp": {
          "scheme": "tcp",
          "protocol": "tcp",
          "transport": "tcp",
          "targetPort": 1025
        }
      }
    },
    "newsletterservice": {
      "type": "project.v0",
      "path": "../MailDevResource.NewsletterService/MailDevResource.NewsletterService.csproj",
      "env": {
        "OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES": "true",
        "OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES": "true",
        "OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY": "in_memory",
        "ASPNETCORE_FORWARDEDHEADERS_ENABLED": "true",
        "ConnectionStrings__maildev": "{maildev.connectionString}"
      },
      "bindings": {
        "http": {
          "scheme": "http",
          "protocol": "tcp",
          "transport": "http"
        },
        "https": {
          "scheme": "https",
          "protocol": "tcp",
          "transport": "http"
        }
      }
    }
  }
}
```

Because the `MailDevResource` implements <xref:Aspire.Hosting.ApplicationModel.IResourceWithConnectionString> the manifest publishing logic in .NET Aspire knows that even though `MailDevResource` is a container resource, it also needs a `connectionString` field. The `connectionString` field references other parts of the `maildev` resource in the manifest to produce the final string:

```json
{
    // ... other content omitted.
    "connectionString": "smtp://{maildev.bindings.smtp.host}:{maildev.bindings.smtp.port}"
}
```

.NET Aspire knows how to form this string because it looks at <xref:Aspire.Hosting.ApplicationModel.IResourceWithConnectionString.ConnectionStringExpression> and builds up the final string via the <xref:Aspire.Hosting.ApplicationModel.IManifestExpressionProvider> interface (in much the same way as the <xref:Aspire.Hosting.ApplicationModel.IValueProvider> interface is used).

The `MailDevResource` automatically gets included in the manifest because it's derived from <xref:Aspire.Hosting.ApplicationModel.ContainerResource>. Resource authors can choose to suppress outputting content to the manifest by using the <xref:Aspire.Hosting.ResourceBuilderExtensions.ExcludeFromManifest%2A> extension method on the resource builder.

```csharp
public static IResourceBuilder<MailDevResource> AddMailDev(
    this IDistributedApplicationBuilder builder, 
    string name,
    int? httpPort = null,
    int? smtpPort = null)
{
    var resource = new MailDevResource(name);

    return builder.AddResource(resource)
                  .WithImage(MailDevContainerImageTags.Image)
                  .WithImageRegistry(MailDevContainerImageTags.Registry)
                  .WithImageTag(MailDevContainerImageTags.Tag)
                  .WithHttpEndpoint(
                      targetPort: 1080,
                      port: httpPort,
                      name: MailDevResource.HttpEndpointName)
                  .WithEndpoint(
                      targetPort: 1025,
                      port: smtpPort,
                      name: MailDevResource.SmtpEndpointName)
                  .ExcludeFromManifest(); // This line was added!
}
```

Careful consideration should be given as to whether the resource should be present in the manifest, or whether it should be suppressed. If the resource is being added to the manifest it should be configured in such a way that it 's safe and secure to use.

## Summary

In the custom resource tutorial, you learned how to create a custom .NET Aspire resource which uses an existing containerized application (MailDev). You then used that to improve the local development experience by making it easy to test e-mail capabilities that might be used within an apps. These learnings can be applied to building out other custom resources that can be used in .NET Aspire-based applications. This specific example, didn't include any custom components, but it's possible to build out custom components to make it easier for developers to use the resource. In this scenario you were able to rely on the existing `SmtpClient` class in the .NET platform to send e-mails.
