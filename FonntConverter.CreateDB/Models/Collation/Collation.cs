using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FonntConverter.CreateDB.Models;

public class Collation
{
    public Collation()
    {
        CodePoints = [];
        Weights = [];
        Comment = string.Empty;
    }

    public Collation(List<uint> codePoints, List<CollationWeight> weights, string? comment) : this()
    {
        CodePoints = codePoints;
        Weights = weights;
        Comment = comment ?? string.Empty;
    }

    public List<uint> CodePoints { get; set; }
    public List<CollationWeight> Weights { get; set; }
    public string Comment { get; set; }
}
