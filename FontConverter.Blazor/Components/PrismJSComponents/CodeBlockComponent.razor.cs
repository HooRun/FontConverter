using FontConverter.Blazor.Components.LeftSidebarComponents;
using FontConverter.Blazor.Interfaces;
using FontConverter.Blazor.Models.GlyphsView;
using FontConverter.Blazor.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Radzen.Blazor.Markdown;
using System.Net;
using System.Text;

namespace FontConverter.Blazor.Components.PrismJSComponents;

public partial class CodeBlockComponent : ComponentBase, IRerenderable
{
    [Inject]
    IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    MainViewModel MainViewModel { get; set; } = default!;

    [Parameter]
    public string Language { get; set; } = "c";

    [Parameter]
    public string Code { get; set; } = string.Empty;

    [Parameter]
    public string Style { get; set; } = string.Empty;

    private List<string> _Lines = [];

    private int _LinesCount = 0;
    private int _SizeOfCode = 0;

    private MarkupString EscapedCode;

    private string infoText = string.Empty;

    int key = 0;

    string _FileName = string.Empty;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        MainViewModel.RegisterComponent(nameof(CodeBlockComponent), this);
        _Lines = string.IsNullOrEmpty(Code)
            ? new List<string>()
            : Code.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None).ToList();
        _LinesCount = _Lines.Count;
        _SizeOfCode = Encoding.UTF8.GetByteCount(Code);
        EscapedCode = (MarkupString)System.Net.WebUtility.HtmlEncode(Code);
        if (Language == "c")
            _FileName = MainViewModel.LVGLFont.FontSettings.FontName + ".c";
        else if (Language == "asciidoc")
            _FileName = MainViewModel.LVGLFont.FontSettings.FontName + ".bin";
        else if (Language== "xml")
            _FileName = MainViewModel.LVGLFont.FontSettings.FontName + ".svg";
        else
            _FileName = MainViewModel.LVGLFont.FontSettings.FontName + "." + Language;
        infoText = $"{_LinesCount:N0} lines · {_SizeOfCode:N0} bytes";
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
            await HighlightAsync();

    }

    private async Task HighlightAsync()
    {
        await JSRuntime.InvokeVoidAsync("hljs.highlightAll");
        await JSRuntime.InvokeVoidAsync("hljs.initLineNumbersOnLoad");
    }

    public async Task ForceRender()
    {
        await InvokeAsync(StateHasChanged);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        StateHasChanged();
    }

    public async Task Update(string code)
    {
        Code = code;
        key++;
        await Refresh();
        
    }

    async Task Refresh()
    {
        _Lines = Code.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None)
           .Select(line => line)
           .ToList();
        _LinesCount = _Lines.Count;
        _SizeOfCode = Encoding.UTF8.GetByteCount(Code);
        EscapedCode = (MarkupString)System.Net.WebUtility.HtmlEncode(Code);
        infoText = $"{_LinesCount:N0} lines · {_SizeOfCode:N0} bytes";
        if (Language == "c")
            _FileName = MainViewModel.LVGLFont.FontSettings.FontName + ".c";
        else if (Language == "asciidoc")
            _FileName = MainViewModel.LVGLFont.FontSettings.FontName + ".bin";
        else if (Language == "xml")
            _FileName = MainViewModel.LVGLFont.FontSettings.FontName + ".svg";
        else
            _FileName = MainViewModel.LVGLFont.FontSettings.FontName + "." + Language;
        await InvokeAsync(StateHasChanged);

    }

    async Task CopyToClipboard()
    {
        await JSRuntime.InvokeVoidAsync("copyTextToClipboard", Code);
    }

    async Task OnDownloadFile()
    {
        if (Language=="c")
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(Code);
            string base64 = Convert.ToBase64String(bytes);

            await JSRuntime.InvokeVoidAsync("downloadFileFromBytes", _FileName, base64);
        }
    }
}
