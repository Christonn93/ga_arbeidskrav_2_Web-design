using Blazorise;
using Blazorise.Icons.Lucide;
using CafeDiceRoller.Components;
using CafeDiceRoller.Data;
using CafeDiceRoller.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Razor components with Interactive Server rendering.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Blazorise with Lucide icons.
builder.Services
    .AddBlazorise()
    .AddEmptyProviders()
    .AddLucideIcons();

// In-memory catalog.
builder.Services.AddScoped<GameData>();

// SQLite database.
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("GameDb")
        ?? "Data Source=cafe_terningkast.db"));

// Typed HTTP client.
builder.Services.AddHttpClient<GameApiClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["GameApi:BaseUrl"]
        ?? throw new InvalidOperationException(
            "Missing configuration: GameApi:BaseUrl"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(
        "/Error",
        createScopeForErrors: true);

    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();

// Register the CRUD API endpoints.
app.MapGameEndpoints();

// Register Blazor components.
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Apply migrations and seed the database.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<GameDbContext>();

    db.Database.Migrate();
    SeedData.Initialize(db);
}

app.Run();