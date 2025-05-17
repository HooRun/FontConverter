using FontConverter.SharedLibrary.Models;
using System.Text;
using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseNameTableHelper
{
    public static FontNameTable ParseNameTable(OpenTypeTableBinaryData tableBinaryData)
    {
        FontNameTable nameTable = new();
        try
        {
            using var ms = new MemoryStream(tableBinaryData.RawData);
            using var reader = new BinaryReader(ms);

            ushort format = ReadUInt16BigEndian(reader);
            ushort count = ReadUInt16BigEndian(reader);
            ushort stringOffset = ReadUInt16BigEndian(reader);

            var langTags = new List<string>();

            // Read langTags for format 1 (Optional)
            long recordStart = reader.BaseStream.Position;
            if (format == 1)
            {
                reader.BaseStream.Position = recordStart + count * 12;
                ushort langTagCount = ReadUInt16BigEndian(reader);
                for (int i = 0; i < langTagCount; i++)
                {
                    ushort length = ReadUInt16BigEndian(reader);
                    ushort offset = ReadUInt16BigEndian(reader);
                    if (offset + length <= tableBinaryData.RawData.Length - stringOffset)
                    {
                        long pos = reader.BaseStream.Position;
                        reader.BaseStream.Position = stringOffset + offset;
                        var bytes = reader.ReadBytes(length);
                        langTags.Add(System.Text.Encoding.ASCII.GetString(bytes));
                        reader.BaseStream.Position = pos;
                    }
                }
            }

            // Read name records
            reader.BaseStream.Position = recordStart;
            for (int i = 0; i < count; i++)
            {
                ushort platformID = ReadUInt16BigEndian(reader);
                ushort encodingID = ReadUInt16BigEndian(reader);
                ushort languageID = ReadUInt16BigEndian(reader);
                ushort nameID = ReadUInt16BigEndian(reader);
                ushort length = ReadUInt16BigEndian(reader);
                ushort offset = ReadUInt16BigEndian(reader);

                if (platformID > 3 || length == 0 || length >= 256 || offset + length > tableBinaryData.RawData.Length - stringOffset)
                    continue;

                try
                {
                    reader.BaseStream.Position = stringOffset + offset;
                    var stringBytes = reader.ReadBytes(length);

                    string? name = platformID switch
                    {
                        0 => Encoding.BigEndianUnicode.GetString(stringBytes),
                        1 => Encoding.ASCII.GetString(stringBytes),
                        3 when encodingID == 1 || encodingID == 10 => Encoding.BigEndianUnicode.GetString(stringBytes),
                        3 => Encoding.UTF8.GetString(stringBytes),
                        _ => null
                    };

                    if (string.IsNullOrWhiteSpace(name)) continue;
                    if (!Enum.IsDefined(typeof(NameType), nameID)) continue;

                    var nameType = (NameType)nameID;

                    // Append lang tag if format 1 and languageID >= 0x8000
                    if (format == 1 && languageID >= 0x8000)
                    {
                        int langIndex = languageID - 0x8000;
                        if (langIndex < langTags.Count)
                            name += $" ({langTags[langIndex]})";
                    }

                    // Fill appropriate field
                    switch (nameType)
                    {
                        case NameType.Copyright: nameTable.Copyright = name; break;
                        case NameType.FontFamily: nameTable.FontFamily = name; break;
                        case NameType.FontSubfamily: nameTable.FontSubfamily = name; break;
                        case NameType.UniqueIdentifier: nameTable.UniqueIdentifier = name; break;
                        case NameType.FullFontName: nameTable.FullFontName = name; break;
                        case NameType.Version: nameTable.Version = name; break;
                        case NameType.PostScriptName: nameTable.PostScriptName = name; break;
                        case NameType.Trademark: nameTable.Trademark = name; break;
                        case NameType.Manufacturer: nameTable.Manufacturer = name; break;
                        case NameType.Designer: nameTable.Designer = name; break;
                        case NameType.Description: nameTable.Description = name; break;
                        case NameType.VendorURL: nameTable.VendorURL = name; break;
                        case NameType.DesignerURL: nameTable.DesignerURL = name; break;
                        case NameType.License: nameTable.License = name; break;
                        case NameType.LicenseURL: nameTable.LicenseURL = name; break;
                        case NameType.PreferredFamily: nameTable.PreferredFamily = name; break;
                        case NameType.PreferredSubfamily: nameTable.PreferredSubfamily = name; break;
                        case NameType.CompatibleFull: nameTable.CompatibleFull = name; break;
                        case NameType.SampleText: nameTable.SampleText = name; break;
                        case NameType.PostScriptCID: nameTable.PostScriptCID = name; break;
                        case NameType.WWSFamily: nameTable.WWSFamily = name; break;
                        case NameType.WWSSubfamily: nameTable.WWSSubfamily = name; break;
                        case NameType.LightBackgroundPalette: nameTable.LightBackgroundPalette = name; break;
                        case NameType.DarkBackgroundPalette: nameTable.DarkBackgroundPalette = name; break;
                        case NameType.VariationsPostScriptPrefix: nameTable.VariationsPostScriptPrefix = name; break;
                    }
                }
                catch { continue; }
            }
        }
        catch { }
        return nameTable;
    }
}
