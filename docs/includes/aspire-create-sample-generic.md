## Create the sample solution

Visual Studio provides app templates to get started with Aspire that handle some of the initial setup configurations for you. Complete the following steps to properly set up a project for this article:

1. At the top of Visual Studio, navigate to **File** > **New** > **Project**.
1. In the dialog window, search for *ASP.NET Core* and select **ASP.NET Core Web App**. Choose **Next**.
1. On the **Configure your new project** screen:
    - Enter a **Project Name** of **AspireSample**.
    - Leave the rest of the values at their defaults and select **Next**.
1. On the **Additional information** screen:
    - Make sure **.NET 9.0** is selected.
    - Ensure that **Enlist in Aspire orchestration** is checked and select **Next**.

Visual Studio will create a new ASP.NET Core solution that is structured to use Aspire. The solution consists of the following projects:

- **AspireSample** - A Razor Pages UI project with default Aspire service configurations.
- **AspireSample.AppHost** - An orchestrator project designed to connect and configure the different projects and services of your app. The orchestrator should be set as the startup project.
- **AspireSample.Shared** - A shared class library to hold code that can be reused across the projects in your solution.

You also need to add a `WeatherForecast` class to the **AspireSample** app, which will be used later to hold data retrieved from the API.

1. Right-click on the **AspireSample** app and select **Add** > **Class**.
1. Name the class *WeatherForecast* and select **Add**.

Replace the contents of the `WeatherForecast` class with the following:

```csharp
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
```
