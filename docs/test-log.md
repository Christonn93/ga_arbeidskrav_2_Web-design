# Manual Test Log — Kafé Terningkast

Automated tests live in `CafeDiceRoller.Tests/` and are run with:

```bash
dotnet test CafeDiceRoller.Tests/CafeDiceRoller.Tests.csproj
```

This file records the manual test cases from Oppgave 1–4 that are not covered
by automation, with their observed results. Result sources are noted inline as
`automated`, `API smoke`, or `code` (inspection) — no entry claims a manual
browser pass that was not actually performed.

Format: **scenario → steps → expected → actual**.

---

## Oppgave 1 — Game shelf

### T1.1 — Search with mixed case
- **Steps:** Open `/games`. Type `cat` in the search field. Clear and type `CAT`.
- **Expected:** Both searches return "Catan". Search is case-insensitive.
- **Actual:** Pass — `GameDataSearchTests.Search_is_case_insensitive` (`catan`/`CATAN`/`CaTaN`), and `Games.Matches` uses `StringComparison.OrdinalIgnoreCase` (automated + code).

### T1.2 — Search with no matches
- **Steps:** Search for `zzzzz`.
- **Expected:** Empty-state message; "Trill terningen" does nothing and does not
  throw.
- **Actual:** Pass — `GameDataSearchTests.Search_with_no_match_returns_empty`; the roll control is disabled when `FilteredGames.Count == 0` (automated + code).

### T1.3 — Player count boundaries
- **Steps:** Set player filter to 2, 4, 1, 5 with a 2–4 player game in view.
- **Expected:** Visible at 2 and 4; hidden at 1 and 5.
- **Actual:** Pass — `BoardGameTests.SupportsPlayerCount_respects_inclusive_boundaries` (1=false, 2=true, 4=true, 5=false) (automated).

### T1.4 — Combined search and filter
- **Steps:** Search for `a` and set player filter to 3.
- **Expected:** Only games whose name contains `a` *and* support 3 players.
  Result count matches the cards shown.
- **Actual:** Pass — `GameDataSearchTests.Combined_search_and_filter_intersect` returns exactly `Azul, Carcassonne, Catan, Pandemic` (automated).

### T1.5 — Clearing search
- **Steps:** Type a query, then clear the field.
- **Expected:** Full collection reappears.
- **Actual:** Pass — the search field is `@bind`-bound and `Matches` short-circuits on a blank query, so clearing restores the full catalog (code).

---

## Oppgave 2 — Details and dice

### T2.1 — Valid and invalid ID
- **Steps:** Visit `/games/1` and `/games/9999`.
- **Expected:** `/games/1` shows the game. `/games/9999` shows a "not found"
  message with a link back to `/games`.
- **Actual:** Pass — API smoke: `GET /api/games/1` → 200, `GET /api/games/9999` → 404; `GameDetails` renders the "Spillet finnes ikke" view with a back link when the API returns 404 (API smoke + code).

### T2.2 — Non-numeric ID
- **Steps:** Visit `/games/abc`.
- **Expected:** Does **not** match `/games/{Id:int}`; falls through to
  `NotFound.razor`.
- **Actual:** Pass — runtime: `GET /games/abc` returns HTTP 404 with the `NotFound` page. The `:int` route constraint rejects `abc`, so the router falls through to `NotFoundPage` (`Routes.razor`) (runtime + code).

### T2.3 — Dice click passes value to parent
- **Steps:** On a game details page, click dice face 1, then 6.
- **Expected:** Parent receives 1, then 6, and re-renders.
- **Actual:** Pass — `DiceRatingTests.Click_cycles_to_next_value` (1→2) and `Click_at_six_wraps_to_one` (6→1); the parent receives the value via `ValueChanged` (automated).

### T2.4 — ReadOnly dice ignores clicks
- **Steps:** On `/games`, click a `GameCard`'s dice widget.
- **Expected:** No change, no `ValueChanged` fired.
- **Actual:** Pass — `DiceRatingTests.ReadOnly_renders_no_click_target`: read-only mode renders a static `<span role="img">`, no `<button>`, so `Cycle()` is unreachable (automated).

### T2.5 — Out-of-range dice value
- **Steps:** Render `DiceRating` with `Value=7` (or `0`).
- **Expected:** Documented behavior: the value is clamped to the 1–6 range for display — `Value=7` shows ⚅ / `6/6`, `Value=0` shows ⚀ / `1/6` (clamping via `Math.Clamp`).
- **Actual:** Pass — `DiceRatingTests.Out_of_range_value_is_clamped` (automated).

### T2.6 — Active nav link
- **Steps:** Navigate between `/`, `/games`, `/game-night`.
- **Expected:** The active `NavLink` is highlighted on each page.
- **Actual:** Pass — `Header.razor` uses `NavLink` (`Match="NavLinkMatch.All"` for `/`, prefix match for `/games`, `/game-night`, `/admin/games`); the framework applies the active class (code, not re-screenshotted).

