using System;
using System.Collections.Concurrent;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.Caching.Memory;
using PhotoViewer.UI.Helpers;
using System.Diagnostics;

namespace PhotoViewer.UI.Caching
{
    // Кэш миниатюр с ограничением памяти
    public class ThumbnailCacheService : IDisposable
    {
        private readonly MemoryCache _cache;
        private readonly ConcurrentDictionary<string, byte> _keys;
        private bool _disposed;

        public static ThumbnailCacheService Instance { get; } = new ThumbnailCacheService();

        public ThumbnailCacheService()
        {
            // Создаём кэш БЕЗ SizeLimit - это проще и избегает проблем с null key
            _cache = new MemoryCache(new MemoryCacheOptions());
            _keys = new ConcurrentDictionary<string, byte>();
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
            catch (Exception ex)
            {
                Debug.WriteLine($"[ThumbnailCacheService] Error loading thumbnail: {ex.Message}");
                return null;
            }

            // Добавляем в кэш с временем жизни 10 минут
            var cacheOptions = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };
            
            _cache.Set(key, thumbnail, cacheOptions);
            _keys.TryAdd(key, 0);
            
            Debug.WriteLine($"[ThumbnailCacheService] ✓ Cached thumbnail: {path}");
            return thumbnail;
        }

        // Удалить конкретную миниатюру
        public void Remove(string path, int decodeWidth = 256)
        {
            var key = $"{path}|{decodeWidth}";
            _cache.Remove(key);
            _keys.TryRemove(key, out _);
        }

        // Очистить весь кэш
        public void Clear()
        {
            foreach (var key in _keys.Keys)
            {
                _cache.Remove(key);
                _keys.TryRemove(key, out _);
            }
            Debug.WriteLine($"[ThumbnailCacheService] Cache cleared");
        }

        public void Dispose()
        {
            if (_disposed) return;
            Clear();
            _cache?.Dispose();
            _disposed = true;
        }
    }
}
