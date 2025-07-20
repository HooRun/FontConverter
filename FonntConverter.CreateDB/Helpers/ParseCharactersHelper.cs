using FonntConverter.CreateDB.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static FonntConverter.CreateDB.Helpers.UCDEnumsHelper;

namespace FonntConverter.CreateDB.Helpers;

public static class ParseCharactersHelper
{
    public static async Task<SortedDictionary<uint, Character>> ParseUnicodeDataAsync(string resourceName, CancellationToken cancellationToken = default)
    {
        SortedDictionary<uint, Character> unicodeData = new();

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
                if (string.IsNullOrWhiteSpace(line)) return;

                var parts = line.Split(';', StringSplitOptions.TrimEntries);
                if (parts.Length < 6) return;

                uint codePoint = Convert.ToUInt32(parts[0], 16);
                string? name = parts[1];
                string? alternateName = parts.Length > 10 ? parts[10] : string.Empty;

                if (name.StartsWith('<') && name.EndsWith('>'))
                {
                    if (name == "<control>" && !string.IsNullOrWhiteSpace(alternateName))
                        name = alternateName;
                }

                if (string.IsNullOrWhiteSpace(name))
                    name = string.Empty;

                DecompositionTypeEnum decompositionType = DecompositionTypeEnum.DECOMPOSITION_TYPE_NONE;
                List<uint> decompositionMapping = [];

                var decomposition = parts[5];
                if (!string.IsNullOrWhiteSpace(decomposition))
                {
                    var tokens = decomposition.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    int index = 0;

                    if (tokens[0].StartsWith('<') && tokens[0].EndsWith('>'))
                    {
                        var tag = tokens[0][1..^1].ToUpperInvariant(); // remove < >
                        decompositionType = tag switch
                        {
                            "FONT" => DecompositionTypeEnum.DECOMPOSITION_TYPE_FONT,
                            "NOBREAK" => DecompositionTypeEnum.DECOMPOSITION_TYPE_NOBREAK,
                            "INITIAL" => DecompositionTypeEnum.DECOMPOSITION_TYPE_INITIAL,
                            "MEDIAL" => DecompositionTypeEnum.DECOMPOSITION_TYPE_MEDIAL,
                            "FINAL" => DecompositionTypeEnum.DECOMPOSITION_TYPE_FINAL,
                            "ISOLATED" => DecompositionTypeEnum.DECOMPOSITION_TYPE_ISOLATED,
                            "CIRCLE" => DecompositionTypeEnum.DECOMPOSITION_TYPE_CIRCLE,
                            "SUPER" => DecompositionTypeEnum.DECOMPOSITION_TYPE_SUPER,
                            "SUB" => DecompositionTypeEnum.DECOMPOSITION_TYPE_SUB,
                            "VERTICAL" => DecompositionTypeEnum.DECOMPOSITION_TYPE_VERTICAL,
                            "WIDE" => DecompositionTypeEnum.DECOMPOSITION_TYPE_WIDE,
                            "NARROW" => DecompositionTypeEnum.DECOMPOSITION_TYPE_NARROW,
                            "SMALL" => DecompositionTypeEnum.DECOMPOSITION_TYPE_SMALL,
                            "SQUARE" => DecompositionTypeEnum.DECOMPOSITION_TYPE_SQUARE,
                            "FRACTION" => DecompositionTypeEnum.DECOMPOSITION_TYPE_FRACTION,
                            "COMPAT" => DecompositionTypeEnum.DECOMPOSITION_TYPE_COMPAT,
                            _ => DecompositionTypeEnum.DECOMPOSITION_TYPE_COMPAT
                        };
                        index = 1;
                    }
                    else
                    {
                        decompositionType = DecompositionTypeEnum.DECOMPOSITION_TYPE_CANONICAL;
                    }

                    for (int i = index; i < tokens.Length; i++)
                    {
                        if (uint.TryParse(tokens[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint mappedCodePoint))
                        {
                            decompositionMapping.Add(mappedCodePoint);
                        }
                    }
                }


                var character = new Character(codePoint, name, decompositionType, decompositionMapping);

                lock (unicodeData)
                {
                    unicodeData.TryAdd(codePoint, character);
                }
            });

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
