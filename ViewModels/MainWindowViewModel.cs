using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PerpetuaNet.Views;

namespace PerpetuaNet.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private object _currentView;

    public MainWindowViewModel()
    {
        CurrentView = new HomeView();
    }

    [RelayCommand]
    private void ShowHome() => CurrentView = new HomeView();

    [RelayCommand]
    private void ShowTorrents() => CurrentView = new TorrentsView();

    [RelayCommand]
    private void ShowSync() => CurrentView = new SyncView();

    [RelayCommand]
    private void ShowSettings() => CurrentView = new SettingsView();
}