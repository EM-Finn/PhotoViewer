using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using PhotoViewer.UI.Helpers;
using System.Diagnostics;

namespace PhotoViewer.UI.Converters
{
    public class FullImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = value as string;
            Debug.WriteLine($"[FullImageConverter] Path: {path ?? "null"}");

            if (string.IsNullOrEmpty(path))
                return null;

            int decodeWidth = 0;
            if (parameter != null && int.TryParse(parameter.ToString(), out var w))
                decodeWidth = w;

            try
            {
                var bitmap = ImageHelper.LoadBitmap(path, decodeWidth);
                Debug.WriteLine($"[FullImageConverter] ✓ Successfully loaded: {path}");
                return bitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[FullImageConverter] ✗ Error loading {path}: {ex.Message}");
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}