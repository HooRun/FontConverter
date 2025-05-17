using FontConverter.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Helpers;

public static class InitialStandardMacintoshGlyphNameHelper
{
    public static async Task<SortedList<int, string>> InitialStandardMacintoshGlyphName(CancellationToken cancellationToken = default)
    {
        string resourceName = "StandardMacintoshGlyphNames.txt";
        SortedList<int, string> standardMacintoshGlyphNames = [];
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
                int glyphID = Convert.ToInt32(parts[0].Trim(), 16);
                var glyphName = parts[1].Trim();

                standardMacintoshGlyphNames.Add(glyphID, glyphName);
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
        return standardMacintoshGlyphNames;
    }
}
