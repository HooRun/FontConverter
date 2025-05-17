using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;

namespace FontConverter.SharedLibrary.Models;

public class FontKernTable
{
    public FontKernTable()
    {
        
    }

    public List<KernSubtable> Subtables { get; set; } = new();
}

public abstract class KernSubtable
{
    public KernCoverage Coverage { get; set; }
    public ushort Format { get; set; }
}

public class KernFormat0Subtable : KernSubtable
{
    public List<KernPair> Pairs { get; set; } = new();
}

public class KernPair
{
    public ushort Left { get; set; }
    public ushort Right { get; set; }
    public short Value { get; set; }
}

public class KernFormat2Subtable : KernSubtable
{
    public ushort RowWidth { get; set; }
    public ushort LeftClassTableOffset { get; set; }
    public ushort RightClassTableOffset { get; set; }
    public ushort ArrayOffset { get; set; }

    public ushort NumLeftClasses { get; set; }
    public ushort NumRightClasses { get; set; }

    public ushort[,] KerningValues { get; set; }
}