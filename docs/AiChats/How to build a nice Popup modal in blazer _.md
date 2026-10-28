<img src="https://r2cdn.perplexity.ai/pplx-full-logo-primary-dark%402x.png" style="height:64px;margin-right:32px"/>

# How to build a nice Popup modal in blazer ?

A nice Blazor popup is a reusable component: a fixed backdrop, a centered panel, and a `bool` you toggle. You do not need JavaScript for a solid modal.

## Component

`Components/Modal.razor`

```razor
@if (Visible)
{
    <div class="modal-root" @onclick="OnBackdropClick">
        <div class="modal-panel"
             role="dialog"
             aria-modal="true"
             aria-labelledby="modal-title"
             @onclick:stopPropagation="true"
             @onkeydown="OnKeyDown"
             tabindex="-1"
             @ref="_panel">
            <header class="modal-header">
                <h2 id="modal-title">@Title</h2>
                <button type="button" class="icon-btn" @onclick="Close" aria-label="Close">×</button>
            </header>

            <div class="modal-body">
                @ChildContent
            </div>

            @if (Footer is not null)
            {
                <footer class="modal-footer">
                    @Footer
                </footer>
            }
        </div>
    </div>
}

@code {
    private ElementReference _panel;

    [Parameter] public bool Visible { get; set; }
    [Parameter] public EventCallback<bool> VisibleChanged { get; set; }
    [Parameter] public string Title { get; set; } = "";
    [Parameter] public bool CloseOnBackdrop { get; set; } = true;
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public RenderFragment? Footer { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Visible)
            await _panel.FocusAsync();
    }

    private async Task Close()
    {
        Visible = false;
        await VisibleChanged.InvokeAsync(false);
    }

    private async Task OnBackdropClick()
    {
        if (CloseOnBackdrop)
            await Close();
    }

    private async Task OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape")
            await Close();
    }
}
```

`Components/Modal.razor.css`

```css
.modal-root {
    position: fixed;
    inset: 0;
    z-index: 1000;
    display: grid;
    place-items: center;
    padding: 1.5rem;
    background: rgb(15 23 42 / 0.55);
    backdrop-filter: blur(4px);
}

.modal-panel {
    width: min(32rem, 100%);
    border-radius: 1rem;
    background: #fff;
    color: #0f172a;
    box-shadow: 0 24px 60px rgb(15 23 42 / 0.25);
    outline: none;
    animation: pop 160ms ease-out;
}

.modal-header,
.modal-footer {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: 1rem;
    padding: 1rem 1.25rem;
}

.modal-header {
    border-bottom: 1px solid #e2e8f0;
}

.modal-header h2 {
    margin: 0;
    font-size: 1.125rem;
}

.modal-body {
    padding: 1.25rem;
}

.modal-footer {
    justify-content: flex-end;
    border-top: 1px solid #e2e8f0;
}

.icon-btn {
    border: 0;
    background: transparent;
    font-size: 1.5rem;
    line-height: 1;
    cursor: pointer;
    color: #64748b;
}

@keyframes pop {
    from { transform: translateY(8px) scale(0.98); opacity: 0; }
    to { transform: none; opacity: 1; }
}
```


## Usage

```razor
<button @onclick="() => show = true">Open</button>

<Modal @bind-Visible="show" Title="Delete project">
    <p>This cannot be undone.</p>
    <Footer>
        <button @onclick="() => show = false">Cancel</button>
        <button @onclick="Confirm">Delete</button>
    </Footer>
</Modal>

@code {
    private bool show;

    private async Task Confirm()
    {
        // do the work
        show = false;
    }
}
```

`@bind-Visible` is the Blazor version of a React `open` / `onOpenChange` pair. `ChildContent` is the default slot. `Footer` is a named slot.

## What makes it feel finished

- Backdrop click and Escape close the dialog. `@onclick:stopPropagation` keeps clicks inside the panel from closing it.
- `role="dialog"` and `aria-modal="true"` are the minimum accessibility markup. `FocusAsync` moves focus into the panel when it opens.
- Keep the panel width with `min(32rem, 100%)` so it stays usable on small screens.
- Do not render the tree when `Visible` is false. That avoids hidden forms still submitting.

If you want Material-style dialogs, snackbars, and focus trapping already built, use [MudBlazor Dialog](https://mudblazor.com/components/dialog) and add `<MudDialogProvider />` in `MainLayout`. For one custom look that you own, the component above is enough.

<span style="display:none">[^1_1][^1_10][^1_11][^1_12][^1_13][^1_14][^1_15][^1_2][^1_3][^1_4][^1_5][^1_6][^1_7][^1_8][^1_9]</span>

<div align="center">⁂</div>
