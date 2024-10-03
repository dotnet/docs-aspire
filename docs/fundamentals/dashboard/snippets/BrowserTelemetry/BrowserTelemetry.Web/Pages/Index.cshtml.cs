using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BrowserTelemetry.Web.Pages;

public class IndexModel(ILogger<IndexModel> logger) : PageModel
{
    public void OnGet()
    {
        logger.LogInformation("Get index");
    }
}
