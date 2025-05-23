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

            var unicodeTask = InitialUnicodeBlockCollectionHelper.InitialUnicodeBlockCollection(cancellationToken);
            var macintoshTask = InitialStandardMacintoshGlyphNameHelper.InitialStandardMacintoshGlyphName(cancellationToken);
            var bitPerPixelTask = InitialBitPerPixelListHelper.InitialBitPerPixelList(cancellationToken);
            var subPixelTask = InitialSubPixelListHelper.InitialSubPixellList(cancellationToken);
            var glyphStyleTask = InitialGlyphStyleListHelper.InitialGlyphStyleList(cancellationToken);
            var embeddedFontsTask = InitialEmbeddedLVGLFontsListHelper.InitialEmbeddedLVGLFontsList(cancellationToken);

            await Task.WhenAll(unicodeTask, macintoshTask, bitPerPixelTask, subPixelTask, glyphStyleTask, embeddedFontsTask);

            UnicodeBlockCollection = unicodeTask.Result;
            StandardMacintoshGlyphNames = macintoshTask.Result;
            BitPerPixelList = bitPerPixelTask.Result;
            SubPixelList = subPixelTask.Result;
            GlyphStyleList = glyphStyleTask.Result;
            EmbeddedLVGLFontsList = embeddedFontsTask.Result;
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
