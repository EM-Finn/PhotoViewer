using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace PhotoViewer.UI.Helpers
{
    public static class ImageHelper
    {
        public static BitmapImage LoadBitmap(string path, int decodeWidth = 0)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            var bitmap = new BitmapImage();

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                if (decodeWidth > 0) bitmap.DecodePixelWidth = decodeWidth;
                bitmap.StreamSource = fs;
                bitmap.EndInit();
                bitmap.Freeze();
            }

            return bitmap;
        }
    }
}