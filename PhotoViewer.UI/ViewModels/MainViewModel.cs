using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PhotoViewer.Core.Models;
using PhotoViewer.Services.Indexing;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using PhotoViewer.UI.Helpers;
using PhotoViewer.Services.Navigation;
using PhotoViewer.Services.Search;
using PhotoViewer.Services.State;
using System.Windows.Media;
using System.Diagnostics;

namespace PhotoViewer.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly PhotoIndexingService _indexingService;
    private readonly NavigationService _navigationService;

    private readonly AppStateService _state;
    private readonly SearchService _searchService;

    private CancellationTokenSource _loadCts;

    [ObservableProperty]
    private ImageSource currentPhotoImageSource;

    public MainViewModel(PhotoIndexingService indexingService, NavigationService navigationService, AppStateService state, SearchService searchService)
    {
        _indexingService = indexingService;

        _navigationService = navigationService;

        _state = state;

        _searchService = searchService;

        Groups = _state.VisibleGroups;
    }

    [ObservableProperty]
    private ObservableCollection<PhotoGroup> groups = new();

    [ObservableProperty]
    private PhotoGroup? selectedGroup;

    [ObservableProperty]
    private BitmapImage? currentPhoto;

    [ObservableProperty]
    private Photo? selectedPhoto;

    [ObservableProperty]
    private int currentPhotoIndex;

    [ObservableProperty]
    private string manualGroupName = string.Empty;

    [ObservableProperty]
    private string searchText = string.Empty;

    [RelayCommand]
    private async Task OpenFolder()
    {
        var dialog = new OpenFolderDialog();

        if (dialog.ShowDialog() != true)
            return;

        var groupsResult = await _indexingService
            .IndexFolderAsync(dialog.FolderName);

        Groups = new ObservableCollection<PhotoGroup>(groupsResult);

        _state.AllGroups = new ObservableCollection<PhotoGroup>(groupsResult);
        _state.VisibleGroups = Groups;

        SelectedGroup = Groups.FirstOrDefault();

        if (SelectedGroup != null)
        {
            selectedPhoto = SelectedGroup.Photos.FirstOrDefault();
        }
    }

    partial void OnSelectedGroupChanged(PhotoGroup? value)
    {
        if (value == null)
            return;

        currentPhotoIndex = 0;

        selectedPhoto = value.Photos.FirstOrDefault();
    }

    partial void OnSelectedPhotoChanged(Photo? value)
    {
        LoadCurrentPhotoAsync();
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplySearch();
    }

    private async Task LoadCurrentPhotoAsync()
    {
        _loadCts?.Cancel();
        _loadCts = new CancellationTokenSource();
        var ct = _loadCts.Token;

        // Очищаем перед новой загрузкой
        CurrentPhotoImageSource = null;

        try
        {
            var path = SelectedPhoto?.FilePath;
            if (string.IsNullOrEmpty(path))
            {
                Debug.WriteLine($"[MainViewModel] No photo selected");
                return;
            }

            Debug.WriteLine($"[MainViewModel] Loading photo: {path}");

            // Загружаем в фоне
            var bitmap = await Task.Run(() => ImageHelper.LoadBitmap(path, 0), ct); // 0 = полное разрешение

            if (ct.IsCancellationRequested) return;

            Debug.WriteLine($"[MainViewModel] Photo loaded successfully, size: {bitmap.PixelWidth}x{bitmap.PixelHeight}");
            CurrentPhotoImageSource = bitmap;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MainViewModel] Error loading photo: {ex.Message}\n{ex.StackTrace}");
        }
    }

    public void Dispose()
    {
        _loadCts?.Cancel();
        _loadCts?.Dispose();
    }

    [RelayCommand]
    private void NextPhoto()
    {
        if (SelectedGroup == null)
            return;

        if (currentPhotoIndex >= SelectedGroup.Photos.Count - 1)
            return;

        currentPhotoIndex++;

        selectedPhoto =
            SelectedGroup.Photos[currentPhotoIndex];
    }

    [RelayCommand]
    private void PreviousPhoto()
    {
        if (SelectedGroup == null)
            return;

        if (currentPhotoIndex <= 0)
            return;

        currentPhotoIndex--;

        selectedPhoto =
            SelectedGroup.Photos[currentPhotoIndex];
    }

    [RelayCommand]
    private void ApplyManualGroup()
    {
        if (SelectedPhoto == null)
            return;

        if (string.IsNullOrWhiteSpace(manualGroupName))
            return;

        SelectedPhoto.DisplayGroupName = manualGroupName;

        if (SelectedGroup != null)
        {
            SelectedGroup.Name = manualGroupName;
        }
    }

    [RelayCommand]
    private void NextGroup()
    {
        if (SelectedGroup == null)
            return;

        var next =
            _navigationService.GetNextGroup(
                Groups.ToList(),
                SelectedGroup);

        if (next == null)
            return;

        SelectedGroup = next;
    }

    [RelayCommand]
    private void PreviousGroup()
    {
        if (SelectedGroup == null)
            return;

        var previous =
            _navigationService.GetPreviousGroup(
                Groups.ToList(),
                SelectedGroup);

        if (previous == null)
            return;

        SelectedGroup = previous;
    }

    [RelayCommand]
    private void ApplySearch()
    {
        var filtered =
            _searchService.FilterGroups(
                _state.AllGroups,
                searchText);

        Groups =
            new ObservableCollection<PhotoGroup>(
                filtered);
    }
}
