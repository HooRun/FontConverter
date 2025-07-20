using FonntConverter.CreateDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FonntConverter.CreateDB.Helpers;

public static class ParseBlocksHelper
{

    public static async Task<Blocks> ParseUnicodeBlocksAsync(string resourceName, CancellationToken cancellationToken = default)
    {
        Blocks blocks = new Blocks();

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var assembly = Assembly.GetExecutingAssembly();
            var fullName = assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith(resourceName))
                ?? throw new FileNotFoundException($"Resource {resourceName} not found");

            using var stream = assembly.GetManifestResourceStream(fullName)!;
            using var reader = new StreamReader(stream);

            string content = await reader.ReadToEndAsync(cancellationToken);

            var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Parallel.ForEach(lines, line =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var clean = line.Split('#')[0].Trim();
                if (string.IsNullOrWhiteSpace(clean)) return;

                var parts = clean.Split(';', StringSplitOptions.TrimEntries);
                if (parts.Length < 2) return;

                var bounds = parts[0].Split("..", StringSplitOptions.TrimEntries);
                uint start = Convert.ToUInt32(bounds[0], 16);
                uint end = Convert.ToUInt32(bounds[1], 16);
                string name = parts[1];
                Block block = new Block(start, end, name);
                uint lenght = (block.End - block.Start + 1);
                //Parallel.For(0, lenght, i =>
                //{
                //    uint codePoint = (uint)(start + i);
                //    Character character = new Character(codePoint, $"U+{codePoint:X6}", start);
                //    lock (block.Characters)
                //    {
                //        block.Characters.TryAdd(codePoint, character);
                //    }
                //});
                lock (blocks)
                {
                    if (!blocks.ContainsKey(start))
                    {
                        blocks.TryAdd(start, block);
                    }
                }
            });
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load Unicode blocks from {resourceName}.", ex);
        }
        return blocks;
    }
}
