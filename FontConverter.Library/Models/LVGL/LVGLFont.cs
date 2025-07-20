using LVGLFontConverter.Library.Data;
using LVGLFontConverter.Library.Models.OpenType;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.Library.Models;

public class LVGLFont : IList, IKeyIndexMapping, INotifyCollectionChanged
{
    public LVGLFont()
    {
        FontData = new();
        FontProperties = new();
        FontAdjusment = new();
        Glyphs = new();
    }

    public LVGLFont(IEnumerable<LVGLFontGlyph> collection): this()
    {
        InitializeCollection(collection);
    }

    
    public LVGLFontData FontData { get; set; }
    public LVGLFontProperties FontProperties { get; set; }
    public LVGLFontAdjusment FontAdjusment { get; set; }


    public List<LVGLFontGlyph> Glyphs { get; set; }
    public uint GlyphsCount { get; set; }

    public uint EmptyGlyphsCount { get; set; } = 0;
    public uint UnMappedGlyphsCount { get; set; } = 0;
    public uint SingleMappedGlyphsCount { get; set; } = 0;
    public uint MultiMappedGlyphsCount { get; set; } = 0;

    public List<(uint CodePoint, string Name)> Unicodes { get; set; } = new();
    public SortedDictionary<UnicodeBlock, int> UnicodeRanges { get; set; } = new();



    public object this[int index] { get => ((IList)Glyphs)[index]; set => ((IList)Glyphs)[index] = value; }
    public bool IsFixedSize => ((IList)Glyphs).IsFixedSize;
    public bool IsReadOnly => ((IList)Glyphs).IsReadOnly;
    public int Count => ((ICollection)Glyphs).Count;
    public bool IsSynchronized => ((ICollection)Glyphs).IsSynchronized;
    public object SyncRoot => ((ICollection)Glyphs).SyncRoot;
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public int Add(object value)
    {
        CollectionChanged(this,
        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add));
        return ((IList)Glyphs).Add(value);
    }

    public void Clear()
    {
        ((IList)Glyphs).Clear();
    }

    public bool Contains(object value)
    {
        return ((IList)Glyphs).Contains(value);
    }

    public void CopyTo(Array array, int index)
    {
        ((ICollection)Glyphs).CopyTo(array, index);
    }

    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable)Glyphs).GetEnumerator();
    }

    public int IndexOf(object value)
    {
        return ((IList)Glyphs).IndexOf(value);
    }

    public void Insert(int index, object value)
    {
        ((IList)Glyphs).Insert(index, value);
    }

    public void Remove(object value)
    {
        ((IList)Glyphs).Remove(value);
    }

    public void RemoveAt(int index)
    {
        ((IList)Glyphs).RemoveAt(index);
    }

    public int IndexFromKey(string key)
    {
        foreach (LVGLFontGlyph glyph in Glyphs)
        {
            if (glyph.Index.ToString().Equals(key))
            {
                return Glyphs.IndexOf(glyph);
            }
        }
        return -1;

    }

    public string KeyFromIndex(int index)
    {
        return Glyphs[index].Index.ToString();

    }

    public void InitializeCollection(IEnumerable<LVGLFontGlyph> collection)
    {
        Glyphs.Clear();
        if (collection != null)
        {
            Glyphs.AddRange(collection);
        }

        if (CollectionChanged != null)
        {
            CollectionChanged(this,
        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}




//public string GetGlyphDescriptor { get; set; }
//public string GetGlyphBitmap { get; set; }
//public string FontDescriptor { get; set; }
//public int FontUserData { get; set; }
