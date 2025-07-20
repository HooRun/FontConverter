using FonntConverter.CreateDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FonntConverter.CreateDB.Helpers;

public static class ParseAllKeysCLDRHelper
{
    public static async Task<SortedDictionary<string, Collation>> ParseAllKeysCLDR(string resourceName, CancellationToken cancellationToken = default)
    {
        var dict = new SortedDictionary<string, Collation>();

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

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var commentSplit = line.Split('#', 2);
                var baseContent = commentSplit[0].Trim();
                var comment = commentSplit.Length > 1 ? commentSplit[1].Trim() : null;

                var parts = baseContent.Split(';');
                if (parts.Length != 2)
                    continue;

                var codePointPart = parts[0].Trim();
                var weightPart = parts[1].Trim();

                var codePoints = codePointPart
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(cp => Convert.ToUInt32(cp, 16))
                    .ToList();

                var weightMatches = System.Text.RegularExpressions.Regex.Matches(weightPart, @"\[(.*?)\]");
                var weights = new List<CollationWeight>();

                foreach (Match match in weightMatches)
                {
                    var raw = match.Groups[1].Value.TrimStart('.', '*');
                    var segments = raw.Split('.');

                    if (segments.Length != 3)
                        continue;

                    weights.Add(new CollationWeight
                    {
                        Primary = segments[0],
                        Secondary = segments[1],
                        Tertiary = segments[2]
                    });
                }

                var key = string.Join("_", codePoints.Select(cp => cp.ToString("X4")));

                dict[key] = new Collation(codePoints, weights, comment);
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load Unicode characters from {resourceName}.", ex);
        }
        return dict;
    }

}
