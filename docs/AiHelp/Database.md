# Setting up the database (EF Core + SQLite)

This was the first time I had set up a database from scratch. I had no prior
experience with Entity Framework Core, migrations, or SQLite. AI was used
heavily as a **guided walkthrough** for this part, in a back-and-forth format
rather than a single prompt.

## Context and constraints

Before starting, I knew:

- The assignment asks for a REST API and CRUD, but does not require a specific
  database.
- I wanted something local and simple — no server to install.
- I had heard of Drizzle ORM and initially asked whether that could be used.

The first thing the AI did was correct a misconception: **Drizzle is a
TypeScript/JavaScript ORM and does not run inside a .NET Blazor app.** It
recommended **Entity Framework Core + SQLite** as the idiomatic .NET choice,
and explained why: one runtime, one language, native DI integration, and a
single-file database.

**What I kept:** the recommendation and the reasoning.
**What I rejected:** nothing at this stage — this was new information, and the
trade-offs matched the assignment's constraints.

## What AI helped with

The setup was walked through step by step:

1. **Choosing package versions.** The AI first suggested EF Core 9, which was
   wrong for my project. After I pointed out that I was not on .NET 9, it
   asked me to confirm my SDK and target framework, then corrected to
   `10.*`. It explained that the package major version must match the target
   framework major version, and recommended letting NuGet resolve versions
   rather than hardcoding them.

2. **Installing the CLI tool.** When `dotnet ef --version` failed, the AI
   explained that the `Design` NuGet package provides the *libraries* but not
   the *CLI*. It showed the `dotnet tool install --global dotnet-ef` command
   and the `export PATH="$PATH:$HOME/.dotnet/tools"` fix, plus the note to
   persist it in `~/.bashrc`.

3. **Writing `GameDbContext.cs`.** The AI provided the context class,
   including check constraints for `Rating BETWEEN 1 AND 6`,
   `MinPlayers >= 1 AND MaxPlayers >= MinPlayers`, and
   `PlayTimeMinutes > 0`. It explained *why* these belong in the database as
   well as the UI: defence in depth.

4. **Registering the context in `Program.cs`.** The AI showed the
   `AddDbContext` call, the connection string in `appsettings.json`, and the
   `db.Database.Migrate()` call on startup.

5. **Running migrations.** The AI explained what each command does
   (`migrations add` writes the C# migration; `database update` runs it
   against the `.db` file) and what output to expect.

6. **Seeding.** The AI wrote `SeedData.cs` with an idempotent `Any()` check
   and 10 games covering solo, party, strategy, family, abstract, and co-op
   categories — chosen deliberately so search and player-count filters have
   meaningful edge cases to test.

## What I changed or verified myself

- **Version correction.** The initial `9.0.0` suggestion was wrong; I caught
  it and the AI corrected to `10.*`. This is a good example of why the
  candidate still has to sanity-check AI output against the environment.
- **Ran every command manually.** I typed each `dotnet ef` command, read the
  output, and confirmed the `.db` file was created. The AI predicted the
  output; I verified it.
- **Read the generated migration.** I opened
  `Migrations/20261012170545_InitialCreate.cs` to see the actual `CREATE TABLE`
  SQL, and confirmed the three check constraints were present.
- **Confirmed the seed.** After running the app once, I checked the row count
  in `sqlite3` and saw 10 rows.

## Errors, limitations, and things to watch for

- **Version mismatch.** The AI's first package suggestion used EF Core 9,
  which would have failed restore on .NET 10. Lesson: always check
  `dotnet --version` *and* the `<TargetFramework>` in the `.csproj` before
  accepting package versions from an AI.
- **CLI vs library confusion.** The AI initially implied that installing the
  `Design` NuGet package was enough to run `dotnet ef`. It wasn't — the
  global tool is separate. This was corrected in a follow-up.
- **No guidance on git-ignoring the DB.** The AI did not proactively mention
  that `.db`, `.db-shm`, and `.db-wal` files should be git-ignored. I added
  them to `.gitignore` myself and noted it in the README.
- **The API still uses in-memory storage.** The database is currently used on
  the read side (`GameData`). Refactoring the REST API in Oppgave 4 to use
  `GameDbContext` is a follow-up task, documented as a known limitation.

## What I understand now that I didn't before

- **Code-first vs database-first.** I can explain why we define `BoardGame`
  in C# and let EF generate the schema, rather than writing SQL by hand.
- **What a migration is.** A migration is a versioned C# description of a
  schema change. Committing it to Git means every clone can reconstruct the
  exact schema.
- **What `DbContext` does.** It is a unit-of-work and a session with the
  database. It tracks changes, translates LINQ to SQL, and manages the
  connection.
- **Why check constraints matter.** Even if the UI or API has a bug,
  the database refuses invalid rows. Defence in depth is cheap.
- **DI lifetime and `Scoped`.** `AddDbContext` registers as `Scoped` by
  default, which is correct for Blazor Server — one context per SignalR
  circuit, not shared across users.

## Net effect on the final solution

The database layer is the part of the project I understood the least when I
started. After this walkthrough, I can:

- Explain every line in `GameDbContext.cs` and `Program.cs`.
- Create a new migration from scratch when the model changes.
- Reset the database and re-seed without asking for help.
- Articulate why SQLite + EF Core was the right choice for this assignment,
  and what would be different with Postgres or with an in-memory store.

The AI accelerated the *learning*, not the *doing*. Every command was run by
me, and every file was read before it was accepted.
