using System.Windows.Input;
using PerpetuaNet.Helpers;
using PerpetuaNet.ViewModels;

namespace PerpetuaNet.ViewModels
{
    public class InicioViewModel
    {
        public ICommand ShowLoginCommand { get; }

        public InicioViewModel()
        {
            ShowLoginCommand = new RelayCommand(_ =>
            {
                MainWindowViewModel.Instance.SelectedTabIndex = 8; // Agora funciona corretamente
            });
        }
    }
}
