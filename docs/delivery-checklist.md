# Delivery Checklist – Kafé Terningkast

Deadline: **30 October 2026 at 23:59**. Submit one ZIP file in Moodle containing the code, `README.md`, design documentation, AI reflection, and a video file or video link. This checklist is based on the assignment brief.

**Status key:** `[x]` = done · `[ ] 🟡` = partial · `[ ]` = not done.
Assessed against the current codebase (project builds cleanly: 0 warnings, 0 errors).

---

## 1. Project and common requirements

- [x] The solution is based on the Blazor Web App template.
- [x] The solution can be built and run by following the instructions in `README.md`.
- [x] The game collection contains at least eight games. (`GameData` has 8, `SeedData` has 10.)
- [x] `BoardGame` contains all required fields: `Id`, `Name`, `MinPlayers`, `MaxPlayers`, `PlayTimeMinutes`, `Category`, `Rating`, and `Description`.
- [x] C# identifiers, component names, file names, and routes are in English.
- [x] PascalCase and camelCase are used as described in the brief.
- [x] Components are responsible for display and user actions; data, state, and API calls live in separate classes. (Data access is in `GameApiClient`, state in `GameNightState`, API routes in `GameEndpoints`. Filter logic still lives in `Games.razor.cs`, which is a display concern.)
- [x] The required components are self-developed, not replaced by a finished component library.
- [x] Naming is descriptive, and comments explain what the code does not already make clear.
- [ ] 🟡 Relevant Git history documents the work. (5 commits; most of the work is uncommitted.)

## 2. Oppgave 1 – The game shelf

### Functionality

- [x] `/games` shows the game collection.
- [x] Game data lives outside the component itself.
- [x] `GameCard` receives one game as a parameter and is not responsible for the list.
- [x] The list is rendered with `@foreach`.
- [x] `@if` is used to distinguish between matches and an empty list.
- [x] The search field uses `@bind` and filters on name while the user types.
- [x] Search is case-insensitive.
- [x] The player filter includes games when the count is between `MinPlayers` and `MaxPlayers`, inclusive.
- [x] The match count updates when the search or filter changes.
- [x] "Roll the dice" uses `@onclick`, picks among the matches, and highlights the chosen game.
- [x] The split of responsibility between the page and `GameCard` is explained in `README.md`.

### Tests

- [x] Search with lower and upper case gives the correct result.
- [x] A search with no matches shows a clear message.
- [x] "Roll the dice" does not crash with zero matches.
- [x] The player filter is tested with the boundary values 1, 2, 4, and 5 for a game with 2–4 players.
- [x] The combination of search and player filter gives the correct number of matches.
- [x] Clearing the search field shows the expected collection again.

## 3. Oppgave 2 – Dice rolls and game pages

### Functionality

- [x] `DiceRating` has the parameters `Value`, `ValueChanged` of type `EventCallback<int>`, and `ReadOnly`.
- [x] `DiceRating` supports `@bind-Value`.
- [x] The component does not change its own parameter value directly, but raises the new value through `ValueChanged`.
- [x] `DiceRating` is used read-only in `GameCard`.
- [x] `DiceRating` is used clickable on the game page.
- [x] `GameCard` reports "Add to game night" to the parent through an `EventCallback`.
- [x] `/games/{Id:int}` shows all the details for one game.
- [x] The game card lets the user open the game page.
- [x] A missing game gives a clear message and a link back to the shelf.
- [x] A new dice roll is displayed immediately.
- [x] The menu has a `NavLink` to the home page, the game shelf, and Min spillekveld, with active-link highlighting.
- [ ] `NavigationManager` is used for navigation from code in at least one place. (No `NavigationManager`/`NavigateTo` usage found; navigation is via `<a href>` and `NavLink`.)
- [x] Unknown addresses show a dedicated "Page not found" view with a link to the home page.
- [x] The parameters and events for `DiceRating` and `GameCard` are described in `README.md`.

### Tests

- [x] `/games/1` and a missing ID, e.g. `/games/9999`, are tested.
- [x] 🟡 `/games/abc` is tested, and it is explained why the address does not hit the game page. (The reason is noted in `docs/test-log.md`, but the case is not executed.)
- [ ] Dice roll 1 and 6 send the correct value to the parent.
- [ ] `ReadOnly` prevents changes on click.
- [ ] Values outside 1–6 have a defined and documented rendering.
- [x] Active menu highlighting follows the page the user is on.

