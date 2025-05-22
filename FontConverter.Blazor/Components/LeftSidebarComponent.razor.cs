using Microsoft.AspNetCore.Components;
using Radzen;
using SkiaSharp;

namespace FontConverter.Blazor.Components;

public partial class LeftSidebarComponent : ComponentBase
{
    [Inject] 
    NotificationService NotificationService { get; set; } = default!;

    private bool expanded;
    [Parameter]
    public bool Expanded { get; set; }

    [Parameter]
    public EventCallback<bool> ExpandedChanged { get; set; }

    protected override void OnParametersSet()
    {
        if (expanded != Expanded)
        {
            expanded = Expanded;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (expanded != Expanded)
        {
            await ExpandedChanged.InvokeAsync(Expanded);
        }
    }

    
}