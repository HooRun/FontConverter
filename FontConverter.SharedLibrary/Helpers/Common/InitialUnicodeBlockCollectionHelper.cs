using FontConverter.SharedLibrary.Models;
using System.Reflection;

namespace FontConverter.SharedLibrary.Helpers;

public static class InitialUnicodeBlockCollectionHelper
{

    public static async Task<UnicodeBlockCollection> InitialUnicodeBlockCollection(CancellationToken cancellationToken = default)
    {
        UnicodeBlockCollection unicodeBlockCollection = new UnicodeBlockCollection();

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var loadBlocksTask = InitialUnicodeBlocksAsync("Blocks.txt", cancellationToken);
            var loadCharactersTask = InitialUnicodeCharactersAsync("UnicodeData.txt", cancellationToken);
            await Task.WhenAll(loadBlocksTask, loadCharactersTask);
            unicodeBlockCollection.Blocks = await loadBlocksTask;
            List<UnicodeCharacter> characters = await loadCharactersTask;
            foreach (var character in characters)
            {
                int codePoint = character.CodePoint;
                UnicodeBlock? containingBlock = null;
                foreach (var pair in unicodeBlockCollection.Blocks)
                {
                    var blockRange = pair.Key;
                    var block = pair.Value;
                    if (blockRange.Start > codePoint)
                    {
                        break;
                    }
                    if (codePoint >= blockRange.Start && codePoint <= blockRange.End)
                    {
                        containingBlock = block;
                        break;
                    }
                }
                if (containingBlock != null)
                {
                    if (!containingBlock.Characters.TryAdd(codePoint, character))
                    {

                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }

        return unicodeBlockCollection;
    }

    public static async Task<SortedList<(int Start, int End), UnicodeBlock>> InitialUnicodeBlocksAsync(string resourceName, CancellationToken cancellationToken = default)
    {
        SortedList<(int Start, int End), UnicodeBlock> blocks = new SortedList<(int Start, int End), UnicodeBlock>();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var assembly = Assembly.GetExecutingAssembly();
            var fullName = assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith(resourceName));
            if (fullName == null)
                throw new FileNotFoundException($"Resource {resourceName} not found");
            using var stream = assembly.GetManifestResourceStream(fullName)!;
            using var reader = new StreamReader(stream);
            string? line;
            while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var clean = line.Split('#')[0].Trim();
                if (string.IsNullOrWhiteSpace(clean)) continue;

                var parts = clean.Split(';');
                var range = parts[0].Trim();
                var name = parts[1].Trim();

                var bounds = range.Split("..");
                int start = Convert.ToInt32(bounds[0], 16);
                int end = Convert.ToInt32(bounds[1], 16);
                if (!blocks.ContainsKey((start, end)))
                {
                    blocks.TryAdd((start, end), new UnicodeBlock(start, end, name));
                }

            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
        return blocks;
    }

    public static async Task<List<UnicodeCharacter>> InitialUnicodeCharactersAsync(string resourceName, CancellationToken cancellationToken = default)
    {
        var unicodeData = new List<UnicodeCharacter>();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var assembly = Assembly.GetExecutingAssembly();
            var fullName = assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith(resourceName));
            if (fullName == null)
                throw new FileNotFoundException($"Resource {resourceName} not found");
            using var stream = assembly.GetManifestResourceStream(fullName)!;
            using var reader = new StreamReader(stream);
            string? line;
            while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(';');
                if (parts.Length < 2) continue;

                int codePoint = Convert.ToInt32(parts[0], 16);
                string name = parts[1];
                string alternateName = parts[10];

                if (name.StartsWith('<') && name.EndsWith('>'))
                {
                    if (name == "<control>" && !string.IsNullOrWhiteSpace(alternateName))
                        name = alternateName;
                }

                if (string.IsNullOrWhiteSpace(name))
                    name = $"U+{codePoint:X4}";

                unicodeData.Add(new UnicodeCharacter(codePoint, name));
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
        return unicodeData;
    }
}
