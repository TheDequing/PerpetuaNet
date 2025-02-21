using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PerpetuaNet.Views;
using System.Timers;

namespace PerpetuaNet.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private object _currentView;

    [ObservableProperty]
    private bool _isAdminLoggedIn;

    private readonly Timer _syncTimer;

    public MainWindowViewModel()
    {
        CurrentView = new HomeView();
        IsAdminLoggedIn = false;

        // Configura sincronização automática
        StartWebRTCSync();

        // Timer para verificar sincronização a cada 1 hora (3600000 ms)
        _syncTimer = new Timer(3600000);
        _syncTimer.Elapsed += (s, e) => StartWebRTCSync();
        _syncTimer.AutoReset = true;
        _syncTimer.Start();
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

    public void Login(string username)
    {
        var authorizedAdmins = new List<string> { "seu_usuario", "outro_admin" };
        IsAdminLoggedIn = authorizedAdmins.Contains(username.ToLower());
    }

    private async void StartWebRTCSync()
    {
        try
        {
            var syncService = new WebRTCSyncService();
            await syncService.InitializeAndSync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro na sincronização WebRTC: {ex.Message}");
        }
    }
}