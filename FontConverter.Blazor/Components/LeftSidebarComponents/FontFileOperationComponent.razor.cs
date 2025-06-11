using BlazorPro.BlazorSize;
using FontConverter.Blazor.Components.FontAnalayzerDialogComponents;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using Radzen;
using SkiaSharp;

namespace FontConverter.Blazor.Components.LeftSidebarComponents;

public partial class FontFileOperationComponent : ComponentBase
{
    [Inject]
    NotificationService NotificationService { get; set; } = default!;

    [Inject]
    public DialogService dialogService { get; set; } = default!;

    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    private Radzen.FileInfo? fontFile = null;
    private string fontFileName = "";
    private long fontFileSize = 0;
    private string fontFileType = "";
    private bool busy;

    private SKTypeface? typeface = null;
    private CancellationTokenSource? fontLoadingCancellationToken;

    private void OnChange(UploadChangeEventArgs args)
    {
        if (args.Files == null || args.Files.Count() == 0)
        {
            fontFile = null;
            return;
        }
        fontFile = args.Files.First();
        fontFileName = fontFile.Name;
        fontFileSize = fontFile.Size;
        fontFileType = Path.GetExtension(fontFile.Name).ToLower();
    }



    private async Task OnBusyClick()
    {
        try
        {
            busy = true;
            if (fontFile != null)
            {
                fontLoadingCancellationToken?.Cancel();
                fontLoadingCancellationToken = new CancellationTokenSource();
                MainViewModel.MappingsFromViewModelToModel();
                var dialogResult = await dialogService.OpenAsync<FontAnalayzerDialogComponent>(
                    string.Empty,
                    new Dictionary<string, object>
                    {
                        { "FontFile", fontFile },
                        { "FontLoadingCancellationToken", fontLoadingCancellationToken },
                    },
                    new DialogOptions
                    {
                        ShowClose = false,
                        ShowTitle = false,
                    });

                if (dialogResult is bool result && result)
                {
                    MainViewModel.MappingsFromModelToViewModel();
                }
                else
                {
                    MainViewModel.MappingsFromViewModelToModel();
                }
                fontLoadingCancellationToken.CancelAfter(1);
                
            }
            else
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = "Get Font Data",
                    Detail = "Please Select Font",
                    ShowProgress = true
                });
            }
        }
        catch (Exception ex)
        {
            NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Get Font Data",
                Detail = $"Error: {ex.Message}",
                ShowProgress = true
            });
        }
        finally
        {
            await InvokeAsync(StateHasChanged);
            busy = false;
        }
    }

    public void Dispose()
    {
        fontLoadingCancellationToken?.Dispose();
        fontLoadingCancellationToken = null;
    }
}
