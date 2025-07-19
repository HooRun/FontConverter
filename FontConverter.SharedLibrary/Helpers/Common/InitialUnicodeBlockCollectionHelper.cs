using FontConverter.SharedLibrary.Models;
using MessagePack;
using System.Reflection;

namespace FontConverter.SharedLibrary.Helpers;

public static class InitialUnicodeBlockCollectionHelper
{

    public static async Task<UnicodeBlockCollection> InitialUnicodeBlockCollection(CancellationToken cancellationToken = default)
    {
        await Task.Yield();
        UnicodeBlockCollection unicodeBlockCollection = new UnicodeBlockCollection();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var assembly = Assembly.GetExecutingAssembly();
            string? resourceName = assembly
                .GetManifestResourceNames()
                .FirstOrDefault(n => n.EndsWith("unicode_database.bin", StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
                throw new FileNotFoundException($"Embedded resource '{"unicode_database.bin"}' not found.");

            using Stream stream = assembly.GetManifestResourceStream(resourceName)!;
            if (stream == null)
                throw new NullReferenceException("Stream is null.");

            var blocks = await MessagePackSerializer.DeserializeAsync<SortedDictionary<uint, UnicodeBlock>>(stream);
            if (blocks == null)
            {
                throw new InvalidOperationException("Failed to deserialize Unicode blocks from embedded resource.");
            }
            else
            {
                unicodeBlockCollection.Blocks = blocks;
            }

        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize Unicode block collection.", ex);
        }

        return unicodeBlockCollection;
    }
    
}
