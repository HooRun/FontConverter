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
        SubTitle = string.Empty;
        Icon = string.Empty;
        Count = 0;
        IsSelected = false;
        Items = [];
        Contents = new();
    }

    public LVGLFontContent(string header, string subTitle, string icon, int count, bool isSelected, List<int>? items, SortedList<string, LVGLFontContent> contents) : this()
    {
        Header = header;
        SubTitle = subTitle;
        Icon = icon;
        Count = count;
        IsSelected = isSelected;
        Items = items ?? [];
        Contents = contents;
    }

    public string Header { get; set; }
    public string SubTitle { get; set; } = string.Empty;
    public string Icon { get; set; }
    public int Count { get; set; }
    public bool IsSelected { get; set; }
    public List<int> Items { get; set; }
    public SortedList<string, LVGLFontContent> Contents { get; set; }
}
