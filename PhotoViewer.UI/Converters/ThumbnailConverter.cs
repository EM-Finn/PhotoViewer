using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using PhotoViewer.UI.Caching;
using System.Diagnostics;

namespace PhotoViewer.UI.Converters
{
    public class ThumbnailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = value as string;
            if (string.IsNullOrEmpty(path))
                return null;

            int decodeWidth = 256;
            if (parameter != null && int.TryParse(parameter.ToString(), out var w))
                decodeWidth = w;

            try
            {
                var result = ThumbnailCacheService.Instance.GetThumbnail(path, decodeWidth);
                if (result == null)
                    Debug.WriteLine($"[ThumbnailConverter] GetThumbnail returned null for: {path}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ThumbnailConverter] Error: {ex.Message} for path: {path}");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}