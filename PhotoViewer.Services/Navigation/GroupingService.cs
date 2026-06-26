using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoViewer.Core.Models;

namespace PhotoViewer.Services.Navigation;

public class GroupingService
{
    public List<PhotoGroup> BuildGroups(
        List<Photo> photos)
    {
        var result = photos
            .GroupBy(GetGroupKey)
            .Select(group => new PhotoGroup
            {
                Name = group.Key,

                Photos = group
                    .OrderBy(x => x.ParsedName?.Index)
                    .ThenBy(x => x.FileName)
                    .ToList()
            })
            .OrderBy(x => x.Name)
            .ToList();

        return result;
    }

    private string GetGroupKey(Photo photo)
    {
        if (photo.ParsedName == null)
            return "Unknown";

        if (photo.ParsedName.IsComposite)
        {
            return string.Join(
                " & ",
                photo.ParsedName.Entities.Distinct());
        }

        return photo.ParsedName.Entities.FirstOrDefault()
               ?? "Unknown";
    }
}