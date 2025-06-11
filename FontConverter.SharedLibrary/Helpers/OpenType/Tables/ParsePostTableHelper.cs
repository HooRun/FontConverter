using FontConverter.SharedLibrary.Models;
using System.Text;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParsePostTableHelper
{
    const int chunkSize = 500;
    public static async Task<FontPostTable> ParsePostTable(OpenTypeTableBinaryData tableBinaryData, int numGlyphs, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        FontPostTable postTable = new()
        {
            GlyphNameIndex = new List<ushort>(numGlyphs),
            PascalStrings = new List<string>(),
            GlyphOffsets = new List<float>(numGlyphs)
        };

        using var ms = new MemoryStream(tableBinaryData.RawData);
        using var reader = new BinaryReader(ms);

        ushort majorVersion = ReadUInt16BigEndian(reader);
        ushort minorVersion = ReadUInt16BigEndian(reader);
        postTable.Version = FixedToDouble((uint)majorVersion << 16 | minorVersion);

        postTable.ItalicAngle = FixedToDouble(ReadUInt32BigEndian(reader));
        postTable.UnderlinePosition = ReadInt16BigEndian(reader);
        postTable.UnderlineThickness = ReadInt16BigEndian(reader);
        postTable.IsFixedPitch = ReadUInt32BigEndian(reader);
        postTable.MinMemType42 = ReadUInt32BigEndian(reader);
        postTable.MaxMemType42 = ReadUInt32BigEndian(reader);
        postTable.MinMemType1 = ReadUInt32BigEndian(reader);
        postTable.MaxMemType1 = ReadUInt32BigEndian(reader);

        if (postTable.Version == 2.0f)
        {
            ushort numGlyphsInTable = ReadUInt16BigEndian(reader);
            for (int i = 0; i < numGlyphsInTable; i += chunkSize)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int batchEnd = Math.Min(i + chunkSize, numGlyphsInTable);
                for (int j = i; j < batchEnd; j++)
                {
                    postTable.GlyphNameIndex.Add(ReadUInt16BigEndian(reader));
                }
                await Task.Delay(1, cancellationToken);
            }

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                cancellationToken.ThrowIfCancellationRequested();

                byte length = reader.ReadByte();

                if (reader.BaseStream.Position + length > reader.BaseStream.Length)
                    throw new InvalidDataException($"Unexpected end of data while reading Pascal string. Position={reader.BaseStream.Position}, Length={length}");

                byte[] strBytes = reader.ReadBytes(length);
                postTable.PascalStrings.Add(Encoding.ASCII.GetString(strBytes));

                if (postTable.PascalStrings.Count % chunkSize == 0)
                    await Task.Delay(1, cancellationToken);
            }
        }
        else if (postTable.Version == 2.5f)
        {

            for (int i = 0; i < numGlyphs; i += chunkSize)
            {
                cancellationToken.ThrowIfCancellationRequested();
                int batchEnd = Math.Min(i + chunkSize, numGlyphs);
                for (int j = i; j < batchEnd; j++)
                {
                    sbyte offset = reader.ReadSByte();
                    postTable.GlyphOffsets.Add(offset);
                }
                await Task.Delay(1, cancellationToken);
            }
        }

        return postTable;
    }
}