using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoViewer.Core.Models;

namespace PhotoViewer.Services.Navigation;

public class NavigationService
{
    public Photo? GetNextPhoto(
        PhotoGroup group,
        Photo current)
    {
        var index =
            group.Photos.IndexOf(current);

        if (index < 0)
            return null;

        if (index >= group.Photos.Count - 1)
            return null;

        return group.Photos[index + 1];
    }

    public Photo? GetPreviousPhoto(
        PhotoGroup group,
        Photo current)
    {
        var index =
            group.Photos.IndexOf(current);

        if (index <= 0)
            return null;

        return group.Photos[index - 1];
    }

    public PhotoGroup? GetNextGroup(
        List<PhotoGroup> groups,
        PhotoGroup current)
    {
        var index = groups.IndexOf(current);

        if (index < 0)
            return null;

        if (index >= groups.Count - 1)
            return null;

        return groups[index + 1];
    }

    public PhotoGroup? GetPreviousGroup(
        List<PhotoGroup> groups,
        PhotoGroup current)
    {
        var index = groups.IndexOf(current);

        if (index <= 0)
            return null;

        return groups[index - 1];
    }
}