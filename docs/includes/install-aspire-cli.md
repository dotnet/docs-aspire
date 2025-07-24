---
title: Install the Aspire CLI
ms.date: 07/24/2025
---

To install the Aspire CLI:

1. Open a terminal.
1. Run the following command to install the Aspire CLI:

    ### [Unix](#tab/unix)

    ```sh
    curl -sSL https://aspire.dev/install.sh | bash
    ```

    #### Script options

    The Unix install script (`install.sh`) supports the following options:

    | Parameter | Short form | Description | Default value |
    |-----------|------------|-------------|---------------|
    | `--install-path` | `-i` | Directory to install the CLI | `$HOME/.aspire/bin` |
    | `--version` | | Version of the Aspire CLI to download | `9.4` |
    | `--quality` | `-q` | Quality to download (`dev`, `staging`, `release`) | `release` |
    | `--os` | | Operating system (auto-detected if not specified) | auto-detect |
    | `--arch` | | Architecture (auto-detected if not specified) | auto-detect |
    | `--keep-archive` | `-k` | Keep downloaded archive files after installation | `false` |
    | `--verbose` | `-v` | Enable verbose output | `false` |
    | `--help` | `-h` | Show help message | |

    #### Quality options

    The `--quality` option determines which build of the Aspire CLI to install:

    - **`dev`**: Latest builds from the `main` branch (development builds).
    - **`staging`**: Builds from the current release branch (pre-release builds).
    - **`release`**: Latest generally available release (stable builds).

    #### Usage examples

    Install to a custom directory:

    ```sh
    curl -sSL https://aspire.dev/install.sh | bash -s -- --install-path "/usr/local/bin"
    ```

    Install with verbose output:

    ```sh
    curl -sSL https://aspire.dev/install.sh | bash -s -- --verbose
    ```

    Install a specific version:

    ```sh
    curl -sSL https://aspire.dev/install.sh | bash -s -- --version "9.4"
    ```

    Install development builds:

    ```sh
    curl -sSL https://aspire.dev/install.sh | bash -s -- --quality "dev"
    ```

    #### Default installation path

    The script installs the Aspire CLI to `$HOME/.aspire/bin` by default.

    ### [Windows](#tab/windows)

    ```powershell
    Invoke-Expression "& { $(Invoke-RestMethod https://aspire.dev/install.ps1) }"
    ```

    #### Script options

    The Windows install script (`install.ps1`) supports the following options:

    | Parameter | Description | Default value |
    |-----------|-------------|---------------|
    | `-InstallPath` | Directory to install the CLI | `%USERPROFILE%\.aspire\bin` |
    | `-Version` | Version of the Aspire CLI to download | `9.4` |
    | `-Quality` | Quality to download (`dev`, `staging`, `release`) | `release` |
    | `-OS` | Operating system (auto-detected if not specified) | auto-detect |
    | `-Architecture` | Architecture (auto-detected if not specified) | auto-detect |
    | `-KeepArchive` | Keep downloaded archive files after installation | `false` |
    | `-Help` | Show help message | |

    #### Quality options

    The `-Quality` option determines which build of the Aspire CLI to install:

    - **`dev`**: Latest builds from the `main` branch (development builds).
    - **`staging`**: Builds from the current release branch (pre-release builds).
    - **`release`**: Latest generally available release (stable builds).

    #### Usage examples

    Install to a custom directory:

    ```powershell
    Invoke-Expression "& { $(Invoke-RestMethod https://aspire.dev/install.ps1) } -InstallPath 'C:\Tools\Aspire'"
    ```

    Install with verbose output:

    ```powershell
    Invoke-Expression "& { $(Invoke-RestMethod https://aspire.dev/install.ps1) } -Verbose"
    ```

    Install a specific version:

    ```powershell
    Invoke-Expression "& { $(Invoke-RestMethod https://aspire.dev/install.ps1) } -Version '9.4'"
    ```

    Install development builds:

    ```powershell
    Invoke-Expression "& { $(Invoke-RestMethod https://aspire.dev/install.ps1) } -Quality 'dev'"
    ```

    #### Default installation path

    The script installs the Aspire CLI to `%USERPROFILE%\.aspire\bin` by default.

    ---
