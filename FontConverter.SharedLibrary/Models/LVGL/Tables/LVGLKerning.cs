using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Models;

public class LVGLKerning
{
    public int ClassID { get; set; }
    public List<int> GlyphIDs { get; set; } = new();
}
