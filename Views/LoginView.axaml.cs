using Avalonia.Controls;

namespace PerpetuaNet.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
        DataContext = new PerpetuaNet.ViewModels.LoginViewModel();
    }
}