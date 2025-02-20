using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PerpetuaNet.ViewModels;

namespace PerpetuaNet.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Define o DataContext utilizando a instância singleton do ViewModel
            DataContext = MainWindowViewModel.Instance;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
