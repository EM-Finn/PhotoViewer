using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using PhotoViewer.UI.Caching;

namespace PhotoViewer.UI.Converters
{
    public class ThumbnailConverter : IValueConverter
    {
        // parameter: optional decodeWidth (int)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = value as string;
            if (string.IsNullOrEmpty(path)) return null;

            int decodeWidth = 256;
            if (parameter != null && int.TryParse(parameter.ToString(), out var w))
                decodeWidth = w;

            try
            {
                return ThumbnailCacheService.Instance.GetThumbnail(path, decodeWidth);
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}