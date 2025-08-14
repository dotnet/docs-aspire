---
title: Container runtime 'podman' could not be found in WSL
description: Learn how to troubleshoot the error "Container runtime 'podman' could not be found" when using Podman in Windows Subsystem for Linux (WSL).
ms.date: 08/04/2025
---

# Container runtime 'podman' could not be found in WSL

.NET Aspire requires a container runtime to be available in the system PATH. This article describes how to resolve issues when Podman isn't found in Windows Subsystem for Linux (WSL) environments.

## Symptoms

When starting your .NET Aspire application, you see an error message similar to:

```Output
Container runtime 'podman' could not be found. The error from the container runtime check was: exec: "podman": executable file not found in $PATH
```

This occurs even though running `podman images` or other Podman commands work successfully in your WSL terminal.

## Cause

This issue occurs in WSL environments when:

- Podman is installed in a separate WSL distribution than where your .NET Aspire application is running.
- You're using shell aliases instead of having the actual Podman executable in your PATH.
- The Podman executable isn't available in the system PATH that .NET Aspire searches.

.NET Aspire resolves container runtimes by searching for the executable in the system PATH. Shell aliases (like those defined in `~/.bash_aliases`) aren't recognized during this process.

## Solution

Choose one of the following solutions:

### Install Podman in the current WSL distribution

Install Podman directly in the WSL distribution where you're running your .NET Aspire application:

```bash
# For Ubuntu/Debian-based distributions
sudo apt update
sudo apt install -y podman
```

For other distributions, see [Install Podman on Linux](https://podman.io/docs/installation#installing-on-linux).

### Create a symbolic link

If you have Podman installed elsewhere, create a symbolic link:

```bash
# Find where Podman is installed
which podman-remote-static-linux_amd64

# Create a symbolic link in a directory that's in your PATH
sudo ln -s /path/to/podman-remote-static-linux_amd64 /usr/local/bin/podman
```

### Add Podman directory to PATH

Add the directory containing the Podman executable to your PATH:

```bash
# Add to your shell profile
echo 'export PATH="/path/to/podman/directory:$PATH"' >> ~/.bashrc
source ~/.bashrc
```

## Verify the solution

Confirm that Podman is correctly configured:

```bash
# Check that Podman is in your PATH
which podman

# Verify Podman is working
podman --version

# Test that Podman can list containers
podman ps
```

All commands should succeed before running your .NET Aspire application.

## See also

- [Container runtime setup](../fundamentals/setup-tooling.md#container-runtime)
- [Container runtime appears to be unhealthy](container-runtime-unhealthy.md)
