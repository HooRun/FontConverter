using FontConverter.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Helpers;

public static class ParseCFFTableHelper
{

    // https://adobe-type-tools.github.io/font-tech-notes/pdfs/5176.CFF.pdf
    // Appendix A: Standard Strings (pages 31 - 35)
    internal const int StandardStringsCount = 390;
    internal static readonly string[] StandardStrings = [
            ".notdef",
            "space",
            "exclam",
            "quotedbl",
            "numbersign",
            "dollar",
            "percent",
            "ampersand",
            "quoteright",
            "parenleft",
            "parenright",
            "asterisk",
            "plus",
            "comma",
            "hyphen",
            "period",
            "slash",
            "zero",
            "one",
            "two",
            "three",
            "four",
            "five",
            "six",
            "seven",
            "eight",
            "nine",
            "colon",
            "semicolon",
            "less",
            "equal",
            "greater",
            "question",
            "at",
            "A",
            "B",
            "C",
            "D",
            "E",
            "F",
            "G",
            "H",
            "I",
            "J",
            "K",
            "L",
            "M",
            "N",
            "O",
            "P",
            "Q",
            "R",
            "S",
            "T",
            "U",
            "V",
            "W",
            "X",
            "Y",
            "Z",
            "bracketleft",
            "backslash",
            "bracketright",
            "asciicircum",
            "underscore",
            "quoteleft",
            "a",
            "b",
            "c",
            "d",
            "e",
            "f",
            "g",
            "h",
            "i",
            "j",
            "k",
            "l",
            "m",
            "n",
            "o",
            "p",
            "q",
            "r",
            "s",
            "t",
            "u",
            "v",
            "w",
            "x",
            "y",
            "z",
            "braceleft",
            "bar",
            "braceright",
            "asciitilde",
            "exclamdown",
            "cent",
            "sterling",
            "fraction",
            "yen",
            "florin",
            "section",
            "currency",
            "quotesingle",
            "quotedblleft",
            "guillemotleft",
            "guilsinglleft",
            "guilsinglright",
            "fi",
            "fl",
            "endash",
            "dagger",
            "daggerdbl",
            "periodcentered",
            "paragraph",
            "bullet",
            "quotesinglbase",
            "quotedblbase",
            "quotedblright",
            "guillemotright",
            "ellipsis",
            "perthousand",
            "questiondown",
            "grave",
            "acute",
            "circumflex",
            "tilde",
            "macron",
            "breve",
            "dotaccent",
            "dieresis",
            "ring",
            "cedilla",
            "hungarumlaut",
            "ogonek",
            "caron",
            "emdash",
            "AE",
            "ordfeminine",
            "Lslash",
            "Oslash",
            "OE",
            "ordmasculine",
            "ae",
            "dotlessi",
            "lslash",
            "oslash",
            "oe",
            "germandbls",
            "onesuperior",
            "logicalnot",
            "mu",
            "trademark",
            "Eth",
            "onehalf",
            "plusminus",
            "Thorn",
            "onequarter",
            "divide",
            "brokenbar",
            "degree",
            "thorn",
            "threequarters",
            "twosuperior",
            "registered",
            "minus",
            "eth",
            "multiply",
            "threesuperior",
            "copyright",
            "Aacute",
            "Acircumflex",
            "Adieresis",
            "Agrave",
            "Aring",
            "Atilde",
            "Ccedilla",
            "Eacute",
            "Ecircumflex",
            "Edieresis",
            "Egrave",
            "Iacute",
            "Icircumflex",
            "Idieresis",
            "Igrave",
            "Ntilde",
            "Oacute",
            "Ocircumflex",
            "Odieresis",
            "Ograve",
            "Otilde",
            "Scaron",
            "Uacute",
            "Ucircumflex",
            "Udieresis",
            "Ugrave",
            "Yacute",
            "Ydieresis",
            "Zcaron",
            "aacute",
            "acircumflex",
            "adieresis",
            "agrave",
            "aring",
            "atilde",
            "ccedilla",
            "eacute",
            "ecircumflex",
            "edieresis",
            "egrave",
            "iacute",
            "icircumflex",
            "idieresis",
            "igrave",
            "ntilde",
            "oacute",
            "ocircumflex",
            "odieresis",
            "ograve",
            "otilde",
            "scaron",
            "uacute",
            "ucircumflex",
            "udieresis",
            "ugrave",
            "yacute",
            "ydieresis",
            "zcaron",
            "exclamsmall",
            "Hungarumlautsmall",
            "dollaroldstyle",
            "dollarsuperior",
            "ampersandsmall",
            "Acutesmall",
            "parenleftsuperior",
            "parenrightsuperior",
            "twodotenleader",
            "onedotenleader",
            "zerooldstyle",
            "oneoldstyle",
            "twooldstyle",
            "threeoldstyle",
            "fouroldstyle",
            "fiveoldstyle",
            "sixoldstyle",
            "sevenoldstyle",
            "eightoldstyle",
            "nineoldstyle",
            "commasuperior",
            "threequartersemdash",
            "periodsuperior",
            "questionsmall",
            "asuperior",
            "bsuperior",
            "centsuperior",
            "dsuperior",
            "esuperior",
            "isuperior",
            "lsuperior",
            "msuperior",
            "nsuperior",
            "osuperior",
            "rsuperior",
            "ssuperior",
            "tsuperior",
            "ff",
            "ffi",
            "ffl",
            "parenleftinferior",
            "parenrightinferior",
            "Circumflexsmall",
            "hyphensuperior",
            "Gravesmall",
            "Asmall",
            "Bsmall",
            "Csmall",
            "Dsmall",
            "Esmall",
            "Fsmall",
            "Gsmall",
            "Hsmall",
            "Ismall",
            "Jsmall",
            "Ksmall",
            "Lsmall",
            "Msmall",
            "Nsmall",
            "Osmall",
            "Psmall",
            "Qsmall",
            "Rsmall",
            "Ssmall",
            "Tsmall",
            "Usmall",
            "Vsmall",
            "Wsmall",
            "Xsmall",
            "Ysmall",
            "Zsmall",
            "colonmonetary",
            "onefitted",
            "rupiah",
            "Tildesmall",
            "exclamdownsmall",
            "centoldstyle",
            "Lslashsmall",
            "Scaronsmall",
            "Zcaronsmall",
            "Dieresissmall",
            "Brevesmall",
            "Caronsmall",
            "Dotaccentsmall",
            "Macronsmall",
            "figuredash",
            "hypheninferior",
            "Ogoneksmall",
            "Ringsmall",
            "Cedillasmall",
            "questiondownsmall",
            "oneeighth",
            "threeeighths",
            "fiveeighths",
            "seveneighths",
            "onethird",
            "twothirds",
            "zerosuperior",
            "foursuperior",
            "fivesuperior",
            "sixsuperior",
            "sevensuperior",
            "eightsuperior",
            "ninesuperior",
            "zeroinferior",
            "oneinferior",
            "twoinferior",
            "threeinferior",
            "fourinferior",
            "fiveinferior",
            "sixinferior",
            "seveninferior",
            "eightinferior",
            "nineinferior",
            "centinferior",
            "dollarinferior",
            "periodinferior",
            "commainferior",
            "Agravesmall",
            "Aacutesmall",
            "Acircumflexsmall",
            "Atildesmall",
            "Adieresissmall",
            "Aringsmall",
            "AEsmall",
            "Ccedillasmall",
            "Egravesmall",
            "Eacutesmall",
            "Ecircumflexsmall",
            "Edieresissmall",
            "Igravesmall",
            "Iacutesmall",
            "Icircumflexsmall",
            "Idieresissmall",
            "Ethsmall",
            "Ntildesmall",
            "Ogravesmall",
            "Oacutesmall",
            "Ocircumflexsmall",
            "Otildesmall",
            "Odieresissmall",
            "OEsmall",
            "Oslashsmall",
            "Ugravesmall",
            "Uacutesmall",
            "Ucircumflexsmall",
            "Udieresissmall",
            "Yacutesmall",
            "Thornsmall",
            "Ydieresissmall",
            "001.000",
            "001.001",
            "001.002",
            "001.003",
            "Black",
            "Bold",
            "Book",
            "Light",
            "Medium",
            "Regular",
            "Roman",
            "Semibold"  ];

