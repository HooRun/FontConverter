using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FonntConverter.CreateDB.Models;

public class Blocks : SortedDictionary<uint, Block>
{
    public Blocks() : base()
    {

    }

    public Blocks(IDictionary<uint, Block> dictionary) : base(dictionary)
    {

    }

    public Block? GetBlock(uint codePoint)
    {
        if (this.Count == 0) return null;
        var block = this.LastOrDefault(b => b.Value.GetBlockStart(codePoint) != null);
        return block.Value;
    }

    public uint? GetBlockStart(uint codePoint)
    {
        if (this.Count == 0) return null;
        var block = this.LastOrDefault(b => b.Value.GetBlockStart(codePoint) != null);
        return block.Key;
    }
}
