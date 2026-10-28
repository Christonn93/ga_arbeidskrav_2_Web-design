<img src="https://r2cdn.perplexity.ai/pplx-full-logo-primary-dark%402x.png" style="height:64px;margin-right:32px"/>

# If I want to remove the @code part and use inject instead. Where would you recommend that I would place this and how would the code look like ?

@page "/games/{Id:int}"
@using CafeDiceRoller.Models
@using CafeDiceRoller.Services
@using CafeDiceRoller.Components.Ui.Rating
@inject GameData GameData

```
\<PageTitle>@(Game?.Name ?? "Spill ikke funnet") – Kafé Terningkast\</PageTitle>
```

\<div class="game-detail">
\<nav class="game-detail\_\_navigation" aria-label="Tilbake">
\<a href="/games" class="game-detail\_\_back">

```
\<span aria-hidden="true">←\</span>
```

Tilbake til spillhylla
\</a>
\</nav>

```
@if (Game is not { } game)
{
    <section class="game-detail__empty" aria-labelledby="not-found-title">
        <p class="game-detail__eyebrow">Spillhylla</p>

        <h1 id="not-found-title" class="game-detail__empty-title">
            Spillet finnes ikke
        </h1>

        <p class="game-detail__description">
            Vi fant ingen spill med ID @Id i hylla.
            Det kan ha blitt fjernet, eller lenken kan være feil.
        </p>

        <a href="/games" class="game-detail__button">
            Se alle spill
        </a>
    </section>
}
else
{
    <article class="game-detail__layout" aria-labelledby="game-title">
        <div class="game-detail__main">
            <header class="game-detail__header">
                <p class="game-detail__eyebrow">@game.Category</p>

                <h1 id="game-title" class="game-detail__title">
                    @game.Name
                </h1>

                <p class="game-detail__description">
                    @game.Description
                </p>
            </header>

            <dl class="game-detail__facts">
                <div class="game-detail__fact">
                    <dt>Spillere</dt>
                    <dd>
                        @if (game.MinPlayers == game.MaxPlayers)
                        {
                            @game.MinPlayers
                        }
                        else
                        {
                            <span>@game.MinPlayers–@game.MaxPlayers</span>
                        }
                        <span class="game-detail__unit">personer</span>
                    </dd>
                </div>

                <div class="game-detail__fact">
                    <dt>Spilletid</dt>
                    <dd>
                        @game.PlayTimeMinutes
                        <span class="game-detail__unit">minutter</span>
                    </dd>
                </div>
            </dl>
        </div>

        <aside class="game-detail__panel" aria-labelledby="game-night-title">
            <div class="game-detail__panel-heading">
                <p class="game-detail__eyebrow">Neste spillekveld</p>

                <h2 id="game-night-title" class="game-detail__panel-title">
                    Ta det med til bordet
                </h2>

                <p class="game-detail__panel-description">
                    Legg spillet til i utvalget for spillekvelden.
                </p>
            </div>

            <button type="button"
                    class="game-detail__button"
                    @onclick="HandleAddToNight"
                    disabled="@IsInGameNight">
                @(IsInGameNight
                    ? "Lagt til i spillekveld"
                    : "Legg til i spillekveld")
            </button>

            <p class="game-detail__status"
               role="status"
               aria-atomic="true">
                @(IsInGameNight
                    ? "Spillet er valgt for denne visningen."
                    : "Samle spillene du vil spille.")
            </p>

            <section class="game-detail__rating"
                     aria-labelledby="rating-title">
                <div class="game-detail__rating-heading">
                    <h3 id="rating-title" class="game-detail__rating-title">
                        Ditt terningkast
                    </h3>

                    <span class="game-detail__rating-scale">
                        1–6
                    </span>
                </div>

                <div class="game-detail__rating-control">
                    <DiceRating @bind-Value="LocalRating" />
                </div>

                <p class="game-detail__hint">
                    Klikk på terningen for å endre vurderingen.
                    Vurderingen lagres ikke ennå.
                </p>
            </section>
        </aside>
    </article>
}
```

\</div>

@code {
[Parameter]
public int Id { get; set; }

```
private BoardGame? Game { get; set; }

private int LocalRating { get; set; } = 1;

private bool IsInGameNight { get; set; }

private int? _loadedGameId;

protected override void OnParametersSet()
{
    Game = GameData.GetById(Id);

    if (_loadedGameId == Id)
    {
        return;
    }

    _loadedGameId = Id;
    LocalRating = Game?.Rating ?? 1;
    IsInGameNight = false;
}

private void HandleAddToNight()
{
    if (Game is null || IsInGameNight)
    {
        return;
    }

    // Local placeholder until GameNightState is connected.
    IsInGameNight = true;
}
```

}

