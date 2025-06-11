using FontConverter.SharedLibrary.Models;
using System;
using System.Text;
using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseNameTableHelper
{
    private const int DefaultChunkSize = 200;

    public static async Task<FontNameTable> ParseNameTable(OpenTypeTableBinaryData tableBinaryData, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (tableBinaryData?.RawData == null || tableBinaryData.RawData.Length == 0)
        {
            return new FontNameTable();
        }

        FontNameTable nameTable = new();

        using var ms = new MemoryStream(tableBinaryData.RawData);
        using var reader = new BinaryReader(ms);

        if (reader.BaseStream.Length < 6)
        {
            return nameTable;
        }

        ushort format = ReadUInt16BigEndian(reader);
        ushort count = ReadUInt16BigEndian(reader);
        ushort stringOffset = ReadUInt16BigEndian(reader);

        if (stringOffset >= tableBinaryData.RawData.Length)
        {
            return nameTable;
        }
        if (count * 12 > tableBinaryData.RawData.Length - 6)
        {
            return nameTable;
        }

        var langTags = new List<string>();

        long recordStart = reader.BaseStream.Position;
        if (format == 1)
        {
            if (reader.BaseStream.Position + 2 > reader.BaseStream.Length)
            {
                return nameTable;
            }
            reader.BaseStream.Position = recordStart + count * 12;
            ushort langTagCount = ReadUInt16BigEndian(reader);
            langTags = new List<string>(langTagCount);
            for (int i = 0; i < langTagCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (reader.BaseStream.Position + 4 > reader.BaseStream.Length)
                {
                    break;
                }
                ushort length = ReadUInt16BigEndian(reader);
                ushort offset = ReadUInt16BigEndian(reader);

                if (offset >= 0 && offset + length <= tableBinaryData.RawData.Length - stringOffset && length > 0)
                {
                    long pos = reader.BaseStream.Position;
                    try
                    {
                        reader.BaseStream.Position = stringOffset + offset;
                        if (reader.BaseStream.Position + length > reader.BaseStream.Length)
                        {
                            continue;
                        }
                        var bytes = reader.ReadBytes(length);
                        langTags.Add(Encoding.ASCII.GetString(bytes));
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        reader.BaseStream.Position = pos;
                    }
                }

            }
        }

        int chunkSize = Math.Min(DefaultChunkSize, count > 0 ? count : 1);

        reader.BaseStream.Position = recordStart;
        for (int i = 0; i < count; i += chunkSize)
        {
            cancellationToken.ThrowIfCancellationRequested();
            int batchEnd = Math.Min(i + chunkSize, count);
            for (int j = i; j < batchEnd; j++)
            {
                if (reader.BaseStream.Position + 12 > reader.BaseStream.Length)
                {
                    break;
                }

                ushort platformID = ReadUInt16BigEndian(reader);
                ushort encodingID = ReadUInt16BigEndian(reader);
                ushort languageID = ReadUInt16BigEndian(reader);
                ushort nameID = ReadUInt16BigEndian(reader);
                ushort length = ReadUInt16BigEndian(reader);
                ushort offset = ReadUInt16BigEndian(reader);

                if (platformID > 3 || length == 0 || length >= 256 || offset < 0 || stringOffset + offset + length > tableBinaryData.RawData.Length)
                {
                    continue;
                }

                try
                {
                    long currentPos = reader.BaseStream.Position;
                    reader.BaseStream.Position = stringOffset + offset;

                    if (reader.BaseStream.Position + length > reader.BaseStream.Length)
                    {
                        reader.BaseStream.Position = currentPos;
                        continue;
                    }

                    var stringBytes = reader.ReadBytes(length);

                    string? name = platformID switch
                    {
                        0 => Encoding.BigEndianUnicode.GetString(stringBytes),
                        1 => Encoding.ASCII.GetString(stringBytes),
                        3 when encodingID is 1 or 10 => Encoding.BigEndianUnicode.GetString(stringBytes),
                        3 => Encoding.UTF8.GetString(stringBytes),
                        _ => null
                    };

                    reader.BaseStream.Position = currentPos;

                    if (string.IsNullOrWhiteSpace(name) || !Enum.IsDefined(typeof(NameType), nameID))
                    {
                        continue;
                    }

                    var nameType = (NameType)nameID;

                    if (format == 1 && languageID >= 0x8000)
                    {
                        int langIndex = languageID - 0x8000;
                        if (langIndex >= 0 && langIndex < langTags.Count)
                        {
                            name += $" ({langTags[langIndex]})";
                        }
                    }

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
                catch (Exception)
                {
                    throw;
                }
            }
            await Task.Delay(1, cancellationToken);
        }

        return nameTable;
    }
}