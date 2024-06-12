---
title: Troubleshoot untrusted localhost certificate in .NET Aspire
description: Explore strategies for troubleshooting issues when working with untrusted localhost certificates in .NET Aspire.
ms.date: 05/15/2024
---

# Troubleshoot untrusted localhost certificate in .NET Aspire

This article provides guidance on how to troubleshoot issues that you might encounter when working with untrusted localhost certificates in .NET Aspire.

## Symptoms

Several .NET Aspire templates include ASP.NET Core projects that are configured to use HTTPS by default. If this is the first time you're running the project, and you're using Visual Studio, you're prompted to install a localhost certificate.

- There are situations in which you trust/install the development certificate, but you don't close all your browser windows. In these cases, your browser might indicate that the certificate isn't trusted.

- There are also situations where you don't trust the certificate at all. In these cases, your browser might indicate that the certificate isn't trusted.

Additionally, there are warning messages from Kestrel written to the console that indicate that the certificate is not trusted.

## Possible solutions

Close all browser windows and try again. If you're still experiencing the issue, then attempt to resolve this by trusting the self-signed development certificate with the .NET CLI. To trust the certificate, run the following commands. First, remove the existing certificates.

> [!NOTE]
> This will remove all existing development certificates on the local machine.

```dotnetcli
dotnet dev-certs https --clean
```

To trust the certificate:

```dotnetcli
dotnet dev-certs https --trust
```

For more troubleshooting, see [Troubleshoot certificate problems such as certificate not trusted](/aspnet/core/security/enforcing-ssl#troubleshoot-certificate-problems-such-as-certificate-not-trusted).

## See also

- [Trust the ASP.NET Core HTTPS development certificate on Windows and macOS](/aspnet/core/security/enforcing-ssl#trust-the-aspnet-core-https-development-certificate-on-windows-and-macos)
- [Trust HTTPS certificate on Linux](/aspnet/core/security/enforcing-ssl##trust-https-certificate-on-linux)
- [.NET CLI: dotnet dev-certs](/dotnet/core/tools/dotnet-dev-certs)
- [Trust localhost certificate on Linux](https://github.com/dotnet/aspnetcore/issues/32842)
