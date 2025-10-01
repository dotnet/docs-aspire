using AspirePostgreSQLEFCore.Data;
using AspirePostgreSQLEFCore.MigrationService;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Add .NET Aspire service defaults
builder.AddServiceDefaults();

// Add PostgreSQL Entity Framework Core
builder.AddNpgsqlDbContext<TicketContext>("ticketdb");

// Add the worker service
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();