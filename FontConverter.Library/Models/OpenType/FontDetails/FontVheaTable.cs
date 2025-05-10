using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.Library.Models;

public class FontVheaTable
{
    public FontVheaTable()
    {
        
    }

    public double Version { get; set; } // 0x00010000 = 1.0
    public short Ascender { get; set; }
    public short Descender { get; set; }
    public short LineGap { get; set; }
    public ushort AdvanceHeightMax { get; set; }
    public short MinTopSideBearing { get; set; }
    public short MinBottomSideBearing { get; set; }
    public short YMaxExtent { get; set; }
    public short CaretSlopeRise { get; set; }
    public short CaretSlopeRun { get; set; }
    public short CaretOffset { get; set; }
    public short Reserved1 { get; set; }
    public short Reserved2 { get; set; }
    public short Reserved3 { get; set; }
    public short Reserved4 { get; set; }
    public short MetricDataFormat { get; set; } // Should be 0
    public ushort NumberOfLongVerMetrics { get; set; }
}
