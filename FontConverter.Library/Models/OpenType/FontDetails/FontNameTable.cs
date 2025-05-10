using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LVGLFontConverter.Library.Models;

public class FontNameTable
{
    public FontNameTable()
    {
        Copyright = string.Empty;
        FontFamily = string.Empty;
        FontSubfamily = string.Empty;
        UniqueIdentifier = string.Empty;
        FullFontName = string.Empty;
        Version = string.Empty;
        PostScriptName = string.Empty;
        Trademark = string.Empty;
        Manufacturer = string.Empty;
        Designer = string.Empty;
        Description = string.Empty;
        VendorURL = string.Empty;
        DesignerURL = string.Empty;
        License = string.Empty;
        LicenseURL = string.Empty;
        PreferredFamily = string.Empty;
        PreferredSubfamily = string.Empty;
        CompatibleFull = string.Empty;
        SampleText = string.Empty;
        PostScriptCID = string.Empty;
        WWSFamily = string.Empty;
        WWSSubfamily = string.Empty;
        LightBackgroundPalette = string.Empty;
        DarkBackgroundPalette = string.Empty;
        VariationsPostScriptPrefix = string.Empty;
    }

    // Name Table Information
    public string Copyright { get; set; }
    public string FontFamily { get; set; }
    public string FontSubfamily { get; set; }
    public string UniqueIdentifier { get; set; }
    public string FullFontName { get; set; }
    public string Version { get; set; }
    public string PostScriptName { get; set; }
    public string Trademark { get; set; }
    public string Manufacturer { get; set; }
    public string Designer { get; set; }
    public string Description { get; set; }
    public string VendorURL { get; set; }
    public string DesignerURL { get; set; }
    public string License { get; set; }
    public string LicenseURL { get; set; }
    public string PreferredFamily { get; set; }
    public string PreferredSubfamily { get; set; }
    public string CompatibleFull { get; set; }
    public string SampleText { get; set; }
    public string PostScriptCID { get; set; }
    public string WWSFamily { get; set; }
    public string WWSSubfamily { get; set; }
    public string LightBackgroundPalette { get; set; }
    public string DarkBackgroundPalette { get; set; }
    public string VariationsPostScriptPrefix { get; set; }
}
