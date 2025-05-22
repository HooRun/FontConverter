using FontConverter.SharedLibrary.Helpers;
using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components;
using Radzen;
using SkiaSharp;
using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;
using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.Blazor.Components.FontAnalayzerDialog;

public partial class FontAnalayzerDialogComponent : ComponentBase
{
    [Inject]
    public DialogService dialogService { get; set; } = default!;

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

    private OpenTypeFont openTypeFont = new();
    private List<LVGLGlyphBitmapData> glyphsRenderData = new();

    protected override async Task OnInitializedAsync()
    {
        if (FontFile != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await FontFile.OpenReadStream(maxAllowedSize: 100 * 1024 * 1024).CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                Typeface = SKTypeface.FromStream(memoryStream, 0);
            }
            await Task.Delay(100);

            SortedList<OpenTypeTables, OpenTypeTableBinaryData> tables = new();
            if (Typeface!=null)
            {
                fontNameClass = textIsValidClass;
                fontNameText = Typeface.FamilyName;
                fontIsValid = true;
            }
            fontNameProgressVisibility = false;
            await InvokeAsync(StateHasChanged);

            if (fontIsValid && Typeface != null)
            {
                tablesCountProgressMode = ProgressBarMode.Determinate;
                await InvokeAsync(StateHasChanged);
                tablesCountProgressValue = 0;
                tablesCountProgressMaxValue = Typeface.TableCount;
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

                if (tablesCountIsValid)
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
                        openTypeFont = await ParseTablesDataHelper.ParseTablesAsync(tables, progressTablesData, FontLoadingCancellationToken!.Token).ConfigureAwait(false);
                        parsingTablesIsValid = openTypeFont.Tables.Count > 0;
                    }
                    catch (Exception)
                    {
                        parsingTablesIsValid = false;
                        //throw;
                    }
                    parsingTablesProgressStyle = parsingTablesIsValid ? ProgressBarStyle.Success : ProgressBarStyle.Danger;
                    await InvokeAsync(StateHasChanged);

                    if (parsingTablesIsValid)
                    {
                        glyphCountsProgressVisibility = false;
                        if (openTypeFont.MaxpTable.NumGlyphs > 0)
                        {
                            glyphCountsClass = textIsValidClass;
                            glyphCountsText = string.Format("{0:N0}", openTypeFont.MaxpTable.NumGlyphs);
                            glyphCountsIsValid = true;
                        }
                        await InvokeAsync(StateHasChanged);
                        if (glyphCountsIsValid)
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
                                SKFont font = new SKFont(Typeface, 12);
                                glyphsRenderData.Clear();
                                glyphsRenderData = await RenderGlyphToBitmapArrayHelper.RenderGlyphsAsync(font, openTypeFont.GlyfTable, new LVGLFontAdjusments(), 12, (byte)BIT_PER_PIXEL_ENUM.BPP_8, progressRenderGlyphs, FontLoadingCancellationToken!.Token).ConfigureAwait(false);
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
                }
            }
        }
    }

    
}
