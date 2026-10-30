# Setting up the automated tests (xUnit + bUnit)

Before this, I had never added a real test suite to a Blazor Web App. I knew the
assignment required a separate test project with at least five automated tests,
at least three covering non-component logic, and at least one exercising a
component such as `DiceRating` with bUnit — but I had no prior experience with
bUnit, or with deciding what belongs in a unit test versus a component test.

## What AI helped with

The setup happened in one pass, then grew as checklist gaps surfaced:

1. **Choosing the project shape.** With no solution file, the AI added the test
   project next to the web app and wired it with a project reference. It
   explained that the test project should target `net10.0` like the app, and —
   critically — that it needs a `FrameworkReference` to `Microsoft.AspNetCore.App`,
   otherwise Blazor components and bUnit cannot build.

2. **Selecting package versions.** The AI queried NuGet for the latest stable
   versions and pinned: xUnit 2.9.3, bUnit 1.40.0, Microsoft.NET.Test.Sdk
   17.14.1, and xunit.runner.visualstudio 2.8.2. It noted the runner major
   version must pair with the xUnit major version.

3. **Writing the initial suite.** The AI wrote:
   - `GameNightState`: add/remove, duplicates, total playtime, change event.
   - `BoardGameValidator`: every validation rule plus boundary values.
   - `BoardGame`: the inclusive player-count rule at boundaries 1/2/4/5.
   - `GameData`: case-insensitive search, no match, player range, combined filter.
   - `DiceRating` (bUnit): click cycle, the 6→1 wrap, read-only, out-of-range clamp.
   - `GameCard` (bUnit): rendering, the add event, and the disabled state.

4. **Diagnosing the first build failure.** The first `dotnet test` produced a
   wall of `CS0246` errors. The cause: the web `.csproj` globs `**/*.cs`, so it
   was compiling the test source files into the app build (which has no xUnit).
   The AI fixed it with a `DefaultItemExcludes` entry for
   `CafeDiceRoller.Tests/**`.

5. **Diagnosing a bUnit API detail.** The next compile error was `AddSingleton`
   not being found on bUnit's service provider. The AI identified the reason:
   `TestServiceProvider` implements `IServiceCollection`, so the DI extension
   methods only appear with `using Microsoft.Extensions.DependencyInjection;`.
   It also found that bUnit auto-registers a `FakeNavigationManager`, so
   components that inject `NavigationManager` render with no extra setup.

6. **Simulating the API.** For the error-handling and page tests, the AI showed
   how to inject `GameApiClient` backed by a custom `HttpMessageHandler` — one
   that throws (to prove the "API down" error card) or one that returns canned
   JSON (to test `GameDetails` reload-on-Id-change and the "next game" link).

## What I changed or verified myself

- **Ran the suite myself.** I ran `dotnet test` after each addition and watched
  the count go from 41 to 48 passing, with zero failures.
- **Confirmed the app still builds clean.** After the web `.csproj` edit, I ran
  `dotnet build` on the app and confirmed 0 warnings, 0 errors.
- **Reviewed the tests for real members.** I checked that the tests exercise
  actual code (`GameNightState.Add`, `BoardGameValidator.Validate`, the
  `DiceRating` cycle) rather than assumptions, and read each generated test file
  before accepting it.
- **Kept the manual side honest.** Alongside automation I carried out the REST
  API smoke test and a runtime route check (`/games/1`, `/games/9999`,
  `/games/abc`) so every "actual" in `docs/test-log.md` has real evidence.

## Errors, limitations, and things to watch for

- **Globbing pitfall.** The web project compiled the test `.cs` files until the
  `DefaultItemExcludes` exclusion was added. Lesson: a test project nested
  inside a web SDK project needs an explicit exclude when the app globs source.
- **Missing DI using.** `AddSingleton` did not resolve until
  `using Microsoft.Extensions.DependencyInjection;` was added — a subtle bUnit
  1.x difference from older examples found online.
- **A vulnerability advisory.** `dotnet test` reports NU1902: bUnit 1.40.0 pins
  AngleSharp 1.2.0 (a test-only HTML parser) which has a moderate advisory.
  There is no exploit path in these tests, so I left the version as-is rather
  than force-overriding it, and documented the warning instead of hiding it.

## What I understand now that I didn't before

- **Why bUnit is needed.** Plain xUnit cannot render a Razor component or click
  its buttons; bUnit provides a real renderer and event dispatcher.
- **Logic vs. component tests.** Pure classes (`GameNightState`, validators,
  filter logic) are tested directly; components are tested by rendering them
  with controlled parameters and asserting markup and raised events.
- **Testing the data layer without a database.** Swapping the `HttpClient`'s
  handler simulates an API outage or a canned response with no network or
  SQLite involved.
- **The web SDK globbing rule.** Why a nested test project needs an explicit
  exclusion, and how `DefaultItemExcludes` guards it.

## Net effect on the final solution

The test project started from nothing and became `CafeDiceRoller.Tests/` with
48 passing tests — 32 logic tests and 16 bUnit component tests. It satisfies
the assignment's requirements (≥5 tests, ≥3 logic, ≥1 bUnit component test, and
both normal and edge cases), and the results are recorded in `docs/test-log.md`.

The AI accelerated the *scaffolding and diagnosis* — choosing the frameworks,
writing the first suite, and resolving two non-obvious build errors — while I
ran every command, read the output, and decided what to keep. Writing tests is
now a repeatable part of my workflow for this project.
