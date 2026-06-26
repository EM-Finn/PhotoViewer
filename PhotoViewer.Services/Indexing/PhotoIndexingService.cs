using PhotoViewer.Core.Models;
using PhotoViewer.Services.Navigation;
using System.Text.RegularExpressions;

namespace PhotoViewer.Services.Indexing;

public class PhotoIndexingService
{
    private static readonly string[] SupportedExtensions =
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    private readonly FilenameParserService _parserService;
    private readonly GroupingService _groupingService;

    public PhotoIndexingService(
    FilenameParserService parserService,
    GroupingService groupingService)
    {
        _parserService = parserService;
        _groupingService = groupingService;
    }

    public async Task<List<PhotoGroup>> IndexFolderAsync(
        string folderPath)
    {
        return await Task.Run(() =>
        {
            var files = Directory
                .GetFiles(folderPath)
                .Where(x =>
                    SupportedExtensions.Contains(
                        Path.GetExtension(x).ToLower()))
                .ToList();

            var photos = files
                .Select(CreatePhoto)
                .ToList();

            var groups = _groupingService.BuildGroups(photos);

            return groups;
        });
    }

    private Photo CreatePhoto(string filePath)
    {
        var info = new FileInfo(filePath);

        var fileName = Path.GetFileNameWithoutExtension(filePath);

        var parsed = _parserService.Parse(fileName);

        var groupName =
            ExtractGroupName(fileName);

        return new Photo
        {
            Id = Guid.NewGuid(),

            FilePath = filePath,

            FileName = fileName,
            
            GroupName = groupName,

            ThumbnailPath = filePath,

            CreatedAt = info.CreationTime,

            LastModified = info.LastWriteTime,

            FileSize = info.Length,

            ParsedName = parsed,
            DisplayGroupName = parsed.Entities.FirstOrDefault() ?? "Unknown",
        };
    }

    private string ExtractGroupName(string fileName)
    {
        return Regex
            .Replace(fileName, @"[\s_-]?\d+$", "")
            .Trim();
    }
}