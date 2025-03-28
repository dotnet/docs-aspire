using System.Text.Json;
using Microsoft.Playwright;
using Xunit;

namespace Aspire.Dashboard.ScreenCapture.Extensions;

internal static class PageExtensions
{
    private static readonly JsonSerializerOptions s_options = new(JsonSerializerDefaults.Web);

    public static async Task RedactElementTextAsync(this IPage page, string selector)
    {
        await page.EvaluateAsync($$"""
            const el = document.querySelector('{{selector}}');
            if (el) {
              const text = el.textContent;
              el.textContent = '█'.repeat(text.length);
            } else {
              console.error('Element not found: {{selector}}');
            }
            """);
    }

    public static async Task BlurElementTextAsync(this IPage page, string selector)
    {
        await page.EvaluateAsync($$"""
            const el = document.querySelector('{{selector}}');
            if (el) {
              el.style.textDecoration = 'line-through';
              el.style.filter = 'blur(.2rem)';
            } else {
              console.error('Element not found: {{selector}}');
            }
            """);
    }

    public static async Task ClickAndDragShadowRootElementAsync(
        this IPage page, string hostSelector, string shadowSelector, MouseMovement mouseMovement)
    {
        var shadowHost = page.Locator(hostSelector);

        var source = await shadowHost.EvaluateHandleAsync(
            $"el => el.shadowRoot.querySelector('{shadowSelector}')");

        var element = source.AsElement();
        if (element is null)
        {
            return;
        }

        // Hover, click, and drag
        await element.HoverAsync();
        await page.Mouse.DownAsync();

        var (x, y) = mouseMovement;

        await page.Mouse.MoveAsync(x, y);
        await page.Mouse.UpAsync();
    }

    public static async Task HighlightElementAsync(this IPage page, string selector)
    {
        await page.EvaluateAsync($$"""
            const el = document.querySelector('{{selector}}');
            if (el) {
              el.style.borderRadius = 0;
              el.style.border = '3px solid red';
            } else {
              console.error('Element not found: {{selector}}');
            }
            """);
    }

    public static async Task HighlightElementsAsync(this IPage page, params string[] selectors)
    {
        var array = JsonSerializer.Serialize(selectors, s_options);

        await page.EvaluateAsync($$"""
            for (const selector of {{array}}) {
              const el = document.querySelector(selector);
              if (el) {
                el.style.borderRadius = 0;
                el.style.border = '3px solid red';
              } else {
                console.error('Element not found: ' + selector);
              }
            }
            """);
    }

    public static async Task LoginAsync(this IPage page, string token)
    {
        var response = await page.GotoAsync($"/login?t={token}");

        Assert.NotNull(response);
        Assert.True(response.Ok, $"Failed to navigate to login page: {response.Status}");

        await Assertions.Expect(page).ToHaveURLAsync("/");
    }

    public static async Task LoginAndWaitForCacheResourceAsync(this IPage page, string token)
    {
        await page.LoginAsync(token);

        var cacheResource = page.GetByText(DashboardSelectors.ResourcePage.CacheResource);

        await Assertions.Expect(cacheResource).ToBeVisibleAsync();
    }
}

internal readonly record struct MouseMovement(int X, int Y)
{
    public static implicit operator MouseMovement((int x, int y) tuple) => new(tuple.x, tuple.y);
}