Please, provide me full code and structure suggestion but also explain it for me.

I recommend placing the page’s C\# logic in a **code-behind** file named `GameDetails.razor.cs`, directly beside `GameDetails.razor`.

You can then remove the entire `@code` block and inject `GameData` using `[Inject]` in the C\# file. Blazor supports splitting a component’s markup and C\# into matching partial-class files.[^1_1][^1_2]

One important distinction: injection does not replace your component logic. It supplies dependencies such as `GameData`. Your route parameter, lifecycle method, click handler, and local UI state still belong to the component—just in a separate file.[^1_3][^1_4]

## 1. Recommended structure

Keep your existing structure and add one file:

```text
CafeDiceRoller/
├── Components/
│   ├── Home/
│   ├── Layout/
│   ├── Pages/
│   │   ├── AdminGames.razor
│   │   ├── Error.razor
│   │   ├── GameDetails.razor
│   │   ├── GameDetails.razor.cs    ← Add this
│   │   ├── GameNight.razor
│   │   ├── Games.razor
│   │   ├── Home.razor
│   │   └── NotFound.razor
│   ├── Shared/
│   ├── Ui/
│   ├── _Imports.razor
│   ├── App.razor
│   └── Routes.razor
├── Models/
│   ├── BoardGame.cs
│   ├── GameCategories.cs
│   └── GameFormModel.cs
├── Services/
│   └── GameData.cs
└── Program.cs
```

For this page, I would not introduce a separate `GameDetailsService` or `GameDetailsBase` yet. The code currently handles page-specific behavior, so keeping it beside the page is a straightforward separation of markup from logic.

## 2. `Components/Pages/GameDetails.razor`

This contains your existing markup, without `@code` or `@inject`.

The `GameData` dependency moves into the C\# file, so the service and model `@using` directives are no longer needed here.

```razor
@page "/games/{Id:int}"
@using CafeDiceRoller.Components.Ui.Rating

<PageTitle>@(Game?.Name ?? "Spill ikke funnet") – Kafé Terningkast</PageTitle>

<div class="game-detail">
    <nav class="game-detail__navigation" aria-label="Tilbake">
        <a href="/games" class="game-detail__back">
            <span aria-hidden="true">←</span>
            Tilbake til spillhylla
        </a>
    </nav>

    @if (Game is not { } game)
    {
        <section class="game-detail__empty" aria-labelledby="not-found-title">
            <p class="game-detail__eyebrow">Spillhylla</p>

            <h1 id="not-found-title" class="game-detail__empty-title">
                Spillet finnes ikke
            </h1>

            <p class="game-detail__description">
                Vi fant ingen spill med ID @Id i hylla.
                Det kan ha blitt fjernet, eller lenken kan være feil.
            </p>

            <a href="/games" class="game-detail__button">
                Se alle spill
            </a>
        </section>
    }
    else
    {
        <article class="game-detail__layout" aria-labelledby="game-title">
            <div class="game-detail__main">
                <header class="game-detail__header">
                    <p class="game-detail__eyebrow">@game.Category</p>

                    <h1 id="game-title" class="game-detail__title">
                        @game.Name
                    </h1>

                    <p class="game-detail__description">
                        @game.Description
                    </p>
                </header>

                <dl class="game-detail__facts">
                    <div class="game-detail__fact">
                        <dt>Spillere</dt>
                        <dd>
                            @if (game.MinPlayers == game.MaxPlayers)
                            {
                                @game.MinPlayers
                            }
                            else
                            {
                                <span>@game.MinPlayers–@game.MaxPlayers</span>
                            }

                            <span class="game-detail__unit">personer</span>
                        </dd>
                    </div>

                    <div class="game-detail__fact">
                        <dt>Spilletid</dt>
                        <dd>
                            @game.PlayTimeMinutes
                            <span class="game-detail__unit">minutter</span>
                        </dd>
                    </div>
                </dl>
            </div>

            <aside class="game-detail__panel" aria-labelledby="game-night-title">
                <div class="game-detail__panel-heading">
                    <p class="game-detail__eyebrow">Neste spillekveld</p>

                    <h2 id="game-night-title" class="game-detail__panel-title">
                        Ta det med til bordet
                    </h2>

                    <p class="game-detail__panel-description">
                        Legg spillet til i utvalget for spillekvelden.
                    </p>
                </div>

                <button type="button"
                        class="game-detail__button"
                        @onclick="HandleAddToNight"
                        disabled="@IsInGameNight">
                    @(IsInGameNight
                        ? "Lagt til i spillekveld"
                        : "Legg til i spillekveld")
                </button>

                <p class="game-detail__status"
                   role="status"
                   aria-atomic="true">
                    @(IsInGameNight
                        ? "Spillet er valgt for denne visningen."
                        : "Samle spillene du vil spille.")
                </p>

                <section class="game-detail__rating"
                         aria-labelledby="rating-title">
                    <div class="game-detail__rating-heading">
                        <h3 id="rating-title" class="game-detail__rating-title">
                            Ditt terningkast
                        </h3>

                        <span class="game-detail__rating-scale">
                            1–6
                        </span>
                    </div>

                    <div class="game-detail__rating-control">
                        <DiceRating @bind-Value="LocalRating" />
                    </div>

                    <p class="game-detail__hint">
                        Klikk på terningen for å endre vurderingen.
                        Vurderingen lagres ikke ennå.
                    </p>
                </section>
            </aside>
        </article>
    }
</div>
```

