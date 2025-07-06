using FontConverter.Blazor.Components.ExportComponents;
using FontConverter.Blazor.Components.LeftSidebarComponents;
using FontConverter.Blazor.Components.LeftSidebarComponents.FontFileComponents;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace FontConverter.Blazor.Components;

public partial class ToolbarComponent : ComponentBase, IRerenderable
{
    [Inject]
    public DialogService _DialogService { get; set; } = default!;

    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.RegisterComponent(nameof(ToolbarComponent), this);
    }

    public async Task ForceRender()
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnExportClick()
    {
        try
        {
            MainViewModel.MappingsFromViewModelToModel(true);
            var dialogResult = await _DialogService.OpenAsync<ExportDialogComponent>(
                    "Export Font",
                    new Dictionary<string, object>(),
                    new DialogOptions
                    {
                        ShowClose = false,
                        ShowTitle = false,
                    });
        }
        catch (Exception)
        {

        }

    }
}
