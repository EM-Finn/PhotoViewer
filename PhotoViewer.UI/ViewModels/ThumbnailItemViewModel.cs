using CommunityToolkit.Mvvm.ComponentModel;
using PhotoViewer.Core.Models;
using System.Windows.Media.Imaging;

namespace PhotoViewer.UI.ViewModels;

public partial class ThumbnailItemViewModel
    : ObservableObject
{
    public Photo Photo { get; }

    [ObservableProperty]
    private BitmapImage? thumbnail;

    public ThumbnailItemViewModel(Photo photo)
    {
        Photo = photo;
    }
}