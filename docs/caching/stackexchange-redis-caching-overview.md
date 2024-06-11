---
title: Stack Exchange Redis caching overview
description: Learn about Stack Exchange Redis caching and how to use it in your applications.
ms.date: 06/11/2024
---

# Stack Exchange Redis caching overview

With .NET Aspire, there are several ways to use caching in your applications. One popular option is to use [Stack Exchange Redis](https://stackexchange.github.io/StackExchange.Redis), which is a high-performance data store that can be used to store frequently accessed data. This article provides an overview of Stack Exchange Redis caching and links to resources that help you use it in your applications.

To use multiple Redis caching components in your application, see [Tutorial: Implement caching with .NET Aspire components](caching-components.md). If you're interested in using the Redis Cache for Azure, see [Tutorial: Deploy a .NET Aspire app with a Redis Cache to Azure](caching-components-deployment.md).

## Caching

Caching is a technique used to store frequently accessed data in memory. This helps to reduce the time it takes to retrieve the data from the original source, such as a database or a web service. Caching can significantly improve the performance of an application by reducing the number of requests made to the original source. To access the Redis `IConnectionMultiplexer` object, you use the `Aspire.StackExchange.Redis` NuGet package:

> [!div class="nextstepaction"]
> [.NET Aspire Stack Exchange Redis component](stackexchange-redis-component.md)

## Distributed caching

Distributed caching is a type of caching that stores data across multiple servers. This allows the data to be shared between multiple instances of an application, which can help to improve scalability and performance. Distributed caching can be used to store a wide variety of data, such as session state, user profiles, and frequently accessed data. To use Redis distributed caching in your application (the `IDistributedCache` interface), use the `Aspire.StackExchange.Redis.DistributedCaching` NuGet package:

> [!div class="nextstepaction"]
> [.NET Aspire Stack Exchange Redis distributed caching component](stackexchange-redis-distributed-caching-component.md)

## Output caching

Output caching is a type of caching that stores the output of a web page or API response. This allows the response to be served directly from the cache, rather than generating it from scratch each time. Output caching can help to improve the performance of a web application by reducing the time it takes to generate a response. To use declarative Redis output caching with either the `OutputCache` attribute or the `CacheOutput` method in your application, use the `Aspire.StackExchange.Redis.OutputCaching` NuGet package:

> [!div class="nextstepaction"]
> [.NET Aspire Stack Exchange Redis output caching component](stackexchange-redis-output-caching-component.md)

## See also

- [Caching in .NET](/dotnet/core/extensions/caching)
- [Overview of Caching in ASP.NET Core](/aspnet/core/performance/caching/overview)
- [Distributed caching in .NET](/dotnet/core/extensions/caching#distributed-caching)
- [Distributed caching in ASP.NET Core](/aspnet/core/performance/caching/distributed)
- [Output caching middleware in ASP.NET Core](/aspnet/core/performance/caching/output)
