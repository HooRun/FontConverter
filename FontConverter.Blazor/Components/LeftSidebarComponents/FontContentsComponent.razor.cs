using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace FontConverter.Blazor.Components.LeftSidebarComponents;

public partial class FontContentsComponent : ComponentBase, IRerenderable
{
    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    private async Task OnTreeChange(TreeEventArgs args)
    {
        var selectedItem = args.Value as FontContentViewModel;
        if (selectedItem != null)
        {
            await MainViewModel.SelectTreeItemAsync(selectedItem);
            StateHasChanged();
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.RegisterComponent(nameof(FontContentsComponent), this);
    }

    public void ForceRender()
    {
        StateHasChanged();
    }
}
