using System.Windows.Media.Imaging;

namespace PhotoViewer.UI.Helpers;

public static class ImageHelper
{
    public static BitmapImage LoadBitmap(string path, int decodeWidth = 0)
    {
        var bitmap = new BitmapImage();

        bitmap.BeginInit();

        bitmap.CacheOption = BitmapCacheOption.OnLoad;

        if (decodeWidth > 0)
        {
            bitmap.DecodePixelWidth = decodeWidth;
        }

        bitmap.UriSource = new Uri(path);

        bitmap.EndInit();

        bitmap.Freeze();

        return bitmap;
    }
}