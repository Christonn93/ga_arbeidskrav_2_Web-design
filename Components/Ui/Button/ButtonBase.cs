// ButtonBase.cs

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CafeDiceRoller.Components.Ui.Button;

public class ButtonBase : ComponentBase
{
    // Raised when the button is clicked.
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    // HTML button type attribute (button, submit, reset).
    [Parameter] public string Type { get; set; } = "button";
    // Disables the button when true.
    [Parameter] public bool Disabled { get; set; }
    // Extra CSS classes appended to the variant class.
    [Parameter] public string? Class { get; set; }
    // Visual variant: primary, secondary, or danger.
    [Parameter] public string Variant { get; set; } = "primary";
    // Content rendered inside the button.
    [Parameter] public RenderFragment? ChildContent { get; set; }

    // Captures any unmatched attributes and forwards them to the button.
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    // Invokes the OnClick callback with mouse arguments.
    protected Task HandleClick(MouseEventArgs args) => OnClick.InvokeAsync(args);

    // Combines the variant class with any custom class.
    protected string CssClass =>
        string.IsNullOrWhiteSpace(Class) ? VariantClass : $"{VariantClass} {Class}";

    // Maps the variant name to its CSS class string.
    private string VariantClass => Variant switch
    {
        "secondary" => SecondaryClass,
        "danger" => DangerClass,
        _ => PrimaryClass,
    };

    // CSS for the primary variant.
    private const string PrimaryClass =
        "inline-flex items-center justify-center gap-2 rounded-lg bg-accent px-5 py-3 text-sm font-medium text-cafe-bg transition hover:bg-accent-hover focus:outline-none focus:ring-2 focus:ring-accent focus:ring-offset-2 focus:ring-offset-cafe-bg disabled:cursor-not-allowed disabled:opacity-40";

    // CSS for the secondary variant.
    private const string SecondaryClass =
        "inline-flex items-center justify-center gap-2 rounded-lg border border-cafe-border px-5 py-3 text-sm font-medium text-cafe-text transition hover:border-accent hover:text-accent focus:outline-none focus:ring-2 focus:ring-accent focus:ring-offset-2 focus:ring-offset-cafe-bg disabled:cursor-not-allowed disabled:opacity-40";

    // CSS for the danger variant.
    private const string DangerClass =
        "inline-flex items-center justify-center gap-2 rounded-md bg-red-700 px-3 py-1.5 text-sm font-medium text-white transition hover:bg-red-800 focus:outline-none focus:ring-2 focus:ring-red-700 focus:ring-offset-2 focus:ring-offset-cafe-bg disabled:cursor-not-allowed disabled:opacity-40";
}