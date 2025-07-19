using FontConverter.Blazor.Components.LeftSidebarComponents;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.Models.GlyphsView;
using FontConverter.Blazor.ViewModels;
using FontConverter.SharedLibrary.Helpers;
using FontConverter.SharedLibrary.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Radzen.Blazor.Markdown;
using System.Net;
using System.Text;
using static FontConverter.SharedLibrary.Helpers.LVGLFontEnums;

namespace FontConverter.Blazor.Components.PrismJSComponents;

public partial class CodeBlockComponent : ComponentBase, IRerenderable
{
    [Inject]
    IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    MainViewModel MainViewModel { get; set; } = default!;

    [Parameter]
    public LVGL_FILE_TYPE FileType { get; set; } = LVGL_FILE_TYPE.LVGL_FILE_TYPE_NONE;

    [Parameter]
    public IList<LVGLGlyph>? Glyphs { get; set; }

    [Parameter]
    public SortedDictionary<uint, UnicodeBlock> Blocks { get; set; } = new();

    [Parameter]
    public string Style { get; set; } = string.Empty;

    private const int _KBValue = 1024;
    private const int _MBValue = 1024 * _KBValue;
    private const int _GBValue = 1024 * _MBValue;

    private const string _BytesString = "Bytes";
    private const string _KBString = "KB";
    private const string _MBString = "MB";
    private const string _GBString = "GB";

    private string _Code { get; set; } = string.Empty;
    private int _SizeOfCode = 0;
    private string _SizeText = string.Empty;
    string _FileName = string.Empty;

    private bool _IsDownloadBusy = false;
    private bool _IsCopyBusy = false;
    private bool _IsGenerateBusy = false;

    private IList<LVGLGlyph> _MergedGlyphs = [];

    protected override async Task OnInitializedAsync()
    {
        MainViewModel.RegisterComponent(nameof(CodeBlockComponent), this);
        _FileName = await UpdateFileName();
        _SizeText = await CalculateSize(); ;
        _MergedGlyphs = await Task.Run(MergeDuplicateGlyphsByIndex);
    }

    protected override async Task OnParametersSetAsync()
    {
        _MergedGlyphs = await Task.Run(MergeDuplicateGlyphsByIndex);
        //await InvokeAsync(StateHasChanged);
    }

    public async Task ForceRender()
    {
        await InvokeAsync(StateHasChanged);
    }

    async Task Refresh()
    {
        _SizeText = await CalculateSize();
        _FileName = await UpdateFileName();
        await InvokeAsync(StateHasChanged);

    }

    async Task CopyToClipboard()
    {
        _IsCopyBusy = true;
        await Task.Run(() =>
        {
            JSRuntime.InvokeVoidAsync("copyTextToClipboard", _Code);
        });
        _IsCopyBusy = false;
    }

    async Task OnDownloadFile()
    {
        _IsDownloadBusy = true;
        await Task.Run(() =>
        {
            byte[] bytes = Array.Empty<byte>();
            if (FileType == LVGL_FILE_TYPE.LVGL_FILE_TYPE_BINARY)
            {
                bytes = Convert.FromBase64String(_Code);
            }
            else
            {
                bytes = System.Text.Encoding.UTF8.GetBytes(_Code);
            }
            string base64 = Convert.ToBase64String(bytes);
            JSRuntime.InvokeVoidAsync("downloadFileFromBytes", _FileName, base64);
        });
        _IsDownloadBusy = false;
    }

    private async Task<string> CalculateSize()
    {
        await Task.Yield(); // Ensure we are not blocking the UI thread
        _SizeOfCode = Encoding.UTF8.GetByteCount(_Code);
        if (_SizeOfCode < _KBValue)
        {
            return $"{_SizeOfCode} {_BytesString}";
        }
        else if (_SizeOfCode < _MBValue)
        {
            return $"{_SizeOfCode / (double)_KBValue:N2} {_KBString}";
        }
        else if (_SizeOfCode < _GBValue)
        {
            return $"{_SizeOfCode / (double)_MBValue:N2} {_MBString}";
        }
        else
        {
            return $"{_SizeOfCode / (double)_GBValue:N2} {_GBString}";
        }
    }

    private async Task<string> UpdateFileName()
    {
        await Task.Yield(); // Ensure we are not blocking the UI thread
        if (FileType == LVGL_FILE_TYPE.LVGL_FILE_TYPE_C)
            return MainViewModel.LVGLFont.FontSettings.FontName + ".c";
        else if (FileType == LVGL_FILE_TYPE.LVGL_FILE_TYPE_SYMBOL)
            return MainViewModel.LVGLFont.FontSettings.FontName + "_symbol" + ".h";
        else if (FileType == LVGL_FILE_TYPE.LVGL_FILE_TYPE_BINARY)
            return MainViewModel.LVGLFont.FontSettings.FontName + ".bin";
        else if (FileType == LVGL_FILE_TYPE.LVGL_FILE_TYPE_SVG_C)
            return MainViewModel.LVGLFont.FontSettings.FontName + "_svg" + ".c";
        else if (FileType == LVGL_FILE_TYPE.LVGL_FILE_TYPE_SVG_H)
            return MainViewModel.LVGLFont.FontSettings.FontName + "_svg" + ".h";
        else
            return string.Empty;
    }

    private async Task OnGenerateClick()
    {
        _IsGenerateBusy = true;
        if (FileType == LVGL_FILE_TYPE.LVGL_FILE_TYPE_C)
        {
            if (Glyphs != null && Glyphs.Count > 0)
                _Code = await Task.Run(() => ExportToCHelper.ExportToC(MainViewModel.LVGLFont, Glyphs, Blocks));
        }
        else if (FileType == LVGL_FILE_TYPE.LVGL_FILE_TYPE_SYMBOL)
        {
            if (Glyphs != null && Glyphs.Count > 0)
                _Code = await Task.Run(() => ExportToSymbolHelper.ExportToSymbol(MainViewModel.LVGLFont, Glyphs));
        }
        else if (FileType == LVGL_FILE_TYPE.LVGL_FILE_TYPE_BINARY)
        {
            if (Glyphs != null && Glyphs.Count > 0)
                _Code = await Task.Run(() => ExportToBinHelper.ExportFontToLvglBinary(MainViewModel.OpenTypeFont, MainViewModel.LVGLFont, Glyphs, Blocks));
        }
        else if (FileType == LVGL_FILE_TYPE.LVGL_FILE_TYPE_SVG_H)
        {
            if (Glyphs != null && Glyphs.Count > 0)
                _Code = await Task.Run(() => ExportToSVGHelper.ExportToSVG(MainViewModel.LVGLFont, Glyphs));
        }
        else
        {
            _Code = string.Empty;
        }

        await Refresh();
        _IsGenerateBusy = false;
    }

    private async Task<List<LVGLGlyph>> MergeDuplicateGlyphsByIndex()
    {
        await Task.Yield();

        var result = new List<LVGLGlyph>();

        if (Glyphs is null || Glyphs.Count == 0)
            return result;

        var grouped = Glyphs.GroupBy(g => g.Index);

        foreach (var group in grouped)
        {
            var baseGlyph = group.First();

            foreach (var other in group.Skip(1))
            {
                foreach (var kv in other.CodePoints)
                {
                    if (!baseGlyph.CodePoints.ContainsKey(kv.Key))
                    {
                        baseGlyph.CodePoints.Add(kv.Key, kv.Value);
                    }
                }
            }

            result.Add(baseGlyph);
        }

        return result;
    }

}
