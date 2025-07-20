namespace FontConverter.SharedLibrary.Models;

public class LVGLKerningClassResult
{
    public List<byte> LeftClassMap { get; set; } = [];
    public List<byte> RightClassMap { get; set; } = [];
    public sbyte[] ClassValues { get; set; } = [];
    public int LeftClassCount { get; set; }
    public int RightClassCount { get; set; }
    public int Scale { get; set; }
}