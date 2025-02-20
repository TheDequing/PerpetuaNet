using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PerpetuaNet.Helpers;
using PerpetuaNet.ViewModels;

namespace PerpetuaNet.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username = "";
        private string _password = "";
        private string _errorMessage = "";

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand ShowRegisterCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(_ => Login());
            ShowRegisterCommand = new RelayCommand(_ =>
            {
                MainWindowViewModel.Instance.SelectedTabIndex = 9; // Agora funciona corretamente
            });
        }

        private void Login()
        {
            if (Username == "admin" && Password == "1234")
            {
                MainWindowViewModel.Instance.IsLoggedIn = true;
                MainWindowViewModel.Instance.SelectedTabIndex = 0; // Retorna para a aba inicial
            }
            else
            {
                ErrorMessage = "Usuário ou senha incorretos!";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
