using System;
using System.IO;
using System.Windows.Media.Imaging;
using System.Diagnostics;

namespace PhotoViewer.UI.Helpers
{
    public static class ImageHelper
    {
        public static BitmapImage LoadBitmap(string path, int decodeWidth = 0)
        {
            Debug.WriteLine($"[ImageHelper] LoadBitmap called with path: {path}, decodeWidth: {decodeWidth}");

            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
            {
                Debug.WriteLine($"[ImageHelper] File does not exist: {path}");
                throw new FileNotFoundException($"File not found: {path}");
            }

            var bitmap = new BitmapImage();

            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    // НЕ используем IgnoreImageCache с локальными файлами - это вызывает ошибку
                    // bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    if (decodeWidth > 0) bitmap.DecodePixelWidth = decodeWidth;
                    bitmap.StreamSource = fs;
                    bitmap.EndInit();
                    bitmap.Freeze();
                }

                Debug.WriteLine($"[ImageHelper] ✓ Successfully loaded image, size: {bitmap.PixelWidth}x{bitmap.PixelHeight}");
                return bitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ImageHelper] ✗ Error loading bitmap: {ex.Message}");
                Debug.WriteLine($"[ImageHelper] Stack trace: {ex.StackTrace}");
                Debug.WriteLine($"[ImageHelper] Inner exception: {ex.InnerException?.Message}");
                throw;
            }
        }
    }
}
