using FonntConverter.CreateDB.Helpers;
using FonntConverter.CreateDB.Models;
using System;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading;
using MessagePack;

class Program
{
    const string blockResourceName = "Blocks.txt";
    const string characterResourceName = "UnicodeData.txt";
    // https://github.com/unicode-org/cldr/tree/main/common/uca
    const string collationResourceName = "allkeys_CLDR.txt";

    static async Task Main(string[] args)
    {
        await CreateDatabaseAsync();
    }

    private static async Task CreateDatabaseAsync()
    {
        try
        {
            var blocks = await ParseBlocksHelper.ParseUnicodeBlocksAsync(blockResourceName);
            var characters = await ParseCharactersHelper.ParseUnicodeDataAsync(characterResourceName);
            var collations = await ParseAllKeysCLDRHelper.ParseAllKeysCLDR(collationResourceName);
            ApplyUnicodeCharactersHelper.ApplyUnicodeCharacters(blocks, characters);
            ApplyCollationCommentsHelper.ApplyCollationComments(blocks, collations);

            Console.WriteLine($"Blocks: {blocks.Count}");
            Console.WriteLine($"Characters: {characters.Count}");
            Console.WriteLine($"Collations: {collations.Count}");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            string projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string filePath = Path.Combine(projectDir, "GeneratedFiles", "unicode_database.bin");
            using var stream = File.Create(filePath);
            MessagePackSerializer.Serialize(stream, blocks);
            Console.WriteLine("Database created successfully at: " + filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}