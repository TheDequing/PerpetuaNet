using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PerpetuaNet.Views;

namespace PerpetuaNet.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private object _currentView;

    [ObservableProperty]
    private bool _isAdminLoggedIn;

    public MainWindowViewModel()
    {
        CurrentView = new HomeView();
        IsAdminLoggedIn = false; // Começa como não logado
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

    [RelayCommand]
    private void ShowAdmin() => CurrentView = new AdminView();

    // Método para verificar login e liberar Admin
    public void Login(string username)
    {
        // Lista de usuários autorizados (você pode expandir isso)
        var authorizedAdmins = new List<string> { "seu_usuario", "outro_admin" };
        IsAdminLoggedIn = authorizedAdmins.Contains(username.ToLower());
    }
}