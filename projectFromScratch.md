# Building PointCounter from Scratch

This document covers the key decisions, setup steps, and things to consider if you were starting this project fresh today.

---

## 1. Understand the scope before you write a line of code

Before scaffolding anything, map out the two distinct feature areas this application covers:

- **Team management system**: Players, Teams, Games, and Scoreboards — classic CRUD with relational data.
- **Quick Match**: A shareable, real-time point counter that anyone with the link can view live.

These two areas have very different requirements. The first needs a proper relational schema with foreign keys and join tables. The second needs a simple schema but adds a WebSocket layer. Knowing this upfront shapes every architectural decision that follows.

---

## 2. Choose your architecture: who serves the frontend?

There are two common approaches for a .NET + Angular project:

**Option A — Single server (this project's approach)**
The Angular app is compiled to static files and placed in the .NET project's `wwwroot` folder. The .NET process serves everything — the REST API, the SignalR hub, and the Angular files — from one port.

- **Pro**: One process to run, simple deployment, no CORS complexity in production.
- **Con**: Every code change requires rebuilding the Angular app before you see it via the .NET server. Use `ng serve` with a proxy during active frontend development to work around this.

**Option B — Separate servers**
Run the Angular dev server on port 4200 and the .NET API on port 5092 all the time, configure CORS to allow `localhost:4200`, and deploy them independently.

- **Pro**: Clean separation, standard for teams where frontend and backend are separate concerns.
- **Con**: Two processes to manage, CORS must be configured for both development and production environments.

For a learning or single-developer project, Option A is the right call. It removes the operational overhead without sacrificing anything meaningful.

---

## 3. Design the database schema first

Spend time on the schema before writing service code. The patterns that matter here:

### Many-to-many relationships need explicit join tables
`Player ↔ Team` and `Team ↔ Game` are both many-to-many. EF Core can manage these with implicit join tables, but defining them explicitly (`TeamPlayer`, `GameTeam`) gives you control over cascade behavior and makes the schema easier to reason about.

### Choose the right cascade behavior
- `Cascade`: Deleting the parent deletes all children. Use for tightly-owned data (e.g., deleting a `PointMatch` should delete its `PointMatchPlayers`).
- `Restrict`: You cannot delete the parent if children exist. Use to prevent accidentally orphaning data (e.g., a `Scoreboard` row should not be deleted just because a `Team` is deleted accidentally).
- `SetNull`: Children remain but their foreign key is set to null. Rarely the right choice for required relationships.

Think through each relationship and ask: "What should happen to the children if the parent is deleted?"

### Use public IDs for shareable resources
For the `PointMatch` feature, the URL needs to be shareable. Using the database's integer primary key (`/match/1`, `/match/2`) leaks internal IDs and is easy to enumerate. Instead, generate a random `PublicId` (a GUID) when the match is created and use that in the URL (`/match/a3f9d...`). Keep the integer `Id` as the real primary key for database joins — it is never exposed to the client.

---

## 4. Set up the .NET backend

```bash
dotnet new webapi -n pointCounterBackend
cd pointCounterBackend
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Swashbuckle.AspNetCore
```

### Register services in `Program.cs` in this order
1. `AddControllers()`
2. `AddDbContext<AppDbContext>(...)` with the SQL Server provider
3. Scoped services for each domain (`IPlayerService`, `ITeamService`, etc.)
4. `AddSignalR()` if you need real-time features
5. `AddCors(...)` — configure it to allow the Angular dev server origin (`localhost:4200`) explicitly with `AllowCredentials()`, which SignalR requires
6. `AddEndpointsApiExplorer()` + `AddSwaggerGen()` for the API explorer
7. Map routes: `MapControllers()`, `MapHub<YourHub>(...)`, `MapFallbackToFile("index.html")`

### Important CORS note for SignalR
SignalR WebSocket connections require `AllowCredentials()` on the CORS policy. Without it, the connection will fail with a vague browser error. The Angular origin must be listed explicitly — `AllowAnyOrigin()` does not work with `AllowCredentials()`.

---

## 5. Set up the Angular frontend

```bash
ng new Point-Counter --routing --style css
cd Point-Counter
```

### Point the build output at the .NET wwwroot folder
In `angular.json`, change the `outputPath` to point at the backend's `wwwroot`:

```json
"outputPath": {
  "base": "../../Backend/pointCounterBackend/wwwroot",
  "browser": ""
}
```

This means `ng build` drops the compiled Angular files directly where .NET expects them. No manual copy step needed.

### Set up a dev proxy
During development, the Angular dev server (`ng serve`, port 4200) needs to forward API and SignalR calls to the .NET server (port 5092). Create `proxy.conf.json`:

```json
{
  "/api": {
    "target": "http://127.0.0.1:5092",
    "secure": false
  },
  "/hubs": {
    "target": "http://127.0.0.1:5092",
    "secure": false,
    "ws": true
  }
}
```

And reference it in `angular.json` under `serve.options.proxyConfig`.

---

## 6. Wire the Angular build into the .NET build

To allow `dotnet run` to start the entire application (no separate `ng serve` needed), add a pre-build MSBuild target to the `.csproj` that calls `npm run build`:

```xml
<PropertyGroup>
  <SpaRoot>$(MSBuildProjectDirectory)\..\..\FrontEnd\Point-Counter\</SpaRoot>
</PropertyGroup>

<Target Name="BuildAngular" BeforeTargets="Build">
  <Exec Command="npm install" WorkingDirectory="$(SpaRoot)"
        Condition="!Exists('$(SpaRoot)node_modules')" />
  <Exec Command="npm run build" WorkingDirectory="$(SpaRoot)" />
</Target>
```

**Trade-off**: This runs a full Angular production build every time `dotnet run` is invoked. It is the right default for simplicity, but during active frontend development you will want to comment it out and use `ng serve` instead for instant hot reloads.

---

## 7. Structure your backend services carefully

Every controller should delegate all logic to a service. Controllers only:
- Parse the incoming DTO
- Call the service
- Map the result to an HTTP response (`Ok()`, `NotFound()`, `BadRequest()`, `CreatedAtAction()`)

Services handle all database interaction and business rules. This separation makes services independently testable and keeps controllers thin.

Define interfaces for every service (`IPlayerService`, `ITeamService`, etc.) and register the concrete implementations as scoped in `Program.cs`. EF Core `DbContext` should also be scoped.

---

## 8. Add real-time with SignalR

SignalR is the right choice for the Quick Match feature because:
- It works over WebSockets with automatic fallback to long-polling.
- It supports named groups, so each match can have its own broadcast channel.
- The ASP.NET Core integration is first-class and the client library (`@microsoft/signalr`) is official.

### Hub design
Keep the hub itself thin. Only expose `JoinMatch` and `LeaveMatch` methods — these manage group membership. Never put business logic in the hub. Broadcasting is done from the controller using `IHubContext<YourHub>`, injected via DI.

### Frontend hub service
Create a singleton Angular service that manages one `HubConnection` for the entire app lifetime. Key points:
- Guard every browser-only call with `isPlatformBrowser()` — Angular SSR will try to run the service on the server where WebSockets don't exist.
- Use `withAutomaticReconnect()` and re-join the match group inside `onreconnected()` so users survive brief network drops.
- Expose the incoming events as RxJS `Observable` so Angular components can subscribe declaratively.

---

## 9. Things that are easy to get wrong

### CORS with SignalR
`AllowAnyOrigin()` is incompatible with `AllowCredentials()`. You must list each allowed origin explicitly. Forgetting this causes silent WebSocket failures.

### EF Core cascade conflicts
SQL Server raises an error if multiple cascade paths to the same table exist. If you have a `Scoreboard` with foreign keys to both `Game` (Cascade) and `Team` (Cascade), and both `Game` and `Team` could be deleted, SQL Server rejects the schema. Solve it by making one of the relationships use `Restrict` or `SetNull` instead.

### Angular SSR and browser APIs
Angular 21 uses SSR by default (`@angular/ssr`). Any service or component that calls `window`, `document`, or WebSocket APIs will crash during server-side rendering. Always guard browser-only code with `isPlatformBrowser(inject(PLATFORM_ID))`.

### Static file serving order in Program.cs
`app.UseDefaultFiles()` must come before `app.UseStaticFiles()`, and `app.MapFallbackToFile("index.html")` must come after `app.MapControllers()` and `app.MapHub(...)`. If the fallback is registered before the API routes, every API call will return the Angular index page instead of JSON.

### Integer IDs vs GUIDs for public URLs
Do not expose integer primary keys in URLs intended for sharing. Integer IDs are sequential and enumerable — anyone can iterate `/match/1`, `/match/2`, etc. Generate a random `PublicId` at creation time and use that in all client-facing routes.

---

## 10. Suggested development order

1. Create the .NET project and get a database connection working with a single empty entity.
2. Add Entity Framework migrations and verify the schema is created correctly.
3. Build one complete feature end-to-end (e.g., Players: entity → service → controller → test in Swagger).
4. Repeat for Teams, Games, Scoreboards — the pattern is the same, the relationships get progressively more complex.
5. Set up the Angular project and point the output at wwwroot.
6. Build the first Angular feature (Players) and test the full stack end to end.
7. Add the Quick Match feature — it is independent of the team management system.
8. Add SignalR last, once the HTTP layer is solid. Real-time is easier to layer on top of working synchronous code than to debug alongside it.
