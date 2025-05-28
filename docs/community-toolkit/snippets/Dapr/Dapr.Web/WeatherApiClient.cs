using Dapr.Client;

namespace Dapr.Web;

public class WeatherApiClient(DaprClient client)
{
    public async Task<WeatherForecast[]> GetWeatherAsync(
        int maxItems = 10, CancellationToken cancellationToken = default)
    {
        List<WeatherForecast>? forecasts =
            await client.InvokeMethodAsync<List<WeatherForecast>>(
                HttpMethod.Get,
                "apiservice",
                "weatherforecast",
                cancellationToken);

        return forecasts?.Take(maxItems)?.ToArray() ?? [];
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
