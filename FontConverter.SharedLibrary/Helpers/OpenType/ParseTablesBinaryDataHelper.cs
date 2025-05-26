using FontConverter.SharedLibrary.Models;
using SkiaSharp;
using System.Diagnostics.CodeAnalysis;
using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseTablesBinaryDataHelper
{
    public static async Task<SortedList<OpenTypeTables, OpenTypeTableBinaryData>> GetFontTablesAsync(SKTypeface typeface, [AllowNull] IProgress<string> progress = null, CancellationToken cancellationToken = default)
    {
        if (typeface == null)
            throw new ArgumentNullException(nameof(typeface));

        return await Task.Run(async () =>
        {
            var tables = new SortedList<OpenTypeTables, OpenTypeTableBinaryData>();
            uint[] tableTags = typeface.GetTableTags() ?? Array.Empty<uint>();

            if (tableTags.Length == 0)
                return tables;


            foreach (uint tag in tableTags)
            {
                if (!typeface.TryGetTableData(tag, out byte[] tableData) || tableData.Length == 0)
                    continue;

                if (!Enum.IsDefined(typeof(OpenTypeTables), tag))
                {
                    continue;
                }

                var table = new OpenTypeTableBinaryData
                {
                    Tag = tag,
                    TagName = FontTableValueConverterHelper.GetTableReadableTag(tag),
                    Length = typeface.GetTableSize(tag),
                    RawData = tableData
                };
                tables.Add((OpenTypeTables)tag, table);
                progress?.Report("Processed");
                await Task.Delay(1).ConfigureAwait(false);
            }

            return tables;
        }, cancellationToken);
    }
}
