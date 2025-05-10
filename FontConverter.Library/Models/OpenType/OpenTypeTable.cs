using System;

namespace LVGLFontConverter.Library.Models;

public class OpenTypeTable
{
    public OpenTypeTable()
    {
        TagName = string.Empty;
        RawData = Array.Empty<byte>();
    }
    public string TagName { get; set; }
    public uint Tag { get; set; }
    public uint Offset { get; set; }
    public int Length { get; set; }
    public byte[] RawData { get; set; }
}