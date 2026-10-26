// NavLinkBase.cs

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace CafeDiceRoller.Components.Ui.Typography;

public class LinkBase : ComponentBase
{
    [Parameter] public string Href { get; set; } = string.Empty;
    [Parameter] public NavLinkMatch Match { get; set; } = NavLinkMatch.Prefix;
    [Parameter] public string? Class { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    protected string CssClass =>
        string.IsNullOrWhiteSpace(Class) ? DefaultClass : $"{DefaultClass} {Class}";

    private const string DefaultClass =
        "font-medium text-cafe-text underline-offset-4 hover:text-accent hover:underline focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-accent";
}