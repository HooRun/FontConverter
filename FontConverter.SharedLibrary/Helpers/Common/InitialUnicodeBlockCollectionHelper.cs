using FontConverter.SharedLibrary.Models;
using System.Collections.Concurrent;
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

            unicodeBlockCollection.AllBlocks = unicodeBlockCollection.Blocks.ToDictionary(b => b.Key, b => b.Value);
            unicodeBlockCollection.AllCharacters = characters.ToDictionary(c => c.CodePoint, c => c);

            var sortedBlocks = unicodeBlockCollection.Blocks
                .Select(b => (Range: b.Key, Block: b.Value))
                .OrderBy(b => b.Range.Start)
                .ToArray();

            Parallel.ForEach(characters, character =>
            {
                uint codePoint = character.CodePoint;
                UnicodeBlock? containingBlock = FindContainingBlock(sortedBlocks, codePoint);
                if (containingBlock != null)
                {
                    lock (containingBlock.Characters) 
                    {
                        containingBlock.Characters.TryAdd(codePoint, character);
                    }
                }
            });
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

    private static UnicodeBlock? FindContainingBlock((ValueTuple<uint, uint> Range, UnicodeBlock Block)[] sortedBlocks, uint codePoint)
    {
        int left = 0, right = sortedBlocks.Length - 1;
        while (left <= right)
        {
            int mid = (left + right) / 2;
            var (start, end) = sortedBlocks[mid].Range;
            if (codePoint >= start && codePoint <= end)
                return sortedBlocks[mid].Block;
            if (codePoint < start)
                right = mid - 1;
            else
                left = mid + 1;
        }
        return null;
    }

    public static async Task<SortedList<(uint Start, uint End), UnicodeBlock>> InitialUnicodeBlocksAsync(string resourceName, CancellationToken cancellationToken = default)
    {
        var blocks = new SortedList<(uint Start, uint End), UnicodeBlock>();
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var assembly = Assembly.GetExecutingAssembly();
            var fullName = assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith(resourceName))
                ?? throw new FileNotFoundException($"Resource {resourceName} not found");

            using var stream = assembly.GetManifestResourceStream(fullName)!;
            using var reader = new StreamReader(stream);

            string content = await reader.ReadToEndAsync(cancellationToken);

            var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Parallel.ForEach(lines, line =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var clean = line.Split('#')[0].Trim();
                if (string.IsNullOrWhiteSpace(clean)) return;

                var parts = clean.Split(';', StringSplitOptions.TrimEntries);
                if (parts.Length < 2) return;

                var bounds = parts[0].Split("..", StringSplitOptions.TrimEntries);
                uint start = Convert.ToUInt32(bounds[0], 16);
                uint end = Convert.ToUInt32(bounds[1], 16);
                string name = parts[1];

                lock (blocks)
                {
                    if (!blocks.ContainsKey((start, end)))
                    {
                        blocks.TryAdd((start, end), new UnicodeBlock(start, end, name));
                    }
                }
            });
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load Unicode blocks from {resourceName}.", ex);
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
            var fullName = assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith(resourceName))
                ?? throw new FileNotFoundException($"Resource {resourceName} not found");

            using var stream = assembly.GetManifestResourceStream(fullName)!;
            using var reader = new StreamReader(stream);

            string content = await reader.ReadToEndAsync(cancellationToken);

            var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var tempList = new ConcurrentBag<UnicodeCharacter>();

            Parallel.ForEach(lines, line =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (string.IsNullOrWhiteSpace(line)) return;

                var parts = line.Split(';', StringSplitOptions.TrimEntries);
                if (parts.Length < 2) return;

                uint codePoint = Convert.ToUInt32(parts[0], 16);
                string name = parts[1];
                string alternateName = parts.Length > 10 ? parts[10] : string.Empty;

                if (name.StartsWith('<') && name.EndsWith('>'))
                {
                    if (name == "<control>" && !string.IsNullOrWhiteSpace(alternateName))
                        name = alternateName;
                }

                if (string.IsNullOrWhiteSpace(name))
                    name = $"U+{codePoint:X4}";

                tempList.Add(new UnicodeCharacter(codePoint, name));
            });

            unicodeData.AddRange(tempList);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load Unicode characters from {resourceName}.", ex);
        }
        return unicodeData;
    }
}
