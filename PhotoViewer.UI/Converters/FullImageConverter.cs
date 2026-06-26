using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using PhotoViewer.UI.Helpers;

namespace PhotoViewer.UI.Converters
{
    public class FullImageConverter : IValueConverter
    {
        // parameter: optional decodeWidth (int)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = value as string;
            if (string.IsNullOrEmpty(path)) return null;

            int decodeWidth = 0;
            if (parameter != null && int.TryParse(parameter.ToString(), out var w))
                decodeWidth = w;

            try
            {
                return ImageHelper.LoadBitmap(path, decodeWidth);
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