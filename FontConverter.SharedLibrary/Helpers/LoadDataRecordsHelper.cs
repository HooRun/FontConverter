using FontConverter.SharedLibrary.Data;
using System.Reflection;

namespace FontConverter.SharedLibrary.Helpers;

public static class LoadDataRecordsHelper
{

    public static async Task<List<UnicodeBlock>> LoadUnicodeBlocksAsync(string resourceName, CancellationToken cancellationToken = default)
    {
        var blocks = new List<UnicodeBlock>();
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

            blocks.Add(new UnicodeBlock(start, end, name));
        }
        return blocks;
    }

    public static async Task<List<UnicodeCharacterName>> LoadUnicodeNamesAsync(string resourceName, CancellationToken cancellationToken = default)
    {
        var unicodeData = new List<UnicodeCharacterName>();
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

            unicodeData.Add(new UnicodeCharacterName(codePoint, name));
        }
        return unicodeData;
    }

    public static async Task<List<StandardMacintoshGlyphName>> LoadStandardMacintoshGlyphNamesAsync(string resourceName, CancellationToken cancellationToken = default)
    {
        var blocks = new List<StandardMacintoshGlyphName>();
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
            int glyphID = Convert.ToInt32(parts[0].Trim(), 16);
            var glyphName = parts[1].Trim();

            blocks.Add(new StandardMacintoshGlyphName(glyphID, glyphName));
        }
        return blocks;
    }

}
