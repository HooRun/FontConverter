using System.IO;
using System.Linq;
using LVGLFontConverter.Library.Models;
using static LVGLFontConverter.Library.Helpers.FontTableValueConverter;

namespace LVGLFontConverter.Library.Helpers;

public static class KernTableDataParser
{
    public static KernFormat0Subtable KernParseFormat0(BinaryReader reader, long subtableOffset)
    {
        var subtable = new KernFormat0Subtable();
        ushort nPairs = ReadUInt16BigEndian(reader);
        ushort searchRange = ReadUInt16BigEndian(reader); // ignored
        ushort entrySelector = ReadUInt16BigEndian(reader); // ignored
        ushort rangeShift = ReadUInt16BigEndian(reader); // ignored

        for (int i = 0; i < nPairs; i++)
        {
            ushort left = ReadUInt16BigEndian(reader);
            ushort right = ReadUInt16BigEndian(reader);
            short value = ReadInt16BigEndian(reader);
            subtable.Pairs.Add(new KernPair { Left = left, Right = right, Value = value });
        }

        return subtable;
    }

    public static KernFormat2Subtable KernParseFormat2(BinaryReader reader, long subtableOffset)
    {
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
        for (int i = 0; i < subtable.NumLeftClasses; i++)
        {
            for (int j = 0; j < subtable.NumRightClasses; j++)
            {
                subtable.KerningValues[i, j] = ReadUInt16BigEndian(reader);
            }
        }

        return subtable;
    }
}
