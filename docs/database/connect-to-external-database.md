---
title: Connect a .NET Aspire microservice to an external database
description: Learn how to configure a .NET Aspire solution with a connection to an external database that isn't hosted in a .NET Aspire container.
ms.date: 03/07/2025
ms.topic: tutorial
uid: database/connect-to-external-database
---

# Tutorial: Connect a .NET Aspire microservice to an external database

.NET Aspire is designed to make it easy and quick to develop cloud-native solutions. It uses containers to host the services, such as databases, that underpin each microservice. However, if you want your microservice to query a database that already exists, you must connect your microservice to it instead of creating a database container whenever you run the solution.

In this tutorial, you create a .NET Aspire solution with an API that connects to an external database. You'll learn how to:

> [!div class="checklist"]
>
> - Create an API microservice that uses Entity Framework Core (EF Core) to interact with a database.
> - Configure the .NET Aspire App Host project with a connection string for the external database.
> - Pass the connection string to the API and use it to connect to the database.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

> [!IMPORTANT]
> This tutorial also assumes you have a Microsoft SQL Server instance running on your local machine. You can connect to a database elsewhere by providing an appropriate connection string instead of the one suggested in this article. To create a new local instance, download and install [SQL Server Developer Edition](https://www.microsoft.com/sql-server/sql-server-downloads).

> [!TIP]
> In this tutorial, you use EF Core with SQL Server. Other database integrations, including both those that use EF Core and others, can use the same approach to connect to an external database.

## Create a new .NET Aspire solution

