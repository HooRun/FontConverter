using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Models;

public class UnicodeBlockCollection
{
    public UnicodeBlockCollection()
    {
        Blocks = new();
    }

    public SortedList<(int Start, int End), UnicodeBlock> Blocks { get; set; }
}
