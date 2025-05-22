using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Models;

public class LVGLFontContent
{
    public LVGLFontContent()
    {
        Header = string.Empty;
        Count = 0;
        IsSelected = false;
        Contents = new();
    }

    public LVGLFontContent(string header, int count, bool isSelected, SortedList<string, LVGLFontContent> contents) : this()
    {
        Header = header;
        Count = count;
        IsSelected = isSelected;
        Contents = contents;
    }

    public string Header { get; set; }
    public int Count { get; set; }
    public bool IsSelected { get; set; }
    public SortedList<string, LVGLFontContent> Contents { get; set; }
}
