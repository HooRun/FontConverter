namespace FontConverter.SharedLibrary.Models;

public class FontGsubTable
{
    public ushort MajorVersion { get; set; }
    public ushort MinorVersion { get; set; }
    public ScriptListTable ScriptList { get; set; } = new();
    public FeatureListTable FeatureList { get; set; } = new();
    public LookupListTable LookupList { get; set; } = new();
    public FeatureVariationsTable FeatureVariations { get; set; } = new();
}

public class ScriptListTable
{
    public List<ScriptRecord> ScriptRecords { get; set; } = new();
}

public class ScriptRecord
{
    public string Tag { get; set; } = string.Empty;
    public ScriptTable Script { get; set; } = new();
}

public class ScriptTable
{
    public LangSysTable DefaultLangSys { get; set; } = new();
    public List<LangSysRecord> LangSysRecords { get; set; } = new();
}

public class LangSysRecord
{
    public string Tag { get; set; } = string.Empty;
    public LangSysTable LangSys { get; set; } = new();
}

public class LangSysTable
{
    public ushort LookupOrder { get; set; } 
    public ushort ReqFeatureIndex { get; set; }
    public List<ushort> FeatureIndices { get; set; } = new();
}

public class FeatureListTable
{
    public List<FeatureRecord> FeatureRecords { get; set; } = new();
}

public class FeatureRecord
{
    public string Tag { get; set; } = string.Empty;
    public FeatureTable Feature { get; set; } = new();
}

public class FeatureTable
{
    public ushort FeatureParamsOffset { get; set; } // Optional, used for some feature types
    public List<ushort> LookupListIndices { get; set; } = new();
}

public class LookupListTable
{
    public List<LookupTable> Lookups { get; set; } = new();
}

public class LookupTable
{
    public ushort LookupType { get; set; }
    public ushort LookupFlag { get; set; }
    public List<SubTable> SubTables { get; set; } = new();
    public ushort MarkFilteringSet { get; set; } // Optional depending on flag
}

public abstract class SubTable { }

public class FeatureVariationsTable
{
    public List<FeatureVariationRecord> Records { get; set; } = new();
}

public class FeatureVariationRecord
{
    public ConditionSetTable ConditionSet { get; set; } = new();
    public FeatureTableSubstitution FeatureSubstitution { get; set; } = new();
}

public class ConditionSetTable
{
    public List<ConditionTable> Conditions { get; set; } = new();
}

public class ConditionTable
{
    public ushort Format { get; set; }
    public string AxisTag { get; set; } = string.Empty;
    public float FilterRangeMinValue { get; set; }
    public float FilterRangeMaxValue { get; set; }
}

public class FeatureTableSubstitution
{
    public List<FeatureSubstitutionRecord> Records { get; set; } = new();
}

public class FeatureSubstitutionRecord
{
    public ushort FeatureIndex { get; set; }
    public FeatureTable SubstitutedFeature { get; set; } = new();
}
