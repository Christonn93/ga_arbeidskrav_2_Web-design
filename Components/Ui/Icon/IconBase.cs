using Microsoft.AspNetCore.Components;

namespace CafeDiceRoller.Components.Ui.Icon;

public class IconBase : ComponentBase
{
    [Parameter] public string Variant { get; set; } = "";
    [Parameter] public string? Class { get; set; }
}
