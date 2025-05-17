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

    public async Task InitialPrimaryData(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            UnicodeBlockCollection = await InitialUnicodeBlockCollectionHelper.InitialUnicodeBlockCollection(cancellationToken);
            StandardMacintoshGlyphNames = await InitialStandardMacintoshGlyphNameHelper.InitialStandardMacintoshGlyphName(cancellationToken);
            BitPerPixelList = await InitialBitPerPixelListHelper.InitialBitPerPixelList(cancellationToken);
            SubPixelList = await InitialSubPixelListHelper.InitialSubPixellList(cancellationToken);
            GlyphStyleList = await InitialGlyphStyleListHelper.InitialGlyphStyleList(cancellationToken);
            EmbeddedLVGLFontsList = await InitialEmbeddedLVGLFontsListHelper.InitialEmbeddedLVGLFontsList(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
