using SkiaSharp;
using System.Diagnostics;
using FontConverter.SharedLibrary.Data;
using FontConverter.SharedLibrary.Helpers;

namespace FontConverter.SharedLibrary;

public class OpenTypeFontAnalyzer
{
    public OpenTypeFontAnalyzer()
    {
        
    }

    #region Private Variables
    private static readonly string _UnicodeBlocksPath = "Blocks.txt";
    private static readonly string _UnicodeDataPath = "UnicodeData.txt";
    private static readonly string _StandardMacintoshGlyphNamesPath = "StandardMacintoshGlyphNames.txt";
    #endregion Private Variables

    #region Public Variables
    public List<UnicodeBlock> UnicodeBlocks { get; private set; } = [];
    public List<UnicodeCharacterName> UnicodeCharacterNames { get; private set; } = [];
    public List<StandardMacintoshGlyphName> StandardMacintoshGlyphNames { get; private set; } = [];
    #endregion Public Variables

    #region Private Methods
    public async Task LoadPrimaryDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Run all load operations concurrently
            var loadBlocksTask = LoadDataRecordsHelper.LoadUnicodeBlocksAsync(_UnicodeBlocksPath, cancellationToken);
            var loadNamesTask = LoadDataRecordsHelper.LoadUnicodeNamesAsync(_UnicodeDataPath, cancellationToken);
            var loadGlyphsTask = LoadDataRecordsHelper.LoadStandardMacintoshGlyphNamesAsync(_StandardMacintoshGlyphNamesPath, cancellationToken);

            // Wait for all tasks to complete
            await Task.WhenAll(loadBlocksTask, loadNamesTask, loadGlyphsTask);

            // Assign results
            UnicodeBlocks = await loadBlocksTask;
            UnicodeCharacterNames = await loadNamesTask;
            StandardMacintoshGlyphNames = await loadGlyphsTask;
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation
            UnicodeBlocks = [];
            UnicodeCharacterNames = [];
            StandardMacintoshGlyphNames = [];
            throw; // Re-throw to notify caller
        }
        catch (Exception ex)
        {
            // Log error and set default values
            Debug.WriteLine($"Error loading primary data: {ex.Message}");
            UnicodeBlocks = [];
            UnicodeCharacterNames = [];
            StandardMacintoshGlyphNames = [];
            throw; // Re-throw to allow caller to handle
        }
    }
    #endregion Private Methods
}
