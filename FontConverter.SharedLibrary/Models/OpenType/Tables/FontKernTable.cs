using static FontConverter.SharedLibrary.Helpers.FontTablesEnumHelper;

namespace FontConverter.SharedLibrary.Models;

public class FontKernTable
{
    public FontKernTable()
    {

    }

    public List<KernSubtable> Subtables { get; set; } = new();

    public List<KernPair> AllPairs
    {
        get
        {
            var result = new List<KernPair>();

            foreach (var sub in Subtables)
            {
                switch (sub)
                {
                    case KernFormat0Subtable fmt0:
                        result.AddRange(fmt0.Pairs);
                        break;

                    case KernFormat2Subtable fmt2:
                        if (fmt2.KerningValues != null)
                        {
                            for (int left = 0; left < fmt2.NumLeftClasses; left++)
                            {
                                for (int right = 0; right < fmt2.NumRightClasses; right++)
                                {
                                    short value = (short)fmt2.KerningValues[left, right];
                                    if (value != 0)
                                    {
                                        result.Add(new KernPair
                                        {
                                            Left = (ushort)left,
                                            Right = (ushort)right,
                                            Value = value
                                        });
                                    }
                                }
                            }
                        }
                        break;
                }
            }

            return result;
        }
    }
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

    public ushort[,]? KerningValues { get; set; }
}