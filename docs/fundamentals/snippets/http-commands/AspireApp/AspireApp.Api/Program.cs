using AspireApp.Api;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddRedisDistributedCache(connectionName: "cache");

builder.Services.AddScoped<ICacheService, RedisCacheService>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()  // Allow requests from any origin
                  .AllowAnyHeader()  // Allow any headers in requests
                  .AllowAnyMethod(); // Allow any HTTP methods (GET, POST, etc.)
        });
    });
}

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");

    app.MapOpenApi();
    app.MapScalarApiReference();

    // <post>
    app.MapPost("/cache/invalidate", static async (
        [FromHeader(Name = "X-CacheInvalidation-Key")] string? header,
        ICacheService registry,
        IConfiguration config) =>
    {
        var hasValidHeader = config.GetValue<string>("ApiCacheInvalidationKey") is { } key
            && header == $"Key: {key}";

        if (hasValidHeader is false)
        {
            return Results.Unauthorized();
        }

        await registry.ClearAllAsync();

        return Results.Ok();
    });
    // </post>
}

app.UseHttpsRedirection();

app.MapGet("/api/products/{id:int}", async (string id, ICacheService cache) =>
{
    var product = await cache.GetAsync($"product:{id}", AppJsonContext.Default.Product);

    IResult result = product is null ? Results.NotFound() : Results.Ok(product);

    return result;
});

app.Lifetime.ApplicationStarted.Register(async () =>
{
    using var scope = app.Services.CreateScope();
    var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();

    foreach (var product in Products.DefaultProducts)
    {
        await cache.SetAsync($"product:{product.Id}", product, AppJsonContext.Default.Product);
    }
});

app.Run();
