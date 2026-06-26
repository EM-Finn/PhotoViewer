using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Windows.Media.Imaging;

namespace PhotoViewer.Services.Caching;

public class ThumbnailCacheService
{
    private readonly ConcurrentDictionary<string, BitmapImage>
        _cache = new();

    public BitmapImage GetThumbnail(
        string path,
        int decodeWidth = 256)
    {
        if (_cache.TryGetValue(path, out var cached))
            return cached;

        var bitmap = new BitmapImage();

        bitmap.BeginInit();

        bitmap.CacheOption = BitmapCacheOption.OnLoad;

        bitmap.DecodePixelWidth = decodeWidth;

        bitmap.UriSource = new Uri(path);

        bitmap.EndInit();

        bitmap.Freeze();

        _cache[path] = bitmap;

        return bitmap;
    }

    public void Clear()
    {
        _cache.Clear();
    }
}