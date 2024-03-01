using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

int insertionRows = builder.Configuration.GetValue<int>("InsertionRows", 1);

builder.AddServiceDefaults();

builder.AddSqlServerDbContext<MyDbContext>("db");

var app = builder.Build();

app.MapGet("/", async (MyDbContext context) =>
{
    // You wouldn't normally do this on every call,
    // but doing it here just to make this simple.
    context.Database.EnsureCreated();

    for (var i = 0; i < insertionRows; i++)
    {
        var entry = new Entry();
        await context.Entries.AddAsync(entry);
    }

    await context.SaveChangesAsync();

    var entries = await context.Entries.ToListAsync();

    return new
    {
        totalEntries = entries.Count,
        entries
    };
});

app.Run();
