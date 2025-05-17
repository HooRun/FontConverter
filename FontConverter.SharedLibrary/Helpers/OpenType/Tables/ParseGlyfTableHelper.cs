using FontConverter.SharedLibrary.Models;
using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseGlyfTableHelper
{
    public static FontGlyfTable ParseGlyfTable(OpenTypeTableBinaryData tableBinaryData, List<uint> offsets)
    {
        FontGlyfTable glyfTable = new();

        using var ms = new MemoryStream(tableBinaryData.RawData);
        using var reader = new BinaryReader(ms);

        for (int i = 0; i < offsets.Count - 1; i++)
        {
            uint offset = offsets[i];
            uint nextOffset = offsets[i + 1];

            if (offset == nextOffset)
            {
                glyfTable.Glyphs.Add(new Glyph());
                continue;
            }

            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            short numberOfContours = ReadInt16BigEndian(reader);
            short xMin = ReadInt16BigEndian(reader);
            short yMin = ReadInt16BigEndian(reader);
            short xMax = ReadInt16BigEndian(reader);
            short yMax = ReadInt16BigEndian(reader);

            var glyph = new Glyph
            {
                NumberOfContours = numberOfContours,
                XMin = xMin,
                YMin = yMin,
                XMax = xMax,
                YMax = yMax
            };

            if (numberOfContours >= 0)
            {
                //glyph.Simple = ParseSimpleGlyph(reader, numberOfContours);
            }
            else
            {
                //glyph.Composite = ParseCompositeGlyph(reader);
            }

            glyfTable.Glyphs.Add(glyph);
        }

        return glyfTable;
    }

    internal static SimpleGlyph ParseSimpleGlyph(BinaryReader reader, short numberOfContours)
    {
        var glyph = new SimpleGlyph();
        glyph.EndPtsOfContours = new List<ushort>();
        for (int i = 0; i < numberOfContours; i++)
        {
            glyph.EndPtsOfContours.Add(ReadUInt16BigEndian(reader));
        }

        glyph.InstructionLength = ReadUInt16BigEndian(reader);
        glyph.Instructions = reader.ReadBytes(glyph.InstructionLength);

        int numPoints = glyph.EndPtsOfContours.Count > 0 ? glyph.EndPtsOfContours[^1] + 1 : 0;

        // Flags
        var flags = new List<SimpleGlyphFlags>();
        for (int i = 0; i < numPoints;)
        {
            SimpleGlyphFlags flag = (SimpleGlyphFlags)reader.ReadByte();
            flags.Add(flag);
            i++;
            if (flag.HasFlag(SimpleGlyphFlags.REPEAT_FLAG))
            {
                byte repeatCount = reader.ReadByte();
                for (int j = 0; j < repeatCount; j++)
                {
                    flags.Add(flag);
                    i++;
                }
            }
        }

        // Coordinates
        var xCoordinates = DecodeCoordinates(reader, flags, true);
        var yCoordinates = DecodeCoordinates(reader, flags, false);

        glyph.Points = new List<GlyphPoint>();
        for (int i = 0; i < numPoints; i++)
        {
            glyph.Points.Add(new GlyphPoint
            {
                X = xCoordinates[i],
                Y = yCoordinates[i],
                OnCurve = flags[i].HasFlag(SimpleGlyphFlags.ON_CURVE_POINT)
            });
        }

        return glyph;
    }

    internal static List<short> DecodeCoordinates(BinaryReader reader, List<SimpleGlyphFlags> flags, bool isX)
    {
        var coords = new List<short>();
        short current = 0;
        for (int i = 0; i < flags.Count; i++)
        {
            var flag = flags[i];
            bool isShort = isX ? flag.HasFlag(SimpleGlyphFlags.X_SHORT_VECTOR) : flag.HasFlag(SimpleGlyphFlags.Y_SHORT_VECTOR);
            bool sameOrPositive = isX ? flag.HasFlag(SimpleGlyphFlags.X_IS_SAME_OR_POSITIVE_X_SHORT_VECTOR) : flag.HasFlag(SimpleGlyphFlags.Y_IS_SAME_OR_POSITIVE_Y_SHORT_VECTOR);

            short delta = 0;
            if (isShort)
            {
                byte b = reader.ReadByte();
                delta = (short)(sameOrPositive ? b : -b);
            }
            else if (!sameOrPositive)
            {
                delta = ReadInt16BigEndian(reader);
            }
            // else: same as previous (delta = 0)

            current += delta;
            coords.Add(current);
        }
        return coords;
    }

    internal static CompositeGlyph ParseCompositeGlyph(BinaryReader reader)
    {
        var composite = new CompositeGlyph();
        bool hasMore = true;

        while (hasMore)
        {
            var flags = (ComponentGlyphFlags)ReadUInt16BigEndian(reader);
            ushort glyphIndex = ReadUInt16BigEndian(reader);
            short arg1, arg2;
            if (flags.HasFlag(ComponentGlyphFlags.ARG_1_AND_2_ARE_WORDS))
            {
                arg1 = ReadInt16BigEndian(reader);
                arg2 = ReadInt16BigEndian(reader);
            }
            else
            {
                arg1 = reader.ReadSByte();
                arg2 = reader.ReadSByte();
            }

            var component = new Component
            {
                Flags = flags,
                GlyphIndex = glyphIndex,
                Argument1 = arg1,
                Argument2 = arg2
            };

            if (flags.HasFlag(ComponentGlyphFlags.WE_HAVE_A_SCALE))
            {
                component.Scale = ReadF2Dot14(reader);
            }
            else if (flags.HasFlag(ComponentGlyphFlags.WE_HAVE_AN_X_AND_Y_SCALE))
            {
                component.ScaleXY = (ReadF2Dot14(reader), ReadF2Dot14(reader));
            }
            else if (flags.HasFlag(ComponentGlyphFlags.WE_HAVE_A_TWO_BY_TWO))
            {
                component.Scale10 = ReadF2Dot14(reader);
                component.Scale01 = ReadF2Dot14(reader);
                component.ScaleXY = (ReadF2Dot14(reader), ReadF2Dot14(reader));
            }

            composite.Components.Add(component);
            hasMore = flags.HasFlag(ComponentGlyphFlags.MORE_COMPONENTS);
        }

        if (composite.Components[^1].Flags.HasFlag(ComponentGlyphFlags.WE_HAVE_INSTRUCTIONS))
        {
            composite.InstructionLength = ReadUInt16BigEndian(reader);
            composite.Instructions = reader.ReadBytes(composite.InstructionLength);
        }

        return composite;
    }
}
