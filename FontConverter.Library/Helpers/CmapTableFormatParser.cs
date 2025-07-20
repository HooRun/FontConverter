using System.Collections.Generic;
using System.IO;
using static LVGLFontConverter.Library.Helpers.FontTableValueConverter;

namespace LVGLFontConverter.Library.Helpers;

public static class CmapTableFormatParser
{
    internal static Dictionary<uint, ushort> ParseFormat0(BinaryReader reader, long offset)
    {
        var result = new Dictionary<uint, ushort>();
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

        return result;
    }

    internal static Dictionary<uint, ushort> ParseFormat4(BinaryReader reader, long offset)
    {
        var result = new Dictionary<uint, ushort>();
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        ushort format = ReadUInt16BigEndian(reader);
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
            for (uint c = startCode[i]; c <= endCode[i]; c++)
            {
                if (idRangeOffset[i] == 0)
                {
                    ushort glyphIndex = (ushort)((c + idDelta[i]) % 65536);
                    if (glyphIndex != 0)
                        result[c] = glyphIndex;
                }
                else
                {
                    long glyphIndexOffset = idRangeOffsetStart + (2 * i) + idRangeOffset[i] + 2 * (c - startCode[i]);
                    if (glyphIndexOffset + 2 <= reader.BaseStream.Length)
                    {
                        reader.BaseStream.Seek(glyphIndexOffset, SeekOrigin.Begin);
                        ushort glyphIndex = ReadUInt16BigEndian(reader);
                        if (glyphIndex != 0)
                        {
                            glyphIndex = (ushort)((glyphIndex + idDelta[i]) % 65536);
                            result[c] = glyphIndex;
                        }
                    }
                }
            }
        }
        return result;
    }

    internal static Dictionary<uint, ushort> ParseFormat6(BinaryReader reader, long offset)
    {
        var result = new Dictionary<uint, ushort>();
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        ReadUInt16BigEndian(reader); // format
        ReadUInt16BigEndian(reader); // length
        ReadUInt16BigEndian(reader); // language
        ushort firstCode = ReadUInt16BigEndian(reader);
        ushort entryCount = ReadUInt16BigEndian(reader);
        for (uint i = 0; i < entryCount; i++)
        {
            ushort glyphId = ReadUInt16BigEndian(reader);
            if (glyphId != 0)
                result[firstCode + i] = glyphId;
        }
        return result;
    }

    internal static Dictionary<uint, ushort> ParseFormat10(BinaryReader reader, long offset)
    {
        var result = new Dictionary<uint, ushort>();
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        ReadUInt16BigEndian(reader); // format
        ReadUInt16BigEndian(reader); // reserved
        ReadUInt32BigEndian(reader); // length
        ReadUInt32BigEndian(reader); // language
        uint startCharCode = ReadUInt32BigEndian(reader);
        uint numChars = ReadUInt32BigEndian(reader);
        for (uint i = 0; i < numChars; i++)
        {
            ushort glyphId = ReadUInt16BigEndian(reader);
            if (glyphId != 0)
                result[startCharCode + i] = glyphId;
        }
        return result;
    }

    internal static Dictionary<uint, ushort> ParseFormat12(BinaryReader reader, long offset)
    {
        var result = new Dictionary<uint, ushort>();
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        ReadUInt16BigEndian(reader); // format
        ReadUInt16BigEndian(reader); // reserved
        ReadUInt32BigEndian(reader); // length
        ReadUInt32BigEndian(reader); // language
        uint nGroups = ReadUInt32BigEndian(reader);
        for (uint i = 0; i < nGroups; i++)
        {
            uint startCharCode = ReadUInt32BigEndian(reader);
            uint endCharCode = ReadUInt32BigEndian(reader);
            uint startGlyphID = ReadUInt32BigEndian(reader);
            for (uint c = startCharCode; c <= endCharCode; c++)
            {
                result[c] = (ushort)(startGlyphID + (c - startCharCode));
            }
        }
        return result;
    }

    internal static Dictionary<uint, ushort> ParseFormat13(BinaryReader reader, long offset)
    {
        var result = new Dictionary<uint, ushort>();
        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        ReadUInt16BigEndian(reader); // format
        ReadUInt16BigEndian(reader); // reserved
        ReadUInt32BigEndian(reader); // length
        ReadUInt32BigEndian(reader); // language
        uint nGroups = ReadUInt32BigEndian(reader);
        for (uint i = 0; i < nGroups; i++)
        {
            uint startCharCode = ReadUInt32BigEndian(reader);
            uint endCharCode = ReadUInt32BigEndian(reader);
            uint glyphID = ReadUInt32BigEndian(reader);
            for (uint c = startCharCode; c <= endCharCode; c++)
            {
                if (glyphID != 0)
                    result[c] = (ushort)glyphID;
            }
        }
        return result;
    }

    internal static Dictionary<uint, ushort> ParseFormat14(BinaryReader reader, long offset)
    {
        return new();
    }
}
