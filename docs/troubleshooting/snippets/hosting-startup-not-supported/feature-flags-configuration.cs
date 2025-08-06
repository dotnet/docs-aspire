// Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Conditional service registration based on configuration
var databaseProvider = builder.Configuration["DatabaseProvider"];
switch (databaseProvider)
{
    case "PostgreSQL":
        builder.AddNpgsqlDbContext<MyDbContext>("postgres");
        break;
    case "SqlServer":
        builder.AddSqlServerDbContext<MyDbContext>("sqlserver");
        break;
    default:
        throw new InvalidOperationException($"Unsupported database provider: {databaseProvider}");
}

var telemetryProvider = builder.Configuration["TelemetryProvider"];
switch (telemetryProvider)
{
    case "ApplicationInsights":
        builder.Services.AddApplicationInsightsTelemetry();
        break;
    case "OpenTelemetry":
        // OpenTelemetry is included with service defaults
        break;
}

var app = builder.Build();