---
title: Stack Exchange Redis caching overview
description: Learn about Stack Exchange Redis caching and how to use it in your applications.
ms.date: 08/12/2024
---

# Stack Exchange Redis caching overview

With .NET Aspire, there are several ways to use caching in your applications. One popular option is to use [Stack Exchange Redis](https://stackexchange.github.io/StackExchange.Redis), which is a high-performance data store that can be used to store frequently accessed data. This article provides an overview of Stack Exchange Redis caching and links to resources that help you use it in your applications.

To use multiple Redis caching integrations in your application, see [Tutorial: Implement caching with .NET Aspire integrations](caching-integrations.md). If you're interested in using the Redis Cache for Azure, see [Tutorial: Deploy a .NET Aspire project with a Redis Cache to Azure](caching-integrations-deployment.md).

## Redis serialization protocol (RESP)

The Redis serialization protocol (RESP) is a binary-safe protocol that Redis uses to communicate with clients. RESP is a simple, text-based protocol that is easy to implement and efficient to parse. RESP is used to send commands to Redis and receive responses from Redis. RESP is designed to be fast and efficient, making it well-suited for use in high-performance applications. For more information, see [Redis serialization protocol specification](https://redis.io/docs/latest/develop/reference/protocol-spec/).

In addition to Redis itself, there are two well-maintained implementations of RESP for .NET:

- [Garnet](https://github.com/microsoft/Garnet): Garnet is a remote cache-store from Microsoft Research that offers strong performance (throughput and latency), scalability, storage, recovery, cluster sharding, key migration, and replication features. Garnet can work with existing Redis clients.
- [Valkey](https://github.com/valkey-io/valkey): A flexible distributed key-value datastore that supports both caching and beyond caching workloads.

.NET Aspire lets you easily model either the Redis, Garnet, or Valkey RESP protocol in your applications and you can choose which one to use based on your requirements. All of the .NET Aspire Redis integrations can be used with either the Redis, Garnet, or Valkey RESP protocol.

## Caching

Caching is a technique used to store frequently accessed data in memory. This helps to reduce the time it takes to retrieve the data from the original source, such as a database or a web service. Caching can significantly improve the performance of an application by reducing the number of requests made to the original source. To access the Redis `IConnectionMultiplexer` object, you use the `Aspire.StackExchange.Redis` NuGet package:

> [!div class="nextstepaction"]
> [.NET Aspire Stack Exchange Redis integration](stackexchange-redis-integration.md)

> [!div class="nextstepaction"]
> [.NET Aspire Stack Exchange Redis integration (Garnet)](stackexchange-redis-integration.md?pivots=garnet)

> [!div class="nextstepaction"]
> [.NET Aspire Stack Exchange Redis integration (Valkey)](stackexchange-redis-integration.md?pivots=valkey)

## Distributed caching

Distributed caching is a type of caching that stores data across multiple servers. This allows the data to be shared between multiple instances of an application, which can help to improve scalability and performance. Distributed caching can be used to store a wide variety of data, such as session state, user profiles, and frequently accessed data. To use Redis distributed caching in your application (the `IDistributedCache` interface), use the `Aspire.StackExchange.Redis.DistributedCaching` NuGet package:

> [!div class="nextstepaction"]
> [.NET Aspire Stack Exchange Redis distributed caching integration](stackexchange-redis-distributed-caching-integration.md)

> [!div class="nextstepaction"]
> [.NET Aspire Stack Exchange Redis distributed caching integration (Garnet)](stackexchange-redis-distributed-caching-integration.md?pivots=garnet)

> [!div class="nextstepaction"]
> [.NET Aspire Stack Exchange Redis distributed caching integration (Valkey)](stackexchange-redis-distributed-caching-integration.md?pivots=valkey)

## Output caching

Output caching is a type of caching that stores the output of a web page or API response. This allows the response to be served directly from the cache, rather than generating it from scratch each time. Output caching can help to improve the performance of a web application by reducing the time it takes to generate a response. To use declarative Redis output caching with either the `OutputCache` attribute or the `CacheOutput` method in your application, use the `Aspire.StackExchange.Redis.OutputCaching` NuGet package:

> [!div class="nextstepaction"]
> [.NET Aspire Stack Exchange Redis output caching integration](stackexchange-redis-output-caching-integration.md)

> [!div class="nextstepaction"]
> [.NET Aspire Stack Exchange Redis output caching integration (Garnet)](stackexchange-redis-output-caching-integration.md?pivots=garnet)

> [!div class="nextstepaction"]
> [.NET Aspire Stack Exchange Redis output caching integration (Valkey)](stackexchange-redis-output-caching-integration.md?pivots=valkey)

## See also

- [Caching in .NET](/dotnet/core/extensions/caching)
- [Overview of Caching in ASP.NET Core](/aspnet/core/performance/caching/overview)
- [Distributed caching in .NET](/dotnet/core/extensions/caching#distributed-caching)
- [Distributed caching in ASP.NET Core](/aspnet/core/performance/caching/distributed)
- [Output caching middleware in ASP.NET Core](/aspnet/core/performance/caching/output)
