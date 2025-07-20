using FontConverter.Blazor.Components.GlyphsListViewComponents;
using FontConverter.Blazor.Components.LeftSidebarComponents.FontFileComponents;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.Layout;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary.Helpers;
using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using SkiaSharp;

namespace FontConverter.Blazor.Components.LeftSidebarComponents.UpdateGlyphsComponents;

public partial class UpdateGlyphsComponent : ComponentBase, IRerenderable
{
    [Inject]
    private NotificationService _NotificationService { get; set; } = default!;

    [Inject]
    private DialogService _DialogService { get; set; } = default!;

    [Inject]
    private MainViewModel _MainViewModel { get; set; } = default!;

    private bool _UpdateBusy;
    private CancellationTokenSource? _UpdatingCancellationToken;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _MainViewModel.RegisterComponent(nameof(FontAdjusmentsComponent), this);
    }

    public async Task ForceRender()
    {
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnUpdateGlyphsClick()
    {
        try
        {
            _UpdateBusy = true;
            if (_MainViewModel.GlyphsList.Count > 0)
            {

                _UpdatingCancellationToken?.Cancel();
                _UpdatingCancellationToken = new CancellationTokenSource();
                _MainViewModel.MappingsFromViewModelToModel();
                _MainViewModel.OpenTypeFont.SKFont = new SKFont(_MainViewModel.OpenTypeFont.SKTypeface, _MainViewModel.LVGLFont.FontSettings.FontSize);
                var dialogResult = await _DialogService.OpenAsync<UpdateGlyphsDialogComponent>(
                    string.Empty,
                    new Dictionary<string, object>
                    {
                        { "UpdateGlyphsCancellationToken", _UpdatingCancellationToken },
                    },
                    new DialogOptions
                    {
                        ShowClose = false,
                        ShowTitle = false,
                    });

                if (dialogResult is SortedList<int, LVGLGlyph> result && result.Count > 0)
                {
                    foreach (var glyph in result)
                    {
                        if (_MainViewModel.GlyphsList.ContainsKey(glyph.Key))
                        {
                            _MainViewModel.GlyphsList[glyph.Key].Bitmap = glyph.Value.Bitmap;
                            _MainViewModel.GlyphsList[glyph.Key].Descriptor = glyph.Value.Descriptor;

                            _MainViewModel.GlyphsList[glyph.Key].Adjusments.AntiAlias = _MainViewModel.FontAdjusmentsViewModel.AntiAlias;
                            _MainViewModel.GlyphsList[glyph.Key].Adjusments.Dither = _MainViewModel.FontAdjusmentsViewModel.Dither;
                            _MainViewModel.GlyphsList[glyph.Key].Adjusments.ColorFilter = _MainViewModel.FontAdjusmentsViewModel.ColorFilter;
                            _MainViewModel.GlyphsList[glyph.Key].Adjusments.Shader = _MainViewModel.FontAdjusmentsViewModel.Shader;
                            _MainViewModel.GlyphsList[glyph.Key].Adjusments.Style = _MainViewModel.FontAdjusmentsViewModel.Style;
                            _MainViewModel.GlyphsList[glyph.Key].Adjusments.Gamma = _MainViewModel.FontAdjusmentsViewModel.Gamma;
                            _MainViewModel.GlyphsList[glyph.Key].Adjusments.Threshold = _MainViewModel.FontAdjusmentsViewModel.Threshold;
                        }
                    }
                    FinalizingFontHelper.UpdateGlyphViewItemProperties(_MainViewModel.OpenTypeFont, _MainViewModel.LVGLFont);
                    _MainViewModel.MappingsFromModelToViewModel();
                    _MainViewModel.RerenderMany(nameof(MainLayout), nameof(GlyphListComponent));
                }
                else
                {

                }
                _UpdatingCancellationToken.CancelAfter(1);

            }
            else
            {
                _NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = "Update Glyphs",
                    Detail = "There are no glyphs to update.",
                    ShowProgress = true
                });
            }
        }
        catch (Exception ex)
        {
            _NotificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Update Glyphs",
                Detail = $"Error: {ex.Message}",
                ShowProgress = true
            });
        }
        finally
        {
            await InvokeAsync(StateHasChanged);
            _UpdateBusy = false;
        }
    }

    public void Dispose()
    {
        _UpdatingCancellationToken?.Dispose();
        _UpdatingCancellationToken = null;
    }
}
