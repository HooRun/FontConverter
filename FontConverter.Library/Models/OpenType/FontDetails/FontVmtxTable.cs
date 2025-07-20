using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.Library.Models;

public class FontVmtxTable
{
    public FontVmtxTable()
    {
        
    }

    public List<GlyphVerticalMetric> GlyphMetrics { get; set; } = new();
}

public class GlyphVerticalMetric
{
    public ushort AdvanceHeight { get; set; }
    public short TopSideBearing { get; set; }
}