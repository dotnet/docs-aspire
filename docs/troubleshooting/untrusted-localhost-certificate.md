---
title: Troubleshoot untrusted localhost certificate in .NET Aspire
description: Explore strategies for troubleshooting issues when working with untrusted localhost certificates in .NET Aspire.
ms.date: 05/14/2024
---

# Troubleshoot untrusted localhost certificate in .NET Aspire

This article provides guidance on how to troubleshoot issues that you might encounter when working with untrusted localhost certificates in .NET Aspire.

## Symptoms

Several .NET Aspire templates include ASP.NET Core projects that are configured to use HTTPS by default. If this is the first time you're running the project, ASP.NET Core prompts you to install a localhost certificate. There are situations in which you trust/install the development certificate, but you don't close all your browser windows.

## Possible solution

Close all browser windows. Then attempt to resolve this issue by trusting the self-signed development certificate. To trust the certificate, run the following commands. First, remove the existing certificates:

```dotnetcli
dotnet dev-certs https --clean
```

Next, trust the certificate:

```dotnetcli
dotnet dev-certs https --trust
```

## See also

- [Trust the ASP.NET Core HTTPS development certificate on Windows and macOS](/aspnet/core/security/enforcing-ssl#trust-the-aspnet-core-https-development-certificate-on-windows-and-macos)
- [Trust HTTPS certificate on Linux](/aspnet/core/security/enforcing-ssl##trust-https-certificate-on-linux)
- [.NET CLI: dotnet dev-certs](/dotnet/core/tools/dotnet-dev-certs)
