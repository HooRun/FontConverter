using AutoMapper;
using FontConverter.Blazor.Components;
using FontConverter.Blazor.Components.GlyphsListViewComponents;
using FontConverter.Blazor.Components.LeftSidebarComponents;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.Layout;
using FontConverter.Blazor.Models.GlyphsView;
using FontConverter.Blazor.Services;
using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components;
using Radzen.Blazor.Rendering;
using System.Reflection.PortableExecutable;

namespace FontConverter.Blazor.ViewModels;

public class MainViewModel : BaseViewModel
{
    public MainViewModel(IMapper mapper, GlyphRenderQueueService glyphRenderQueueService)
    {
        _Mapper = mapper;
        _GlyphRenderQueueService = glyphRenderQueueService;
        _OpenTypeFont = new();
        _LVGLFont = new();
        _FontSettingsViewModel = new();
        _FontAdjusmentsViewModel = new();
        _FontContentsViewModel = new();
        _FontInformationsViewModel = new();
        _GlyphViewItemPropertiesViewModel = new();
        _GlyphsList = new();
        _GlyphsGroupedList = new();
        _LeftSidebarExpanded = false;
        _RightSidebarExpanded = false;
        _HaveSelectedGlyph = false;

        MappingsFromModelToViewModel();
    }

    private readonly IMapper _Mapper;
    private readonly GlyphRenderQueueService _GlyphRenderQueueService;
    private readonly Dictionary<string, IRerenderable> _Components = new();

    private OpenTypeFont _OpenTypeFont;
    private LVGLFont _LVGLFont;
    private FontSettingsViewModel _FontSettingsViewModel;
    private FontAdjusmentsViewModel _FontAdjusmentsViewModel;
    private FontContentsViewModel _FontContentsViewModel;
    private FontInformationsViewModel _FontInformationsViewModel;
    private GlyphViewItemPropertiesViewModel _GlyphViewItemPropertiesViewModel;
    private SortedList<int, GlyphItem> _GlyphsList;
    private List<GlyphsGroup> _GlyphsGroupedList;
    private bool _LeftSidebarExpanded;
    private bool _RightSidebarExpanded;
    private FontContentViewModel? _SelectedTreeViewItem;
    private bool _HaveSelectedGlyph;

    public Action<List<(int GroupID, int SelectedItemsCount)>>? OnGlyphSelectionChanged { get; set; }
    public Action<(int GlyphID, bool Selected)>? OnSingleGlyphSelectionChanged { get; set; }
    public Action? OnGlyphZoomChanged { get; set; }

    public bool ZoomInButtonDisabled => GlyphViewItemPropertiesViewModel.Zoom >= 10 ? true : false;
    public bool ZoomOutButtonDisabled => GlyphViewItemPropertiesViewModel.Zoom <= 1 ? true : false;
    public bool DeleteButtonDisabled => !_HaveSelectedGlyph;

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
    public SortedList<int, GlyphItem> GlyphsList
    {
        get { return _GlyphsList; }
        set { SetProperty(ref _GlyphsList, value); }
    }
    public List<GlyphsGroup> GlyphsGroupedList
    {
        get { return _GlyphsGroupedList; }
        set { SetProperty(ref _GlyphsGroupedList, value); }
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
        _Mapper.Map(LVGLFont.FontSettings, FontSettingsViewModel);
        _Mapper.Map(LVGLFont.FontAdjusments, FontAdjusmentsViewModel);
        _Mapper.Map(LVGLFont.FontContents, FontContentsViewModel);
        _Mapper.Map(LVGLFont.FontInformations, FontInformationsViewModel);
        _Mapper.Map(LVGLFont.GlyphViewItemProperties, GlyphViewItemPropertiesViewModel);

        //RerenderMany(
        //    nameof(FontSettingsComponent),
        //    nameof(FontAdjusmentsComponent),
        //    nameof(FontContentsComponent),
        //    nameof(FontInformationsComponent));
        RerenderAll();
    }

    public void MappingsFromViewModelToModel()
    {
        _Mapper.Map(FontSettingsViewModel, LVGLFont.FontSettings);
        _Mapper.Map(FontAdjusmentsViewModel, LVGLFont.FontAdjusments);
        //_Mapper.Map(FontContentsViewModel, LVGLFont.FontContents);
        //_Mapper.Map(FontInformationsViewModel, LVGLFont.FontInformations);
    }

