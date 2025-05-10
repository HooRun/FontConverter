using LVGLFontConverter.Library.Data;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LVGLFontConverter.Library.Helpers;

public static class LoadDataRecordsHelper
{

    public static async Task<List<UnicodeBlock>> LoadUnicodeBlocksAsync(string path, CancellationToken cancellationToken = default)
    {
        var blocks = new List<UnicodeBlock>();
        using var reader = new StreamReader(path);
        string line;
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

    public static async Task<List<UnicodeCharacterName>> LoadUnicodeNamesAsync(string path, CancellationToken cancellationToken = default)
    {
        var unicodeData = new List<UnicodeCharacterName>();
        using var reader = new StreamReader(path);
        string line;
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

    public static async Task<List<StandardMacintoshGlyphName>> LoadStandardMacintoshGlyphNamesAsync(string path, CancellationToken cancellationToken = default)
    {
        var blocks = new List<StandardMacintoshGlyphName>();
        using var reader = new StreamReader(path);
        string line;
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

    public static Task<List<string>> LoadSystemFontsAsync()
    {
        return Task.Run(() =>
        {
            List<string> systemFonts = [];
            using (var fontCollection = new InstalledFontCollection())
            {
                foreach (var font in fontCollection.Families)
                {
                    systemFonts.Add(font.Name);
                }
            }
            return systemFonts;
        });
    }
}
