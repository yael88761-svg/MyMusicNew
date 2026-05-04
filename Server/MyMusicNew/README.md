# MyMusicNew - Music Management API

ASP.NET Core REST API for music, playlists, and user management with JWT authentication.

## Quick Start

```bash
# Restore & build
dotnet restore && dotnet build

# Configure connection string in appsettings.json
# Apply migrations
dotnet ef database update --project DataContext

# Run
cd MyMusicNew && dotnet run
```

API: `http://localhost:5000` | Swagger: `http://localhost:5000/swagger`

## Architecture

```
MyMusicNew/          → Controllers (API endpoints)
DataContext/         → EF Core DbContext & Migrations
Repositories/        → Entities, Repositories, Interfaces
Service/             → Business logic, DTOs, Services
```

## Features

- **User Management**: Registration, authentication, profiles
- **Music Management**: Upload, manage songs with metadata
- **Playlists**: Create and manage custom playlists
- **Play History**: Track user listening activity
- **JWT Auth**: Secure API access with bearer tokens
- **Swagger Docs**: Interactive API documentation

## Main Endpoints

| Resource | GET | POST | PUT | DELETE |
|----------|-----|------|-----|--------|
| `/api/song` | List songs | Upload | Update | Delete |
| `/api/playlist` | List playlists | Create | Update | Delete |
| `/api/user` | List users | Register | Update | Delete |
| `/api/playhistory` | Get history | Log play | - | Delete |
| `/api/account` | - | Login/Register | - | - |

## Configuration

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\MSSQLLocalDB;Database=MusicDatabase;Trusted_Connection=true;"
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-chars",
    "Issuer": "YourIssuer",
    "Audience": "YourAudience"
  }
}
```

## Authentication

1. Register: `POST /api/account/register` with email & password
2. Login: `POST /api/account/login` to get JWT token
3. Use token: Add `Authorization: Bearer <token>` header to requests

## Database

Tables: Users, Songs, Playlists, PlaylistSongs, PlayHistories, AudioFeatures

Apply migrations:
```bash
dotnet ef database update --project DataContext
```

## Requirements

- .NET 8+
- SQL Server 2019+
- Visual Studio 2022 / VS Code

---

**Status**: In Development | **Version**: 1.0.0
