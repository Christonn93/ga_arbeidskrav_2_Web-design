// NavLinkBase.cs

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace CafeDiceRoller.Components.Ui.Typography;

public class LinkBase : ComponentBase
{
    // Target URL for the link.
    [Parameter] public string Href { get; set; } = string.Empty;
    // NavLink match mode for active styling.
    [Parameter] public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;
    // Extra CSS classes appended to the default class.
    [Parameter] public string? Class { get; set; }
    // Content rendered inside the link.
    [Parameter] public RenderFragment? ChildContent { get; set; }

    // Captures unmatched attributes and forwards them to the link.
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    // Combines the default class with any custom class.
    protected string CssClass =>
        string.IsNullOrWhiteSpace(Class) ? DefaultClass : $"{DefaultClass} {Class}";

    // Base CSS classes applied to every link.
    private const string DefaultClass =
        "font-medium text-cafe-text underline-offset-4 hover:text-accent hover:underline focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-accent";
}