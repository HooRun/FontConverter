using LVGLFontConverter.Library.Data;
using LVGLFontConverter.Library.Models;
using LVGLFontConverter.Library.Models.OpenType;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using static LVGLFontConverter.Library.Helpers.FontTableParser;
using static LVGLFontConverter.Library.Helpers.GetGlyphInformation;
using static LVGLFontConverter.Library.Helpers.FontTableValueConverter;
using static LVGLFontConverter.Library.Helpers.LoadDataRecordsHelper;
using System.Threading.Tasks;
using Windows.Foundation;
using System.Threading;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using static LVGLFontConverter.Library.Helpers.GlyphToBitmapArray;

namespace LVGLFontConverter.Library;

public class FontConverter : IDisposable
{

    public FontConverter()
    {
        // Load Data
        LoadPrimaryDataAsync();
        

        FontFamilyPath = string.Empty;  
        OpenTypeFont = new();
        LVGLFont = new();
    }

    #region Private Variables
    private static readonly string _ExeDir = AppContext.BaseDirectory;
    private static readonly string _UnicodeBlocksPath = Path.Combine(_ExeDir, "Data\\UCD\\Blocks.txt");
    private static readonly string _UnicodeDataPath = Path.Combine(_ExeDir, "Data\\UCD\\UnicodeData.txt");
    private static readonly string _StandardMacintoshGlyphNamesPath = Path.Combine(_ExeDir, "Data\\StandardMacintoshGlyphNames.txt");
    private SKTypeface typeface;
    private SKFont font; 
    private List<GlyphToBitmapResult> glyphsBitmapList = [];
    private List<LVGLFontGlyph> glyphsDataList = [];
    #endregion Private Variables

    #region Public Variables
    public List<UnicodeBlock> UnicodeBlocks { get; private set; } = [];
    public List<UnicodeCharacterName> UnicodeCharacterNames { get; private set; } = [];
    public List<StandardMacintoshGlyphName> StandardMacintoshGlyphNames { get; private set; } = [];
    public List<string> SystemFonts { get; private set; } = [];
    public string FontFamilyPath { get; set; }
    public OpenTypeFont OpenTypeFont { get; set; }
    public LVGLFont LVGLFont { get; set; }
    public bool IsFontValid { get; set; }
    public bool IsTablesListReady { get; set; }
    public bool IsTablesDataReady { get; set; }
    public bool IsGlyphsBitmapReady { get; set; }
    public bool IsGlyphsDataReady { get; set; }
    public bool IsFontConverted { get; set; }
    #endregion Public Variables

    #region Private Methods
    private async void LoadPrimaryDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Run all load operations concurrently
            var loadBlocksTask = LoadUnicodeBlocksAsync(_UnicodeBlocksPath, cancellationToken);
            var loadNamesTask = LoadUnicodeNamesAsync(_UnicodeDataPath, cancellationToken);
            var loadGlyphsTask = LoadStandardMacintoshGlyphNamesAsync(_StandardMacintoshGlyphNamesPath, cancellationToken);
            var loadFontsTask = LoadSystemFontsAsync();

            // Wait for all tasks to complete
            await Task.WhenAll(loadBlocksTask, loadNamesTask, loadGlyphsTask, loadFontsTask);

