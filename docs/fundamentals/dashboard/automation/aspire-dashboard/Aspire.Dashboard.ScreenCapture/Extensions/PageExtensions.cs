using Microsoft.Playwright;

namespace Aspire.Dashboard.ScreenCapture.Extensions;

internal static class PageExtensions
{
    public static async Task LoginAsync(this IPage page, string token)
    {
        var response = await page.GotoAsync($"/login?t={token}");

        await Assertions.Expect(page).ToHaveURLAsync("/");
    }

    public static async Task LoginAndWaitForCacheResourceAsync(this IPage page, string token)
    {
        await page.LoginAsync(token);

        await Assertions.Expect(page.GetByText(ResourcePageTitles.CacheResource))
            .ToBeVisibleAsync();
    }
}
