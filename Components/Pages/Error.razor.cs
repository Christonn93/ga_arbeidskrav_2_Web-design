using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;

namespace CafeDiceRoller.Components.Pages;

public partial class Error
{
    // HTTP context, available during server-side rendering.
    [CascadingParameter]
    private HttpContext? HttpContext { get; set; }

    // Request id used to correlate logs with errors.
    private string? RequestId { get; set; }

    // True when a request id is available to display.
    private bool ShowRequestId =>
        !string.IsNullOrEmpty(RequestId);

    // True when running in the Development environment.
    private bool IsDevelopment =>
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
        == "Development";

    // Captures the request id from activity or HTTP context.
    protected override void OnInitialized()
    {
        RequestId = Activity.Current?.Id
                    ?? HttpContext?.TraceIdentifier;
    }
}