var builder = WebApplication.CreateBuilder(args);

builder.AddAzureCosmosClient("cosmos",
    configureClientOptions: static (CosmosClientOptions options) =>
    {
        options.LimitToEndpoint = true;
        options.ConnectionMode = ConnectionMode.Gateway;
        options.ServerCertificateCustomValidationCallback = (cert, chain, errors) => true;
        options.SerializerOptions = new CosmosSerializationOptions
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
        };
    });

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddHostedService<DatabaseInitializationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var programmingGroup = app.MapGroup("programming");

programmingGroup.MapGet("/languages", static async (CosmosClient client) =>
{
    Container container = client.GetContainer("programming", "languages");

    QueryDefinition query = new("SELECT * FROM languages");
    using FeedIterator<ProgrammingLanguage> feed = container.GetItemQueryIterator<ProgrammingLanguage>(query);

    List<ProgrammingLanguage> languages = [];
    while (feed.HasMoreResults)
    {
        languages.AddRange((await feed.ReadNextAsync()).Resource);
    }

    return languages;
});

app.MapDefaultEndpoints();

app.Run();
