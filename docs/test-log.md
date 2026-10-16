# Manual Test Log — Kafé Terningkast

Automated tests live in `CafeDiceRoller.Tests/` and are run with `dotnet test`.
This file records the manual test cases from Oppgave 1–4 that are not covered
by automation.

Format: **scenario → steps → expected → actual**.

---

## Oppgave 1 — Game shelf

### T1.1 — Search with mixed case
- **Steps:** Open `/games`. Type `cat` in the search field. Clear and type `CAT`.
- **Expected:** Both searches return "Catan". Search is case-insensitive.
- **Actual:** _(fill in)_

### T1.2 — Search with no matches
- **Steps:** Search for `zzzzz`.
- **Expected:** Empty-state message; "Trill terningen" does nothing and does not
  throw.
- **Actual:** _(fill in)_

### T1.3 — Player count boundaries
- **Steps:** Set player filter to 2, 4, 1, 5 with a 2–4 player game in view.
- **Expected:** Visible at 2 and 4; hidden at 1 and 5.
- **Actual:** _(fill in)_

### T1.4 — Combined search and filter
- **Steps:** Search for `a` and set player filter to 3.
- **Expected:** Only games whose name contains `a` *and* support 3 players.
  Result count matches the cards shown.
- **Actual:** _(fill in)_

### T1.5 — Clearing search
- **Steps:** Type a query, then clear the field.
- **Expected:** Full collection reappears.
- **Actual:** _(fill in)_

---

## Oppgave 2 — Details and dice

### T2.1 — Valid and invalid ID
- **Steps:** Visit `/games/1` and `/games/9999`.
- **Expected:** `/games/1` shows the game. `/games/9999` shows a "not found"
  message with a link back to `/games`.
- **Actual:** _(fill in)_

### T2.2 — Non-numeric ID
- **Steps:** Visit `/games/abc`.
- **Expected:** Does **not** match `/games/{Id:int}`; falls through to
  `NotFound.razor`.
- **Actual:** _(fill in)_

### T2.3 — Dice click passes value to parent
- **Steps:** On a game details page, click dice face 1, then 6.
- **Expected:** Parent receives 1, then 6, and re-renders.
- **Actual:** _(fill in)_

### T2.4 — ReadOnly dice ignores clicks
- **Steps:** On `/games`, click a `GameCard`'s dice widget.
- **Expected:** No change, no `ValueChanged` fired.
- **Actual:** _(fill in)_

### T2.5 — Out-of-range dice value
- **Steps:** Render `DiceRating` with `Value=7` (or `0`).
- **Expected:** _(define the documented behavior here and verify)_
- **Actual:** _(fill in)_

### T2.6 — Active nav link
- **Steps:** Navigate between `/`, `/games`, `/game-night`.
- **Expected:** The active `NavLink` is highlighted on each page.
- **Actual:** _(fill in)_

---

## Oppgave 3 — Game night

### T3.1 — Badge increments
- **Steps:** Add a game from `/games`. Watch the header badge.
- **Expected:** Badge count increments immediately.
- **Actual:** _(fill in)_

### T3.2 — State survives navigation
- **Steps:** Add a game, navigate to a details page and back.
- **Expected:** The game-night list is unchanged.
- **Actual:** _(fill in)_

### T3.3 — Duplicate add
- **Steps:** Try to add the same game twice from the card and the details page.
- **Expected:** Only one entry; card shows an "already added" state.
- **Actual:** _(fill in)_

### T3.4 — Removing the last game
- **Steps:** Add one game, then remove it.
- **Expected:** Empty-state message; total playtime shows `0`.
- **Actual:** _(fill in)_

### T3.5 — Total playtime with three games
- **Steps:** Add three games with known playtimes.
- **Expected:** Sum matches.
- **Actual:** _(fill in)_

### T3.6 — Two tabs
- **Steps:** Open the app in two tabs and add a game in one.
- **Expected:** _(define based on the chosen service lifetime)_
- **Actual:** _(fill in)_

### T3.7 — Responsive layout
- **Steps:** View `/games` at ~375 px and ≥1200 px.
- **Expected:** One column on narrow, multiple on wide. Nav usable in both.
- **Actual:** _(fill in — attach screenshots to the repo, e.g. `docs/screenshots/`)_

---

## Oppgave 4 — Admin CRUD

### T4.1 — API down
- **Steps:** Stop the API and load `/games` or `/admin/games`.
- **Expected:** Error state shown; app does not crash.
- **Actual:** _(fill in)_

### T4.2 — Empty name
- **Steps:** Submit the admin form with an empty name.
- **Expected:** Validation error shown; no request sent.
- **Actual:** _(fill in)_

### T4.3 — MaxPlayers < MinPlayers
- **Steps:** Set MinPlayers=4, MaxPlayers=2 and submit.
- **Expected:** Validation error shown.
- **Actual:** _(fill in)_

### T4.4 — Invalid payload to the API directly
- **Steps:** Send an invalid JSON body from a `.http` file.
- **Expected:** `400 Bad Request`.
- **Actual:** _(fill in)_

### T4.5 — Edit reflects everywhere
- **Steps:** Edit a game's name in `/admin/games`, then open `/games` and
  `/games/{id}`.
- **Expected:** Both show the updated name.
- **Actual:** _(fill in)_

### T4.6 — Delete a game that is on the game night
- **Steps:** Add a game to the game night, then delete it in the admin page.
- **Expected:** _(define behavior and verify)_
- **Actual:** _(fill in)_

### T4.7 — Details page for a missing ID
- **Steps:** Visit `/games/{id}` where the API returns 404.
- **Expected:** "Not found" message with a link back.
- **Actual:** _(fill in)_

### T4.8 — "Next game" link
- **Steps:** Add a link to another game on the details page; click it.
- **Expected:** The URL and rendered game both change.
- **Actual:** _(fill in)_