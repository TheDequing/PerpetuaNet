using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PerpetuaNet.Helpers;

namespace PerpetuaNet.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // Implementação do padrão Singleton para o ViewModel
        private static MainWindowViewModel? _instance;
        public static MainWindowViewModel Instance => _instance ??= new MainWindowViewModel();

        private int _selectedTabIndex;
        private bool _isLoggedIn;

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                if (_selectedTabIndex != value)
                {
                    _selectedTabIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                if (_isLoggedIn != value)
                {
                    _isLoggedIn = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsNotLoggedIn));
                }
            }
        }

        // Propriedade auxiliar para exibir o botão de Login quando o usuário não estiver logado
        public bool IsNotLoggedIn => !IsLoggedIn;

        // Comandos para navegação
        public ICommand ShowInicioCommand { get; }
        public ICommand ShowBibliotecaCommand { get; }
        public ICommand ShowJogosCommand { get; }
        public ICommand ShowFilmesCommand { get; }
        public ICommand ShowSeriesCommand { get; }
        public ICommand ShowAnimesCommand { get; }
        public ICommand ShowConfiguracoesCommand { get; }
        public ICommand ShowDownloadsCommand { get; }
        public ICommand ShowLoginCommand { get; }
        public ICommand LogOutCommand { get; }

        public MainWindowViewModel()
        {
            SelectedTabIndex = 0;
            IsLoggedIn = false;

            ShowInicioCommand = new RelayCommand(_ =>
            {
                System.Diagnostics.Debug.WriteLine("Comando ShowInicioCommand executado");
                SelectedTabIndex = 0;
            });
            ShowBibliotecaCommand = new RelayCommand(_ => SelectedTabIndex = 1);
            ShowJogosCommand = new RelayCommand(_ => SelectedTabIndex = 2);
            ShowFilmesCommand = new RelayCommand(_ => SelectedTabIndex = 3);
            ShowSeriesCommand = new RelayCommand(_ => SelectedTabIndex = 4);
            ShowAnimesCommand = new RelayCommand(_ => SelectedTabIndex = 5);
            ShowConfiguracoesCommand = new RelayCommand(_ => SelectedTabIndex = 6);
            ShowDownloadsCommand = new RelayCommand(_ =>
            {
                if (IsLoggedIn)
                    SelectedTabIndex = 7;
                else
                    SelectedTabIndex = 8; // Exibe a aba de Login se não estiver logado
            });
            ShowLoginCommand = new RelayCommand(_ => SelectedTabIndex = 8);
            LogOutCommand = new RelayCommand(_ =>
            {
                IsLoggedIn = false;
                SelectedTabIndex = 0;
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