## 4. Oppgave 3 – Min spillekveld (game night)

### Functionality and design

- [ ] 🟡 `/game-night` shows the selected games, the game count, and the total playtime. (Shows the game list, but not the count or total playtime. `GameNightState` exposes `Count` and `TotalPlayTimeMinutes`, but `GameNight.razor` does not render them.)
- [x] Games can be added and removed from both the shelf and the game page.
- [x] The game night lives in a separate state class registered as a service, e.g. `GameNightState`. (Implemented and registered in `Program.cs`.)
- [x] The state survives navigation between pages.
- [x] The selected-game count in the menu updates immediately without a full page load.
- [x] Components unsubscribe from state changes when they are removed, e.g. with `IDisposable`.
- [x] The same game cannot be added twice. (`GameNightState.Add` dedupes; `GameCard` and `GameDetails` disable the button when already added.)
- [x] `GameCard` shows whether the game is already selected. (`IsInGameNight` parameter fed by `Night.Contains`.)
- [x] At least one example of local state is identified and explained. (README "Local vs Application State" table; search/filter/rolled-game are page-local.)
- [x] It is explained why the game night is application state.
- [x] At least two options for sharing state are considered, and the choice is justified.
- [x] The service lifetime (scoped or singleton) is justified.
- [x] The behaviour with two users or two tabs is explained.
- [x] The shelf shows one column at ~375 px and several columns on a wide screen.
- [x] The menu works on both a narrow and a wide screen.

### Tests and screenshots

- [x] Adding a game increases the number in the menu.
- [x] Navigating to a game page and back preserves the game night.
- [x] An attempt at a double add is tested.
- [x] Removing the last game gives an empty state and a total playtime of 0.
- [ ] The total playtime is verified with at least three games.
- [ ] The behaviour in two tabs is tested and documented against the chosen service lifetime.
- [x] Screenshots at ~375 px and at least 1200 px are included in `README.md`.

## 5. Oppgave 4 – API and administration

### API and data access

- [x] The REST API is in the same solution and starts with at least eight games. (`GameEndpoints.cs`; `SeedData` seeds 10.)
- [x] `GET /api/games` returns 200 with all games.
- [x] `GET /api/games/{id}` returns 200 or 404.
- [ ] 🟡 `POST /api/games` returns 201 on creation and 400 on invalid data. (Returns 201, but the endpoint does not validate the body, so invalid data is not rejected with 400.)
- [ ] 🟡 `PUT /api/games/{id}` returns 200 or 204 on success, 400 on invalid data, and 404 for a missing game. (Returns 204/404 and 400 on id mismatch, but no model validation.)
- [x] `DELETE /api/games/{id}` returns 204 or 404.
- [x] All API calls are collected in one service class, e.g. `GameApiClient`.
- [x] The service gets `HttpClient` through dependency injection.
- [x] No component creates `HttpClient` itself or knows the API addresses.
- [x] The calls use `async` and `await`, with no `.Result` or `.Wait()`.
- [x] The correct lifecycle method is used for data fetching. (`OnInitializedAsync` in `Games` and `AdminGames`.)
- [ ] 🟡 The game page fetches the correct game when the URL parameter `Id` changes. (`GameDetails` reads synchronously from the in-memory `GameData` store in `OnParametersSet`, not from the API via `OnParametersSetAsync`.)
- [x] Pages that fetch data handle loading, error, and empty states. (`Games` and `AdminGames` handle loading/error/empty; `GameDetails` handles the not-found case.)

### Administration and validation

- [x] `/admin/games` supports creation, editing, and deletion.
- [x] The form uses data binding and validation, e.g. `EditForm`.
- [x] `Name` is required. (`[Required]` in `GameFormModel` and a DB constraint.)
- [x] `MinPlayers` is at least 1. (`[Range(1, 99)]` and a DB constraint.)
- [x] `MaxPlayers` is not lower than `MinPlayers`. (Cross-field `Validate` and a DB constraint.)
- [x] The playtime is greater than 0. (`[Range(1, 10000)]` and a DB constraint.)
- [x] The dice roll is between 1 and 6. (`[Range(1, 6)]` and a DB constraint.)
- [ ] Invalid data is rejected also on direct calls to the API. (The API endpoints do not validate the model.)
- [x] The user must confirm before deleting.
- [x] The list updates after creation, editing, and deletion without a full page load.
- [ ] Dice rolls from the game page are saved via the API. (The details page hint states "Vurderingen lagres ikke ennå".)
- [x] The chosen render mode is justified, including where the code runs and at least one difference from another mode.

