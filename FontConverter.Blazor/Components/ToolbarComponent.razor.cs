using FontConverter.Blazor.Components.ExportComponents;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace FontConverter.Blazor.Components;

public partial class ToolbarComponent : ComponentBase, IRerenderable
{
    [Inject]
    private DialogService _DialogService { get; set; } = default!;

    [Inject]
    NotificationService _NotificationService { get; set; } = default!;

    [Inject]
    private MainViewModel MainViewModel { get; set; } = default!;

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
            if (MainViewModel.GlyphsList.Count <= 0)
            {
                _NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Export",
                    Detail = "No glyphs available for export.",
                    ShowProgress = true
                });
            }
            else if (string.IsNullOrEmpty(MainViewModel.FontSettingsViewModel.FontName))
            {
                _NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Export",
                    Detail = "Font name is required.",
                    ShowProgress = true
                });
            }
            else if (!MainViewModel.FontSettingsViewModel.FontNameIsValid)
            {
                _NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Export",
                    Detail = "Font name is not valid.",
                    ShowProgress = true
                });
            }
            else if (!MainViewModel.FontSettingsViewModel.FallbackIsValid)
            {
                _NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Export",
                    Detail = "Fallback font name is not valid.",
                    ShowProgress = true
                });
            }
            else
            {
                MainViewModel.MappingsFromViewModelToModel(true);
                var dialogResult = await _DialogService.OpenAsync<ExportDialogComponent>(
                        "Export Font",
                        new Dictionary<string, object>(),
                        new DialogOptions
                        {
                            ShowClose = true,
                            ShowTitle = true,
                        });
            }
        }
        catch (Exception)
        {

        }

    }
}
