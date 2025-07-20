using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FonntConverter.CreateDB.Models;

public class CollationWeight
{
    public CollationWeight()
    {
        Primary = string.Empty;
        Secondary = string.Empty;
        Tertiary = string.Empty;
    }
    public string Primary { get; set; }
    public string Secondary { get; set; }
    public string Tertiary { get; set; } 

    public override string ToString() => $"[{Primary}.{Secondary}.{Tertiary}]";
}
