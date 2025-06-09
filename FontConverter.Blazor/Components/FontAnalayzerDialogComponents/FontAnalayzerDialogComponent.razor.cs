using FontConverter.Blazor.Components.LeftSidebarComponents;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.Models.GlyphsView;
using FontConverter.Blazor.Services;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary.Helpers;
using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using SkiaSharp;
using System.Threading;
using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;
using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.Blazor.Components.FontAnalayzerDialogComponents;

public partial class FontAnalayzerDialogComponent : ComponentBase
{
    [Inject]
    public DialogService dialogService { get; set; } = default!;

    [Inject]
    public PredefinedDataService PredefinedData { get; set; } = default!;

    [Inject]
    public MainViewModel MainViewModel { get; set; } = default!;

    [Parameter]
    public Radzen.FileInfo? FontFile { get; set; }

    [Parameter]
    public SKTypeface? Typeface { get; set; } = null;

    [Parameter]
    public CancellationTokenSource? FontLoadingCancellationToken { get; set; } = default;

    private const string textIsValidClass = "rz-background-color-success-lighter rz-color-on-success-lighter rz-p-1";
    private const string textIsNotValidClass = "rz-background-color-danger-lighter rz-color-on-danger-lighter rz-text-align-center rz-p-1";

    private int leftColumnSize = 4;
    private bool applyButonDisabled = true;

    private bool fontNameProgressVisibility = true;
    private string fontNameClass = textIsNotValidClass;
    private string fontNameText = "Font is not valid!";
    private bool fontIsValid = false;

    private bool tablesCountProgressShowValue = false;
    private double tablesCountProgressValue = 100;
    private double tablesCountProgressMinValue = 0;
    private double tablesCountProgressMaxValue = 100;
    private ProgressBarMode tablesCountProgressMode = ProgressBarMode.Indeterminate;
    private ProgressBarStyle tablesCountProgressStyle = ProgressBarStyle.Primary;
    private bool tablesCountIsValid = false;

    private bool parsingTablesProgressShowValue = false;
    private double parsingTablesProgressValue = 100;
    private double parsingTablesProgressMinValue = 0;
    private double parsingTablesProgressMaxValue = 100;
    private ProgressBarMode parsingTablesProgressMode = ProgressBarMode.Indeterminate;
    private ProgressBarStyle parsingTablesProgressStyle = ProgressBarStyle.Primary;
    private bool parsingTablesIsValid = false;

    private bool glyphCountsProgressVisibility = true;
    private string glyphCountsClass = textIsNotValidClass;
    private string glyphCountsText = "Font missing glyphs.";
    private bool glyphCountsIsValid = false;

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

    private bool finalizingFontProgressShowValue = false;
    private double finalizingFontProgressValue = 100;
    private double finalizingFontProgressMinValue = 0;
    private double finalizingFontProgressMaxValue = 100;
    private ProgressBarMode finalizingFontProgressMode = ProgressBarMode.Indeterminate;
    private ProgressBarStyle finalizingFontProgressStyle = ProgressBarStyle.Primary;
    private bool finalizingFontIsValid = false;

    private bool alertWarningApplyVisibilty = false;

    SKFont? font;
    private SortedList<OpenTypeTables, OpenTypeTableBinaryData> tables = new();
    private SortedList<int, LVGLGlyphBitmapData> glyphsRenderData = new();

    protected override async Task OnInitializedAsync()
    {
        if (FontFile is not null)
        {

            Typeface = await GetOpenTypeFontFace(FontFile, FontLoadingCancellationToken!.Token);

            if (Typeface is not null)
                await UpdateFontNameSection(FontLoadingCancellationToken!.Token);

            if (fontIsValid)
                await UpdateTablescountSection(FontLoadingCancellationToken!.Token);

            if (tablesCountIsValid)
                await UpdateParseTablesSection(FontLoadingCancellationToken!.Token);

            if (parsingTablesIsValid)
                await UpdateGlyphCountsSection(FontLoadingCancellationToken!.Token);

            if (glyphCountsIsValid)
                await UpdateRenderGlyphsSection(FontLoadingCancellationToken!.Token);

            if (renderingGlyphsIsValid)
                await UpdateOrganizingGlyphsSection(FontLoadingCancellationToken!.Token);

            if (organizingGlyphsIsValid)
                await UpdateFinalizingFontSection(FontLoadingCancellationToken!.Token);

            if (finalizingFontIsValid)
            {
                alertWarningApplyVisibilty = true;
                applyButonDisabled = false;
            }

            font?.Dispose();
            font = null;
        }
    }

    private async Task<SKTypeface?> GetOpenTypeFontFace(Radzen.FileInfo? fontFile, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SKTypeface? typeface= null;
        if (fontFile is null) return typeface;
        using (var memoryStream = new MemoryStream())
        {
            await fontFile.OpenReadStream(maxAllowedSize: 100 * 1024 * 1024).CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            typeface = SKTypeface.FromStream(memoryStream, 0);
        }
        return typeface;
    }

