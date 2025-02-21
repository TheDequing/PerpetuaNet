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
    private void ShowCatalog() => CurrentView = new CatalogView();

    [RelayCommand]
    private void ShowDownloads() => CurrentView = new DownloadsView();

    [RelayCommand]
    private void ShowSettings() => CurrentView = new SettingsView();

    [RelayCommand]
    private void ShowLibrary() => CurrentView = new LibraryView();

    [RelayCommand]
    private void ShowLogin() => CurrentView = new LoginView();
}