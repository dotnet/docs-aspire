namespace AspireApp.Web;

public class ProgrammingLanguagesApiClient(HttpClient httpClient)
{
    public async Task<ProgrammingLanguage[]> GetProgrammingLanguagesAsync(
        int maxItems = 10,
        CancellationToken cancellationToken = default)
    {
        List<ProgrammingLanguage>? languages = null;

        await foreach (var forecast in httpClient.GetFromJsonAsAsyncEnumerable<ProgrammingLanguage>(
            "/programming/languages", cancellationToken))
        {
            if (languages?.Count >= maxItems)
            {
                break;
            }

            if (forecast is not null)
            {
                languages ??= [];
                languages.Add(forecast);
            }
        }

        return languages?.ToArray() ?? [];
    }
}