### Tests and documented clarifications

- [ ] An unavailable API gives an error message without the app crashing.
- [ ] An empty name and an invalid player range give validation errors in the form.
- [ ] A direct API call with invalid data returns 400.
- [ ] Changes are visible both in the shelf and on the game page.
- [ ] The behaviour when deleting a game that is in Min spillekveld is defined, tested, and documented.
- [ ] A 404 from the API is handled on the game page.
- [ ] A link to another game, e.g. "Next game", is tested and shows the correct game after the `Id` changes.
- [ ] 🟡 Lifecycle, error handling, and render mode are explained in `README.md`. (Render mode and data flow yes; lifecycle and error handling only partially.)

## 6. Oppgave 5 – Design documentation and testing

### Design documentation

- [x] The documentation is in `README.md` or a separate file, e.g. `docs/design.md`.
- [x] The page overview has a table with route, page component, and purpose.
- [x] The component tree shows pages, components, parameters down, and events up.
- [x] A data-flow diagram shows one user action from click to updated screen.
- [x] The responsibility list describes each component and service in one sentence.
- [ ] 🟡 The diagrams match the finished code. (They now reference implemented `GameNightState`, `AdminGames`, and `GameApiClient`, but `docs/design.md` still contains stale notes — e.g. "the REST API still uses in-memory storage" — and placeholder text like "Replace with your own diagram".)

### Design choices and maintainability

- [x] At least three design choices are documented.
- [x] Each choice describes the problem, at least two alternatives, the chosen solution, and what the choice costs.
- [x] One choice concerns state sharing.
- [x] One choice concerns the render mode.
- [x] At least two examples show what is easy or hard to change in the solution.
- [x] Known limitations are described.

### Automated tests and manual test log

- [ ] A separate test project contains at least five automated tests. (No `CafeDiceRoller.Tests` project exists.)
- [ ] At least three tests cover logic outside the components.
- [ ] At least one test tests a component, e.g. `DiceRating` with bUnit.
- [ ] The tests cover both normal use and edge cases.
- [ ] 🟡 Test cases from Oppgave 1–4 that are not automated are listed in a manual test log. (`docs/test-log.md` exists with drafted cases.)
- [ ] 🟡 The test log contains test case, steps, expected result, and actual result. (Format present; "actual" columns are blank.)
- [ ] The test results are documented.

## 7. Oppgave 6 – Video

- [ ] The video is 5–8 minutes long.
- [ ] The video has screen sharing and clear audio.
- [ ] The same language, Norwegian or English, is used throughout the presentation.
- [ ] The game shelf, Min spillekveld, and the admin page are demonstrated.
- [ ] The app is shown on both a wide and a narrow screen.
- [ ] The component structure is explained based on the component tree.
- [ ] One central piece of code is shown, and the data flow is explained.
- [ ] At least one normal and one deviating case are demonstrated.
- [ ] One design choice is explained with the alternatives that were considered.
- [ ] One challenge and the work to solve it are described.
- [ ] Improvements, further work, and AI use are reflected upon.
- [ ] The video file or an accessible video link is in the ZIP file.

## 8. README and AI reflection

- [x] `README.md` explains how the solution is built and run.
- [x] Pages and components are described.
- [x] Design choices and the responsibility split are documented.
- [ ] 🟡 Test cases, results, and limitations are documented. (Cases and limitations yes; results no.)
- [x] Screenshots and any separate documentation are available from the submission.
- [x] The AI reflection describes how AI was used and what AI contributed.
- [x] The reflection explains how AI affected the solution.
- [x] Own assessments, changes, and further development of AI suggestions are described.
- [x] Any errors, limitations, or challenges with AI use are described.
- [x] If AI was not used, this is stated. (N/A – AI was used, so this is satisfied.)
- [x] You can explain the entire solution, including code, design choices, and central professional assessments.

## 9. Final check before upload

- [ ] All assignment areas and the video are delivered or genuinely attempted.
- [ ] Code, test project, `README.md`, design documentation, test log, AI reflection, and video or video link are included.
- [ ] The solution is mainly runnable, and any gaps are clearly documented.
- [ ] Everything is collected in one ZIP file.
- [ ] The ZIP file is submitted in Moodle before the deadline.
