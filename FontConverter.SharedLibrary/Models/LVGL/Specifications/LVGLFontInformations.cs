namespace FontConverter.SharedLibrary.Models;

public class LVGLFontInformations
{
    public LVGLFontInformations()
    {
        FontName = string.Empty;
        LineHeight = 0;
        BaseLine = 0;
        CharWidthMax = 0;
        AdvanceWidthMax = 0;
        UnderlinePosition = 0;
        UnderlineThickness = 0;
        Ascent = 0;
        Descent = 0;
        XMin = 0;
        YMin = 0;
        XMax = 0;
        YMax = 0;
    }

    public string FontName { get; set; }
    public int LineHeight { get; set; }
    public int BaseLine { get; set; }
    public int CharWidthMax { get; set; }
    public int AdvanceWidthMax { get; set; }
    public int UnderlinePosition { get; set; }
    public int UnderlineThickness { get; set; }
    public int Ascent { get; set; }
    public int Descent { get; set; }
    public int XMin { get; set; }
    public int YMin { get; set; }
    public int XMax { get; set; }
    public int YMax { get; set; }
    

  
    

}