    private async Task UpdateFontNameSection(CancellationToken cancellationToken = default)
    {
        fontNameClass = textIsValidClass;
        fontNameText = Typeface!.FamilyName;
        fontIsValid = true;
        fontNameProgressVisibility = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task UpdateTablescountSection(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        tablesCountProgressMode = ProgressBarMode.Determinate;
        await InvokeAsync(StateHasChanged);
        tablesCountProgressValue = 0;
        tablesCountProgressMaxValue = Typeface!.TableCount;
        tablesCountProgressShowValue = true;
        var progressTableList = new Progress<string>(message =>
        {
            if (message.StartsWith("Processed"))
            {
                tablesCountProgressValue++;
                InvokeAsync(StateHasChanged);
            }
        });
        try
        {
            tables = await ParseTablesBinaryDataHelper.GetFontTablesAsync(Typeface, progressTableList, cancellationToken).ConfigureAwait(false);
            tablesCountIsValid = tables.Count > 0;
        }
        catch (Exception)
        {
            tablesCountIsValid = false;
        }
        tablesCountProgressStyle = tablesCountIsValid ? tablesCountProgressStyle = ProgressBarStyle.Success : ProgressBarStyle.Danger;
        await InvokeAsync(StateHasChanged);
    }

    private async Task UpdateParseTablesSection(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        parsingTablesProgressMode = ProgressBarMode.Determinate;
        await InvokeAsync(StateHasChanged);
        parsingTablesProgressValue = 0;
        parsingTablesProgressMaxValue = 100;
        parsingTablesProgressShowValue = true;
        await InvokeAsync(StateHasChanged);
        var progressTablesData = new Progress<(string tableName, double percentage)>(report =>
        {
            parsingTablesProgressValue = Math.Round(report.percentage, 2);
            InvokeAsync(StateHasChanged);
        });
        try
        {
            MainViewModel.OpenTypeFont = await ParseTablesDataHelper.ParseTablesAsync(tables, progressTablesData, cancellationToken).ConfigureAwait(false);
            parsingTablesIsValid = MainViewModel.OpenTypeFont.Tables.Count > 0;
        }
        catch (Exception)
        {
            parsingTablesIsValid = false;
            //throw;
        }
        parsingTablesProgressStyle = parsingTablesIsValid ? ProgressBarStyle.Success : ProgressBarStyle.Danger;
        await InvokeAsync(StateHasChanged);
    }

    private async Task UpdateGlyphCountsSection(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        glyphCountsProgressVisibility = false;
        if (MainViewModel.OpenTypeFont.MaxpTable.NumGlyphs > 0)
        {
            glyphCountsClass = textIsValidClass;
            glyphCountsText = string.Format("{0:N0}", MainViewModel.OpenTypeFont.MaxpTable.NumGlyphs);
            glyphCountsIsValid = true;
        }
        await InvokeAsync(StateHasChanged);
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
            font = new SKFont(Typeface, MainViewModel.LVGLFont.FontSettings.FontSize);
            glyphsRenderData.Clear();
            glyphsRenderData = await RenderGlyphsToBitmapArrayHelper.RenderGlyphsToBitmapArrayAsync(
                font, 
                MainViewModel.OpenTypeFont,
                MainViewModel.LVGLFont,
                progressRenderGlyphs, 
                cancellationToken).ConfigureAwait(false);
            renderingGlyphsIsValid = glyphsRenderData.Count > 0;
        }
        catch (Exception)
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
            SortedList<int, LVGLGlyph> glyphs = await OrganizeGlyphsHelper.OrganizeGlyphsAsync(
                PredefinedData,
                MainViewModel.OpenTypeFont,
                MainViewModel.LVGLFont,
                glyphsRenderData,
                progressOrganizeGlyphs,
                cancellationToken).ConfigureAwait(false);
            organizingGlyphsIsValid = glyphs.Count > 0;
            if (organizingGlyphsIsValid)
                MainViewModel.LVGLFont.Glyphs = glyphs;
        }
        catch (Exception ex)
        {
            organizingGlyphsIsValid = false;
            //throw;
        }
        organizingGlyphsProgressStyle = organizingGlyphsIsValid ? ProgressBarStyle.Success : ProgressBarStyle.Danger;
        await InvokeAsync(StateHasChanged);
    }

    private async Task UpdateFinalizingFontSection(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        finalizingFontProgressMode = ProgressBarMode.Determinate;
        await InvokeAsync(StateHasChanged);
        finalizingFontProgressValue = 0;
        finalizingFontProgressMaxValue = 100;
        finalizingFontProgressShowValue = true;
        await InvokeAsync(StateHasChanged);
        var progressFinalizingFont = new Progress<double>(report =>
        {
            finalizingFontProgressValue = Math.Round(report, 2);
            _ = InvokeAsync(StateHasChanged);
        });
        try
        {
            if (Typeface is not null && font is not null)
            {
                await FinalizingFontHelper.FinalizingFontAsync(
                    Typeface,
                    font,
                    MainViewModel.OpenTypeFont,
                    MainViewModel.LVGLFont,
                    progressFinalizingFont,
                    cancellationToken).ConfigureAwait(false);
                IProgress<double> iprogressFinalizingFont = progressFinalizingFont;
                MainViewModel.GlyphsList.Clear();
                foreach (var glyph in MainViewModel.LVGLFont.Glyphs)
                {
                    MainViewModel.GlyphsList.Add(glyph.Key, new GlyphItem(glyph.Value));
                }
                iprogressFinalizingFont.Report(100.0);
                await Task.Delay(500, cancellationToken).ConfigureAwait(false);

                finalizingFontIsValid = true;
            }
        }
        catch (Exception ex)
        {
            finalizingFontIsValid = false;
            //throw;
        }
        finalizingFontProgressStyle = finalizingFontIsValid ? ProgressBarStyle.Success : ProgressBarStyle.Danger;
        await InvokeAsync(StateHasChanged);
    }

    
}
