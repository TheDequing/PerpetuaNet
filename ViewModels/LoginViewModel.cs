using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PerpetuaNet.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _username; // Tornar anulável com ?

    private readonly MainWindowViewModel _mainViewModel;

    public LoginViewModel()
    {
        _mainViewModel = new MainWindowViewModel();
    }

    [RelayCommand]
    private void Login()
    {
        if (!string.IsNullOrEmpty(Username))
        {
            _mainViewModel.Login(Username);
        }
    }

    [RelayCommand]
    private void Register()
    {
        System.Diagnostics.Debug.WriteLine($"Registro solicitado para: {Username}");
    }
}