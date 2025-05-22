using FontConverter.Blazor.Components.FontAnalayzerDialog;
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

    private Radzen.FileInfo? fontFile = null;
    private string fontFileName = "";
    private long fontFileSize = 0;
    private string fontFileType = "";
    private bool busy;

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
        busy = true;
        if (fontFile != null)
        {
            SKTypeface? typeface = null;
            CancellationTokenSource? fontLoadingCancellationToken = new CancellationTokenSource();
            var result = await dialogService.OpenAsync<FontAnalayzerDialogComponent>( string.Empty,
                new Dictionary<string, object>
                {
                    { "FontFile", fontFile },
                    { "Typeface", typeface! },
                    { "FontLoadingCancellationToken", fontLoadingCancellationToken! },
                },
                new DialogOptions
                {
                    ShowClose = false,
                    ShowTitle = false,
                    
                }
            );
            fontLoadingCancellationToken.CancelAfter(1);
            fontLoadingCancellationToken.Dispose();
            fontLoadingCancellationToken = null;
            typeface?.Dispose();
            
        }
        else
        {
            NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Warning, Summary = "Get Font Data", Detail = "Please Select Font", ShowProgress = true });
        }
        busy = false;
    }
}
