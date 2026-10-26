// ButtonBase.cs
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CafeDiceRoller.Components.Ui;

public class ButtonBase : ComponentBase
{
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Parameter] public string Type { get; set; } = "button";
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string Variant { get; set; } = "primary";
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    protected Task HandleClick(MouseEventArgs args) => OnClick.InvokeAsync(args);

    protected string CssClass =>
        string.IsNullOrWhiteSpace(Class) ? VariantClass : $"{VariantClass} {Class}";

    private string VariantClass => Variant switch
    {
        "secondary" => SecondaryClass,
        _ => PrimaryClass,
    };

    private const string PrimaryClass =
        "inline-flex items-center justify-center gap-2 rounded-lg bg-accent px-5 py-3 text-sm font-medium text-cafe-bg transition hover:bg-accent-hover focus:outline-none focus:ring-2 focus:ring-accent focus:ring-offset-2 focus:ring-offset-cafe-bg disabled:cursor-not-allowed disabled:opacity-40";

    private const string SecondaryClass =
        "inline-flex items-center justify-center gap-2 rounded-lg border border-cafe-border px-5 py-3 text-sm font-medium text-cafe-text transition hover:border-accent hover:text-accent focus:outline-none focus:ring-2 focus:ring-accent focus:ring-offset-2 focus:ring-offset-cafe-bg disabled:cursor-not-allowed disabled:opacity-40";
}