using Microsoft.AspNetCore.Components;
using Radzen;
using SkiaSharp;

namespace FontConverter.Blazor.Layout.LeftSidebarLayouts;

public partial class FontFileOperationLayout
{
    [Inject]
    NotificationService NotificationService { get; set; } = default!;

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
            using MemoryStream memoryStream = new MemoryStream();
            await fontFile.OpenReadStream(maxAllowedSize: 100 * 1024 * 1024).CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            using var typeface = SKTypeface.FromStream(memoryStream);
        }
        else
        {
            NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Warning, Summary = "Get Font Data", Detail = "Please Select Font", ShowProgress = true });
        }
        busy = false;
    }
}
