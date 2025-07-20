using FontConverter.SharedLibrary.Models;
using System.Text;
using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;
using static FontConverter.SharedLibrary.Helpers.FontTableValueConverterHelper;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseOS2TableHelper
{
    public static async Task<FontOS2Table> ParseOS2Table(OpenTypeTableBinaryData tableBinaryData, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        FontOS2Table os2Table = new()
        {
            FsType = new List<FsTypeFlags>(8),
            UnicodeRange1 = new List<UnicodeRange1>(32),
            UnicodeRange2 = new List<UnicodeRange2>(32),
            UnicodeRange3 = new List<UnicodeRange3>(32),
            UnicodeRange4 = new List<UnicodeRange4>(32),
            CodePageRange1 = new List<CodePageRange1>(32),
            CodePageRange2 = new List<CodePageRange2>(32)
        };

        using var ms = new MemoryStream(tableBinaryData.RawData);
        using var reader = new BinaryReader(ms);

        os2Table.Version = ReadUInt16BigEndian(reader);
        os2Table.XAvgCharWidth = ReadInt16BigEndian(reader);
        os2Table.UsWeightClass = ReadUInt16BigEndian(reader);
        os2Table.UsWidthClass = ReadUInt16BigEndian(reader);
        ushort fsType = ReadUInt16BigEndian(reader);
        foreach (FsTypeFlags flag in Enum.GetValues(typeof(FsTypeFlags)))
        {
            if ((flag & (FsTypeFlags)fsType) == flag)
            {
                os2Table.FsType.Add(flag);
            }
        }

        os2Table.YSubscriptXSize = ReadInt16BigEndian(reader);
        os2Table.YSubscriptYSize = ReadInt16BigEndian(reader);
        os2Table.YSubscriptXOffset = ReadInt16BigEndian(reader);
        os2Table.YSubscriptYOffset = ReadInt16BigEndian(reader);

        os2Table.YSuperscriptXSize = ReadInt16BigEndian(reader);
        os2Table.YSuperscriptYSize = ReadInt16BigEndian(reader);
        os2Table.YSuperscriptXOffset = ReadInt16BigEndian(reader);
        os2Table.YSuperscriptYOffset = ReadInt16BigEndian(reader);

        os2Table.YStrikeoutSize = ReadInt16BigEndian(reader);
        os2Table.YStrikeoutPosition = ReadInt16BigEndian(reader);
        os2Table.SFamilyClass = ReadInt16BigEndian(reader);

        os2Table.Panose.FamilyType = reader.ReadByte();
        os2Table.Panose.SerifStyle = reader.ReadByte();
        os2Table.Panose.Weight = reader.ReadByte();
        os2Table.Panose.Proportion = reader.ReadByte();
        os2Table.Panose.Contrast = reader.ReadByte();
        os2Table.Panose.StrokeVariation = reader.ReadByte();
        os2Table.Panose.ArmStyle = reader.ReadByte();
        os2Table.Panose.LetterForm = reader.ReadByte();
        os2Table.Panose.Midline = reader.ReadByte();
        os2Table.Panose.XHeight = reader.ReadByte();

        uint unicodeRange1 = ReadUInt32BigEndian(reader);
        foreach (UnicodeRange1 flag in Enum.GetValues(typeof(UnicodeRange1)))
        {
            if ((flag & (UnicodeRange1)unicodeRange1) == flag)
            {
                os2Table.UnicodeRange1.Add(flag);
            }
        }
        uint unicodeRange2 = ReadUInt32BigEndian(reader);
        foreach (UnicodeRange2 flag in Enum.GetValues(typeof(UnicodeRange2)))
        {
            if ((flag & (UnicodeRange2)unicodeRange2) == flag)
            {
                os2Table.UnicodeRange2.Add(flag);
            }
        }
        uint unicodeRange3 = ReadUInt32BigEndian(reader);
        foreach (UnicodeRange3 flag in Enum.GetValues(typeof(UnicodeRange3)))
        {
            if ((flag & (UnicodeRange3)unicodeRange3) == flag)
            {
                os2Table.UnicodeRange3.Add(flag);
            }
        }
        uint unicodeRange4 = ReadUInt32BigEndian(reader);
        foreach (UnicodeRange4 flag in Enum.GetValues(typeof(UnicodeRange4)))
        {
            if ((flag & (UnicodeRange4)unicodeRange4) == flag)
            {
                os2Table.UnicodeRange4.Add(flag);
            }
        }

        os2Table.AchVendID = Encoding.ASCII.GetString(reader.ReadBytes(4));

        os2Table.FsSelection = ReadUInt16BigEndian(reader);
        os2Table.UsFirstCharIndex = ReadUInt16BigEndian(reader);
        os2Table.UsLastCharIndex = ReadUInt16BigEndian(reader);

        os2Table.STypoAscender = ReadInt16BigEndian(reader);
        os2Table.STypoDescender = ReadInt16BigEndian(reader);
        os2Table.STypoLineGap = ReadInt16BigEndian(reader);

        os2Table.UsWinAscent = ReadUInt16BigEndian(reader);
        os2Table.UsWinDescent = ReadUInt16BigEndian(reader);

        if (os2Table.Version >= 1)
        {
            uint codePageRange1 = ReadUInt32BigEndian(reader);
            foreach (CodePageRange1 flag in Enum.GetValues(typeof(CodePageRange1)))
            {
                if ((flag & (CodePageRange1)codePageRange1) == flag)
                {
                    os2Table.CodePageRange1.Add(flag);
                }
            }
            uint codePageRange2 = ReadUInt32BigEndian(reader);
            foreach (CodePageRange2 flag in Enum.GetValues(typeof(CodePageRange2)))
            {
                if ((flag & (CodePageRange2)codePageRange2) == flag)
                {
                    os2Table.CodePageRange2.Add(flag);
                }
            }
        }

        if (os2Table.Version >= 2)
        {
            os2Table.SxHeight = ReadInt16BigEndian(reader);
            os2Table.SCapHeight = ReadInt16BigEndian(reader);
            os2Table.UsDefaultChar = ReadUInt16BigEndian(reader);
            os2Table.UsBreakChar = ReadUInt16BigEndian(reader);
            os2Table.UsMaxContext = ReadUInt16BigEndian(reader);
        }

        if (os2Table.Version >= 5)
        {
            os2Table.UsLowerOpticalPointSize = ReadUInt16BigEndian(reader);
            os2Table.UsUpperOpticalPointSize = ReadUInt16BigEndian(reader);
        }

        await Task.Delay(1, cancellationToken);
        return os2Table;
    }
}