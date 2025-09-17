using AspirePostgreSQLEFCore.Data;

var builder = WebApplication.CreateBuilder(args);

// Add .NET Aspire service defaults and PostgreSQL Entity Framework Core
builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<TicketContext>("ticketdb");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();