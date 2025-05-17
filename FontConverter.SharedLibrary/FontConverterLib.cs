using FontConverter.SharedLibrary.Helpers;
using FontConverter.SharedLibrary.Models;

namespace FontConverter.SharedLibrary;

public class FontConverterLib
{
    public FontConverterLib()
    {
        UnicodeBlockCollection = new();
        StandardMacintoshGlyphNames = new();
        BitPerPixelList = new();
        SubPixelList = new();
        GlyphStyleList = new();
        EmbeddedLVGLFontsList = new();
    }

    public UnicodeBlockCollection UnicodeBlockCollection { get; set; }
    public SortedList<int, string> StandardMacintoshGlyphNames { get; set; }
    public SortedList<LVGLFontEnums.BIT_PER_PIXEL_ENUM, string> BitPerPixelList { get; set; }
    public SortedList<LVGLFontEnums.SUB_Pixel_ENUM, string> SubPixelList { get; set; }
    public SortedList<LVGLFontEnums.GLYPH_STYLE, string> GlyphStyleList { get; set; }
    public SortedList<int, string> EmbeddedLVGLFontsList { get; set; }

    public async Task InitializePrimaryDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var unicodeTask = InitialUnicodeBlockCollectionHelper.InitialUnicodeBlockCollection(cancellationToken); 
            var macintoshTask = InitialStandardMacintoshGlyphNameHelper.InitialStandardMacintoshGlyphName(cancellationToken); 
            var bitPerPixelTask = InitialBitPerPixelListHelper.InitialBitPerPixelList(cancellationToken);
            var subPixelTask = InitialSubPixelListHelper.InitialSubPixellList(cancellationToken);
            var glyphStyleTask = InitialGlyphStyleListHelper.InitialGlyphStyleList(cancellationToken);
            var embeddedFontsTask = InitialEmbeddedLVGLFontsListHelper.InitialEmbeddedLVGLFontsList(cancellationToken);

            await Task.WhenAll(unicodeTask, macintoshTask, bitPerPixelTask, subPixelTask, glyphStyleTask, embeddedFontsTask);

            UnicodeBlockCollection = unicodeTask.Result;
            StandardMacintoshGlyphNames =  macintoshTask.Result;
            BitPerPixelList =  bitPerPixelTask.Result;
            SubPixelList =  subPixelTask.Result;
            GlyphStyleList =  glyphStyleTask.Result;
            EmbeddedLVGLFontsList =  embeddedFontsTask.Result;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize primary data.", ex);
        }
    }
}
