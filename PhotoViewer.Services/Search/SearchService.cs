using PhotoViewer.Core.Models;

namespace PhotoViewer.Services.Search;

public class SearchService
{
    public List<PhotoGroup> FilterGroups(
        IEnumerable<PhotoGroup> groups,
        string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return groups.ToList();
        }

        query = query.ToLower();

        var result = groups
            .Select(group =>
            {
                var matchedPhotos = group.Photos
                    .Where(photo =>
                        IsPhotoMatch(photo, query))
                    .ToList();

                if (!matchedPhotos.Any()
                    && !group.Name.ToLower().Contains(query))
                {
                    return null;
                }

                return new PhotoGroup
                {
                    Name = group.Name,
                    Photos = matchedPhotos.Any()
                        ? matchedPhotos
                        : group.Photos
                };
            })
            .Where(x => x != null)
            .Cast<PhotoGroup>()
            .ToList();

        return result;
    }

    private bool IsPhotoMatch(
        Photo photo,
        string query)
    {
        if (photo.FileName
            .ToLower()
            .Contains(query))
        {
            return true;
        }

        if (photo.DisplayGroupName
            .ToLower()
            .Contains(query))
        {
            return true;
        }

        if (photo.ParsedName != null)
        {
            if (photo.ParsedName.Entities
                .Any(x => x.ToLower().Contains(query)))
            {
                return true;
            }

            if (photo.ParsedName.Properties
                .Any(x => x.ToLower().Contains(query)))
            {
                return true;
            }
        }

        return false;
    }
}