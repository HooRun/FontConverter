using Microsoft.AspNetCore.Components;

namespace FontConverter.Blazor.Components.LeftSidebarComponents;

public partial class FontInformationsComponent : ComponentBase
{
    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;
}
