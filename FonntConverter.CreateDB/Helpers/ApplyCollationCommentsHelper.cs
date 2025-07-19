using FonntConverter.CreateDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FonntConverter.CreateDB.Helpers;

public static class ApplyCollationCommentsHelper
{
    public static void ApplyCollationComments(Blocks blocks, SortedDictionary<string, Collation> collations)
    {
        foreach (var collation in collations.Values)
        {
            if (collation.CodePoints.Count != 1 || string.IsNullOrEmpty(collation.Comment))
                continue;

            foreach (var block in blocks.Values)
            {
                if (collation.CodePoints[0] < block.Start || collation.CodePoints[0] > block.End)
                    continue;

                if (block.GetCharacter(collation.CodePoints[0]) is Character cp && cp is not null)
                {
                    cp.Name = collation.Comment;
                }
            }
        }
    }
}
