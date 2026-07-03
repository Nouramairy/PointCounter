# PointCounter

A full-stack web application for managing sports teams, tracking game statistics, and running real-time quick-match point counters. Built with ASP.NET Core 9 and Angular 21.

---

## Features

| Feature | Description |
|---------|-------------|
| **Player Management** | Create and manage player profiles with name, age, address, and phone |
| **Team Management** | Organize players into teams with configurable roster limits |
| **Game Management** | Create games, assign participating teams, and manage matchups |
| **Scoreboard** | Track and view scores per team per game |
| **Quick Match** | Spin up a shareable real-time point counter with live score updates via WebSockets |

---

## System Design

### Overview

The application uses a **monorepo** layout where the Angular frontend is compiled directly into the .NET backend's `wwwroot` folder. The single .NET process serves both the REST API and the Angular SPA — no separate frontend server is needed in production.

```
Browser
  └── http://localhost:5092
        ├── /               → Angular SPA (static files from wwwroot)
        ├── /api/**         → ASP.NET Core REST API
        └── /hubs/**        → SignalR WebSocket endpoint
```

### Project Structure

```
PointCounter/
├── Backend/
│   └── pointCounterBackend/
│       ├── Controllers/         REST API controllers
│       ├── DTOs/                Request and response shapes
│       ├── Entities/            EF Core domain models
│       ├── Services/            Business logic layer
│       │   └── Interfaces/      Service contracts
│       ├── Hubs/                SignalR WebSocket hub
│       ├── Data/                DbContext and EF Core configuration
│       ├── Migrations/          EF Core database migrations
│       └── wwwroot/             Angular production build output (auto-generated)
└── FrontEnd/
    └── Point-Counter/
        └── src/app/
            ├── components/pages/  Feature pages (Home, Games, Players, Teams, Match, Scoreboard)
            ├── models/            TypeScript interfaces matching backend DTOs
            ├── services/          HTTP client services and the SignalR hub service
            └── utils/             Shared helpers
```

### Request Flow

```
User action in Angular
        │
        ▼
Angular Service (HTTP)
        │
        ▼
ASP.NET Core Controller
        │
        ▼
Service Layer (business logic)
        │
        ▼
Entity Framework Core
        │
        ▼
SQL Server (LocalDB)
```

### Real-Time Score Updates (Quick Match)

Score changes in Quick Match are broadcast to all connected viewers using SignalR:

```
User clicks "+1" in browser
        │
        ▼
PUT /api/pointmatches/{publicId}/players/{id}/score
        │
        ▼
PointMatchService.UpdateScoreAsync() → saves to database
        │
        ▼
IHubContext<PointMatchHub>.SendAsync("MatchUpdated", match)
        │
        ▼
All clients subscribed to that match group receive the update instantly
        │
        ▼
Angular PointMatchHubService emits on matchUpdates$ observable
        │
        ▼
Match page re-renders with new scores — no page refresh needed
```

---

## Database Schema

| Table | Key Columns |
|-------|-------------|
| `Players` | Id, Name, Age, Address, Phone, CreatedAt, UpdatedAt |
| `Teams` | Id, Name, MaximumPlayersAllowed, CreatedAt, UpdatedAt |
| `Games` | Id, Name, Duration, CreatedAt, UpdatedAt |
| `Scoreboards` | Id, GameId (FK), TeamId (FK), Score, CreatedAt, UpdatedAt |
| `TeamPlayers` | (TeamId, PlayerId) — many-to-many join |
| `GameTeams` | (GameId, TeamId) — many-to-many join |
| `PointMatches` | Id, PublicId (unique GUID), Name, HigherScoreWins, StartingScore, PlayersLocked, CreatedAt, UpdatedAt |
| `PointMatchPlayers` | Id, PointMatchId (FK), Name, Score, OriginalScore |

