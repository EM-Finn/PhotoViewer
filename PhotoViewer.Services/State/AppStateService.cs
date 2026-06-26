using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using PhotoViewer.Core.Models;
using System.Collections.ObjectModel;

namespace PhotoViewer.Services.State;

public partial class AppStateService : ObservableObject
{
    [ObservableProperty]
    private string currentFolder = string.Empty;

    [ObservableProperty]
    private string searchQuery = string.Empty;

    [ObservableProperty]
    private bool isSearchOpen;

    [ObservableProperty]
    private ObservableCollection<PhotoGroup> allGroups = new();

    [ObservableProperty]
    private ObservableCollection<PhotoGroup> visibleGroups = new();

    [ObservableProperty]
    private PhotoGroup? selectedGroup;

    [ObservableProperty]
    private Photo? selectedPhoto;
}