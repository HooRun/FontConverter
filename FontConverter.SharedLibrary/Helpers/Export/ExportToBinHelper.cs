using FontConverter.SharedLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.SharedLibrary.Helpers;

public static class ExportToBinHelper
{

    public static async Task<string> ExportFontToLvglBinary(OpenTypeFont openTypeFont, LVGLFont lvglFont, IList<LVGLGlyph> glyphsToExport, SortedDictionary<uint, UnicodeBlock> blocks)
    {
        await Task.Yield();

        bool isMonospaced = glyphsToExport.All(g => g.Descriptor.AdvanceWidth == glyphsToExport[0].Descriptor.AdvanceWidth);

        var info = lvglFont.FontInformations;

        var glyphs = glyphsToExport;
        var glyphBitmaps = glyphs.Select(g => g.Bitmap).ToList();

        var cmaps = ExportCmapHelper.GenerateCMapRangesByUnicodeBlocks(glyphsToExport, blocks);

        // Kerning section
        LVGLKerningClassResult? kernResult = ExportKernHelper.CollectFormat3Data(glyphsToExport.ToList());
        bool haveKernings = kernResult != null && kernResult.ClassValues.Length > 0;
        int kerningScale = kernResult !=null ? kernResult.Scale : 0;

        // Glyph bitmap section
        var glyfBin = BuildGlyfSection(lvglFont, glyphsToExport, isMonospaced);

        byte indexToLocFormat = glyfBin.Length > ushort.MaxValue ? (byte)1 : (byte)0;


        // CMap section
        var cmapBin = BuildCmapSection(cmaps);
        
        // Head section
        var headBin = BuildHeadSection(openTypeFont, lvglFont, glyphsToExport, isMonospaced, indexToLocFormat, haveKernings, kerningScale);

        // Loca Section
        var locaBin = BuildLocaSection(glyphsToExport, indexToLocFormat == 1);

        // Kerning Section
        byte[]? kernBin = null;
        if (haveKernings && kernResult!=null)
        {
            var classValues = kernResult.ClassValues;
            if (classValues.Length > 0)
            {
                using var kStream = new MemoryStream();
                using var kWriter = new BinaryWriter(kStream);

                // Format 3: write class pair values, then mappings
                foreach (var v in classValues) kWriter.Write((sbyte)v);
                foreach (var l in kernResult.LeftClassMap) kWriter.Write(l);
                foreach (var r in kernResult.RightClassMap) kWriter.Write(r);

                kernBin = BuildKernSection(kStream.ToArray(), 3);
            }
        }

        // Final binary
        byte[] binFile = ExportToLvglBinary(headBin, cmapBin, locaBin, glyfBin, kernBin);
        return Convert.ToBase64String(binFile);
    }

    public static byte[] ExportToLvglBinary(
        byte[] headBin,
        byte[] cmapBin,
        byte[] locaBin,
        byte[] glyfBin,
        byte[]? kernBin)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        writer.Write(headBin);
        writer.Write(cmapBin);
        writer.Write(locaBin);
        writer.Write(glyfBin);
        if (kernBin != null && kernBin.Length > 0)
            writer.Write(kernBin);