> `PointMatch.PublicId` is a GUID used in shareable URLs instead of the internal integer ID, so users can share a match link without exposing database identifiers.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend framework | ASP.NET Core 9 |
| ORM | Entity Framework Core 9 |
| Database | SQL Server (LocalDB for development) |
| Real-time | SignalR (WebSockets) |
| Frontend framework | Angular 21 |
| Language | C# / TypeScript |
| API explorer | Swagger / Swashbuckle (development only) |

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+ and npm](https://nodejs.org/)
- SQL Server LocalDB — bundled with Visual Studio or the [SQL Server Express installer](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

---

## Getting Started

### 1. Apply the database migration

```bash
cd Backend/pointCounterBackend
dotnet ef database update
```

This creates the `PointCounterLocalDb` database on your LocalDB instance.

### 2. Run the application

```bash
cd Backend/pointCounterBackend
dotnet run
```

On **first run**, the .NET build process automatically installs Angular npm packages and compiles the frontend. This takes approximately 30–60 seconds. Subsequent runs only rebuild the Angular app (not reinstall packages).

Once started, open your browser and navigate to:

```
http://localhost:5092
```

> The Swagger API explorer is available at `http://localhost:5092/swagger` when running in Development mode.

### Development with hot reloading

If you are actively working on the Angular frontend and want live reload, run both processes side by side:

```bash
# Terminal 1 — .NET backend only (skip Angular build)
cd Backend/pointCounterBackend
dotnet watch

# Terminal 2 — Angular dev server with proxy to backend
cd FrontEnd/Point-Counter
ng serve
```

Then visit `http://localhost:4200`. The Angular dev server proxies `/api` and `/hubs` requests to the .NET backend on port 5092.

---

## API Reference

### Players — `/api/players`

| Method | Endpoint | Body | Description |
|--------|----------|------|-------------|
| GET | `/api/players` | — | List all players |
| GET | `/api/players/{id}` | — | Get player by ID |
| POST | `/api/players` | `{ name, age, address, phone }` | Create a player |
| PUT | `/api/players/{id}` | `{ name, age, address, phone }` | Update a player |
| DELETE | `/api/players/{id}` | — | Delete a player |

### Teams — `/api/teams`

| Method | Endpoint | Body | Description |
|--------|----------|------|-------------|
| GET | `/api/teams` | — | List all teams |
| GET | `/api/teams/{id}` | — | Get team by ID |
| POST | `/api/teams` | `{ name, maximumPlayersAllowed, playerIds }` | Create a team |
| PUT | `/api/teams/{id}` | `{ name, maximumPlayersAllowed }` | Update a team |
| DELETE | `/api/teams/{id}` | — | Delete a team |
| POST | `/api/teams/{id}/players/{playerId}` | — | Add player to team |
| DELETE | `/api/teams/{id}/players/{playerId}` | — | Remove player from team |

### Games — `/api/games`

| Method | Endpoint | Body | Description |
|--------|----------|------|-------------|
| GET | `/api/games` | — | List all games |
| GET | `/api/games/{id}` | — | Get game by ID |
| POST | `/api/games` | `{ name, duration, teamIds }` | Create a game |
| PUT | `/api/games/{id}` | `{ name, duration }` | Update a game |
| DELETE | `/api/games/{id}` | — | Delete a game |
| POST | `/api/games/{gameId}/teams/{teamId}` | — | Add team to game |
| DELETE | `/api/games/{gameId}/teams/{teamId}` | — | Remove team from game |

### Scoreboards — `/api/scoreboards`

| Method | Endpoint | Body | Description |
|--------|----------|------|-------------|
| GET | `/api/scoreboards` | — | List all scoreboard entries |
| PUT | `/api/scoreboards/{id}` | `{ score }` | Update a score |

### Quick Match — `/api/pointmatches`

| Method | Endpoint | Body | Description |
|--------|----------|------|-------------|
| POST | `/api/pointmatches` | `{ name, higherScoreWins, startingScore, playersLocked, playerNames }` | Create a match |
| GET | `/api/pointmatches/{publicId}` | — | Get current match state |
| PUT | `/api/pointmatches/{publicId}/players/{id}/score` | `{ score }` | Update a player's score |
| POST | `/api/pointmatches/{publicId}/players` | `{ name }` | Add a player |
| PUT | `/api/pointmatches/{publicId}/players/{id}/name` | `{ name }` | Rename a player |
| DELETE | `/api/pointmatches/{publicId}/players/{id}` | — | Remove a player |
| POST | `/api/pointmatches/{publicId}/restart` | — | Reset all scores to starting score |
| POST | `/api/pointmatches/{publicId}/clone` | — | Duplicate the match with reset scores |

### SignalR — `/hubs/point-matches`

Clients join a per-match group to receive live score updates.

| Direction | Method | Payload |
|-----------|--------|---------|
| Client → Server | `JoinMatch(publicId)` | Subscribe to a match group |
| Client → Server | `LeaveMatch(publicId)` | Unsubscribe from a match group |
| Server → Client | `MatchUpdated` | Full updated `PointMatch` object |

---

## Configuration

The database connection string is configured in `Backend/pointCounterBackend/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=PointCounterLocalDb;Trusted_Connection=True;..."
  }
}
```

Replace the connection string with your SQL Server instance if you are not using LocalDB.