---

## Oppgave 3 — Game night

### T3.1 — Badge increments
- **Steps:** Add a game from `/games`. Watch the header badge.
- **Expected:** Badge count increments immediately.
- **Actual:** Pass — `Header` subscribes to `GameNight.OnChange` and renders `GameCounterBadge`; unit-tested by `GameNightStateTests.Add_game_increases_count` and `OnChange_fires_on_mutation_but_not_on_duplicate_add` (automated + code).

### T3.2 — State survives navigation
- **Steps:** Add a game, navigate to a details page and back.
- **Expected:** The game-night list is unchanged.
- **Actual:** Pass — `GameNightState` is `Scoped` (one instance per circuit), so the same instance is re-injected on each page; independent instances covered by `GameNightStateTests` (automated + code).

### T3.3 — Duplicate add
- **Steps:** Try to add the same game twice from the card and the details page.
- **Expected:** Only one entry; card shows an "already added" state.
- **Actual:** Pass — `GameNightStateTests.Add_duplicate_is_ignored`; `GameCardTests.IsInGameNight_disables_the_add_button` (automated).

### T3.4 — Removing the last game
- **Steps:** Add one game, then remove it.
- **Expected:** Empty-state message; total playtime shows `0`.
- **Actual:** Pass — `GameNightStateTests.Remove_existing_game_decreases_count`; `GameNight.razor` shows `EmptyState` when `Night.Count == 0` (automated + code).

### T3.5 — Total playtime with three games
- **Steps:** Add three games with known playtimes.
- **Expected:** Sum matches.
- **Actual:** Pass — `GameNightStateTests.TotalPlayTime_sums_all_selected_games` (45 + 60 + 90 = 195) (automated).

### T3.6 — Two tabs
- **Steps:** Open the app in two tabs and add a game in one.
- **Expected:** Each browser tab opens its own SignalR circuit, and `GameNightState` is `Scoped` — i.e. per circuit. Two tabs (and two users) therefore get **independent** game nights; adding a game in one tab does not appear in the other.
- **Actual:** Pass — follows the Scoped-per-circuit lifetime (`Program.cs AddScoped<GameNightState>`; README "Known Limitations"). Independent state instances are unit-tested in `GameNightStateTests` (automated + code).

### T3.7 — Responsive layout
- **Steps:** View `/games` at ~375 px and ≥1200 px.
- **Expected:** One column on narrow, multiple on wide. Nav usable in both.
- **Actual:** Screenshots at ~375 px and ≥1200 px are included in `README.md`; the shelf grid uses responsive Tailwind breakpoints (code). Re-run in the browser to re-confirm pixels after CSS changes.

---

## Oppgave 4 — Admin CRUD

### T4.1 — API down
- **Steps:** Stop the API and load `/games` or `/admin/games`.
- **Expected:** Error state shown; app does not crash.
- **Actual:** Pass — automated: `GamesErrorHandlingTests.Api_failure_shows_error_card_without_crashing` renders `Games` against an `HttpClient` whose handler throws and asserts the error card. `AdminGames` uses the same try/catch pattern (automated + code).

### T4.2 — Empty name
- **Steps:** Submit the admin form with an empty name.
- **Expected:** Validation error shown; no request sent.
- **Actual:** Pass — client form test `GameFormValidationTests.Empty_name_shows_required_error` renders the `EditForm` and asserts "Navn er påkrevd"; server `BoardGameValidatorTests.Empty_name_is_rejected`; API smoke: `POST /api/games` with empty name → 400 (automated + API smoke).

### T4.3 — MaxPlayers < MinPlayers
- **Steps:** Set MinPlayers=4, MaxPlayers=2 and submit.
- **Expected:** Validation error shown.
- **Actual:** Pass — client form test `GameFormValidationTests.MaxPlayers_below_min_shows_cross_field_error` plus the `GameFormModel.Validate` cross-field rule and server `BoardGameValidatorTests.MaxPlayers_below_min_is_rejected` (automated).

### T4.4 — Invalid payload to the API directly
- **Steps:** Send an invalid JSON body from a `.http` file.
- **Expected:** `400 Bad Request`.
- **Actual:** Pass — API smoke: `POST /api/games` with an empty name returns `400 ValidationProblem` (API smoke).

### T4.5 — Edit reflects everywhere
- **Steps:** Edit a game's name in `/admin/games`, then open `/games` and
  `/games/{id}`.
- **Expected:** Both show the updated name.
- **Actual:** Pass — `Games` and `GameDetails` both read from `GET /api/games*` (SQLite is the single source of truth), so an edit is reflected when the pages (re)load; API smoke confirms `PUT` returns 204. There is no live push to an already-open shelf page (API smoke + code).