Your existing styles can remain unchanged.

## 3. `Components/Pages/GameDetails.razor.cs`

Create this file next to `GameDetails.razor`:

```csharp
using CafeDiceRoller.Models;
using CafeDiceRoller.Services;
using Microsoft.AspNetCore.Components;

namespace CafeDiceRoller.Components.Pages;

public partial class GameDetails
{
    [Inject]
    private GameData GameData { get; set; } = default!;

    [Parameter]
    public int Id { get; set; }

    private BoardGame? Game { get; set; }

    private int LocalRating { get; set; } = 1;

    private bool IsInGameNight { get; set; }

    private int? _loadedGameId;

    protected override void OnParametersSet()
    {
        Game = GameData.GetById(Id);

        if (_loadedGameId == Id)
        {
            return;
        }

        _loadedGameId = Id;
        LocalRating = Game?.Rating ?? 1;
        IsInGameNight = false;
    }

    private void HandleAddToNight()
    {
        if (Game is null || IsInGameNight)
        {
            return;
        }

        // Local placeholder until GameNightState is connected.
        IsInGameNight = true;
    }
}
```

This preserves your current behavior, including refreshing `Game` when parameters are applied while only resetting the rating and selection flag when the ID changes.

## 4. Check service registration in `Program.cs`

`[Inject]` requires the service to be registered with dependency injection. Services are registered before building the application.[^1_3]

Since your current page already uses `@inject GameData GameData`, this registration may already exist. If it does, leave it alone.

If your existing `GameData` is intended to be scoped, the relevant registration is:

```csharp
using CafeDiceRoller.Services;

// Existing builder setup...

builder.Services.AddScoped<GameData>();

// Existing builder.Build() and application setup...
```

Add it before:

```csharp
var app = builder.Build();
```

Do not replace your whole `Program.cs` with this snippet or add a second registration unnecessarily.

## 5. How the two files work together

### `partial` means “two files, one component”

These files are not separate objects:

```text
GameDetails.razor
        +
GameDetails.razor.cs
        ↓
One GameDetails component
```

Blazor generates a partial component class from the `.razor` file. Your `public partial class GameDetails` contributes the C\# members to that same class. That is why the markup can access `Game`, `LocalRating`, and `HandleAddToNight` even though they are private.[^1_2][^1_1]

The important matches are:


| Item | Value |
| :-- | :-- |
| Razor filename | `GameDetails.razor` |
| C\# filename | `GameDetails.razor.cs` |
| C\# class | `GameDetails` |
| Namespace | `CafeDiceRoller.Components.Pages` |

The namespace above follows your project name and folder structure. If you have explicitly overridden the component namespace with `@namespace` or a different root namespace, the C\# namespace must match that instead.[^1_1]

You do not need `@inherits GameDetails` or a new base class for this approach.[^1_2]

### `[Inject]` supplies the service

Your original directive:

```razor
@inject GameData GameData
```

becomes:

```csharp
[Inject]
private GameData GameData { get; set; } = default!;
```

Both request a `GameData` dependency from Blazor’s dependency injection system. Property injection takes place before the component’s initialization lifecycle methods run.[^1_5][^1_3]

Here:

