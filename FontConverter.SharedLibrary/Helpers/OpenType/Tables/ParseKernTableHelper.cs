using FontConverter.SharedLibrary.Models;
using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseKernTableHelper
{
    const int chunkSize = 500;

    public static async Task<FontKernTable> ParseKernTable(OpenTypeTableBinaryData tableBinaryData, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        FontKernTable kernTable = new();

        using var ms = new MemoryStream(tableBinaryData.RawData);
        using var reader = new BinaryReader(ms);

        ushort version = ReadUInt16BigEndian(reader);
        ushort nTables = ReadUInt16BigEndian(reader);

        for (int i = 0; i < nTables; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            long subtableStart = reader.BaseStream.Position;

            ushort versionOrLength = ReadUInt16BigEndian(reader); // for Mac 0x0000 or Win 0x0001
            ushort length = ReadUInt16BigEndian(reader);
            KernCoverage coverage = (KernCoverage)ReadUInt16BigEndian(reader);
            ushort format = (ushort)(((ushort)coverage & (ushort)KernCoverage.FormatMask) >> 8);

            KernSubtable? subtable = null;
            switch (format)
            {
                case 0:
                    subtable = await KernParseFormat0(reader, subtableStart, cancellationToken).ConfigureAwait(false);
                    break;
                case 2:
                    subtable = await KernParseFormat2(reader, subtableStart, cancellationToken).ConfigureAwait(false);
                    break;
            }

            if (subtable != null)
            {
                subtable.Coverage = coverage;
                subtable.Format = format;
                kernTable.Subtables.Add(subtable);
            }

            reader.BaseStream.Seek(subtableStart + length, SeekOrigin.Begin); // move to next subtable
            await Task.Delay(1).ConfigureAwait(false);
        }

        return kernTable;
    }

    public static async Task<KernFormat0Subtable> KernParseFormat0(BinaryReader reader, long subtableOffset, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var subtable = new KernFormat0Subtable
        {
            Pairs = new List<KernPair>() 
        };

        ushort nPairs = ReadUInt16BigEndian(reader);
        ReadUInt16BigEndian(reader); // searchRange, ignored
        ReadUInt16BigEndian(reader); // entrySelector, ignored
        ReadUInt16BigEndian(reader); // rangeShift, ignored

        subtable.Pairs.Capacity = nPairs;
        for (int i = 0; i < nPairs; i += chunkSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int batchEnd = Math.Min(i + chunkSize, nPairs);
            for (int j = i; j < batchEnd; j++)
            {
                ushort left = ReadUInt16BigEndian(reader);
                ushort right = ReadUInt16BigEndian(reader);
                short value = ReadInt16BigEndian(reader);
                subtable.Pairs.Add(new KernPair { Left = left, Right = right, Value = value });
            }
            await Task.Delay(1).ConfigureAwait(false);
        }

        return subtable;
    }

    public static async Task<KernFormat2Subtable> KernParseFormat2(BinaryReader reader, long subtableOffset, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var subtable = new KernFormat2Subtable();

        subtable.RowWidth = ReadUInt16BigEndian(reader);
        subtable.LeftClassTableOffset = ReadUInt16BigEndian(reader);
        subtable.RightClassTableOffset = ReadUInt16BigEndian(reader);
        subtable.ArrayOffset = ReadUInt16BigEndian(reader);

        long leftClassOffset = subtableOffset + subtable.LeftClassTableOffset;
        long rightClassOffset = subtableOffset + subtable.RightClassTableOffset;
        long arrayOffset = subtableOffset + subtable.ArrayOffset;

        reader.BaseStream.Seek(leftClassOffset, SeekOrigin.Begin);
        ushort leftFirstGlyph = ReadUInt16BigEndian(reader);
        ushort leftGlyphCount = ReadUInt16BigEndian(reader);
        ushort[] leftClasses = new ushort[leftGlyphCount];
        for (int i = 0; i < leftGlyphCount; i++)
            leftClasses[i] = ReadUInt16BigEndian(reader);

        reader.BaseStream.Seek(rightClassOffset, SeekOrigin.Begin);
        ushort rightFirstGlyph = ReadUInt16BigEndian(reader);
        ushort rightGlyphCount = ReadUInt16BigEndian(reader);
        ushort[] rightClasses = new ushort[rightGlyphCount];
        for (int i = 0; i < rightGlyphCount; i++)
            rightClasses[i] = ReadUInt16BigEndian(reader);

        subtable.NumLeftClasses = (ushort)(leftClasses.Max() + 1);
        subtable.NumRightClasses = (ushort)(rightClasses.Max() + 1);

        reader.BaseStream.Seek(arrayOffset, SeekOrigin.Begin);
        subtable.KerningValues = new ushort[subtable.NumLeftClasses, subtable.NumRightClasses];
        
        for (int i = 0; i < subtable.NumLeftClasses; i += chunkSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int batchEnd = Math.Min(i + chunkSize, subtable.NumLeftClasses);
            for (int j = i; j < batchEnd; j++)
            {
                for (int k = 0; k < subtable.NumRightClasses; k++)
                {
                    subtable.KerningValues[j, k] = ReadUInt16BigEndian(reader);
                }
            }
            await Task.Delay(1).ConfigureAwait(false);
        }

        return subtable;
    }
}