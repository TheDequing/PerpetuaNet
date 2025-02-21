using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PerpetuaNet.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private string _username;

    private readonly MainWindowViewModel _mainViewModel;

    public LoginViewModel()
    {
        _mainViewModel = new MainWindowViewModel(); // Pode precisar de injeção de dependência melhor
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
        // Lógica de registro (ex.: abrir uma tela ou salvar usuário)
        // Por enquanto, apenas um placeholder
        System.Diagnostics.Debug.WriteLine($"Registro solicitado para: {Username}");
    }
}