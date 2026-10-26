# Kafé Terningkast — Board Game Café App

A Blazor Web App for **Kafé Terningkast**, a board game café. Guests browse the game shelf, roll dice to rate games, plan a game night, and staff manage the collection. Built as part of *Arbeidskrav 2 – Modul 2: Software Design* (Emne 3: Avansert Backend-programmering).

---

## Tech Stack

| Layer           | Technology                                       |
|-----------------|--------------------------------------------------|
| Runtime         | .NET 10 (Blazor Web App, Interactive Server)     |
| Database        | SQLite via Entity Framework Core 10 (code-first) |
| Styling         | Tailwind CSS v4 (via CLI)                        |
| Package manager | Bun                                              |
| Version control | Git                                              |
| Tests (planned) | xUnit + bUnit                                    |

## Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download) (tested with 10.0.112)
- [EF Core CLI tool](https://learn.microsoft.com/ef/core/cli/dotnet) (`dotnet-ef` 10.x)
- [Bun](https://bun.sh) (JavaScript runtime + package manager)
- Optional: [`sqlite3`](https://sqlite.org/cli.html) CLI for inspecting the DB

Verify installations:

```bash
dotnet --version          # 10.0.x
dotnet ef --version       # 10.0.x
bun --version
```

If `dotnet ef` is not found, install it once:

```bash
dotnet tool install --global dotnet-ef --version '10.*'
export PATH="$PATH:$HOME/.dotnet/tools"
```

Add the `export PATH` line to `~/.bashrc` (or `~/.zshrc`) so the tool stays available across terminal sessions.

---

## Getting Started

### 1. Install dependencies

```bash
bun install
dotnet restore
```

### 2. Create the database

The project uses EF Core migrations. To create or update the SQLite database from scratch:

```bash
dotnet ef database update
```

With the default relative connection string and commands run from the project root, this creates `cafe_terningkast.db` in the project root. The app also runs `db.Database.Migrate()` on startup, so this step is optional if you let startup apply migrations.

To start over from an empty database, stop the application first. The following commands delete local game data:

```bash
rm -f cafe_terningkast.db cafe_terningkast.db-shm cafe_terningkast.db-wal cafe_terningkast.db-journal
dotnet ef database update
```

The seed in `Data/SeedData.cs` will repopulate the table the next time the app starts.

### 3. Build styles and run in development

Run these commands from the project root:

```bash
bunx @tailwindcss/cli -i ./src/app.css -o ./wwwroot/app.css
dotnet build
bun run dev
```

The commands run in this order:

1. Compile the Tailwind stylesheet from `src/app.css` into `wwwroot/app.css`.
2. Build the .NET application after the generated stylesheet exists.
3. Start the Blazor application and Tailwind watcher.

Run the full sequence after cloning the repository, deleting the generated CSS, or troubleshooting a page that loads without styling. `wwwroot/app.css` is git-ignored, so it must be generated locally.

Once the initial build is complete, normal development only requires:

```bash
bun run dev
```

This starts two processes in parallel:

- `dotnet watch` — runs the Blazor app with hot reload.
- `@tailwindcss/cli --watch` — rebuilds the stylesheet as source files change.

Open the address printed by `dotnet`. The current local development setup uses:

```text
http://localhost:5030
```

The API client must point to the same running application. In `appsettings.Development.json`, ensure this section matches the actual listening address:

```json
{
  "GameApi": {
    "BaseUrl": "http://localhost:5030/"
  }
}
```

Merge this section into the existing configuration rather than replacing other settings, such as connection strings. Use the scheme and port printed in the application's startup output. Do not use `https://` with an HTTP-only port.

To check the API while the application is running, use another terminal:

```bash
curl -i http://localhost:5030/api/games
```

A successful request returns `HTTP/1.1 200 OK` followed by the games as JSON, or `[]` for an empty collection.

The API routes must be registered in `Program.cs` before the single `app.Run()` call:

```csharp
app.MapGameEndpoints();
```

### 4. Build for production

```bash
bunx @tailwindcss/cli -i ./src/app.css -o ./wwwroot/app.css --minify
dotnet publish -c Release
```

Output: `bin/Release/net10.0/publish/`.

Configure the deployed API base URL and HTTPS separately for the target environment; the localhost HTTP address above is for local development.

---

## Project Structure

```text
CafeDiceRoller/
├── CafeDiceRoller.csproj
├── Program.cs                    # DI, middleware, routing, migrate-on-startup
├── appsettings.json              # Connection string and application configuration
├── appsettings.Development.json  # Local API base URL and development overrides
├── package.json                  # Bun scripts (dev, dev:dotnet, dev:tailwind)
├── README.md
├── cafe_terningkast.db            # SQLite file (git-ignored)
├── src/
│   └── app.css                   # Tailwind entry (input)
├── wwwroot/
│   ├── app.css                   # Compiled Tailwind (git-ignored)
│   └── favicon.png
├── Data/
│   ├── GameDbContext.cs          # EF Core context + constraints
│   └── SeedData.cs               # Idempotent seed for the initial 10 games
├── Migrations/
│   ├── 20261012170545_InitialCreate.cs
│   ├── 20261012170545_InitialCreate.Designer.cs
│   └── GameDbContextModelSnapshot.cs
├── Models/
│   └── BoardGame.cs
├── Services/
│   ├── GameData.cs
│   ├── GameNightState.cs
│   ├── GameApiClient.cs          # HttpClient wrapper for REST API
│   └── GameEndpoints.cs          # REST API routes backed by EF Core
└── Components/
    ├── App.razor
    ├── Routes.razor
    ├── _Imports.razor
    ├── Layout/
    │   ├── MainLayout.razor
    │   ├── Header.razor
    │   └── Footer.razor
    ├── Pages/
    │   ├── Home.razor
    │   ├── Games.razor
    │   ├── GameDetails.razor
    │   ├── GameNight.razor
    │   ├── AdminGames.razor
    │   ├── Error.razor
    │   └── NotFound.razor
    └── Shared/
        ├── GameCard.razor
        └── DiceRating.razor
```

---

## Database

### Why SQLite + EF Core?

- **SQLite** is a single-file, zero-config database — no server to install, ideal for a café-internal tool and the assignment's scope.
- **EF Core 10** integrates with the DI container and lets us define the schema in C# (code-first).

### Schema

One table, `Games`, matching `BoardGame` from the assignment brief:

| Column            | Type    | Constraint                                 |
|-------------------|---------|--------------------------------------------|
| `Id`              | INTEGER | PK, autoincrement                          |
| `Name`            | TEXT    | NOT NULL, max 120 configured in the model  |
| `MinPlayers`      | INTEGER | NOT NULL                                   |
| `MaxPlayers`      | INTEGER | NOT NULL                                   |
| `PlayTimeMinutes` | INTEGER | NOT NULL                                   |
| `Category`        | TEXT    | NOT NULL, max 60 configured in the model   |
| `Rating`          | INTEGER | NOT NULL                                   |
| `Description`     | TEXT    | NOT NULL, max 1000 configured in the model |

Check constraints configured at the database level:

- `CK_BoardGame_Rating` — `Rating BETWEEN 1 AND 6`
- `CK_BoardGame_PlayerRange` — `MinPlayers >= 1 AND MaxPlayers >= MinPlayers`
- `CK_BoardGame_PlayTime` — `PlayTimeMinutes > 0`

These constraints complement form validation and protect the corresponding database invariants when the UI is bypassed. String-length settings in the EF model should not be treated as proof of SQLite database-level length enforcement.

### Migrations

Migrations live in `Migrations/` and are committed to Git. Common commands:

```bash
dotnet ef migrations add <Name>    # after changing GameDbContext
dotnet ef database update         # apply pending migrations
dotnet ef migrations remove       # remove the last migration, if not applied
```

The app calls `db.Database.Migrate()` on startup. Generate the stylesheet before the first application build as described in Getting Started.

### Seeding

`Data/SeedData.cs` inserts 10 games on first run. It's idempotent: if the `Games` table already has any rows, it does nothing. To re-seed, stop the app, delete the local database files, and restart the app. This removes existing game data.

---

## Tailwind CSS

### Setup

Tailwind runs as a separate build step, not through MSBuild. Two files matter:

| File              | Role                                              | Git-tracked |
|-------------------|---------------------------------------------------|-------------|
| `src/app.css`     | Tailwind input: imports, theme tokens, custom CSS | Yes         |
| `wwwroot/app.css` | Compiled CSS served to the browser                | No          |

The build pipeline is:

```text
src/app.css → Tailwind CLI → wwwroot/app.css → browser
```

Generate `wwwroot/app.css` before the initial .NET build:

```bash
bunx @tailwindcss/cli -i ./src/app.css -o ./wwwroot/app.css
dotnet build
```

`Components/App.razor` references the generated asset:

```razor
<link rel="stylesheet" href="@Assets["app.css"]" />
```

### Running the watcher

`bun run dev` launches two parallel processes:

```json
"scripts": {
  "dev": "bun run --parallel dev:dotnet dev:tailwind",
  "dev:dotnet": "dotnet watch",
  "dev:tailwind": "bunx @tailwindcss/cli -i ./src/app.css -o ./wwwroot/app.css --watch"
}
```

You will see two interleaved output streams in the terminal. Tailwind reports completed CSS builds, while `dotnet watch` reports application builds, hot reloads, and restarts. Reload the browser if a change is not reflected automatically.

### One-shot rebuild (no watcher)

If the watcher is not running, rebuild manually:

```bash
bunx @tailwindcss/cli -i ./src/app.css -o ./wwwroot/app.css
```

Check the output:

```bash
ls -lh wwwroot/app.css
grep -n phosphor wwwroot/app.css
```

The presence or absence of one token alone does not establish whether the entire build succeeded. Inspect the CLI output and generated file as well.

Hard-refresh the browser with `Ctrl+Shift+R` if it still displays an old stylesheet.

### Design tokens (`@theme`)

The palette lives in `src/app.css` inside an `@theme` block:

```css
@import "tailwindcss";

@theme {
  --color-phosphor: oklch(0.85 0.20 145);
  --color-cafe-bg: oklch(0.12 0.01 150);
  --color-dice-1: oklch(0.72 0.18 25);
  /* Other project tokens remain here. */
}
```

These tokens support utility names such as `bg-phosphor`, `text-cafe-bg`, and `border-dice-1`. No `tailwind.config.js` is used. Keep the existing project theme and custom rules rather than replacing the source stylesheet with this example.

### Theme: phosphor terminal

The app uses a CRT/terminal-inspired theme:

- True-black background with a faint green tint (`--color-cafe-bg`).
- Phosphor green accent (`--color-phosphor`) for buttons, focus rings, navigation, and dice highlights.
- Monospace-forward typography (JetBrains Mono, falling back to system mono).
- A subtle scanline overlay on `body::before`.

| Class             | Purpose                                            |
|-------------------|----------------------------------------------------|
| `.surface`        | Card background, border, radius, subtle inner glow |
| `.terminal-label` | Small uppercase monospace label above form fields  |
| `.text-neon`      | Phosphor color and text-shadow glow                |
| `.caret::after`   | Blinking block cursor                              |
| `.nav-link`       | Top-navigation hover and active styling            |
| `.footer-link`    | Footer hover and active styling                    |

Custom component styles live in `@layer components` and plain CSS. Other visual styles use Tailwind utilities directly in Razor markup.

### Custom fonts

JetBrains Mono is declared as the display, body, and mono font. It loads from the system if installed; otherwise the browser falls back to `ui-monospace`. To load it from Google Fonts, add the following to `Components/App.razor`:

```html
<link rel="preconnect" href="https://fonts.googleapis.com" />
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
<link href="https://fonts.googleapis.com/css2?family=JetBrains+Mono:wght@400;500;700&display=swap" rel="stylesheet" />
```

### Troubleshooting

#### The page loads, but all styling is missing

Stop the development processes with `Ctrl+C`, then run:

```bash
bunx @tailwindcss/cli -i ./src/app.css -o ./wwwroot/app.css
dotnet build
bun run dev
```

Hard-refresh the browser with `Ctrl+Shift+R`.

Verify that `wwwroot/app.css` exists and that `Components/App.razor` references it:

```razor
<link rel="stylesheet" href="@Assets["app.css"]" />
```

Check the generated file and HTTP response:

```bash
ls -lh wwwroot/app.css
curl -i http://localhost:5030/app.css
```

In the browser's Network tab, inspect the actual stylesheet request. It should return CSS, not an HTML error page. If the page requests a fingerprinted URL, inspect that exact request too.

#### The theme looks unchanged after editing `src/app.css`

1. Check the Tailwind terminal output for a completed build after saving.
2. Fix any reported CSS errors.
3. Force a one-shot rebuild.
4. Inspect the generated stylesheet for the expected change.
5. Hard-refresh the browser.
6. If necessary, stop the app and repeat the CSS build → .NET build → development startup sequence.

#### The API reports SSL errors or connection refused

Compare `GameApi:BaseUrl` with the application's `Now listening on:` output. For the current HTTP setup, use `http://localhost:5030/`. Do not pair HTTPS with an HTTP-only port or point the client at an inactive port.

Ensure `app.MapGameEndpoints()` is registered before `app.Run()`.

If the app intentionally runs HTTP-only in development, configure HTTPS redirection accordingly. The warning `Failed to determine the https port for redirect` indicates that the middleware could not determine an HTTPS destination; it does not mean the API process failed to start.

#### The SignalR circuit drops ("Rejoining the server…")

Check the server terminal and browser console. Application restarts, unsupported hot-reload changes, and unhandled component exceptions can disrupt the connection. Restart development and reload the page after resolving the underlying error.

#### The Tailwind watcher reports an error

Fix the source CSS syntax and inspect the terminal output. If the watcher exited or does not resume rebuilding, restart `bun run dev`.

### Why not MSBuild integration?

We keep the CSS pipeline explicit in `package.json` rather than integrating it into MSBuild. This keeps frontend tooling separate and makes the input, output, and CLI command visible. The trade-off is a separate watcher and an explicit initial CSS build.

---

## Pages

| Route             | Component           | Purpose                                         |
|-------------------|---------------------|-------------------------------------------------|
| `/`               | `Home.razor`        | Landing page                                    |
| `/games`          | `Games.razor`       | Game shelf: search, player filtering, dice roll |
| `/games/{Id:int}` | `GameDetails.razor` | Game details, rating, add to game night         |
| `/game-night`     | `GameNight.razor`   | Selected games and total playtime               |
| `/admin/games`    | `AdminGames.razor`  | Staff CRUD against the REST API                 |

## Design Decisions

Full write-up: [`docs/design.md`](docs/design.md).

### Database: SQLite + EF Core

Chosen over a separate database server or an in-memory-only store for simple local setup, schema-as-code, migrations, and dependency-injection integration. The database file is not committed. SQLite's write-concurrency limitations are a trade-off for this small application.

### Rendering mode: Interactive Server

State and component execution stay on the server. Staff operations use an HTTP client against the co-hosted API. The trade-off is server round trips and a persistent SignalR connection for interactive behavior.

### State sharing: Service over parameters/cascades

`GameNightState` is a scoped service rather than state threaded through many component levels. Rationale and alternatives are documented in `docs/design.md`.

### Styling: Tailwind v4 with theme tokens

Tokens are declared in `src/app.css` with `@theme`. Tailwind utilities are used in Razor markup; no `tailwind.config.js` is used.

### Custom UI components

Bootstrap was removed from the template. Application UI components and styling are implemented locally with Tailwind. Blazorise and its Lucide icon provider are registered for the project's icon integration; they are not intended to replace the assignment's custom UI components.

## Component Responsibility

Data flows down via parameters; events flow up via `EventCallback`.

| Component        | Responsibility                                     |
|------------------|----------------------------------------------------|
| `MainLayout`     | Header, main content slot, footer                  |
| `Header`         | Navigation and active-link highlighting            |
| `Footer`         | Secondary navigation and copyright                 |
| `Games`          | Local filtering state and rendering game cards     |
| `GameCard`       | Render one game and raise parent events            |
| `GameDetails`    | Load a game by route ID and host rating UI         |
| `DiceRating`     | Display dice faces and communicate value changes   |
| `GameNight`      | Display selected games from `GameNightState`       |
| `AdminGames`     | CRUD form and local UI state                       |
| `GameDbContext`  | EF Core context and database constraints           |
| `GameData`       | Read-side catalog access                           |
| `GameNightState` | Scoped game-night selection state                  |
| `GameApiClient`  | HTTP calls to the REST API                         |
| `GameEndpoints`  | Map API routes and persist changes through EF Core |

## Data Flow

- Read path (guest): `Games` → `GameData` → `GameDbContext` → SQLite → game cards.
- Selection path (guest): `GameCard` event → parent page → `GameNightState.Add(game)` → UI updates.
- Read path (staff): `AdminGames` → `GameApiClient.GetAllAsync()` → HTTP GET `/api/games` → `GameDbContext` → SQLite → JSON response.
- Write path (staff): `AdminGames` form → `GameApiClient.CreateAsync/UpdateAsync/DeleteAsync` → REST API → `GameDbContext` → SQLite.

Diagrams live in `docs/design.md`.

## Local vs Application State

| State                | Location                   | Purpose                               |
|----------------------|----------------------------|---------------------------------------|
| Search text          | `Games.razor`              | Page-local filtering                  |
| Player filter        | `Games.razor`              | Page-local filtering                  |
| Randomly rolled game | `Games.razor`              | Page-local visual selection           |
| Selected games       | `GameNightState` (Scoped)  | Shared within the interactive circuit |
| Game catalog         | SQLite via `GameDbContext` | Persistent catalog                    |

## Testing

### Automated (planned / in progress)

Test project: `CafeDiceRoller.Tests/` (xUnit + bUnit).

- `GameData` filtering: search, player range, combinations.
- `GameNightState`: add, remove, deduplication, total playtime.
- `BoardGame` validation attributes.
- `DiceRating` value-change events.
- `GameCard` parent events.

```bash
dotnet test
```

### Manual test log

See [`docs/test-log.md`](docs/test-log.md). Entries record scenario, steps, expected result, and actual result.

## Resources

- [Learn Razor Pages — Forms](https://www.learnrazorpages.com/razor-pages/forms)
- [Blazorise Icons component](https://blazorise.com/docs/extensions/icons)

## Known Limitations

- SQLite storage is local to the deployment and git-ignored; machines do not share the same local file automatically.
- No authentication: `/admin/games` and its write API require access control before real-world deployment.
- Game-night state is scoped to an interactive server circuit, not durable user-account storage. Separate browser tabs generally establish separate circuits; do not rely on cross-tab sharing.
- No offline support: Interactive Server requires a live connection.
- Tailwind is a separate build step: generate the CSS before the first .NET build and before publishing.

## AI Usage Reflection

See [`AI_DOCUMENTATION.md`](./docs/AI_DOCUMENTATION.md) for the full reflection: contributions, reviewed changes, rejected suggestions, and limitations.

AI was used for scaffolding ideas, Tailwind/Blazor integration troubleshooting, documentation structure, and guided introduction to EF Core + SQLite migrations. All code was reviewed and adapted to this project.

## Assignment Mapping

| Assignment | Feature area            | Primary files                                                                                |
|------------|-------------------------|----------------------------------------------------------------------------------------------|
| Oppgave 1  | Game shelf              | `Games.razor`, `GameCard.razor`, `GameData.cs`, `BoardGame.cs`                               |
| Oppgave 2  | Dice and details        | `DiceRating.razor`, `GameDetails.razor`, `Header.razor`                                      |
| Oppgave 3  | Game-night state        | `GameNightState.cs`, `GameNight.razor`, `Header.razor`                                       |
| Oppgave 4  | Admin, HttpClient, CRUD | `GameApiClient.cs`, `GameEndpoints.cs`, `AdminGames.razor`, `Program.cs`, `GameDbContext.cs` |
| Oppgave 5  | Design docs and tests   | `docs/design.md`, `docs/test-log.md`, `CafeDiceRoller.Tests/`                                |
| Oppgave 6  | Video and reflection    | External video link, `AI_DOCUMENTATION.md`                                                   |

## Git Workflow

Commits follow a loose [Conventional Commits](https://www.conventionalcommits.org/) style:

```text
chore: initial Blazor + Tailwind v4 + Bun setup
feat(layout): header/main/footer shell with Tailwind gaming theme
feat(games): add game shelf with search, filter, and dice roll
feat(dice): add reusable DiceRating component
feat(state): add game-night state service
feat(api): add REST API and GameApiClient
feat(db): add EF Core + SQLite, GameDbContext, and seed data
test: add unit and component tests
docs: add design.md, test-log.md, and AI documentation
```

The project's `.gitignore` excludes `bin/`, `obj/`, `node_modules/`, `bun.lock`, `wwwroot/app.css`, and `cafe_terningkast.db*`.

## License

No license specified yet.