        writer.Flush();
        return stream.ToArray();
    }

    // Head Section
    public static byte[] BuildHeadSection(OpenTypeFont openTypeFont, LVGLFont lvglFont, IList<LVGLGlyph> glyphsToExport, bool isMonospaced, byte indexToLocFormat, bool hasKernings, int kerningScale)
    {
        const uint HEAD_LENGTH = 48;
        var buf = new byte[HEAD_LENGTH];
        using var ms = new MemoryStream(buf);
        using var writer = new BinaryWriter(ms);

        var settings = lvglFont.FontSettings;
        var info = lvglFont.FontInformations;
        var contents = lvglFont.FontContents;

        var scale = lvglFont.FontSettings.FontSize / (double)openTypeFont.HeadTable.UnitsPerEm;

        ushort fontSize = (ushort)settings.FontSize;
        ushort ascent = (ushort)Parse(info.Ascent);
        short descent = (short)((short)Parse(info.Descent) * (short)-1);
        ushort typoAscent = (ushort)Math.Ceiling(scale * openTypeFont.OS2Table.STypoAscender);
        short typoDescent = (short)((short)Math.Floor(scale * openTypeFont.OS2Table.STypoAscender)*(short)-1);
        ushort typoLineGap = (ushort)Math.Floor(scale * openTypeFont.OS2Table.STypoLineGap);
        short yMin = (short)Parse(info.YMin);
        short yMax = (short)Parse(info.YMax);
        ushort defaultAdvanceWidth = isMonospaced ? (ushort)glyphsToExport[0].Descriptor.AdvanceWidth : (ushort)0;
        byte glyphIdFormat = glyphsToExport.Count > 255 ? (byte)1 : (byte)0;
        byte advanceWidthFormat = (byte)0; // 0 => int, 1 => FP4 12:4
        byte bpp = (byte)settings.FontBitPerPixel;

        int maxY = Math.Max(Math.Abs(Parse(info.YMin)), Math.Abs(Parse(info.YMax)));
        int maxX = Math.Max(Math.Abs(Parse(info.XMin)), Math.Abs(Parse(info.XMax)));
        byte xyBits = Math.Max(maxX, maxY) > byte.MaxValue ? (byte)16 : (byte)8;
        byte whBits = Math.Max(Parse(info.CharWidthMax), Parse(info.LineHeight)) > byte.MaxValue ? (byte)16 : (byte)8;
        byte adwBits = Parse(info.AdvanceWidthMax) > byte.MaxValue ? (byte)16 : (byte)8;

        writer.Seek(0, SeekOrigin.Begin);
        writer.Write((uint)HEAD_LENGTH);                        // Size of head table       4 bytes
        writer.Write(Encoding.ASCII.GetBytes("head"));          // Label                    4 bytes
        writer.Write((uint)1);                                  // Version                  4 bytes
        writer.Write((ushort)(hasKernings ? 4 : 3));            // Tables count             2 bytes
        writer.Write((ushort)fontSize);                         // Font size                2 bytes
        writer.Write((ushort)ascent);                           // Ascent                   2 bytes
        writer.Write((short)descent);                           // Descent                  2 bytes
        writer.Write((ushort)typoAscent);                       // Typo Ascent              2 bytes
        writer.Write((short)typoDescent);                       // Typo Descent             2 bytes
        writer.Write((ushort)typoLineGap);                      // Typo Line Gap            2 bytes
        writer.Write((short)yMin);                              // MIN Y                    2 bytes
        writer.Write((short)yMax);                              // MAX Y                    2 bytes
        writer.Write((ushort)defaultAdvanceWidth);              // Default Avance Width     2 bytes
        writer.Write((ushort)(kerningScale));                   // Kerning Scale            2 bytes
        writer.Write((byte)indexToLocFormat);                   // Font is large            1 bytes
        writer.Write((byte)glyphIdFormat);                      // Glyphs more than 255     1 byte
        writer.Write((byte)advanceWidthFormat);                 // Advance Width Format     1 byte
        writer.Write((byte)bpp);                                // Bits Per Pixel           1 byte
        writer.Write((byte)xyBits);                             // XY Bits                  1 byte
        writer.Write((byte)whBits);                             // Width Height Bits        1 byte
        writer.Write((byte)adwBits);                            // Advance Width Bits       1 byte
        writer.Write((byte)0);                                  // Compression Format       1 byte
        writer.Write((byte)SUB_Pixel_ENUM.SUB_PIXEL_NONE);      // Sub Pixels Mode          1 byte
        writer.Write((byte)0);                                  // Reserved (Padding)       1 byte
        writer.Write((short)Parse(info.UnderlinePosition));     // Underline Position       2 byte
        writer.Write((ushort)Parse(info.UnderlineThickness));   // Undeline Thickness       2 bytes

        return buf;
    }

    public static int BitsCount(uint value)
    {
        int bits = 0;
        while (value > 0)
        {
            bits++;
            value >>= 1;
        }
        return bits;
    }

    private static int Parse(string value)
    {
        return int.TryParse(value, out var result) ? result : 0;
    }


    // CMap Section
    public static byte[] BuildCmapSection(List<LVGLCMapRange> cmaps)
    {
        const int HEAD_LENGTH = 12;
        const int SUBHEADER_SIZE = 16;

        // Build Sub Tables Data
        using var subDataMS = new MemoryStream();
        using var subDataWriter = new BinaryWriter(subDataMS);
        foreach (var cmap in cmaps)
        {
            byte[] subTableData = BuildCmapSubData(cmap);
            cmap.Offset = subTableData.Length;
            subDataWriter.Write(subTableData);
        }
        var subDataBuf = subDataMS.ToArray();

        // Build Sub Tables Headers
        var subHeadersBuf = new byte[SUBHEADER_SIZE * cmaps.Count];
        using var subHeadersMS = new MemoryStream(subHeadersBuf);
        using var subHeadersWriter = new BinaryWriter(subHeadersMS);
        int offset = (HEAD_LENGTH + (cmaps.Count * SUBHEADER_SIZE));
        foreach (var cmap in cmaps)
        {
            subHeadersWriter.Write((uint)offset);               // Offset                           4 bytes
            subHeadersWriter.Write((uint)cmap.RangeStart);      // Range Start                      4 bytes
            subHeadersWriter.Write((ushort)cmap.RangeLength);   // Range Length                     2 bytes
            subHeadersWriter.Write((ushort)cmap.GlyphIDStart);  // Start Glyph ID                   2 bytes
            subHeadersWriter.Write((ushort)cmap.ListLength);    // List Length                      2 bytes
            subHeadersWriter.Write((byte)cmap.Type);            // CMap Type                        1 byte
            subHeadersWriter.Write((byte)0);                    // Padding                          1 byte
            offset += cmap.Offset;
        }

        // Build Main Header
        var headerBuf = new byte[HEAD_LENGTH];
        using var headerStream = new MemoryStream(headerBuf);
        using var headerWriter = new BinaryWriter(headerStream);

        int cmapTableLength = HEAD_LENGTH + subHeadersBuf.Length + subDataBuf.Length;
        headerWriter.Write((uint)cmapTableLength);              // CMap Table Length                4 bytes
        headerWriter.Write(Encoding.ASCII.GetBytes("cmap"));    // Label                            4 bytes
        headerWriter.Write((uint)cmaps.Count);                  // Sub Tables Count                 4 bytes

        using var mergedStream = new MemoryStream();
        mergedStream.Write(headerBuf, 0, headerBuf.Length);
        mergedStream.Write(subHeadersBuf, 0, subHeadersBuf.Length);
        mergedStream.Write(subDataBuf, 0, subDataBuf.Length);

        return mergedStream.ToArray();
    }

    private static byte[] BuildCmapSubData(LVGLCMapRange cmap)
    {
        using var ms = new MemoryStream();
        using var w = new BinaryWriter(ms);

        switch (cmap.Type)
        {
            case LVGL_CMAP_TYPE.LV_FONT_FMT_TXT_CMAP_FORMAT0_TINY:
                break; // No Data

            case LVGL_CMAP_TYPE.LV_FONT_FMT_TXT_CMAP_FORMAT0_FULL:
                foreach (var delta in cmap.GlyphIDOffsetList)
                {
                    w.Write((byte)delta);
                }
                break;

            case LVGL_CMAP_TYPE.LV_FONT_FMT_TXT_CMAP_SPARSE_TINY:
                foreach (var u in cmap.UnicodeList)
                    w.Write((ushort)u);
                break;

            case LVGL_CMAP_TYPE.LV_FONT_FMT_TXT_CMAP_SPARSE_FULL:
                foreach (var u in cmap.UnicodeList)
                    w.Write((ushort)u);
                foreach (var g in cmap.GlyphIDOffsetList)
                    w.Write((ushort)g);
                break;

            default:
                throw new Exception("Unknown cmap type");
        }

        // Align to 4 bytes
        int pad = (int)(4 - (ms.Length % 4)) % 4;
        if (pad > 0)
            w.Write(new byte[pad]);

        return ms.ToArray();
    }



    // Loca Section
    public static byte[] BuildLocaSection(IList<LVGLGlyph> glyphsToExport, bool use32bit = false)
    {
        const int HEAD_LENGTH = 12; // 4 + 4 + 4 per reference code

        byte[] offsetsBuffer = Array.Empty<byte>();

        if (use32bit)
        {
            // Write all offsets as UInt32 into a byte array
            using var locaOffsetsStream = new MemoryStream();
            using var locaOffsetsWriter = new BinaryWriter(locaOffsetsStream);
            foreach (var glyph in glyphsToExport)
                locaOffsetsWriter.Write((uint)glyph.Offset);
            offsetsBuffer = Align4(locaOffsetsStream.ToArray());
        }
        else
        {
            // Write all offsets as UInt16 into a byte array
            using var locaOffsetsStream = new MemoryStream();
            using var locaOffsetsWriter = new BinaryWriter(locaOffsetsStream);
            foreach (var glyph in glyphsToExport)
                locaOffsetsWriter.Write((ushort)glyph.Offset);
            offsetsBuffer = Align4(locaOffsetsStream.ToArray());
        }

        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        writer.Write((uint)(HEAD_LENGTH + offsetsBuffer.Length));               // O_SIZE
        writer.Write(Encoding.ASCII.GetBytes("loca"));// O_LABEL
        writer.Write((uint)glyphsToExport.Count);       // O_COUNT
        writer.Write(offsetsBuffer, 0, offsetsBuffer.Length);
        return ms.ToArray();
    }


    // Glyph Section
    public static byte[] BuildGlyfSection(LVGLFont lvglFont, IList<LVGLGlyph> glyphsToExport, bool isMonospaced)
    {
        const int HEAD_LENGTH = 8;

        var info = lvglFont.FontInformations;
        int maxY = Math.Max(Math.Abs(Parse(info.YMin)), Math.Abs(Parse(info.YMax)));
        int maxX = Math.Max(Math.Abs(Parse(info.XMin)), Math.Abs(Parse(info.XMax)));
        int xyBits = Math.Max(maxX, maxY);
        int whBits = Math.Max(Parse(info.CharWidthMax), Parse(info.LineHeight));
        int adwBits = Parse(info.AdvanceWidthMax);
        bool xyIsUshort = xyBits > byte.MaxValue;
        bool whIsUshort = whBits > byte.MaxValue;
        bool adwIsUshort = adwBits > byte.MaxValue;

        using var bodyStream = new MemoryStream();
        using var bodyWriter = new BinaryWriter(bodyStream);
        int offset = HEAD_LENGTH;
        foreach (var glyph in glyphsToExport)
        {
            using var glypfStream = new MemoryStream();
            using var glyphWriter = new BinaryWriter(glypfStream);

            var d = glyph.Descriptor;

            if (!isMonospaced)
            {
                if (adwIsUshort)
                {
                    glyphWriter.Write((ushort)d.AdvanceWidth);
                }
                else
                {
                    glyphWriter.Write((byte)d.AdvanceWidth);
                }
            }

            if (xyIsUshort)
            {
                glyphWriter.Write((short)d.OffsetX);
                glyphWriter.Write((short)d.OffsetY);
            }
            else
            {
                glyphWriter.Write((byte)d.OffsetX);
                glyphWriter.Write((byte)d.OffsetY);
            }

            if (whIsUshort)
            {
                glyphWriter.Write((ushort)d.Width);
                glyphWriter.Write((ushort)d.Height);
            }
            else
            {
                glyphWriter.Write((byte)d.Width);
                glyphWriter.Write((byte)d.Height);
            }

            glyphWriter.Write(glyph.Bitmap, 0, glyph.Bitmap.Length);

            byte[] glyphBuffer = Align4(glypfStream.ToArray());
            bodyWriter.Write(glyphBuffer, 0, glyphBuffer.Length);

            glyph.Offset = offset;
            offset += glyphBuffer.Length;
        }

        var body = bodyStream.ToArray();

        using var ms = new MemoryStream();
        using var finalWriter = new BinaryWriter(ms);

        finalWriter.Write((uint)(HEAD_LENGTH + body.Length));                  // SIZE
        finalWriter.Write(Encoding.ASCII.GetBytes("glyf"));  // LABEL
        finalWriter.Write(body, 0, body.Length);
        return ms.ToArray();
    }




    // Kerning Section
    public static byte[] BuildKernSection(byte[]? format0Or3Data, byte format = 3)
    {
        const int O_SIZE = 0;
        const int O_LABEL = O_SIZE + 4;
        const int O_FORMAT = O_LABEL + 4;
        const int HEAD_LENGTH = 16; // Align4(O_FORMAT + 1)

        if (format0Or3Data == null || format0Or3Data.Length == 0)
            return Array.Empty<byte>();

        // هم‌ترازسازی ۴ بایتی داده‌ی جدول
        var alignedData = Align4(format0Or3Data);

        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        // رزرو فضا برای هدر
        writer.Write(new byte[HEAD_LENGTH]);

        // نوشتن بدنه بعد از هدر
        writer.Write(alignedData);

        // بازگشت به ابتدا برای پر کردن هدر
        ms.Position = O_SIZE;
        writer.Write((uint)(HEAD_LENGTH + alignedData.Length));             // O_SIZE
        writer.Write(Encoding.ASCII.GetBytes("kern"));                      // O_LABEL
        writer.Write(format);                                               // O_FORMAT
        writer.Write(new byte[HEAD_LENGTH - O_FORMAT - 1]);                 // Padding to align (پر کردن تا 16 بایت)

        return ms.ToArray();
    }



    private static byte[] Align4(byte[] input)
    {
        int pad = (4 - (input.Length % 4)) % 4;
        if (pad == 0) return input;

        var aligned = new byte[input.Length + pad];
        Buffer.BlockCopy(input, 0, aligned, 0, input.Length);
        return aligned;
    }

}