- The first `GameData` is the service type.
- The second `GameData` is the property name.
- `default!` suppresses the compiler’s uninitialized non-nullable-property warning; Blazor is responsible for supplying the actual instance.

Use one injection declaration or the other—not both with the same property name.

### `[Parameter]` still belongs to the page

Your route declares:

```razor
@page "/games/{Id:int}"
```

The matching component parameter is:

```csharp
[Parameter]
public int Id { get; set; }
```

Blazor supplies route parameter values to the component’s parameter properties. `Id` is therefore not something to inject: it comes from the URL, not from a service registration.[^1_5][^1_1]

### Keep `OnParametersSet()` for ID-dependent loading

Your existing lifecycle choice is appropriate because the displayed game depends on `Id`. Microsoft recommends `OnParametersSet` or `OnParametersSetAsync` when component state depends on parameter values or changes to those values.[^1_5]

For example, navigating between:

```text
/games/1
/games/2
```

must update the selected game.

Your `_loadedGameId` check also prevents repeated parameter processing for the same ID from resetting the user’s local rating and selection flag.

## 6. What should go in a service later?

For your current implementation, I would keep responsibilities divided like this:


| Responsibility | Recommended location |
| :-- | :-- |
| Page HTML and bindings | `GameDetails.razor` |
| Route parameter and lifecycle handling | `GameDetails.razor.cs` |
| Temporary rating value | `GameDetails.razor.cs` |
| Temporary “added” flag | `GameDetails.razor.cs` |
| Looking up games | Existing `Services/GameData.cs` |
| Selection shared with the game-night page | A future shared game-night state service |
| Saving a rating | Your data/API service when persistence is implemented |

The reason to introduce another service would be shared behavior or shared state—not merely to remove an `@code` block. Injection lets components consume those services, while component-specific event and lifecycle handling can remain in the code-behind.[^1_4][^1_3]

For now, the two-file split is enough: it removes the C\# block from your markup, keeps your existing structure, and avoids changing what the page does.

One operational detail: this refactor does not enable interactivity by itself. Keep your existing interactive render-mode setup so `@onclick` and the rating binding continue to work; the two-file split only changes where the code lives.[^1_1]

<span style="display:none">[^1_10][^1_11][^1_12][^1_13][^1_14][^1_15][^1_16][^1_17][^1_18][^1_19][^1_20][^1_21][^1_6][^1_7][^1_8][^1_9]</span>

<div align="center">⁂</div>

[^1_1]: https://learn.microsoft.com/en-us/aspnet/core/blazor/components/?view=aspnetcore-10.0

[^1_2]: https://claudiobernasconi.ch/blog/how-to-use-code-behind-for-blazor-components/

[^1_3]: https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/dependency-injection?view=aspnetcore-10.0

[^1_4]: https://learn.microsoft.com/en-us/answers/questions/770182/code-section-in-blazor

[^1_5]: https://learn.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle?view=aspnetcore-10.0

[^1_6]: image.jpg

[^1_7]: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0

[^1_8]: https://www.telerik.com/blogs/blazor-basics-dependency-injection-best-practices-use-cases

[^1_9]: https://learn.microsoft.com/en-us/aspnet/core/blazor/components/lifecycle?view=aspnetcore-11.0

[^1_10]: https://blazor-university.com/dependency-injection/injecting-dependencies-into-blazor-components

[^1_11]: https://stackoverflow.com/questions/60551182/how-to-add-a-partial-class-to-a-component-in-blazor-in-visual-studio-2019

[^1_12]: preferences.code_style.shared_classes

[^1_13]: preferences.code_comments.style

[^1_14]: https://www.perplexity.ai/search/f89c0b77-2143-4cca-939e-b221abe12af1

[^1_15]: https://www.perplexity.ai/search/86f81537-7949-4ee1-82ab-419b51307562

[^1_16]: https://www.perplexity.ai/search/02c5c050-0c2a-4abc-a93d-918859bf4a31

[^1_17]: https://www.perplexity.ai/search/a3c98091-5ed2-4ca7-86e8-3943e4156a6b

[^1_18]: https://www.perplexity.ai/search/e9c2cc65-a053-46c1-9580-e8e0c7cb9704

[^1_19]: https://www.perplexity.ai/search/d205b0cf-4c19-4ea2-aa76-c8463816f5b7

[^1_20]: https://www.perplexity.ai/search/9d505d0d-bbde-4d7c-8c10-c03b1aae3e44

[^1_21]: https://www.perplexity.ai/search/5c62810b-e3e0-4724-934c-0cdbb6e88c2e

