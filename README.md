# Kafé Terningkast — Board Game Café App

A Blazor Web App for **Kafé Terningkast**, a board game café. Guests browse the
game shelf, roll dice to rate games, plan a game night, and staff manage the
collection. Built as part of *Arbeidskrav 2 – Modul 2: Software Design*
(Emne 3: Avansert Backend-programmering).

---

## Tech Stack

| Layer            | Technology                                        |
|------------------|---------------------------------------------------|
| Runtime          | .NET 10 (Blazor Web App, Interactive Server)      |
| Database         | SQLite via Entity Framework Core 10 (code-first)  |
| Styling          | Tailwind CSS v4 (via CLI)                         |
| Package manager  | Bun                                               |
| Version control  | Git                                               |
| Tests (planned)  | xUnit + bUnit                                     |

---

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
dotnet tool install --global dotnet-ef --version 10.*
export PATH="$PATH:$HOME/.dotnet/tools"
```

Add the `export PATH` line to `~/.bashrc` (or `~/.zshrc`) so the tool stays
available across terminal sessions.

---

## Getting Started

### 1. Install dependencies

```bash
bun install
dotnet restore
```

### 2. Create the database

The project uses EF Core migrations. To create or update the SQLite database
from scratch:

```bash
dotnet ef database update
```

This creates `cafe_terningkast.db` in the project root. The app also runs
`db.Database.Migrate()` on startup, so in practice this step is only needed
if you want to inspect or reset the DB manually.

To start over from an empty database:

```bash
rm cafe_terningkast.db*
dotnet ef database update
```

The seed in `Data/SeedData.cs` will repopulate the table the next time the app
starts.

### 3. Run in development

```bash
bun run dev
```

This starts two processes in parallel:

- **`dotnet watch`** — runs the Blazor app with hot reload
- **`@tailwindcss/cli --watch`** — recompiles `src/app.css` → `wwwroot/app.css` on save

Open the URL that `dotnet` prints (typically `https://localhost:7xxx`).

### 4. Build for production

```bash
bunx @tailwindcss/cli -i ./src/app.css -o ./wwwroot/app.css --minify
dotnet publish -c Release
```

Output: `bin/Release/net10.0/publish/`.

---

## Project Structure

