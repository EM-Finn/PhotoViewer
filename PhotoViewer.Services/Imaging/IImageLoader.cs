using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PhotoViewer.UI.Imaging;

public interface IImageLoader
{
    Task<BitmapImage> LoadAsync(
        string path,
        int decodeWidth);

    void ClearCache();
}