            // Assign results
            UnicodeBlocks = await loadBlocksTask;
            UnicodeCharacterNames = await loadNamesTask;
            StandardMacintoshGlyphNames = await loadGlyphsTask;
            SystemFonts = await loadFontsTask;
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation
            UnicodeBlocks = [];
            UnicodeCharacterNames = [];
            StandardMacintoshGlyphNames = [];
            SystemFonts = [];
            throw; // Re-throw to notify caller
        }
        catch (Exception ex)
        {
            // Log error and set default values
            Debug.WriteLine($"Error loading primary data: {ex.Message}");
            UnicodeBlocks = [];
            UnicodeCharacterNames = [];
            StandardMacintoshGlyphNames = [];
            SystemFonts = [];
            throw; // Re-throw to allow caller to handle
        }
    }
    #endregion Private Methods

    #region Public Methods

    public async Task ValidateFontAsync(string font, CancellationToken cancellationToken = default)
    {
        bool isSystemFont = false;
        string fontFamilyPath = string.Empty;
        IsFontValid = false;
        // Validate input
        if (string.IsNullOrWhiteSpace(font))
        {
            return;
        }
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Check if font is a file path (asynchronously)
            bool fileExists = await Task.Run(() => File.Exists(font), cancellationToken);
            if (fileExists)
            {
                isSystemFont = false;
                fontFamilyPath = font;
                IsFontValid = true;
            }
            else if (SystemFonts.Contains(font))
            {
                isSystemFont = true;
                fontFamilyPath = font; // SystemFonts[SystemFonts.IndexOf(font)] is redundant since font is already the name
                IsFontValid = true;
            }

            if (IsFontValid)
            {
                // Initialize OpenTypeFont
                OpenTypeFont = new();

                // Dispose previous typeface if it exists
                typeface?.Dispose();
                typeface = null;

                // Load typeface
                typeface = isSystemFont
                    ? SKTypeface.FromFamilyName(fontFamilyPath)
                    : await Task.Run(() => SKTypeface.FromFile(fontFamilyPath), cancellationToken);

                if (typeface == null)
                {
                    IsFontValid = false;
                }
                else
                {
                    IsFontValid = true;
                }
            }
        }
        catch (OperationCanceledException)
        {
            Cleanup();
            throw; // Re-throw to notify caller
        }
        catch (Exception)
        {
            Cleanup();
            throw; // Re-throw to notify caller
        }
        finally
        {

        }
        void Cleanup()
        {
            IsFontValid = false;
            typeface?.Dispose();
            typeface = null;
        }
    }

    public async Task GetTablesListAsync([AllowNull] IProgress<string> progress = null, CancellationToken cancellationToken = default)
    {
        IsTablesListReady = false;
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Get font tables
            OpenTypeFont.Tables = await GetFontTablesAsync(typeface, progress, cancellationToken);
            IsTablesListReady = true;
        }
        catch (OperationCanceledException)
        {
            Cleanup();
            throw; // Re-throw to notify caller
        }
        catch (Exception)
        {
            Cleanup();
            throw; // Re-throw to allow caller to handle
        }
        finally
        {

        }

        void Cleanup()
        {
            IsTablesListReady = false;
            typeface?.Dispose();
            typeface = null;
        }
    }

    public async Task GetTablesDataAsync([AllowNull] IProgress<(string tableName, double percentage)> progress = null, CancellationToken cancellationToken = default)
    {
        IsTablesDataReady = false;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            // Parse tables (assumed to be async or made async)
            await ParseTablesAsync(OpenTypeFont, progress, cancellationToken);
            IsTablesDataReady = true;
        }
        catch (OperationCanceledException)
        {
            Cleanup();
            throw; // Re-throw to notify caller
        }
        catch (Exception)
        {
            Cleanup();
            throw; // Re-throw to allow caller to handle
        }
        finally
        {

        }

        void Cleanup()
        {
            IsTablesDataReady = false;
            typeface?.Dispose();
            typeface = null;
        }
    }

    public async Task GetGlyphsBitmapAsync([AllowNull] IProgress<(int glyphIndex, double percentage)> progress = null, CancellationToken cancellationToken = default)
    {
        IsGlyphsBitmapReady = true;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            font = new SKFont(typeface, LVGLFont.FontProperties.FontSize);
            font.Hinting = SKFontHinting.Full;
            font.Subpixel = true; // Enable subpixel rendering for better quality
            font.Edging = SKFontEdging.SubpixelAntialias; // Use anti-aliasing for smoother edges
            double scale = LVGLFont.FontProperties.FontSize / (double)OpenTypeFont.HeadTable.UnitsPerEm;
            glyphsBitmapList.Clear();

            // Parse tables (assumed to be async or made async)
            glyphsBitmapList = await RenderGlyphsAsync(
                font,
                OpenTypeFont.GlyfTable,
                LVGLFont,
                progress,
                cancellationToken);

        }
        catch (OperationCanceledException)
        {
            Cleanup();
            throw; // Re-throw to notify caller
        }
        catch (Exception)
        {
            Cleanup();
            throw; // Re-throw to allow caller to handle
        }
        finally
        {

        }

        void Cleanup()
        {
            IsGlyphsBitmapReady = false;
            typeface?.Dispose();
            typeface = null;
        }
    }

    public async Task GetGlyphsDataAsync([AllowNull] IProgress<(int glyphIndex, double percentage)> progress = null, CancellationToken cancellationToken = default)
    {
        IsGlyphsDataReady = true;

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            font = new SKFont(typeface, LVGLFont.FontProperties.FontSize);
            font.Hinting = SKFontHinting.Full;
            font.Subpixel = true; // Enable subpixel rendering for better quality
            font.Edging = SKFontEdging.SubpixelAntialias; // Use anti-aliasing for smoother edges
            double scale = LVGLFont.FontProperties.FontSize / (double)OpenTypeFont.HeadTable.UnitsPerEm;
            glyphsDataList.Clear();

            // Parse tables (assumed to be async or made async)
            glyphsDataList = await GetGlyphsAsync(
                OpenTypeFont.GlyfTable,
                OpenTypeFont.CmapTable,
                OpenTypeFont.HmtxTable,
                OpenTypeFont.PostTable,
                glyphsBitmapList,
                StandardMacintoshGlyphNames,
                UnicodeCharacterNames,
                scale,
                LVGLFont,
                progress,
                cancellationToken);

        }
        catch (OperationCanceledException)
        {
            Cleanup();
            IsGlyphsDataReady = false;
            throw; // Re-throw to notify caller
        }
        catch (Exception)
        {
            Cleanup();
            IsGlyphsDataReady = false;
            throw; // Re-throw to allow caller to handle
        }
        finally
        {

        }

        void Cleanup()
        {
            IsGlyphsDataReady = false;
            typeface?.Dispose();
            typeface = null;
        }
    }

    public async Task FinalizeFontDataAsync([AllowNull] IProgress<double> progress = null, CancellationToken cancellationToken = default)
    {

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            IsFontConverted = false;
            double scale = LVGLFont.FontProperties.FontSize / (double)OpenTypeFont.HeadTable.UnitsPerEm;
            LVGLFont.Glyphs = glyphsDataList;
            LVGLFont.EmptyGlyphsCount = 0;
            LVGLFont.UnMappedGlyphsCount = 0;
            LVGLFont.SingleMappedGlyphsCount = 0;
            LVGLFont.MultiMappedGlyphsCount = 0;
            progress.Report(25.0);

            foreach (var glyph in LVGLFont.Glyphs)
            {
                foreach (var unicode in glyph.Unicodes)
                {
                    LVGLFont.Unicodes.Add(unicode);
                }
                if (glyph.IsEmpty) LVGLFont.EmptyGlyphsCount++;
                if (glyph.IsUnMapped) LVGLFont.UnMappedGlyphsCount++;
                if (glyph.IsSingleMapped) LVGLFont.SingleMappedGlyphsCount++;
                if (glyph.IsMultiMapped) LVGLFont.MultiMappedGlyphsCount++;
            }
            progress.Report(50.0);

            foreach (var block in UnicodeBlocks)
            {
                foreach (var unicode in LVGLFont.Unicodes)
                {
                    if (unicode.CodePoint >= block.Start && unicode.CodePoint <= block.End)
                    {
                        bool existingKey = LVGLFont.UnicodeRanges.ContainsKey(block);
                        if (existingKey == false)
                        {
                            LVGLFont.UnicodeRanges.Add(block, 1);
                        }
                        else
                        {
                            LVGLFont.UnicodeRanges[block]++;
                        }
                    }
                }
            }
            progress.Report(75.0);

            LVGLFont.FontData.FullFontName = OpenTypeFont.NameTable.FullFontName;
            LVGLFont.FontData.FontFamily = OpenTypeFont.NameTable.FontFamily;
            LVGLFont.FontData.FontSubfamily = OpenTypeFont.NameTable.FontSubfamily;
            LVGLFont.FontData.Manufacturer = OpenTypeFont.NameTable.Manufacturer;
            LVGLFont.FontData.FontRevision = OpenTypeFont.HeadTable.FontRevision;
            LVGLFont.FontData.Created = OpenTypeFont.HeadTable.Created;
            LVGLFont.FontData.Modified = OpenTypeFont.HeadTable.Modified;
            LVGLFont.FontData.AdvanceWidthMax = (int)Math.Ceiling(scale * OpenTypeFont.HheaTable.AdvanceWidthMax);
            LVGLFont.FontData.Ascent = (int)Math.Ceiling(scale * OpenTypeFont.OS2Table.UsWinAscent);
            LVGLFont.FontData.Descent = (int)Math.Ceiling(scale * OpenTypeFont.OS2Table.UsWinDescent);
            LVGLFont.FontData.XMin = (int)Math.Ceiling(scale * OpenTypeFont.HeadTable.XMin);
            LVGLFont.FontData.YMin = (int)Math.Ceiling(scale * OpenTypeFont.HeadTable.YMin);
            LVGLFont.FontData.XMax = (int)Math.Ceiling(scale * OpenTypeFont.HeadTable.XMax);
            LVGLFont.FontData.YMax = (int)Math.Ceiling(scale * OpenTypeFont.HeadTable.YMax);
            LVGLFont.FontData.MaxCharWidth = (int)Math.Ceiling(font.Metrics.MaxCharacterWidth);
            LVGLFont.FontProperties.YAxisPosition = (int)Math.Floor(font.Metrics.XMin);
            LVGLFont.GlyphsCount = OpenTypeFont.MaxpTable.NumGlyphs;
            LVGLFont.FontProperties.LineHeight = LVGLFont.FontData.Ascent + LVGLFont.FontData.Descent;
            LVGLFont.FontProperties.BaseLine = LVGLFont.FontData.Descent;
            progress.Report(100.0);

            IsFontConverted = true;

        }
        catch (OperationCanceledException)
        {
            Cleanup();
            throw; // Re-throw to notify caller
        }
        catch (Exception)
        {
            Cleanup();
            throw; // Re-throw to allow caller to handle
        }
        finally
        {

        }

        void Cleanup()
        {
            IsFontConverted = false;
            typeface?.Dispose();
            typeface = null;
        }
    }

    public void Dispose()
    {
        font.Dispose();
        font = null;
        typeface.Dispose();
        typeface = null;
    }

    #endregion Public Methods


}
