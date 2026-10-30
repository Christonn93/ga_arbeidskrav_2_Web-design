# Oppgave 6 — Video script: Kafé Terningkast

**Language:** English (consistent throughout — swap to Norwegian if you prefer; just keep it uniform).
**Setup:** app running (`dotnet run`), browser at `/games`, terminal (or IDE) open to `GameApiClient.cs` / `GameEndpoints.cs`, `CafeDiceRoller.Tests/` visible in the editor.

---

## 1. Intro — *(0:25)*

> "Hi, I'm going to walk through Kafé Terningkast — a board-game café app where the staff can manage the game shelf and guests can browse, filter, and pick games for a game night. It's built as a Blazor Web App on .NET 10 with Interactive Server rendering, Tailwind for styling, and SQLite with EF Core for storage. I'll show the three main pages, the component structure, the data flow, a design decision, a challenge I hit, and how it's tested."

**[ACTION]** Full-screen the running app.

---

## 2. The game shelf — *(1:00)*  *(wide screen)*

> "This is the game shelf at `/games`. It reads the catalogue through the REST API. The seeds add ten games across solo, party, strategy, family, abstract, and co-op — chosen so the filters have something to bite on."

**[ACTION]** Type "cat" in the search box → "Catan" remains.

> "Search is case-insensitive and filters by name. The player-count field narrows the list to games that support that number of players."

**[ACTION]** Type **4** in player count, then **1**, showing the list shrink.

> "And the dice rolls a random game from whatever is currently filtered."

**[ACTION]** Click **🎲 Trill terningen** once or twice; the highlighted card moves.

> "The 'Spill i hylla' badge always shows how many of the total are visible."

---

## 3. Details and dice rating — *(0:45)*  *(normal case)*

> "Clicking a card opens the details page at `/games/1`. Here a guest can add the game to their game night, and rate it with a dice from one to six."

**[ACTION]** Click a card → `/games/1`.

> "The dice is a reusable component, `DiceRating`. Clicking it cycles the value — this is a normal case: a rating between one and six. The change is sent up as an event callback and saved through the API automatically."

**[ACTION]** Click the dice 1 → 2 → 3; note the "Lagrer vurdering…" status flashes.

> "There's also a 'Neste spill' link that skips to the next game in the catalogue and wraps around at the end."

**[ACTION]** Hover/click "Neste spill →".

---

## 4. Min spillekveld (game night) — *(0:30)*

> "Added games land in 'Min spillekveld'. It shows the count, the total playtime, and lets you remove games. The state lives in a scoped service, `GameNightState`, which survives navigation within the session."

**[ACTION]** Navigate to `/game-night`; show the list and total time; remove one game.

---

## 5. Admin page + deviating cases — *(1:00)*

> "The staff side is at `/admin/games`. It's a full CRUD form that talks to the API."

**[ACTION]** Open `/admin/games`.

**Normal case:** edit a game's name, save, and reload the shelf to show it changed.

> "That's the normal path. Now two deviating cases. First, validation: an empty name and a max-player count below the minimum are rejected client-side *and* by the server before anything touches the database."

**[ACTION]** Clear the name → submit → red "Navn er påkrevd" appears. Then set Min=4, Max=2 → submit → cross-field error.

> "Second, missing data: a details page for an ID that doesn't exist shows a not-found state with a link back rather than crashing."

**[ACTION]** Navigate to `/games/9999` → "Spillet finnes ikke".

---

## 6. Narrow screen — *(0:25)*

> "The recovery UI is also responsive — here's the shelf at phone width. The grid collapses to one column and the header menu becomes a toggle."

**[ACTION]** Open dev tools, switch **Device Toolbar** to ~375px, show the shelf; then back to full width.

---

## 7. Component structure — *(0:45)*

> "The component tree is the backbone. `App` hosts `Routes`, which renders pages inside `MainLayout`. The layout has the `Header` — with a `GameCounterBadge` for the game-night count — and a footer."

**[ACTION]** Show the tree in `docs/design.md` (or hand-wave at it).

> "The shelf `Games` composes a `GameSearch` and a list of `GameCard`s, each showing a read-only `DiceRating`. `GameDetails` hosts its own writable `DiceRating`. `AdminGames` delegates the form to a shared `GameForm`. Data flows down as parameters and events flow up as callbacks."

---

## 8. Central code + data flow — *(0:50)*

**[ACTION]** Open `Services/GameApiClient.cs` and `Services/GameEndpoints.cs` side by side.

> "The central piece is the data path. Every read and write goes the same route: a page calls `GameApiClient`, which makes an HTTP request to a minimal API endpoint in `GameEndpoints`, which uses `GameDbContext` to reach SQLite. There's no second in-memory copy — SQLite is the single source of truth."

**[ACTION]** Point at `GetAllAsync()` → `MapGet("/api/games")` → `db.Games`.

> "That request/response shape is exactly what the tests exercise: they swap in a fake HTTP handler to simulate both a working API and an API outage."

---

## 9. Design choice + alternatives — *(0:40)*

> "One design decision worth explaining: storage. I considered four options — an in-memory list, raw SQL via `Microsoft.Data.Sqlite`, EF Core over SQLite, and adding a repository layer. I chose EF Core over SQLite: the schema is code, migrations are checked into Git, and check constraints mirror the UI rules as defence-in-depth. An in-memory list gave no persistence; raw SQL meant hand-managing schema; and a repository felt like an abstraction I didn't yet need for a single entity."

**[ACTION]** Show `GameDbContext.cs` `OnModelCreating` check constraints briefly.

---

## 10. A challenge and how I solved it — *(0:30)*

> "The challenge that stung came from testing. I added the test project *inside* the web project, and the first `dotnet test` failed with a wall of errors — the web `.csproj` globs every `.cs` file, so it was trying to compile my test source into the app. The fix was an explicit `DefaultItemExcludes` for the test folder. It taught me to treat a nested test project as its own boundary."

**[ACTION]** Show the `DefaultItemExcludes` block in `CafeDiceRoller.csproj`.

---

## 11. Tests — *(0:25)*

> "That test project now has 48 passing tests: 32 on plain logic like the validator and game-night state, and 16 bUnit component tests covering the dice, the cards, the form validation, the API-down error state, and the details-page reload. A manual log records the cases that aren't automated — like the responsive layout and the two-tab behaviour."

**[ACTION]** Run `dotnet test`; show **48 passed, 0 failed**.

---

## 12. Improvements, further work, AI reflection — *(0:35)*

> "Going forward I'd add authentication so `/admin/games` isn't public, make game-night state per-user instead of per-browser-circuit, and paginate the catalogue when it grows. On AI: I used it as a guided partner — it helped pick the frameworks, scaffold the tests, and diagnose the two build errors I mentioned. I ran every command, read every generated file, and corrected it where it was wrong, like the package versions. The reflection and per-feature notes are in the repo's docs."

---

## 13. Outro — *(0:10)*

> "So: a browseable, filterable shelf; dice ratings and game nights; a validated admin API backed by SQLite; a component tree that keeps responsibilities small; and a test suite that proves the happy paths and the edge cases. Thanks for watching."

---

## Checklist mapping

| Checklist item | Section |
|---|---|
| Shelf, Min spillekveld, admin demonstrated | 2, 4, 5 |
| Wide **and** narrow screen | 2, 6 |
| Component structure from the tree | 7 |
| One central code piece + data flow | 8 |
| Normal + deviating case | 3 (rating) / 5 (validation, 404) |
| Design choice + alternatives | 9 |
| Challenge + how solved | 10 |
| Improvements, further work, AI use | 12 |
| 5–8 min | total ≈ 7:00 |
