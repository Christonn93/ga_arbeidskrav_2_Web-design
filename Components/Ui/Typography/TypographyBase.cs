// TypographyBase.cs
using Microsoft.AspNetCore.Components;

namespace CafeDiceRoller.Components.Ui.Typography;

public class TypographyBase : ComponentBase
{
    [Parameter] public string Variant { get; set; } = "body";
    [Parameter] public string? Class { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    protected string EffectiveClass =>
        string.IsNullOrWhiteSpace(Class) ? VariantClass : $"{VariantClass} {Class}";

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