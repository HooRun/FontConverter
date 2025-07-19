using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FontConverter.SharedLibrary.Helpers;

public static class UCDEnumsHelper
{
    // https://www.unicode.org/reports/tr44/#Character_Decomposition_Mappings
    public enum DecompositionTypeEnum : ushort
    {
        DECOMPOSITION_TYPE_NONE = 0,        // No decomposition
        DECOMPOSITION_TYPE_CANONICAL,       // Canonical decomposition (e.g. compatibility-free normalization form)
        DECOMPOSITION_TYPE_FONT,            // Font variant (for example, a blackletter form)
        DECOMPOSITION_TYPE_NOBREAK,         // No-break version of a space or hyphen
        DECOMPOSITION_TYPE_INITIAL,         // Initial presentation form (Arabic)
        DECOMPOSITION_TYPE_MEDIAL,          // Medial presentation form (Arabic)
        DECOMPOSITION_TYPE_FINAL,           // Final presentation form (Arabic)
        DECOMPOSITION_TYPE_ISOLATED,        // Isolated presentation form (Arabic)
        DECOMPOSITION_TYPE_CIRCLE,          // Encircled form
        DECOMPOSITION_TYPE_SUPER,           // Superscript form
        DECOMPOSITION_TYPE_SUB,             // Subscript form
        DECOMPOSITION_TYPE_VERTICAL,        // Vertical layout presentation form
        DECOMPOSITION_TYPE_WIDE,            // Wide (or zenkaku) compatibility character
        DECOMPOSITION_TYPE_NARROW,          // Narrow (or hankaku) compatibility character
        DECOMPOSITION_TYPE_SMALL,           // Small variant form (CNS compatibility)
        DECOMPOSITION_TYPE_SQUARE,          // CJK squared font variant
        DECOMPOSITION_TYPE_FRACTION,        // Vulgar fraction form
        DECOMPOSITION_TYPE_COMPAT,          // Otherwise unspecified compatibility character
    }
}
