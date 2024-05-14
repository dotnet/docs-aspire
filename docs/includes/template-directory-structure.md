---
title: .NET Aspire Starter Application directory structure
description: An include file that contains the directory structure of the .NET Aspire Starter Application.
ms.topic: include
ms.date: 05/13/2024
---

```Directory
└───📂 AspireSample
     ├───📂 AspireSample.ApiService
     │    ├───📂 Properties
     │    │    └─── launchSettings.json
     │    ├─── appsettings.Development.json
     │    ├─── appsettings.json
     │    ├─── AspireSample.ApiService.csproj
     │    └─── Program.cs
     ├───📂 AspireSample.AppHost
     │    ├───📂 Properties
     │    │    └─── launchSettings.json
     │    ├─── appsettings.Development.json
     │    ├─── appsettings.json
     │    ├─── AspireSample.AppHost.csproj
     │    └─── Program.cs
     ├───📂 AspireSample.ServiceDefaults
     │    ├─── AspireSample.ServiceDefaults.csproj
     │    └─── Extensions.cs
     ├───📂 AspireSample.Web
     │    ├───📂 Components
     │    │    ├───📂 Layout
     │    │    │    ├─── MainLayout.razor
     │    │    │    ├─── MainLayout.razor.css
     │    │    │    ├─── NavMenu.razor
     │    │    │    └─── NavMenu.razor.css
     │    │    ├───📂 Pages
     │    │    │    ├─── Counter.razor
     │    │    │    ├─── Error.razor
     │    │    │    ├─── Home.razor
     │    │    │    └─── Weather.razor
     │    │    ├─── _Imports.razor
     │    │    ├─── App.razor
     │    │    └─── Routes.razor
     │    ├───📂 Properties
     │    │    └─── launchSettings.json
     │    ├───📂 wwwroot
     │    │    ├───📂 bootstrap
     │    │    │    ├─── bootstrap.min.css
     │    │    │    └─── bootstrap.min.css.map
     │    │    ├─── app.css
     │    │    └─── favicon.png
     │    ├─── appsettings.Development.json
     │    ├─── appsettings.json
     │    ├─── AspireSample.Web.csproj
     │    ├─── Program.cs
     │    └─── WeatherApiClient.cs
     └─── AspireSample.sln
```
