// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add service defaults first
builder.AddServiceDefaults();

// Now you can use Aspire integrations
builder.AddNpgsqlDbContext<MyDbContext>("postgres");

var app = builder.Build();

app.MapDefaultEndpoints();
app.Run();