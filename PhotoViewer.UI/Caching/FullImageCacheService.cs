using System;
using System.Collections.Concurrent;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.Caching.Memory;
using PhotoViewer.UI.Helpers;

namespace PhotoViewer.UI.Caching
{
    public class FullImageCacheService : IDisposable
    {
        private readonly MemoryCache _cache;
        private readonly ConcurrentDictionary<string, byte> _keys;
        private readonly MemoryCacheEntryOptions _defaultOptions;
        private bool _disposed;

        public static FullImageCacheService Instance { get; } = new FullImageCacheService();

        public FullImageCacheService()
        {
            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 500_000_000 // 500 МБ максимум
            });
            _keys = new ConcurrentDictionary<string, byte>();
            _defaultOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(5), // 5 минут для полных изображений
                Size = 100_000_000 // примерный размер одного полного изображения
            };
        }

        public BitmapImage GetFullImage(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            if (_cache.TryGetValue(path, out BitmapImage cached))
                return cached;

            BitmapImage image;
            try
            {
                image = ImageHelper.LoadBitmap(path, 0); // 0 = полное разрешение
            }
            catch
            {
                return null;
            }

            _cache.Set(path, image, _defaultOptions);
            _keys.TryAdd(path, 0);
            return image;
        }

        public void Clear()
        {
            foreach (var key in _keys.Keys)
            {
                _cache.Remove(key);
                _keys.TryRemove(key, out _);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _cache.Dispose();
            _disposed = true;
        }
    }
}