using System;
using System.Collections.Concurrent;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.Caching.Memory;
using PhotoViewer.UI.Helpers;

namespace PhotoViewer.UI.Caching
{
    // Класс использует Microsoft.Extensions.Caching.Memory.MemoryCache.
    // Для возможности очистки/итерации мы отдельно храним ключи в ConcurrentDictionary.
    public class ThumbnailCacheService : IDisposable
    {
        private readonly MemoryCache _cache;
        private readonly ConcurrentDictionary<string, byte> _keys;
        private readonly MemoryCacheEntryOptions _defaultOptions;
        private bool _disposed;

        public static ThumbnailCacheService Instance { get; } = new ThumbnailCacheService();

        public ThumbnailCacheService()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _keys = new ConcurrentDictionary<string, byte>();
            _defaultOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            };
        }

        // Возвращает миниатюру или null при ошибке
        public BitmapImage GetThumbnail(string path, int decodeWidth = 256)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            var key = $"{path}|{decodeWidth}";

            if (_cache.TryGetValue(key, out BitmapImage cached))
                return cached;

            BitmapImage thumbnail;
            try
            {
                thumbnail = ImageHelper.LoadBitmap(path, decodeWidth);
            }
            catch
            {
                return null;
            }

            _cache.Set(key, thumbnail, _defaultOptions);
            _keys.TryAdd(key, 0);
            return thumbnail;
        }

        // Удалить конкретную миниатюру
        public void Remove(string path, int decodeWidth = 256)
        {
            var key = $"{path}|{decodeWidth}";
            _cache.Remove(key);
            _keys.TryRemove(key, out _);
        }

        // Очистить кэш
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