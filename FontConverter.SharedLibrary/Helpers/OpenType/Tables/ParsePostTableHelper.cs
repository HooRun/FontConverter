using FontConverter.SharedLibrary.Models;
using System.Text;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;


namespace FontConverter.SharedLibrary.Helpers;

public static class ParsePostTableHelper
{
    public static FontPostTable ParsePostTable(OpenTypeTableBinaryData tableBinaryData, int numGlyphs)
    {
        FontPostTable postTable = new();

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

            for (int i = 0; i < numGlyphsInTable; i++)
                postTable.GlyphNameIndex.Add(ReadUInt16BigEndian(reader));


            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                byte length = reader.ReadByte();
                byte[] strBytes = reader.ReadBytes(length);
                postTable.PascalStrings.Add(Encoding.ASCII.GetString(strBytes));
            }
        }
        else if (postTable.Version == 2.5f)
        {
            for (int i = 0; i < numGlyphs; i++)
            {

                sbyte offset = reader.ReadSByte();
                postTable.GlyphOffsets.Add(offset);
            }
        }

        return postTable;
    }
}
