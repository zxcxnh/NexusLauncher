# NexusLauncher

NexusLauncher is a Windows desktop game library designed for Windows 10 and Windows 11. It helps you organize your installed games, keep them in one place, and launch them directly from a simple launcher interface.

## Overview

This project is built as a WPF application for Windows and focuses on a clean, minimal launcher experience for PC gamers. The app allows you to:

- add games to your library
- store game metadata locally
- launch installed executables directly
- persist settings such as theme and window state
- build a foundation for future features like collections, favorites, discovery, and metadata enrichment

## Features

- Windows 10 / 11 compatible WPF application
- Local SQLite database for game storage
- Simple game library model and management flow
- Add games by selecting an executable
- Save settings in the Windows user profile directory
- Theme support with Dark / Light / System modes
- MVVM architecture with dependency injection
- Unit tests for persistence and database behavior

## Requirements

- Windows 10 or Windows 11
- .NET 10 SDK
- Visual Studio 2022 or later

## Getting Started

1. Clone the repository.
2. Open the solution file in Visual Studio.
3. Restore NuGet packages.
4. Build and run the app.

## Build

```bash
dotnet restore
dotnet build NexusLauncher.sln -c Release
```

## Run

```bash
dotnet run --project src/NexusLauncher/NexusLauncher.csproj
```

## Project Structure

```text
NexusLauncher/
├─ NexusLauncher.sln
├─ src/
│  ├─ NexusLauncher/
│  └─ NexusLauncher.Tests/
└─ README.md
```

## Notes

This project is intended to be a practical local game launcher for Windows users. It is designed to evolve into a richer game library manager over time, while keeping the initial implementation lightweight and easy to maintain.

## License

This project is provided as a starter application for personal and development use.
