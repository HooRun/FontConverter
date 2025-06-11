using FontConverter.SharedLibrary.Models;
using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseGlyfTableHelper
{
    const int chunkSize = 500;
    public static async Task<FontGlyfTable> ParseGlyfTable(OpenTypeTableBinaryData tableBinaryData, List<uint> offsets, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        FontGlyfTable glyfTable = new()
        {
            Glyphs = new List<Glyph>(offsets.Count - 1)
        };

        using var ms = new MemoryStream(tableBinaryData.RawData);
        using var reader = new BinaryReader(ms);

        for (int i = 0; i < offsets.Count - 1; i += chunkSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int batchEnd = Math.Min(i + chunkSize, offsets.Count - 1);
            for (int j = i; j < batchEnd; j++)
            {
                uint offset = offsets[j];
                uint nextOffset = offsets[j + 1];

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
                    //glyph.Simple = await ParseSimpleGlyph(reader, numberOfContours);
                }
                else
                {
                    //glyph.Composite = await ParseCompositeGlyph(reader);
                }

                glyfTable.Glyphs.Add(glyph);
            }
            await Task.Delay(1, cancellationToken);
        }

        return glyfTable;
    }

    public static SimpleGlyph ParseSimpleGlyph(BinaryReader reader, short numberOfContours)
    {
        var glyph = new SimpleGlyph
        {
            EndPtsOfContours = new List<ushort>(numberOfContours)
        };

        for (int i = 0; i < numberOfContours; i++)
        {
            glyph.EndPtsOfContours.Add(ReadUInt16BigEndian(reader));
        }

        glyph.InstructionLength = ReadUInt16BigEndian(reader);
        glyph.Instructions = reader.ReadBytes(glyph.InstructionLength);

        int numPoints = glyph.EndPtsOfContours.Count > 0 ? glyph.EndPtsOfContours[^1] + 1 : 0;

        // Flags
        var flags = new List<SimpleGlyphFlags>(numPoints);
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

        glyph.Points = new List<GlyphPoint>(numPoints);
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

    public static List<short> DecodeCoordinates(BinaryReader reader, List<SimpleGlyphFlags> flags, bool isX)
    {
        var coords = new List<short>(flags.Count);
        short current = 0;

        for (int i = 0; i < flags.Count; i += chunkSize)
        {
            int batchEnd = Math.Min(i + chunkSize, flags.Count);
            for (int j = i; j < batchEnd; j++)
            {
                var flag = flags[j];
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

                current += delta;
                coords.Add(current);
            }
        }

        return coords;
    }

    public static CompositeGlyph ParseCompositeGlyph(BinaryReader reader)
    {
        var composite = new CompositeGlyph
        {
            Components = new List<Component>()
        };
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