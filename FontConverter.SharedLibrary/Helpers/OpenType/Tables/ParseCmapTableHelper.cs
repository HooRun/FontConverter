using FontConverter.SharedLibrary.Models;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseCmapTableHelper
{
    const int chunkSize = 500;
    public static async Task<FontCmapTable> ParseCmapTable(OpenTypeTableBinaryData tableBinaryData, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        FontCmapTable cmapTable = new();

        using var ms = new MemoryStream(tableBinaryData.RawData);
        using var reader = new BinaryReader(ms);

        ushort version = ReadUInt16BigEndian(reader);
        ushort numTables = ReadUInt16BigEndian(reader);
        var encodingRecords = new List<(ushort platformID, ushort encodingID, uint subtableOffset)>(numTables);

        for (int i = 0; i < numTables; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ushort platformID = ReadUInt16BigEndian(reader);
            ushort encodingID = ReadUInt16BigEndian(reader);
            uint subtableOffset = ReadUInt32BigEndian(reader);
            encodingRecords.Add((platformID, encodingID, subtableOffset));
            await Task.Delay(1).ConfigureAwait(false);
        }

        foreach (var (platformID, encodingID, subtableOffset) in encodingRecords)
        {
            cancellationToken.ThrowIfCancellationRequested();
            long tableStart = subtableOffset;
            reader.BaseStream.Seek(tableStart, SeekOrigin.Begin);
            ushort format = ReadUInt16BigEndian(reader);
            reader.BaseStream.Seek(tableStart, SeekOrigin.Begin);

            var entries = format switch
            {
                0 => await ParseFormat0(reader, tableStart, cancellationToken).ConfigureAwait(false),
                4 => await ParseFormat4(reader, tableStart, cancellationToken).ConfigureAwait(false),
                6 => await ParseFormat6(reader, tableStart, cancellationToken).ConfigureAwait(false),
                10 => await ParseFormat10(reader, tableStart, cancellationToken).ConfigureAwait(false),
                12 => await ParseFormat12(reader, tableStart, cancellationToken).ConfigureAwait(false),
                13 => await ParseFormat13(reader, tableStart, cancellationToken).ConfigureAwait(false),
                14 => await ParseFormat14(reader, tableStart, cancellationToken).ConfigureAwait(false),
                _ => null
            };

            if (entries != null)
            {
                for (int i = 0; i < entries.Count; i += chunkSize)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var chunk = entries.Skip(i).Take(chunkSize);
                    foreach (var (unicode, glyph) in chunk)
                    {
                        cmapTable.UnicodeToGlyphMap[unicode] = glyph;
                    }
                    await Task.Delay(1).ConfigureAwait(false);
                }
            }
            await Task.Delay(1).ConfigureAwait(false);
        }

        return cmapTable;
    }

    public static async Task<Dictionary<uint, ushort>> ParseFormat0(BinaryReader reader, long offset, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = new Dictionary<uint, ushort>(256);
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        ReadUInt16BigEndian(reader); // format
        ReadUInt16BigEndian(reader); // length
        ReadUInt16BigEndian(reader); // language

        for (uint i = 0; i < 256; i++)
        {
            byte glyphId = reader.ReadByte();
            if (glyphId != 0)
                result[i] = glyphId;
        }
        await Task.Delay(1).ConfigureAwait(false);
        return result;
    }

    public static async Task<Dictionary<uint, ushort>> ParseFormat4(BinaryReader reader, long offset, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = new Dictionary<uint, ushort>();
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        ReadUInt16BigEndian(reader); // format
        ushort length = ReadUInt16BigEndian(reader);
        ReadUInt16BigEndian(reader); // language
        ushort segCountX2 = ReadUInt16BigEndian(reader);
        ushort segCount = (ushort)(segCountX2 / 2);
        ReadUInt16BigEndian(reader); // searchRange
        ReadUInt16BigEndian(reader); // entrySelector
        ReadUInt16BigEndian(reader); // rangeShift

        ushort[] endCode = new ushort[segCount];
        for (int i = 0; i < segCount; i++) endCode[i] = ReadUInt16BigEndian(reader);
        ReadUInt16BigEndian(reader); // reservedPad
        ushort[] startCode = new ushort[segCount];
        for (int i = 0; i < segCount; i++) startCode[i] = ReadUInt16BigEndian(reader);
        short[] idDelta = new short[segCount];
        for (int i = 0; i < segCount; i++) idDelta[i] = ReadInt16BigEndian(reader);
        ushort[] idRangeOffset = new ushort[segCount];
        long idRangeOffsetStart = reader.BaseStream.Position;
        for (int i = 0; i < segCount; i++) idRangeOffset[i] = ReadUInt16BigEndian(reader);

        for (int i = 0; i < segCount; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            uint charCount = (uint)(endCode[i] - startCode[i] + 1);
            for (uint c = startCode[i]; c <= endCode[i];)
            {
                cancellationToken.ThrowIfCancellationRequested();
                uint batchEnd = (uint)Math.Min(c + (uint)chunkSize, endCode[i] + 1);
                for (uint batchC = c; batchC < batchEnd; batchC++)
                {
                    if (idRangeOffset[i] == 0)
                    {
                        ushort glyphIndex = (ushort)((batchC + idDelta[i]) % 65536);
                        if (glyphIndex != 0)
                            result[batchC] = glyphIndex;
                    }
                    else
                    {
                        long glyphIndexOffset = idRangeOffsetStart + (2 * i) + idRangeOffset[i] + 2 * (batchC - startCode[i]);
                        if (glyphIndexOffset + 2 <= reader.BaseStream.Length)
                        {
                            reader.BaseStream.Seek(glyphIndexOffset, SeekOrigin.Begin);
                            ushort glyphIndex = ReadUInt16BigEndian(reader);
                            if (glyphIndex != 0)
                            {
                                glyphIndex = (ushort)((glyphIndex + idDelta[i]) % 65536);
                                result[batchC] = glyphIndex;
                            }
                        }
                    }
                }
                c = batchEnd;
                await Task.Delay(1).ConfigureAwait(false);
            }
            await Task.Delay(1).ConfigureAwait(false);
        }
        return result;
    }

    public static async Task<Dictionary<uint, ushort>> ParseFormat6(BinaryReader reader, long offset, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = new Dictionary<uint, ushort>();
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        ReadUInt16BigEndian(reader); // format
        ReadUInt16BigEndian(reader); // length
        ReadUInt16BigEndian(reader); // language
        ushort firstCode = ReadUInt16BigEndian(reader);
        ushort entryCount = ReadUInt16BigEndian(reader);

        for (uint i = 0; i < entryCount; i += (uint)chunkSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            uint batchEnd = Math.Min(i + (uint)chunkSize, entryCount);
            for (uint j = i; j < batchEnd; j++)
            {
                ushort glyphId = ReadUInt16BigEndian(reader);
                if (glyphId != 0)
                    result[firstCode + j] = glyphId;
            }
            await Task.Delay(1).ConfigureAwait(false);
        }
        return result;
    }

    public static async Task<Dictionary<uint, ushort>> ParseFormat10(BinaryReader reader, long offset, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = new Dictionary<uint, ushort>();
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        ReadUInt16BigEndian(reader); // format
        ReadUInt16BigEndian(reader); // reserved
        ReadUInt32BigEndian(reader); // length
        ReadUInt32BigEndian(reader); // language
        uint startCharCode = ReadUInt32BigEndian(reader);
        uint numChars = ReadUInt32BigEndian(reader);

        for (uint i = 0; i < numChars; i += (uint)chunkSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            uint batchEnd = Math.Min(i + (uint)chunkSize, numChars);
            for (uint j = i; j < batchEnd; j++)
            {
                ushort glyphId = ReadUInt16BigEndian(reader);
                if (glyphId != 0)
                    result[startCharCode + j] = glyphId;
            }
            await Task.Delay(1).ConfigureAwait(false);
        }
        return result;
    }

    public static async Task<Dictionary<uint, ushort>> ParseFormat12(BinaryReader reader, long offset, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = new Dictionary<uint, ushort>();
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        ReadUInt16BigEndian(reader); // format
        ReadUInt16BigEndian(reader); // reserved
        ReadUInt32BigEndian(reader); // length
        ReadUInt32BigEndian(reader); // language
        uint nGroups = ReadUInt32BigEndian(reader);

        for (uint i = 0; i < nGroups; i += (uint)chunkSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            uint batchEnd = Math.Min(i + (uint)chunkSize, nGroups);
            for (uint j = i; j < batchEnd; j++)
            {
                uint startCharCode = ReadUInt32BigEndian(reader);
                uint endCharCode = ReadUInt32BigEndian(reader);
                uint startGlyphID = ReadUInt32BigEndian(reader);
                for (uint c = startCharCode; c <= endCharCode;)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    uint charBatchEnd = Math.Min(c + (uint)chunkSize, endCharCode + 1);
                    for (uint batchC = c; batchC < charBatchEnd; batchC++)
                    {
                        result[batchC] = (ushort)(startGlyphID + (batchC - startCharCode));
                    }
                    c = charBatchEnd;
                    await Task.Delay(1).ConfigureAwait(false);
                }
            }
            await Task.Delay(1).ConfigureAwait(false);
        }
        return result;
    }

    public static async Task<Dictionary<uint, ushort>> ParseFormat13(BinaryReader reader, long offset, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = new Dictionary<uint, ushort>();
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        ReadUInt16BigEndian(reader); // format
        ReadUInt16BigEndian(reader); // reserved
        ReadUInt32BigEndian(reader); // length
        ReadUInt32BigEndian(reader); // language
        uint nGroups = ReadUInt32BigEndian(reader);

        for (uint i = 0; i < nGroups; i += (uint)chunkSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            uint batchEnd = Math.Min(i + (uint)chunkSize, nGroups);
            for (uint j = i; j < batchEnd; j++)
            {
                uint startCharCode = ReadUInt32BigEndian(reader);
                uint endCharCode = ReadUInt32BigEndian(reader);
                uint glyphID = ReadUInt32BigEndian(reader);
                for (uint c = startCharCode; c <= endCharCode;)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    uint charBatchEnd = Math.Min(c + (uint)chunkSize, endCharCode + 1);
                    for (uint batchC = c; batchC < charBatchEnd; batchC++)
                    {
                        if (glyphID != 0)
                            result[batchC] = (ushort)glyphID;
                    }
                    c = charBatchEnd;
                    await Task.Delay(1).ConfigureAwait(false);
                }
            }
            await Task.Delay(1).ConfigureAwait(false);
        }
        return result;
    }

    public static async Task<Dictionary<uint, ushort>> ParseFormat14(BinaryReader reader, long offset, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await Task.Delay(1).ConfigureAwait(false);
        return new Dictionary<uint, ushort>();
    }
}