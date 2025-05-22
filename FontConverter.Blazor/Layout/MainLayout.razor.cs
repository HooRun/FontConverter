using Microsoft.AspNetCore.Components;
using Radzen;

namespace FontConverter.Blazor.Layout;

public partial class MainLayout : LayoutComponentBase
{

    [Inject]
    NotificationService NotificationService { get; set; } = default!;

    
    bool isLeftSidebarExpanded = true;

    void ToggleLeftSidebar()
    {
        isLeftSidebarExpanded = !isLeftSidebarExpanded;
    }

    
}