```
CafeDiceRoller/
├── CafeDiceRoller.csproj
├── Program.cs                    # DI, middleware, routing, migrate-on-startup
├── appsettings.json              # Connection string for SQLite
├── package.json                  # Bun scripts (dev, dev:dotnet, dev:tailwind)
├── README.md
├── cafe_terningkast.db           # SQLite file (git-ignored, created on first run)
├── src/
│   └── app.css                   # Tailwind entry (input)
├── wwwroot/
│   ├── app.css                   # Compiled Tailwind (git-ignored)
│   └── favicon.png
├── Data/
│   ├── GameDbContext.cs          # EF Core context + constraints
│   └── SeedData.cs               # Idempotent seed for the initial 10 games
├── Migrations/                   # EF Core migrations (committed to Git)
│   ├── 20261012170545_InitialCreate.cs
│   ├── 20261012170545_InitialCreate.Designer.cs
│   └── GameDbContextModelSnapshot.cs
├── Models/
│   └── BoardGame.cs              # Domain model
├── Services/
│   ├── GameData.cs               # Read queries (wraps GameDbContext)
│   ├── GameNightState.cs         # App-wide game-night state (Oppgave 3)
│   └── GameApiClient.cs          # HttpClient wrapper for REST API (Oppgave 4)
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

- **SQLite** is a single-file, zero-config database — no server to install,
  ideal for a café-internal tool and for the assignment's scope.
- **EF Core 10** is the standard .NET ORM, integrates with the DI container
  the app already uses, and lets us define the schema in C# (code-first).

### Schema

One table, `Games`, matching `BoardGame` from the assignment brief:

| Column            | Type    | Constraint                          |
|-------------------|---------|-------------------------------------|
| `Id`              | INTEGER | PK, autoincrement                   |
| `Name`            | TEXT    | NOT NULL, max 120                   |
| `MinPlayers`      | INTEGER | NOT NULL                            |
| `MaxPlayers`      | INTEGER | NOT NULL                            |
| `PlayTimeMinutes` | INTEGER | NOT NULL                            |
| `Category`        | TEXT    | NOT NULL, max 60                    |
| `Rating`          | INTEGER | NOT NULL                            |
| `Description`     | TEXT    | NOT NULL, max 1000                  |

Check constraints enforced at the database level:

- `CK_BoardGame_Rating` — `Rating BETWEEN 1 AND 6`
- `CK_BoardGame_PlayerRange` — `MinPlayers >= 1 AND MaxPlayers >= MinPlayers`
- `CK_BoardGame_PlayTime` — `PlayTimeMinutes > 0`

These mirror the validation rules in `AdminGames.razor` and the REST API, so
invalid data is rejected even if the UI is bypassed.

### Migrations

Migrations live in `Migrations/` and are committed to Git. Common commands:

```bash
dotnet ef migrations add <Name>    # after changing GameDbContext
dotnet ef database update          # apply pending migrations
dotnet ef migrations remove        # undo the last migration (if not applied)
```

The app calls `db.Database.Migrate()` on startup, so anyone who clones the
repo and runs `dotnet run` gets the correct schema automatically.

### Seeding

`Data/SeedData.cs` inserts 10 games on first run. It's idempotent: if the
`Games` table already has any rows, it does nothing. To re-seed, delete the
`.db` files and restart the app.

---

## Tailwind CSS

### Setup

Tailwind runs as a separate build step, not through MSBuild. Two files matter:

| File               | Role                                                       | Git-tracked |
|--------------------|------------------------------------------------------------|-------------|
| `src/app.css`      | Tailwind **input**: `@import`, `@theme` tokens, custom CSS | ✅ yes      |
| `wwwroot/app.css`  | Tailwind **output**: compiled CSS served to the browser    | ❌ ignored  |

The build pipeline is:

```
src/app.css  ──[ tailwindcss CLI ]──▶  wwwroot/app.css  ──▶  served to browser
```

### Running the watcher

`bun run dev` launches **two** parallel processes:

```json
"scripts": {
  "dev": "bun run --parallel dev:dotnet dev:tailwind",
  "dev:dotnet": "dotnet watch",
  "dev:tailwind": "bunx @tailwindcss/cli -i ./src/app.css -o ./wwwroot/app.css --watch"
}
```

You'll see two interleaved output streams in the terminal. Every time you save
a `.razor`, `.cs`, or `.css` file, Tailwind prints `Done in Xms` and the browser
updates automatically.

### One-shot rebuild (no watcher)

If the watcher isn't running, or you edited `src/app.css` and the browser looks
stale, force a rebuild:

```bash
bunx @tailwindcss/cli -i ./src/app.css -o ./wwwroot/app.css
```

Verify the change landed:

```bash
grep -c phosphor wwwroot/app.css      # should be > 0 if theme tokens are present
ls -la wwwroot/app.css                 # timestamp should be "just now"
```

Then **hard-reload the browser** (`Ctrl+Shift+R`) — the old `app.css` is cached
aggressively, and a normal reload often serves the stale copy.

### Design tokens (`@theme`)

The entire palette lives in `src/app.css` inside an `@theme { … }` block.
Tailwind v4 auto-generates utility classes from these tokens. Defining

```css
@theme {
  --color-phosphor:     oklch(0.85 0.20 145);
  --color-cafe-bg:      oklch(0.12 0.01 150);
  --color-dice-1:       oklch(0.72 0.18 25);
  /* … */
}
```

gives you `bg-phosphor`, `text-cafe-bg`, `border-dice-1`, and so on — no
`tailwind.config.js` needed.

Changing a token is a one-line edit that propagates everywhere the
corresponding utility is used. That's the primary reason the palette is
centralized here instead of scattered across components.

### Theme: phosphor terminal

The app uses a **CRT/terminal-inspired** theme:

- True-black background with a faint green tint (`--color-cafe-bg`)
- Phosphor green accent (`--color-phosphor`) for buttons, focus rings, active
  nav state, and dice highlights
- Monospace-forward typography (JetBrains Mono, falling back to system mono)
- A subtle scanline overlay on `body::before` (`repeating-linear-gradient` at
  4% phosphor, `mix-blend-mode: screen`)
- Utility classes for common patterns:

| Class             | Purpose                                            |
|-------------------|----------------------------------------------------|
| `.surface`        | Card background, border, radius, subtle inner glow |
| `.terminal-label` | Small uppercase monospace label above form fields  |
| `.text-neon`      | Phosphor color + text-shadow glow for headings     |
| `.caret::after`   | Blinking block cursor appended to an element       |
| `.nav-link`       | Top-nav link: hover glow, filled pill when active  |
| `.footer-link`    | Footer link: phosphor on hover/active              |

These live in `@layer components` and plain CSS respectively. Every other
visual style comes from Tailwind utility classes written directly in `.razor`
markup.

### Custom fonts

JetBrains Mono is declared as the display, body, and mono font. It loads from
the system if installed; otherwise the browser falls back to `ui-monospace`,
which is still monospace but may vary by OS. To guarantee the font across
machines, add a Google Fonts `<link>` in `Components/App.razor`:

```html
<link rel="preconnect" href="https://fonts.googleapis.com" />
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
<link href="https://fonts.googleapis.com/css2?family=JetBrains+Mono:wght@400;500;700&display=swap" rel="stylesheet" />
```

### Troubleshooting

**The theme looks unchanged after editing `src/app.css`.**

1. Check the terminal — is there a `Done in Xms` line from `dev:tailwind` after
   your save? If not, the watcher isn't running. Restart `bun run dev`.
2. Force a one-shot rebuild (see above).
3. Check whether the new token is in the compiled output:
   `grep -c your-token-name wwwroot/app.css`. Zero means the build didn't run
   or `src/app.css` doesn't contain the token.
4. Hard-reload the browser. The old `app.css` is heavily cached.

**The SignalR circuit drops ("Rejoining the server…").**

That's not a Tailwind problem — it happens when `dotnet watch` applies a hot
reload that the running circuit can't handle (e.g., you edited `App.razor`,
`_Imports.razor`, or `Program.cs`). Restart `bun run dev` and close any stale
browser tabs.

**Tailwind watcher crashes after a bad save.**

A syntax error in `src/app.css` (missing brace, stray character) will kill the
watcher process. Fix the file, then restart `bun run dev`. The dotnet side
survives; only the CSS pipeline needs a restart.

### Why not MSBuild integration?

There are NuGet packages (e.g. `Tailwind.MSBuild`) that run Tailwind as part of
`dotnet build`. We deliberately don't use them:

- They hide the pipeline behind MSBuild magic, making failures hard to debug.
- They pin Tailwind versions that may lag behind the CLI releases.
- Keeping the build in `package.json` means the frontend toolchain is described
  in one place and can be reasoned about independently of .NET.

The cost is one extra terminal process during development. The benefit is a
transparent, reproducible pipeline anyone can inspect with two commands.

## Pages

| Route              | Component                | Purpose                                                    |
| ------------------ | ------------------------ | ---------------------------------------------------------- |
| `/`                | `Home.razor`             | Landing page with CTA buttons                              |
| `/games`           | `Games.razor`            | Game shelf: search, filter by players, dice roll           |
| `/games/{Id:int}`  | `GameDetails.razor`      | One game's details, dice rating, add to game night         |
| `/game-night`      | `GameNight.razor`        | Games selected for tonight, total playtime                 |
| `/admin/games`     | `AdminGames.razor`       | Staff CRUD against the REST API                            |

---

## Design Decisions

Full write-up of trade-offs in [`docs/design.md`](docs/design.md). Highlights:

### Database: SQLite + EF Core over alternatives

Chosen over a full Postgres/MySQL server or an in-memory-only store because:

- **Zero-config**: the database is a single file that travels with the project.
- **Schema-as-code**: `GameDbContext.OnModelCreating` is the source of truth,
  and migrations are checked into Git.
- **DI-native**: `AddDbContext<GameDbContext>` fits the existing service
  registration style; services receive the context through constructor injection.
- **Cost**: SQLite is not great under heavy concurrent writes, and the file
  must not be committed. For a café-internal tool with a handful of staff,
  both are non-issues.

### Rendering mode: Interactive Server

Chosen over Interactive WebAssembly because:

- **State stays on the server** — `GameNightState` as a `Scoped` service is
  trivial to reason about and test.
- **HttpClient to a co-hosted API** — no CORS, no duplicated DTOs, no cold-start
  download.
- **Cost:** every interaction round-trips to the server (latency-bound), and
  the app needs a persistent SignalR connection.

### State sharing: Service over parameters/cascades

For app-wide state (game night), we use a **`Scoped` service**
(`GameNightState`) rather than threading parameters + `EventCallback` through
many component levels. Rationale and rejected alternatives are in
`docs/design.md`.

### Styling: Tailwind v4 with theme tokens

Design tokens are declared in `src/app.css` inside `@theme { … }`. Tailwind v4
generates utility classes from these tokens (`bg-cafe-bg`, `text-dice-3`,
`border-cafe-border`, …). No `tailwind.config.js` is used.

### No Bootstrap, no component library

The assignment forbids component libraries (MudBlazor, Radzen) from replacing
our own components. Bootstrap was removed from the template; all styling is
ours via Tailwind.

---

## Component Responsibility

Each component has a single clear role. Data flows **down** via parameters,
events flow **up** via `EventCallback`.

| Component        | Responsibility                                                                             |
|------------------|--------------------------------------------------------------------------------------------|
| `MainLayout`     | Page shell: header, main content slot, footer                                              |
| `Header`         | Top navigation with active-link highlighting (`NavLink`)                                   |
| `Footer`         | Secondary navigation and copyright                                                         |
| `Games` (page)   | Owns local UI state (search, filter, rolled game), filters the catalog, renders `GameCard` |
| `GameCard`       | Renders one `BoardGame`. Owns no catalog. Raises events to the parent.                     |
| `GameDetails`    | Loads one game by route `Id`, renders details, hosts `DiceRating`                          |
| `DiceRating`     | Displays 1–6 dice faces. Communicates via `Value`, `ValueChanged`, `ReadOnly`              |
| `GameNight`      | Displays the current game night from `GameNightState`                                      |
| `AdminGames`     | CRUD form against `GameApiClient`                                                          |
| `GameDbContext`  | EF Core context; one `DbSet<BoardGame>` plus constraints                                   |
| `GameData`       | Read-side service wrapping `GameDbContext` for the shelf and details                       |
| `GameNightState` | App-wide game-night state (Scoped); survives navigation                                    |
| `GameApiClient`  | All HTTP calls to the REST API; no component touches `HttpClient`                          |

---

## Data Flow

- **Read path (guest):** `Games` page → `GameData.GetAll()` → `GameDbContext`
  → SQLite → list of `BoardGame` → `@foreach` → `GameCard` (parameter).
- **Write path (guest):** `GameCard` click → `EventCallback` up to `Games`
  page → `GameNightState.Add(game)` → subscribers re-render.
- **Read path (staff):** `AdminGames` → `GameApiClient.GetAllAsync()` →
  HTTP GET `/api/games` → JSON → list.
- **Write path (staff):** `AdminGames` form → `GameApiClient.CreateAsync/UpdateAsync/DeleteAsync`
  → REST API → (in-memory store today, EF Core tomorrow).

Diagrams (component tree, sequence diagrams) live in `docs/design.md`.

---

## Local vs Application State

| State                     | Where it lives                    | Why                                                |
| ------------------------- | --------------------------------- | -------------------------------------------------- |
| Search text               | `Games.razor` (local field)       | Only relevant to this page; reset on navigation.   |
| Player filter             | `Games.razor` (local field)       | Same.                                              |
| Randomly rolled game      | `Games.razor` (local field)       | Purely visual; no other page cares.                |
| Game night (added games)  | `GameNightState` (Scoped)         | Survives navigation; displayed in Header badge.    |
| Game catalog              | SQLite via `GameDbContext`        | Single source of truth; shared across pages.       |

---

## Testing

### Automated (planned / in progress)

Test project: `CafeDiceRoller.Tests/` (xUnit + bUnit).

- Unit: `GameData` filtering (search, player range, combined).
- Unit: `GameNightState` (add, remove, dedupe, total playtime).
- Unit: `BoardGame` validation attributes.
- Component: `DiceRating` raises `ValueChanged` with the correct value.
- Component: `GameCard` emits its `EventCallback` on click.

Run:

```bash
dotnet test
```

### Manual test log

See [`docs/test-log.md`](docs/test-log.md). Each entry records
**scenario → steps → expected → actual**.

---

## Resources

- [Learn Razor Pages - Forms](https://www.learnrazorpages.com/razor-pages/forms)
- [Blazorise Icons component](https://blazorise.com/docs/extensions/icons)

---

## Known Limitations

- **SQLite file is local only.** It is not shared between machines, and the
  file is git-ignored. A real deployment would use a server database.
- **No authentication.** The `/admin/games` page is public. In a real café, it
  would be behind login.
- **Game night is per-server-session**, not per-user. Two browser tabs in the
  same session share it; two users on different sessions do not.
- **No offline support.** Interactive Server requires a live SignalR connection.
- **In-memory API storage (Oppgave 4).** The REST API currently keeps games in
  memory; the database is used on the read side. Migrating the API to EF Core
  is a future task.

---

## AI Usage Reflection

See [`AI_DOCUMENTATION.md`](AI_DOCUMENTATION.md) for a full reflection on how AI
tools were used during this project, what they contributed, what was changed or
rejected, and what limitations were encountered.

Summary: AI was used for scaffolding ideas, Tailwind/Blazor integration
troubleshooting, documentation structure, and — for this milestone — as a
guided introduction to EF Core + SQLite migrations. All code was reviewed,
adapted to this case, and is understood by the author.

---

## Assignment Mapping

| Assignment | Feature Area                         | Primary Files                                                            |
|------------|--------------------------------------|--------------------------------------------------------------------------|
| Oppgave 1  | Game shelf: components, data binding | `Games.razor`, `GameCard.razor`, `GameData.cs`, `BoardGame.cs`           |
| Oppgave 2  | Dice & details: reuse + routing      | `DiceRating.razor`, `GameDetails.razor`, `Header.razor`                  |
| Oppgave 3  | Game night: app state + responsive   | `GameNightState.cs`, `GameNight.razor`, `Header.razor` badge             |
| Oppgave 4  | Admin: HttpClient + CRUD             | `GameApiClient.cs`, `AdminGames.razor`, `Program.cs`, `GameDbContext.cs` |
| Oppgave 5  | Design docs + tests                  | `docs/design.md`, `docs/test-log.md`, `CafeDiceRoller.Tests/`            |
| Oppgave 6  | Video + reflection                   | External video link + `AI_DOCUMENTATION.md`                              |

---

## Git Workflow

Commits follow a loose [Conventional Commits](https://www.conventionalcommits.org/) style:

```
chore: initial Blazor + Tailwind v4 + Bun setup
feat(layout): header/main/footer shell with Tailwind gaming theme
feat(games): add game shelf with search, filter, and dice roll
feat(dice): add reusable DiceRating component
feat(state): add GameNightState service
feat(api): add REST API and GameApiClient
feat(db): add EF Core + SQLite, GameDbContext, and seed data
test: add unit and component tests
docs: add design.md, test-log.md, and AI documentation
```

`.gitignore` excludes `bin/`, `obj/`, `node_modules/`, `bun.lock`,
`wwwroot/app.css`, and `cafe_terningkast.db*`.

---

## License