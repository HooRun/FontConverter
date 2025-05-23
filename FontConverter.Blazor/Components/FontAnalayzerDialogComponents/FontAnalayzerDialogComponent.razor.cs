using FontConverter.SharedLibrary.Helpers;
using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using SkiaSharp;
using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;
using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.Blazor.Components.FontAnalayzerDialogComponents;

public partial class FontAnalayzerDialogComponent : ComponentBase
{
    [Inject]
    public DialogService dialogService { get; set; } = default!;

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

    private SortedList<OpenTypeTables, OpenTypeTableBinaryData> tables = new();
    private SortedList<int, LVGLGlyphBitmapData> glyphsRenderData = new();

    protected override async Task OnInitializedAsync()
    {
        if (FontFile is not null)
        {

            Typeface = await GetOpenTypeFontFace(FontFile);

            if (Typeface is not null)
                await UpdateFontNameSection();

            if (fontIsValid)
                await UpdateTablescountSection();

            if (tablesCountIsValid)
                await UpdateParseTablesSection();

            if (parsingTablesIsValid)
                await UpdateGlyphCountsSection();

            if (glyphCountsIsValid)
                await UpdateRenderGlyphsSection();

            MainViewModel.LVGLFont.FontInformations.FontName = Typeface?.FamilyName ?? string.Empty;

            applyButonDisabled = false;
        }
    }

    private async Task<SKTypeface?> GetOpenTypeFontFace(Radzen.FileInfo? fontFile)
    {
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

    private async Task UpdateFontNameSection()
    {
        fontNameClass = textIsValidClass;
        fontNameText = Typeface!.FamilyName;
        fontIsValid = true;
        fontNameProgressVisibility = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task UpdateTablescountSection()
    {
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
                _ = InvokeAsync(StateHasChanged);
            }
        });
        try
        {
            tables = await ParseTablesBinaryDataHelper.GetFontTablesAsync(Typeface, progressTableList, FontLoadingCancellationToken!.Token).ConfigureAwait(false);
            tablesCountIsValid = tables.Count > 0;
        }
        catch (Exception)
        {
            tablesCountIsValid = false;
        }
        tablesCountProgressStyle = tablesCountIsValid ? tablesCountProgressStyle = ProgressBarStyle.Success : ProgressBarStyle.Danger;
        await InvokeAsync(StateHasChanged);
    }

    private async Task UpdateParseTablesSection()
    {
        parsingTablesProgressMode = ProgressBarMode.Determinate;
        await InvokeAsync(StateHasChanged);
        parsingTablesProgressValue = 0;
        parsingTablesProgressMaxValue = 100;
        parsingTablesProgressShowValue = true;
        await InvokeAsync(StateHasChanged);
        var progressTablesData = new Progress<(string tableName, double percentage)>(report =>
        {
            parsingTablesProgressValue = Math.Round(report.percentage);
            _ = InvokeAsync(StateHasChanged);
        });
        try
        {
            MainViewModel.OpenTypeFont = await ParseTablesDataHelper.ParseTablesAsync(tables, progressTablesData, FontLoadingCancellationToken!.Token).ConfigureAwait(false);
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

    private async Task UpdateGlyphCountsSection()
    {
        glyphCountsProgressVisibility = false;
        if (MainViewModel.OpenTypeFont.MaxpTable.NumGlyphs > 0)
        {
            glyphCountsClass = textIsValidClass;
            glyphCountsText = string.Format("{0:N0}", MainViewModel.OpenTypeFont.MaxpTable.NumGlyphs);
            glyphCountsIsValid = true;
        }
        await InvokeAsync(StateHasChanged);
    }

    private async Task UpdateRenderGlyphsSection()
    {
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
            SKFont font = new SKFont(Typeface, MainViewModel.LVGLFont.FontSettings.FontSize);
            glyphsRenderData.Clear();
            glyphsRenderData = await RenderGlyphsToBitmapArrayHelper.RenderGlyphsToBitmapArrayAsync(
                font, 
                MainViewModel.OpenTypeFont.GlyfTable, 
                MainViewModel.LVGLFont.FontAdjusments,
                MainViewModel.LVGLFont.FontSettings.FontSize,
                MainViewModel.LVGLFont.FontSettings.FontBitPerPixel, 
                progressRenderGlyphs, 
                FontLoadingCancellationToken!.Token).ConfigureAwait(false);
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
}
