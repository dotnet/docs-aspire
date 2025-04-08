using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/admin", () =>
{
    return Results.Content("""
                <!DOCTYPE html>
        <html lang="en">
        <head>
          <meta charset="UTF-8">
          <title>Admin Portal Login</title>
          <meta name="viewport" content="width=device-width, initial-scale=1.0">
        </head>
        <body style="
          margin: 0;
          padding: 0;
          box-sizing: border-box;
          font-family: 'Segoe UI', sans-serif;
          background: linear-gradient(to right, #667eea, #764ba2);
          height: 100vh;
          display: flex;
          align-items: center;
          justify-content: center;
        ">

          <div style="
            background-color: white;
            padding: 2rem;
            border-radius: 12px;
            box-shadow: 0 8px 24px rgba(0, 0, 0, 0.1);
            width: 100%;
            max-width: 400px;
          ">
            <h2 style="
              margin-bottom: 1.5rem;
              text-align: center;
              color: #333;
            ">Admin Portal</h2>

            <form>
              <label for="email" style="display: block; margin-bottom: 0.5rem; color: #555;">Email</label>
              <input type="email" id="email" name="email" placeholder="admin@example.com" style="
                width: 100%;
                padding: 0.75rem;
                margin-bottom: 1rem;
                border: 1px solid #ccc;
                border-radius: 8px;
                font-size: 1rem;
              " required>

              <label for="password" style="display: block; margin-bottom: 0.5rem; color: #555;">Password</label>
              <input type="password" id="password" name="password" placeholder="••••••••" style="
                width: 100%;
                padding: 0.75rem;
                margin-bottom: 1.5rem;
                border: 1px solid #ccc;
                border-radius: 8px;
                font-size: 1rem;
              " required>

              <button type="submit" style="
                width: 100%;
                padding: 0.75rem;
                background-color: #667eea;
                color: white;
                border: none;
                border-radius: 8px;
                font-size: 1rem;
                cursor: pointer;
                transition: background-color 0.3s ease;
              ">Login</button>
            </form>
          </div>

        </body>
        </html>
        """, "text/html");
});

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
