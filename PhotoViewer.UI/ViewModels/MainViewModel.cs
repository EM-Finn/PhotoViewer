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

namespace PhotoViewer.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly PhotoIndexingService _indexingService;
    private readonly NavigationService _navigationService;

    private readonly AppStateService _state;
    private readonly SearchService _searchService;

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

    public MainViewModel(PhotoIndexingService indexingService)
    {
        _indexingService = indexingService;
    }

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
        LoadCurrentPhoto();
    }

    partial void OnSearchTextChanged(string value)
    {
        ApplySearch();
    }

    private void LoadCurrentPhoto()
    {
        if (selectedPhoto == null)
            return;

        CurrentPhoto = ImageHelper.LoadBitmap(
            selectedPhoto.FilePath,
            1600);
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