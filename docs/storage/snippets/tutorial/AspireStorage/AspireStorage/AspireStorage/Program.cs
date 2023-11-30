using AspireStorage.Client.Pages;
using AspireStorage.Components;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;

var builder = WebApplication.CreateBuilder(args);

builder.AddAzureBlobService("BlobConnection");
builder.AddAzureQueueService("QueueConnection");

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();

    var blobs = app.Services.GetRequiredService<BlobServiceClient>();
    var docsContainer = blobs.GetBlobContainerClient("fileuploads");
    await docsContainer.CreateIfNotExistsAsync();

    var queues = app.Services.GetRequiredService<QueueServiceClient>();
    var queueClient = queues.GetQueueClient("tickets");
    await queueClient.CreateIfNotExistsAsync();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

app.Run();