    public async Task SelectTreeItemAsync(FontContentViewModel selectedItem)
    {
        if (selectedItem == null || FontContentsViewModel?.Contents == null)
            return;
        _SelectedTreeViewItem = selectedItem;
        await Task.Run(() => UpdateTreeSelection(FontContentsViewModel.Contents.Values, selectedItem));

        UpdateGlyphsListView(_SelectedTreeViewItem);
        UpdateGroupSelectedItems();
        RerenderMany(nameof(MainLayout), nameof(GlyphListComponent));
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

    private void UpdateGlyphsListView(FontContentViewModel? selectedItem)
    {
        if (selectedItem is null) return;

        GlyphsGroupedList = new();
        if (string.Equals(selectedItem.Header, LVGLFont.FontContents.UnicodesHeader))
        {
            if (selectedItem.Contents.Count > 0)
            {
                foreach (var content in selectedItem.Contents.Values)
                {
                    GlyphsGroup group = new();
                    group.Icon = content.Icon;
                    group.Header = content.Header;
                    group.SubTitle = content.SubTitle;
                    group.LoadItemsAsync = null;
                    if (content.Items != null && content.Items.Count > 0)
                    {
                        group.Items.AddRange(content.Items);
                    }
                    else
                    {
                        group.Items = [];
                    }
                    group.Items.Sort();

                    group.IsExpanded = true;
                    group.IsLoaded = true;
                    GlyphsGroupedList.Add(group);
                }
            }
            else
            {
                GlyphsGroup group = new();
                group.Icon = selectedItem.Icon;
                group.Header = selectedItem.Header;
                group.SubTitle = selectedItem.SubTitle;
                group.LoadItemsAsync = null;
                group.Items = [];
                group.IsExpanded = true;
                group.IsLoaded = true;
                GlyphsGroupedList.Add(group);
            }
        }
        else
        {
            GlyphsGroup group = new();
            group.Icon = selectedItem.Icon;
            group.Header = selectedItem.Header;
            group.SubTitle = selectedItem.SubTitle;
            group.LoadItemsAsync = null;
            group.Items = selectedItem.Items;
            group.Items.Sort();
            group.IsExpanded = true;
            group.IsLoaded = true;
            GlyphsGroupedList.Add(group);
        }
    }
    

    public void RegisterComponent(string name, IRerenderable? component)
    {
        if (component != null)
        {
            _Components[name] = component;
        }  
    }

    public void UnRegisterComponent(string name)
    {
        if (_Components.ContainsKey(name))
        {
            _Components.Remove(name);
        }
    }

    public void Rerender(string name)
    {
        if (_Components.TryGetValue(name, out var component))
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
        foreach (var c in _Components.Values)
        {
            c.ForceRender();
        }
    }










  
    public int TotalGlyphsCount => GlyphsList.Count;


 

    public void ZoomInChanged()
    {
        GlyphViewItemPropertiesViewModel.Zoom++;
        if (GlyphViewItemPropertiesViewModel.Zoom >= 10)
            GlyphViewItemPropertiesViewModel.Zoom = 10;
        OnGlyphZoomChanged?.Invoke();
        RerenderMany(nameof(MainLayout), nameof(GlyphListComponent), nameof(GlyphsToolbarComponent));
    }

    public void ZoomOutChanged()
    {
        GlyphViewItemPropertiesViewModel.Zoom--;
        if (GlyphViewItemPropertiesViewModel.Zoom <= 1)
            GlyphViewItemPropertiesViewModel.Zoom = 1;
        OnGlyphZoomChanged?.Invoke();
        RerenderMany(nameof(MainLayout), nameof(GlyphListComponent), nameof(GlyphsToolbarComponent));
    }



    public void GlyphSelectionChanged(int glyphID, bool isSelected)
    {
        foreach (var group in GlyphsGroupedList)
        {
            if (group.Items.Contains(glyphID))
            {
                if (isSelected)
                {
                    if (!group.SelectedItems.Contains(glyphID))
                    {
                        group.SelectedItems.Add(glyphID);
                    }
                }
                else
                {
                    group.SelectedItems.Remove(glyphID);
                }
            }
        }
        UpdateGroupSelectedItems();
        List<(int GroupID, int SelectedItemsCount)> selectionInfo = [];
        int groupID = 0;
        foreach (var group in GlyphsGroupedList)
        {
            if (group.Items.Contains(glyphID))
            {
                selectionInfo.Add((groupID, group.SelectedItems.Count));
            }
            groupID++;
        }
        OnGlyphSelectionChanged?.Invoke(selectionInfo);
        RerenderMany(nameof(MainLayout), nameof(GlyphListComponent), nameof(GlyphsToolbarComponent));
    }


    public void GroupSelectionChanged(int groupID, bool isSelected)
    {
        if (groupID < 0 || groupID >= GlyphsGroupedList.Count)
            return;
        var group = GlyphsGroupedList[groupID];
        if (isSelected)
        {
            group.SelectedItems.Clear();
            group.SelectedItems.AddRange(group.Items);
        }
        else
        {
            group.SelectedItems.Clear();
        }
        foreach (var item in group.Items)
        {
            if (GlyphsList.ContainsKey(item))
            {
                GlyphsList[item].IsSelected = isSelected;
            }
        }
        UpdateGroupSelectedItems();
        List<(int GroupID, int SelectedItemsCount)> selectionInfo = [];
        for (int i = 0; i < GlyphsGroupedList.Count; i++)
        {
            var g = GlyphsGroupedList[i];
            selectionInfo.Add((i, g.SelectedItems.Count));
        }
        OnGlyphSelectionChanged?.Invoke(selectionInfo);
        RerenderMany(nameof(MainLayout), nameof(GlyphListComponent), nameof(GlyphsToolbarComponent));
    }

    private void UpdateGroupSelectedItems()
    {
        _HaveSelectedGlyph = false;
        foreach (var group in GlyphsGroupedList)
        {
            group.SelectedItems.Clear();
            foreach (var item in group.Items)
            {
                if (GlyphsList.TryGetValue(item, out var glyph))
                {
                    if (glyph.IsSelected)
                    {
                        group.SelectedItems.Add(item);
                        _HaveSelectedGlyph = true;
                    }
                    OnSingleGlyphSelectionChanged?.Invoke((glyph.Index, glyph.IsSelected));
                }
            }
        }
    }
}