using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor.Rendering;
using SkiaSharp;

namespace FontConverter.Blazor.Layout;

public partial class MainLayout
{

    [Inject]
    NotificationService NotificationService { get; set; } = default!;

    
    bool isLeftSidebarExpanded = true;

    void ToggleLeftSidebar()
    {
        isLeftSidebarExpanded = !isLeftSidebarExpanded;
    }

    
}
