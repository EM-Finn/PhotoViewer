using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Windows.Media.Imaging;

namespace PhotoViewer.UI.Imaging;

public class BitmapImageLoader : IImageLoader
{
    private readonly ConcurrentDictionary<string, WeakReference<BitmapImage>>
        _cache = new();

    public async Task<BitmapImage> LoadAsync(
        string path,
        int decodeWidth)
    {
        if (_cache.TryGetValue(path, out var weak))
        {
            if (weak.TryGetTarget(out var existing))
                return existing;
        }

        return await Task.Run(() =>
        {
            using var stream = File.OpenRead(path);

            var bitmap = new BitmapImage();

            bitmap.BeginInit();

            bitmap.CacheOption = BitmapCacheOption.OnLoad;

            bitmap.CreateOptions =
                BitmapCreateOptions.IgnoreColorProfile;

            bitmap.DecodePixelWidth = decodeWidth;

            bitmap.StreamSource = stream;

            bitmap.EndInit();

            bitmap.Freeze();

            _cache[path] =
                new WeakReference<BitmapImage>(bitmap);

            return bitmap;
        });
    }

    public void ClearCache()
    {
        _cache.Clear();

        GC.Collect();

        GC.WaitForPendingFinalizers();

        GC.Collect();
    }
}
