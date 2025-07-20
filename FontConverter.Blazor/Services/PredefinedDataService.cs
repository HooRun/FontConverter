using FontConverter.SharedLibrary.Helpers;
using FontConverter.SharedLibrary.Models;

namespace FontConverter.Blazor.Services;

public class PredefinedDataService : PredefinedData
{
    public PredefinedDataService() : base()
    {
        
    }

    public async Task InitializePrimaryDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            StandardMacintoshGlyphNames = InitialStandardMacintoshGlyphNameHelper.InitialStandardMacintoshGlyphName();
            BitPerPixelList = InitialBitPerPixelListHelper.InitialBitPerPixelList();
            SubPixelList = InitialSubPixelListHelper.InitialSubPixellList();
            GlyphStyleList = InitialGlyphStyleListHelper.InitialGlyphStyleList();
            EmbeddedLVGLFontsList = InitialEmbeddedLVGLFontsListHelper.InitialEmbeddedLVGLFontsList();
            Blocks = await InitialUnicodeBlockCollectionHelper.InitialUnicodeBlockCollection(cancellationToken);
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
