using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PhotoViewer.UI.ViewModels;
using CommunityToolkit.Mvvm.Input;

namespace PhotoViewer.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();

        DataContext = vm;
        KeyDown += OnKeyDown;
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not MainViewModel vm)
            return;

        switch (e.Key)
        {
            case Key.Right:
                vm.NextPhotoCommand.Execute(null);
                break;

            case Key.Left:
                vm.PreviousPhotoCommand.Execute(null);
                break;

            case Key.Down:
                vm.NextGroupCommand.Execute(null);
                break;

            case Key.Up:
                vm.PreviousGroupCommand.Execute(null);
                break;
        }

        if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.F)
        {
            SearchBox.Focus();
        }
    }
}