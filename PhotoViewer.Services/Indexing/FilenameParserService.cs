using PhotoViewer.Core.Models;

using System.Text.RegularExpressions;

namespace PhotoViewer.Services.Indexing;

public class FilenameParserService
{
    public ParsedPhotoName Parse(string fileName)
    {
        var parsed = new ParsedPhotoName
        {
            OriginalName = fileName
        };

        var normalized =
            Path.GetFileNameWithoutExtension(fileName)
                .Trim();

        var indexMatch =
            Regex.Match(normalized, @"_(\d+)$|\s(\d+)$");

        if (indexMatch.Success)
        {
            var value =
                indexMatch.Groups[1].Value;

            if (string.IsNullOrWhiteSpace(value))
                value = indexMatch.Groups[2].Value;

            if (int.TryParse(value, out var index))
            {
                parsed.Index = index;
            }

            normalized =
                normalized.Substring(0, indexMatch.Index)
                    .Trim();
        }

        if (normalized.Contains('&'))
        {
            parsed.IsComposite = true;

            var parts = normalized
                .Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToList();

            foreach (var part in parts)
            {
                ExtractEntityAndProperties(part, parsed);
            }
        }
        else
        {
            ExtractEntityAndProperties(normalized, parsed);
        }

        return parsed;
    }

    private void ExtractEntityAndProperties(
        string text,
        ParsedPhotoName parsed)
    {
        var tokens =
            text.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToList();

        if (!tokens.Any())
            return;

        parsed.Entities.Add(tokens[0]);

        if (tokens.Count <= 1)
            return;

        for (var i = 1; i < tokens.Count; i++)
        {
            var propertyTokens =
                tokens[i]
                    .Split('_', StringSplitOptions.RemoveEmptyEntries);

            parsed.Properties.AddRange(propertyTokens);
        }
    }
}