// TypographyBase.cs
using Microsoft.AspNetCore.Components;

namespace CafeDiceRoller.Components.Ui.Typography;

public class TypographyBase : ComponentBase
{
    // Typography variant name (h1, body, muted, etc.).
    [Parameter] public string Variant { get; set; } = "body";
    // Extra CSS classes appended to the variant class.
    [Parameter] public string? Class { get; set; }
    // Content rendered inside the typography element.
    [Parameter] public RenderFragment? ChildContent { get; set; }

    // Combines the variant class with any custom class.
    protected string EffectiveClass =>
        string.IsNullOrWhiteSpace(Class) ? VariantClass : $"{VariantClass} {Class}";

    // Maps the variant name to its CSS class string.
    private string VariantClass => Variant switch
    {
        "h1" => "font-display text-3xl font-bold tracking-tight sm:text-4xl",
        "h2" => "font-display text-2xl font-bold tracking-tight sm:text-3xl",
        "h3" => "font-display text-xl font-bold tracking-tight",
        "body" => "text-base text-cafe-text",
        "error" => "text-sm text-red-600 dark:text-red-400",
        "lead" => "text-lg text-cafe-text",
        "muted" => "text-sm text-cafe-muted",
        "small" => "text-xs text-cafe-muted",
        _ => "text-base text-cafe-text",
    };
}