using FonntConverter.CreateDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FonntConverter.CreateDB.Helpers;

public static class ApplyUnicodeCharactersHelper
{
    public static void ApplyUnicodeCharacters(Blocks blocks, SortedDictionary<uint, Character> characters)
    {
        foreach (var character in characters.Values)
        {
            foreach (var block in blocks.Values)
            {
                if (character.CodePoint < block.Start || character.CodePoint > block.End)
                    continue;

                if (block.GetCharacter(character.CodePoint) is Character cp && cp is not null)
                {
                    if (!string.IsNullOrEmpty(character.Name))
                        cp.Name = character.Name;
                    cp.DecompositionType = character.DecompositionType;
                    cp.DecompositionMapping.Clear();
                    cp.DecompositionMapping.AddRange(character.DecompositionMapping);
                }
                else
                {
                    Character newCharacter = new Character(character.CodePoint, character.Name, character.DecompositionType, character.DecompositionMapping);
                    newCharacter.Block = block.Start;
                    if (!block.Characters.ContainsKey(newCharacter.CodePoint))
                    {
                        block.Characters.Add(newCharacter.CodePoint, newCharacter);
                    }
                }
            }
        }
    }
}
