using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhotoViewer.Services.Indexing;
using PhotoViewer.UI.ViewModels;
using PhotoViewer.UI.Views;
using System.Windows;
using PhotoViewer.Services.Navigation;
using PhotoViewer.Services.Caching;
using PhotoViewer.Services.Search;
using PhotoViewer.Services.State;

namespace PhotoViewer.UI;
public partial class App : Application
{
    public static IHost HostContainer { get; private set; } = null!;

    public App()
    {
        HostContainer = Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<PhotoIndexingService>();

                services.AddSingleton<MainViewModel>();

                services.AddSingleton<MainWindow>();

                services.AddSingleton<FilenameParserService>();

                services.AddSingleton<GroupingService>();

                services.AddSingleton<ThumbnailCacheService>();

                services.AddSingleton<NavigationService>();

                services.AddSingleton<AppStateService>();

                services.AddSingleton<SearchService>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await HostContainer.StartAsync();

        var mainWindow =
            HostContainer.Services.GetRequiredService<MainWindow>();

        mainWindow.Show();

        base.OnStartup(e);
    }
}