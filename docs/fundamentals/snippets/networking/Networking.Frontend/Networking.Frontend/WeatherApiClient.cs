public class WeatherApiClient(HttpClient httpClient)
{
    public IAsyncEnumerable<WeatherForecast?> GetWeatherAsync()
    {
        return httpClient.GetFromJsonAsAsyncEnumerable<WeatherForecast>("/weatherforecast");
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