    public static async Task<List<string>> ParseCFFTable(OpenTypeTableBinaryData tableBinaryData, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var ms = new MemoryStream(tableBinaryData.RawData);
        using var reader = new BinaryReader(ms);

        // 1. Header
        byte major = reader.ReadByte();
        byte minor = reader.ReadByte();
        byte headerSize = reader.ReadByte();
        byte offSize = reader.ReadByte();
        ms.Seek(headerSize, SeekOrigin.Begin);

        // 2. Name INDEX
        var nameIndex = ReadIndex(reader);

        // 3. Top DICT INDEX
        var topDict = ReadIndex(reader).FirstOrDefault();
        var (charStringsOffset, charsetOffset) = ParseTopDict(topDict);

        // 4. String INDEX
        var stringIndex = ReadIndex(reader).Select(b => Encoding.ASCII.GetString(b)).ToList();

        // 5. Global Subrs INDEX - skip
        _ = ReadIndex(reader);

        // 6. CharStrings INDEX
        ms.Seek(charStringsOffset, SeekOrigin.Begin);
        int glyphCount = ReadIndex(reader).Count;

        // 7. Charset
        ms.Seek(charsetOffset, SeekOrigin.Begin);
        ushort[] sids = ReadCharset(reader, glyphCount);

        // 8. Names
        List<string> glyphNames = [];
        foreach (ushort sid in sids)
        {
            if (sid < StandardStrings.Length)
                glyphNames.Add(StandardStrings[sid]);
            else
            {
                int customIndex = sid - StandardStrings.Length;
                glyphNames.Add(customIndex < stringIndex.Count
                    ? stringIndex[customIndex]
                    : $"Glyph_{sid}");
            }
        }

        return glyphNames;
    }

