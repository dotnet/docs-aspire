---
ms.topic: include
---

### Connect to an existing Azure Storage account

You might have an existing Azure Storage account that you want to connect to. Instead of representing a new Azure Storage resource, you can add a connection string to the AppHost. To add a connection to an existing Azure Storage account, call the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var blobs = builder.AddConnectionString("blobs");

builder.AddProject<Projects.WebApplication>("web")
       .WithReference(blobs);

// After adding all resources, run the app...
```

[!INCLUDE [connection-strings-alert](../../includes/connection-strings-alert.md)]

The connection string is configured in the AppHost's configuration, typically under [User Secrets](/aspnet/core/security/app-secrets), under the `ConnectionStrings` section. The AppHost injects this connection string as an environment variable into all dependent resources, for example:

```json
{
    "ConnectionStrings": {
        "blobs": "https://{account_name}.blob.core.windows.net/"
    }
}
```

The dependent resource can access the injected connection string by calling the <xref:Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString*> method, and passing the connection name as the parameter, in this case `"blobs"`. The `GetConnectionString` API is shorthand for `IConfiguration.GetSection("ConnectionStrings")[name]`.

### Connect to storage resources

When the .NET Aspire AppHost runs, the storage resources can be accessed by external tools, such as the [Azure Storage Explorer](https://azure.microsoft.com/features/storage-explorer/). If your storage resource is running locally using Azurite, it will automatically be picked up by the Azure Storage Explorer.

> [!NOTE]
> The Azure Storage Explorer discovers Azurite storage resources assuming the default ports are used. If you've [configured the Azurite container to use different ports](#configure-azurite-container-ports), you'll need to configure the Azure Storage Explorer to connect to the correct ports.

To connect to the storage resource from Azure Storage Explorer, follow these steps:

1. Run the .NET Aspire AppHost.
1. Open the Azure Storage Explorer.
1. View the **Explorer** pane.
1. Select the **Refresh all** link to refresh the list of storage accounts.
1. Expand the **Emulator & Attached** node.
1. Expand the **Storage Accounts** node.
1. You should see a storage account with your resource's name as a prefix:

    :::image type="content" source="../media/azure-storage-explorer.png" lightbox="../media/azure-storage-explorer.png" alt-text="Azure Storage Explorer: Azurite storage resource discovered.":::

You're free to explore the storage account and its contents using the Azure Storage Explorer. For more information on using the Azure Storage Explorer, see [Get started with Storage Explorer](/azure/storage/storage-explorer/vs-azure-tools-storage-manage-with-storage-explorer).
