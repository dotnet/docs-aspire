---
title: .NET Aspire preview 5
description: .NET Aspire preview 5 is now available and includes many improvements and new capabilities
ms.date: 04/02/2024
---

# .NET Aspire preview 5

Breaking Changes

- The major package split, Aspire.*and Aspire.Hosting.*
- AppHost projects now need to reference Aspire.Hosting.AppHost
- Each resource was split into individual packages
  - Aspire.Hosting.Redis
  - Aspire.Hosting.PostgreSQL
  - Aspire.Hosting.MongoDB
  - Aspire.Hosting.RabbitMQ
  - etc.
- The same applies to the Azure resources
- Aspire.Hosting.Azure.Redis (etc)
- Async all the things, various callbacks in the app model APIs
are now async. (GetConnectionStringAsync etc.)

Dashboard

- Support for authentication
  - OTLP
  - UI (Frontend)
- LOTS of performance improvements
  - Console log virtualization
  - Load time improvements
  - Trace ingestion improvements
- Run on a single port (OTLP and UI)
- Standalone container now forces you to choose auth

Templates

- HTTPs by default
- Test project support

AppModel

- Forwarded headers enabled by default for projects with endpoints
- Custom resources support in dashboard
  - Can publish notifications to the dashboard
  - Can log console output to the dashboard
- Built in methods for containers with well known volume mounts
- Add RabbitMQ WithManagementUI method.
- Applied consistent scheme for resources that support passwords
  - They can autogenerate passwords, or take parameters
- Support for composing environment variables using string interpolation
that can capture resource properties. (ReferenceExpression, WithEnvironment overload)

Service Discovery

- Service discovery API changes
- Service discovery auto scheme detection

Tooling

- VS Code support
- Prompting for parameters in Visual Studio

Components

- Azure Events Hubs
- Renamed all of the methods to end with *Client e.g. AddRedisClient
- Nats OpenTelemetry support

Azure

- Azure CDK Support (introducing the new Azure CDK)
  - All Azure resources were refactored to use the CDK
- Azure Provisioning for Development
- Azure OpenAI provisioning

Manifest

- Express container volumes and bindmounts in the manifest
- Support for multiple endpoints in the manifest
- Renamed containerPort to targetPort
- Added port as the "exposed port"

Azure Deployment

- Service selection prompt gone (WithExternalHttpEndpoints in apphost)
- Support for multiple endpoints in ACA
- Support for adding volumes to containers in ACA

- IDE protocol changes
- There's a new IDE protcol
