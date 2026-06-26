namespace PhotoViewer.Core.Models;

public class PhotoGroup
{
    public string Name { get; set; } = string.Empty;

    public List<Photo> Photos { get; set; } = new();
}