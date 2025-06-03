using AutoMapper;
using FontConverter.Blazor.Components;
using FontConverter.Blazor.Components.GlyphsListViewComponents;
using FontConverter.Blazor.Components.LeftSidebarComponents;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.Layout;
using FontConverter.SharedLibrary.Models;
using Radzen.Blazor.Rendering;

namespace FontConverter.Blazor.ViewModels;

public class MainViewModel : BaseViewModel
{
    public MainViewModel(IMapper mapper)
    {
        _mapper = mapper;
        _OpenTypeFont = new();
        _LVGLFont = new();
        _FontSettingsViewModel = new();
        _FontAdjusmentsViewModel = new();
        _FontContentsViewModel = new();
        _FontInformationsViewModel = new();
        _GlyphViewItemPropertiesViewModel = new();

        _LeftSidebarExpanded = false;
        _RightSidebarExpanded = false;

        MappingsFromModelToViewModel();
    }

    private readonly IMapper _mapper;
    private OpenTypeFont _OpenTypeFont;
    private LVGLFont _LVGLFont;
    private FontSettingsViewModel _FontSettingsViewModel;
    private FontAdjusmentsViewModel _FontAdjusmentsViewModel;
    private FontContentsViewModel _FontContentsViewModel;
    private FontInformationsViewModel _FontInformationsViewModel;
    private GlyphViewItemPropertiesViewModel _GlyphViewItemPropertiesViewModel;

    private bool _LeftSidebarExpanded;
    private bool _RightSidebarExpanded;

    public OpenTypeFont OpenTypeFont
    {
        get { return _OpenTypeFont; }
        set { SetProperty(ref _OpenTypeFont, value); }
    }
    public LVGLFont LVGLFont
    {
        get { return _LVGLFont; }
        set { SetProperty(ref _LVGLFont, value); }
    }
    public FontSettingsViewModel FontSettingsViewModel
    {
        get { return _FontSettingsViewModel; }
        set { SetProperty(ref _FontSettingsViewModel, value); }
    }
    public FontAdjusmentsViewModel FontAdjusmentsViewModel
    {
        get { return _FontAdjusmentsViewModel; }
        set { SetProperty(ref _FontAdjusmentsViewModel, value); }
    }
    public FontContentsViewModel FontContentsViewModel
    {
        get { return _FontContentsViewModel; }
        set { SetProperty(ref _FontContentsViewModel, value); }
    }
    public FontInformationsViewModel FontInformationsViewModel
    {
        get { return _FontInformationsViewModel; }
        set { SetProperty(ref _FontInformationsViewModel, value); }
    }
    public GlyphViewItemPropertiesViewModel GlyphViewItemPropertiesViewModel
    {
        get { return _GlyphViewItemPropertiesViewModel; }
        set { SetProperty(ref _GlyphViewItemPropertiesViewModel, value); }
    }
    public bool LeftSidebarExpanded
    {
        get { return _LeftSidebarExpanded; }
        set 
        {
            if (SetProperty(ref _LeftSidebarExpanded, value))
            {
                RerenderMany(nameof(MainLayout), nameof(ToolbarComponent), nameof(LeftSidebarComponent));
            }
        }
    }
    public bool RightSidebarExpanded
    {
        get { return _RightSidebarExpanded; }
        set 
        {
            if (SetProperty(ref _RightSidebarExpanded, value))
            {
                RerenderMany(nameof(MainLayout), nameof(ToolbarComponent), nameof(LeftSidebarComponent));
            }
        }
    }

    public void MappingsFromModelToViewModel()
    {
        _mapper.Map(LVGLFont.FontSettings, FontSettingsViewModel);
        _mapper.Map(LVGLFont.FontAdjusments, FontAdjusmentsViewModel);
        _mapper.Map(LVGLFont.FontContents, FontContentsViewModel);
        _mapper.Map(LVGLFont.FontInformations, FontInformationsViewModel);
        _mapper.Map(LVGLFont.GlyphViewItemProperties, GlyphViewItemPropertiesViewModel);

        //RerenderMany(
        //    nameof(FontSettingsComponent),
        //    nameof(FontAdjusmentsComponent),
        //    nameof(FontContentsComponent),
        //    nameof(FontInformationsComponent));
        RerenderAll();
    }

    public void MappingsFromViewModelToModel()
    {
        _mapper.Map(FontSettingsViewModel, LVGLFont.FontSettings);
        _mapper.Map(FontAdjusmentsViewModel, LVGLFont.FontAdjusments);
        //_mapper.Map(FontContentsViewModel, LVGLFont.FontContents);
        //_mapper.Map(FontInformationsViewModel, LVGLFont.FontInformations);
    }

    public async Task SelectTreeItemAsync(FontContentViewModel selectedItem)
    {
        if (selectedItem == null || FontContentsViewModel?.Contents == null)
            return;

        await Task.Run(() => UpdateTreeSelection(FontContentsViewModel.Contents.Values, selectedItem));
    }

    private void UpdateTreeSelection(IEnumerable<FontContentViewModel> nodes, FontContentViewModel selectedItem)
    {
        if (nodes == null)
            return;

        foreach (var node in nodes)
        {
            node.IsSelected = node == selectedItem;
            UpdateTreeSelection(node.Children, selectedItem);
        }
    }


    private readonly Dictionary<string, IRerenderable> _components = new();

    public void RegisterComponent(string name, IRerenderable? component)
    {
        if (component != null)
        {
            _components[name] = component;
        }  
    }

    public void Rerender(string name)
    {
        if (_components.TryGetValue(name, out var component))
        {
            component.ForceRender();
        }
    }

    public void RerenderMany(params string[] names)
    {
        foreach (var name in names)
        {
            Rerender(name);
        }
    }

    public void RerenderAll()
    {
        foreach (var c in _components.Values)
        {
            c.ForceRender();
        }
    }










    public Task<List<LVGLGlyph>> GetGlyphsAsync(int startIndex, int count)
    {
        if (startIndex < 0 || count <= 0 || startIndex >= LVGLFont.Glyphs.Count)
            return Task.FromResult(new List<LVGLGlyph>());
        var endIndex = Math.Min(startIndex + count, LVGLFont.Glyphs.Count);
        var glyphs = new List<LVGLGlyph>();
        for (int i = startIndex; i < endIndex; i++)
        {
            if (LVGLFont.Glyphs.TryGetValue(i, out var glyph))
            {
                glyphs.Add(glyph);
            }
        }
        return Task.FromResult(glyphs);
    }


    public int TotalGlyphsCount => LVGLFont.Glyphs.Count;


    public int GlyphItemHeight => (GlyphViewItemPropertiesViewModel.ItemHeight * GlyphViewItemPropertiesViewModel.Zoom) + GlyphViewItemPropertiesViewModel.ItemPadding + 20;
    public int GlyphItemWidth => (GlyphViewItemPropertiesViewModel.ItemWidth * GlyphViewItemPropertiesViewModel.Zoom) + GlyphViewItemPropertiesViewModel.ItemPadding;



}