    private static List<byte[]> ReadIndex(BinaryReader reader)
    {
        ushort count = ReadUInt16(reader);
        if (count == 0) return new();

        byte offSize = reader.ReadByte();
        uint[] offsets = new uint[count + 1];
        for (int i = 0; i <= count; i++)
            offsets[i] = ReadOffset(reader, offSize);

        long basePos = reader.BaseStream.Position;
        var result = new List<byte[]>();

        for (int i = 0; i < count; i++)
        {
            int length = (int)(offsets[i + 1] - offsets[i]);
            reader.BaseStream.Seek(basePos + offsets[i] - 1, SeekOrigin.Begin);
            result.Add(reader.ReadBytes(length));
        }

        reader.BaseStream.Seek(basePos + offsets[^1] - 1, SeekOrigin.Begin);
        return result;
    }

    private static (int charStringsOffset, int charsetOffset) ParseTopDict(byte[] data)
    {
        using var reader = new BinaryReader(new MemoryStream(data));
        int charStringsOffset = 0;
        int charsetOffset = 0;

        while (reader.BaseStream.Position < reader.BaseStream.Length)
        {
            object operand = ReadOperand(reader);
            if (reader.BaseStream.Position >= reader.BaseStream.Length)
                break;

            byte op = reader.ReadByte();
            if (op == 0x0F) // CharStrings offset operator
                charStringsOffset = Convert.ToInt32(operand);
            else if (op == 0x0E) // Charset offset
                charsetOffset = Convert.ToInt32(operand);
            else if (op == 0x15) // Charset
                charsetOffset = Convert.ToInt32(operand);
        }

        return (charStringsOffset, charsetOffset);
    }

    private static ushort[] ReadCharset(BinaryReader reader, int glyphCount)
    {
        var sids = new List<ushort>();
        if (glyphCount <= 1) return sids.ToArray();

        byte format = reader.ReadByte();
        int count = 1;

        if (format == 0)
        {
            while (count++ < glyphCount)
                sids.Add(ReadUInt16(reader));
        }
        else if (format == 1)
        {
            while (count < glyphCount)
            {
                ushort first = ReadUInt16(reader);
                byte nLeft = reader.ReadByte();
                for (int i = 0; i <= nLeft; i++)
                    sids.Add((ushort)(first + i));
                count += nLeft + 1;
            }
        }
        else if (format == 2)
        {
            while (count < glyphCount)
            {
                ushort first = ReadUInt16(reader);
                ushort nLeft = ReadUInt16(reader);
                for (int i = 0; i <= nLeft; i++)
                    sids.Add((ushort)(first + i));
                count += nLeft + 1;
            }
        }

        return sids.ToArray();
    }

    private static object ReadOperand(BinaryReader reader)
    {
        byte b0 = reader.ReadByte();
        if (b0 >= 32 && b0 <= 246)
            return b0 - 139;
        if (b0 >= 247 && b0 <= 250)
            return ((b0 - 247) * 256) + reader.ReadByte() + 108;
        if (b0 >= 251 && b0 <= 254)
            return -((b0 - 251) * 256) - reader.ReadByte() - 108;
        if (b0 == 28)
            return (short)((reader.ReadByte() << 8) | reader.ReadByte());
        return 0;
    }

    private static ushort ReadUInt16(BinaryReader reader)
    {
        return (ushort)((reader.ReadByte() << 8) | reader.ReadByte());
    }

    private static uint ReadOffset(BinaryReader reader, int size)
    {
        uint val = 0;
        for (int i = 0; i < size; i++)
            val = (val << 8) | reader.ReadByte();
        return val;
    }
}
