---
title: Container runtime 'podman' could not be found in WSL
description: Learn how to troubleshoot the error "Container runtime 'podman' could not be found" when using Podman in Windows Subsystem for Linux (WSL).
ms.date: 08/04/2025
---

# Container runtime 'podman' could not be found in WSL

When using Podman with Windows Subsystem for Linux (WSL), you might encounter issues where .NET Aspire can't find the Podman container runtime, even though Podman commands work in your terminal.

## Symptoms

When starting your .NET Aspire application, you see an error message similar to:

```Output
Container runtime 'podman' could not be found. The error from the container runtime check was: exec: "podman": executable file not found in $PATH
```

This occurs even though running `podman images` or other Podman commands work successfully in your WSL terminal.

## Root cause

This issue typically occurs in WSL environments when:

1. **Podman is installed in a separate WSL distribution** than where your .NET Aspire application is running.
1. **You're using shell aliases** instead of having the actual Podman executable in your PATH.
1. **The Podman executable isn't available in the system PATH** that .NET Aspire searches.

.NET Aspire resolves container runtimes by searching for the executable in the system PATH. Shell aliases (like those defined in `~/.bash_aliases`) aren't recognized during this process.

## Solutions

### Solution 1: Install Podman in the current WSL distribution

The most straightforward solution is to install Podman directly in the WSL distribution where you're running your .NET Aspire application:

```bash
# For Ubuntu/Debian-based distributions
sudo apt update
sudo apt install -y podman

# For other distributions, see: https://podman.io/docs/installation#installing-on-linux
```

### Solution 2: Create a symbolic link

If you have Podman installed elsewhere and want to make it available in your current distribution:

```bash
# Find where Podman is installed (example path)
which podman-remote-static-linux_amd64

# Create a symbolic link in a directory that's in your PATH
sudo ln -s /path/to/podman-remote-static-linux_amd64 /usr/local/bin/podman
```

### Solution 3: Add to PATH

If you know the directory containing the Podman executable, add it to your PATH:

```bash
# Add to your shell profile (example for bash)
echo 'export PATH="/path/to/podman/directory:$PATH"' >> ~/.bashrc
source ~/.bashrc
```

### Solution 4: Use environment variable

Set the `ASPIRE_CONTAINER_RUNTIME` environment variable to help .NET Aspire locate Podman:

```bash
export ASPIRE_CONTAINER_RUNTIME=podman
```

## Verify the solution

After applying any of the solutions above, verify that Podman is correctly configured:

```bash
# Check that Podman is in your PATH
which podman

# Verify Podman is working
podman --version

# Test that Podman can list containers (should not error)
podman ps
```

All commands should succeed before running your .NET Aspire application.

## Additional considerations

- **Aliases don't work**: Shell aliases like `alias podman='podman-remote-static-linux_amd64'` won't be recognized by .NET Aspire's executable resolution process.
- **Distribution isolation**: Each WSL distribution has its own filesystem and installed packages. If you switch distributions, you might need to reconfigure Podman.
- **Permission issues**: Ensure that the user running the .NET Aspire application has permission to execute the Podman binary.

## See also

- [Container runtime setup](../fundamentals/setup-tooling.md#container-runtime)
- [Install Podman on Linux](https://podman.io/docs/installation#installing-on-linux)
- [Container runtime appears to be unhealthy](container-runtime-unhealthy.md)
