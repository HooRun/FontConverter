using FontConverter.Blazor.Services;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary.Helpers;
using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace FontConverter.Blazor.Components.LeftSidebarComponents.UpdateGlyphsComponents;

public partial class UpdateGlyphsDialogComponent : ComponentBase
{
    [Inject]
    public DialogService dialogService { get; set; } = default!;

    [Inject]
    public PredefinedDataService PredefinedData { get; set; } = default!;

    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    [Parameter]
    public CancellationTokenSource? UpdateGlyphsCancellationToken { get; set; } = default;

    private int leftColumnSize = 4;
    private bool applyButonDisabled = true;

    private string glyphCountsText = "Font missing glyphs.";

    private bool renderingGlyphsProgressShowValue = false;
    private double renderingGlyphsProgressValue = 100;
    private double renderingGlyphsProgressMinValue = 0;
    private double renderingGlyphsProgressMaxValue = 100;
    private ProgressBarMode renderingGlyphsProgressMode = ProgressBarMode.Indeterminate;
    private ProgressBarStyle renderingGlyphsProgressStyle = ProgressBarStyle.Primary;
    private bool renderingGlyphsIsValid = false;

    private bool organizingGlyphsProgressShowValue = false;
    private double organizingGlyphsProgressValue = 100;
    private double organizingGlyphsProgressMinValue = 0;
    private double organizingGlyphsProgressMaxValue = 100;
    private ProgressBarMode organizingGlyphsProgressMode = ProgressBarMode.Indeterminate;
    private ProgressBarStyle organizingGlyphsProgressStyle = ProgressBarStyle.Primary;
    private bool organizingGlyphsIsValid = false;

    private bool alertWarningApplyVisibilty = false;
    private SortedList<int, LVGLGlyphBitmapData> glyphsRenderData = new();
    private SortedList<int, LVGLGlyph> responseUpddatedGlyphs = new();

    protected override async Task OnInitializedAsync()
    {
        glyphCountsText = string.Format("{0:N0}", MainViewModel.OpenTypeFont.MaxpTable.NumGlyphs);

        await UpdateRenderGlyphsSection(UpdateGlyphsCancellationToken!.Token);

        if (renderingGlyphsIsValid)
            await UpdateOrganizingGlyphsSection(UpdateGlyphsCancellationToken!.Token);

        if (organizingGlyphsIsValid)
        {
            alertWarningApplyVisibilty = true;
            applyButonDisabled = false;
        }
    }

    private async Task UpdateRenderGlyphsSection(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        renderingGlyphsProgressMode = ProgressBarMode.Determinate;
        await InvokeAsync(StateHasChanged);
        renderingGlyphsProgressValue = 0;
        renderingGlyphsProgressMaxValue = 100;
        renderingGlyphsProgressShowValue = true;
        await InvokeAsync(StateHasChanged);
        var progressRenderGlyphs = new Progress<(int glyphIndex, double percentage)>(report =>
        {
            renderingGlyphsProgressValue = Math.Round(report.percentage, 2);
            _ = InvokeAsync(StateHasChanged);
        });
        try
        {
            glyphsRenderData.Clear();
            glyphsRenderData = await RenderGlyphsToBitmapArrayHelper.RenderGlyphsToBitmapArrayAsync(
                MainViewModel.OpenTypeFont!,
                MainViewModel.LVGLFont!,
                progressRenderGlyphs,
                cancellationToken);
            renderingGlyphsIsValid = glyphsRenderData.Count > 0;
        }
        catch (Exception ex)
        {
            renderingGlyphsIsValid = false;
            //throw;
        }
        renderingGlyphsProgressStyle = renderingGlyphsIsValid ? ProgressBarStyle.Success : ProgressBarStyle.Danger;
        await InvokeAsync(StateHasChanged);
    }

    private async Task UpdateOrganizingGlyphsSection(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        organizingGlyphsProgressMode = ProgressBarMode.Determinate;
        await InvokeAsync(StateHasChanged);
        organizingGlyphsProgressValue = 0;
        organizingGlyphsProgressMaxValue = 100;
        organizingGlyphsProgressShowValue = true;
        await InvokeAsync(StateHasChanged);
        var progressOrganizeGlyphs = new Progress<(int glyphIndex, double percentage)>(report =>
        {
            organizingGlyphsProgressValue = Math.Round(report.percentage, 2);
            _ = InvokeAsync(StateHasChanged);
        });
        try
        {
            responseUpddatedGlyphs = await OrganizeGlyphsHelper.OrganizeGlyphsAsync(
                PredefinedData,
                MainViewModel.OpenTypeFont,
                MainViewModel.LVGLFont,
                glyphsRenderData,
                progressOrganizeGlyphs,
                cancellationToken,
                true);
            organizingGlyphsIsValid = responseUpddatedGlyphs.Count > 0;
        }
        catch (Exception ex)
        {
            organizingGlyphsIsValid = false;
            //throw;
        }
        organizingGlyphsProgressStyle = organizingGlyphsIsValid ? ProgressBarStyle.Success : ProgressBarStyle.Danger;
        await InvokeAsync(StateHasChanged);
    }


}
