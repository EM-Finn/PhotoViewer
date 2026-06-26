namespace PhotoViewer.Core.Models;

public class Photo
{
    public Guid Id { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string GroupName { get; set; } = string.Empty;

    public string ThumbnailPath { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime LastModified { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }

    public long FileSize { get; set; }

    public ParsedPhotoName? ParsedName { get; set; }

    public string DisplayGroupName { get; set; } = string.Empty;

    public string? SubGroupName { get; set; }

    public List<PhotoTag> Tags { get; set; } = new();
}