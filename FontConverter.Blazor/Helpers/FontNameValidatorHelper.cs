using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FontConverter.Blazor.Helpers;

public class FontNameValidatorHelper
{
    
    private static readonly HashSet<string> CKeywords = new HashSet<string>
    {
        "auto", "break", "case", "char", "const", "continue", "default", "do",
        "double", "else", "enum", "extern", "float", "for", "goto", "if",
        "inline", "int", "long", "register", "restrict", "return", "short",
        "signed", "sizeof", "static", "struct", "switch", "typedef", "union",
        "unsigned", "void", "volatile", "while", "_Bool", "_Complex", "_Imaginary"
    };

    public bool ValidateFontName(string? fontName)
    {
        if (string.IsNullOrWhiteSpace(fontName))
        {
            return false;
        }

        if (CKeywords.Contains(fontName))
        {
            return false;
        }

        var regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
        return regex.IsMatch(fontName);
    }

    public string ValidateFontNameMessage(string? fontName, bool isCheckNullEmpty = false)
    {
        if (string.IsNullOrWhiteSpace(fontName) && isCheckNullEmpty)
        {
            return "Font name is required.";
        }

        if (!string.IsNullOrWhiteSpace(fontName))
        {
            if (CKeywords.Contains(fontName))
            {
                return "Do not use C reserved keywords when naming fonts.";
            }

            var regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
            return !regex.IsMatch(fontName) ? "Font name is not valid" : "";
        }
        else
        {
            return "";
        }
    }
    


}
