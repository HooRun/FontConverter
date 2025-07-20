using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.Library.Models;

public class FontHmtxTable
{
    public FontHmtxTable()
    {
        
    }

    public List<GlyphHorizontalMetric> GlyphMetrics { get; set; } = new();
}

public class GlyphHorizontalMetric
{
    public ushort AdvanceWidth { get; set; }
    public short LeftSideBearing { get; set; }
}