### T4.6 — Delete a game that is on the game night
- **Steps:** Add a game to the game night, then delete it in the admin page.
- **Expected:** `GameNightState` keeps its own snapshot and does not observe deletions. Deleting a game in `/admin/games` removes it from the shelf/database but leaves an already-added copy in the current game night (no crash).
- **Actual:** Pass — code inspection: `GameNightState` has no delete hook, and `GameNight.razor` removes only via `Night.Remove`. Recorded as a known limitation.

### T4.7 — Details page for a missing ID
- **Steps:** Visit `/games/{id}` where the API returns 404.
- **Expected:** "Not found" message with a link back.
- **Actual:** Pass — `GET /api/games/404` → 404 renders `GameDetails`' not-found view with a back link (API smoke + code).

### T4.8 — "Next game" link
- **Steps:** Add a link to another game on the details page; click it.
- **Expected:** The URL and rendered game both change.
- **Actual:** Pass — implemented: `GameDetails` renders a "Neste spill" link to the next game in the catalog (wrapping at the end). `GameDetailsTests` covers the link href, the wrap-around, and the Id-change reload (automated).

---

## Automated test results

### How to run

```bash
dotnet test CafeDiceRoller.Tests/CafeDiceRoller.Tests.csproj
```

Frameworks: xUnit 2.9.3 · bUnit 1.40.0 · Microsoft.NET.Test.Sdk 17.14.1.
The test project targets `net10.0` and references the application project; the
main app still builds with **0 warnings, 0 errors**.

### Result (2026-10-30)

`Passed: 48 · Failed: 0 · Skipped: 0` — 32 logic tests, 16 bUnit component tests.

| Test file                  | Count | Area                      | Notes                                             |
|----------------------------|-------|---------------------------|---------------------------------------------------|
| `GameNightStateTests`      | 8     | Game-night state (logic)  | Add/remove, dedupe, total playtime, change event   |
| `BoardGameValidatorTests`  | 12    | Server validation (logic) | Valid + each edge rule (empty name, bounds, range) |
| `BoardGameTests`           | 6     | Player-count rule (logic) | Boundaries 1/2/4/5 and single-player band         |
| `GameDataSearchTests`      | 6     | Shelf filtering (logic)   | Case-insensitive, no match, range, combined       |
| `DiceRatingTests`          | 6     | Component (bUnit)         | Cycle, wrap 6→1, read-only, out-of-range clamp    |
| `GameCardTests`            | 3     | Component (bUnit)         | Render/link, add event, disabled when added       |
| `GameFormValidationTests`  | 3     | Component (bUnit)         | Empty name, min-players range, cross-field        |
| `GamesErrorHandlingTests`  | 1     | Component (bUnit)         | API-down renders error card, no crash             |
| `GameDetailsTests`         | 3     | Component (bUnit)         | Next-game link, wrap-around, Id-change reload     |

Coverage of the assignment's test requirements:

- **≥3 tests on non-component logic** — 32 logic tests (`GameNightState`,
  `BoardGameValidator`, `BoardGame`, `GameData`).
- **≥1 component test with bUnit** — 16 tests over `DiceRating`, `GameCard`,
  `GameForm`, `Games`, and `GameDetails`.
- **Normal and edge cases** — normal paths plus duplicates, missing keys,
  empty/whitespace input, out-of-range dice, boundary player counts, the
  6→1 wrap, a failing API, and form-validation error states.

### REST API smoke test (throwaway database)

Run against `ConnectionStrings__GameDb` pointing at a temporary SQLite file, so
the repository's `cafe_terningkast.db` is never modified by the tests.

| Case                      | Request                              | Result |
|---------------------------|--------------------------------------|--------|
| List all                  | `GET /api/games`                     | 200, 10 games |
| Get one                   | `GET /api/games/1`                   | 200    |
| Missing id                | `GET /api/games/9999`                | 404    |
| Invalid create            | `POST /api/games` (empty name)       | 400    |
| Create                    | `POST /api/games` (valid)            | 201    |
| Update                    | `PUT /api/games/{id}` (valid)        | 204    |
| Update, id mismatch       | `PUT /api/games/{id}` (body id ≠ route) | 400 |
| Delete                    | `DELETE /api/games/{id}`             | 204    |
| Get deleted               | `GET /api/games/{id}` (after delete) | 404    |

### Known warning

`dotnet test` reports one transitive-dependency advisory: **NU1902** —
`AngleSharp` 1.2.0, pinned by bUnit 1.40.0 (a test-only HTML parser used at test
time, never shipped with the app). Severity is moderate and there is no exploit
path in these tests; the version is pinned by bUnit and is left as-is rather
than force-overridden, to avoid breaking bUnit's internals.
