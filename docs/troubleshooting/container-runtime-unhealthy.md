---
title: Container runtime appears to be unhealthy
description: Learn how to troubleshoot the error "Container runtime 'docker' was found but appears to be unhealthy" during execution of your app.
ms.date: 07/03/2024
---

# Container runtime appears to be unhealthy

.NET Aspire requires Docker (or Podman) to be running and healthy. This topic describes a possible symptom you may see if Docker isn’t in a healthy state.

## Symptoms

When starting the AppHost the dashboard doesn't show up and an exception stack trace similar to this example is displayed in the console:

```Output
info: Aspire.Hosting.DistributedApplication[0]
      Aspire version: 8.1.0-dev
info: Aspire.Hosting.DistributedApplication[0]
      Distributed application starting.
info: Aspire.Hosting.DistributedApplication[0]
      Application host directory is: D:\aspire\playground\PostgresEndToEnd\PostgresEndToEnd.AppHost
fail: Microsoft.Extensions.Hosting.Internal.Host[11]
      Hosting failed to start
      Aspire.Hosting.DistributedApplicationException: Container runtime 'docker' was found but appears to be unhealthy. The error from the container runtime check was error during connect: this error may indicate that the docker daemon is not running: Get "http://%2F%2F.%2Fpipe%2Fdocker_engine/v1.45/containers/json?limit=1": open //./pipe/docker_engine: The system cannot find the file specified..
```

## Possible solutions

Confirm that Docker is installed and running:

- On Windows, check that in the system tray the Docker icon is present and marked as "Running".
- On Linux, check that `docker ps -a` returns success.
